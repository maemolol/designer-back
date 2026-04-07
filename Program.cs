using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using MongoDB.Driver;
using System.Text;
using Config;
using DotNetEnv;
using Microsoft.Extensions.Configuration;

Console.OutputEncoding = Encoding.UTF8;
// For Self:
var solutionRoot = Directory.GetParent(Directory.GetCurrentDirectory())!.FullName;
Env.Load(Path.Combine(solutionRoot, ".env"));
Console.WriteLine("✅ .env downloaded from: " + Path.Combine(solutionRoot, ".env"));

// For Docker:
Env.Load(".env");

var builder = WebApplication.CreateBuilder(args);

try
{
    // Load env vars
    string? mongoUri = Environment.GetEnvironmentVariable("MONGO_URI");
    string? jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
    string? jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
    string? jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
    string? encryptionKey = Environment.GetEnvironmentVariable("ENCRYPTION_KEY");

    void Check(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ {name} is missing.");
            Console.ResetColor();
            throw new Exception($"{name} is required.");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✅ {name} loaded.");
            Console.ResetColor();
        }
    }

    // Validate required vars
    Check(mongoUri, "MONGO_URI");
    Check(jwtKey, "JWT_KEY");
    Check(jwtIssuer, "JWT_ISSUER");
    Check(jwtAudience, "JWT_AUDIENCE");
    Check(encryptionKey, "ENCRYPTION_KEY");

    Console.WriteLine($"Mongo URI: {mongoUri}");

    // MongoDB test
    var mongoClient = new MongoClient(mongoUri);
    var mongoDb = mongoClient.GetDatabase("PaintingsCollection");
    // await mongoDb.RunCommandAsync((Command<MongoDB.Bson.BsonDocument>)"{ping:1}");
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("✅ MongoDB connection OK.");
    Console.ResetColor();

    // Register services
    builder.Services.AddDbContext<AppDbContext>(opts =>
    {
        opts.UseNpgsql(
            builder.Configuration.GetConnectionString("Default"),
            npgsqlOptions => npgsqlOptions.EnableRetryOnFailure()
        );
        opts.EnableSensitiveDataLogging(); // helpful for debugging
        opts.LogTo(Console.WriteLine, LogLevel.Debug);
    });
    builder.Services.AddSingleton<IMongoClient>(mongoClient);
    builder.Services.AddSingleton(mongoDb);
    Console.WriteLine("✅ Usage: http://localhost:5000 and http://localhost:5000/swagger/");
}

catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("❌ Startup error: " + ex.Message);
    Console.ResetColor();
    return;
}

// JWT-service
builder.Services.AddSingleton<JwtService>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
        ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")!)),
        RoleClaimType = ClaimTypes.Role,
        NameClaimType = ClaimTypes.NameIdentifier
    };
});

// Controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddScoped<EmailService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Painting Website API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token. Example: Bearer {your_token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// Image Parsing
builder.Services.AddSingleton<MongoService>();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IServiceScopeFactory>(sp => sp.GetRequiredService<IServiceScopeFactory>());

// Configure Paths
string logPath = Path.Combine(AppContext.BaseDirectory, "Logs", "running_log.log");

// CORS for Frontend
var frontendOrigin = Environment.GetEnvironmentVariable("ALLOWED_FRONTEND_PORT") ?? "http://localhost:3000";
/* builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendOnly", policy =>
    {
        policy.WithOrigins(frontendOrigin)
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
}); */
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});


// Logging off
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

//app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
//app.UseCors("AllowAll");
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

// Auto migrate and seed species
using (var scope = app.Services.CreateScope())
{
    var retries = 10;
    var delay = TimeSpan.FromSeconds(2);

    while (retries > 0)
    {
        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            Console.WriteLine("🔄 Checking database...");

            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                Console.WriteLine($"📦 Applying {pendingMigrations.Count()} migrations...");
                await dbContext.Database.MigrateAsync();
            }
            else
            {
                Console.WriteLine("✅ Database up to date.");
            }

            break; // success
        }
        catch (Exception ex)
        {
            retries--;

            Console.WriteLine($"⏳ DB not ready yet: {ex.Message}");

            if (retries == 0)
                throw;

            await Task.Delay(delay);
        }
    }
}


// Run the application locally
// app.Run();

// Run the application locally in docker or on a server
app.Run($"http://0.0.0.0:{port}");
