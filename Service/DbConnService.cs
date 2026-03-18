using DotNetEnv;
using System;
using System.IO;

public class DbConnectionService
{
    private static string? _cached;

    public static string TestDatabaseConnection()
    {
        if (!string.IsNullOrWhiteSpace(_cached))
            return _cached!;

        var envPath = Path.Combine(Directory.GetCurrentDirectory(), "../.env");
        if (File.Exists(envPath))
        {
            try { Env.Load(envPath); }
            catch { Console.WriteLine("⚠️ Failed to load .env"); }
        }

        var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
        var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
        var database = Environment.GetEnvironmentVariable("DB_NAME") ?? "designer";
        var user = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
        var password = Environment.GetEnvironmentVariable("DB_STATUS") ?? "postgres";

        var direct = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING");
        if (!string.IsNullOrWhiteSpace(direct))
        {
            _cached = direct;
            Console.WriteLine("✅ Using POSTGRES_CONNECTION_STRING");
            return _cached!;
        }

        _cached = $"Host={host};Port={port};Database={database};Username={user};Password={password};";
        Console.WriteLine($"✅ Using PostgreSQL connection: {host}:{port}");
        return _cached!;
    }
}