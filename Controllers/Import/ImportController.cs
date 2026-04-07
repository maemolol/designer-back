using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Dtos;
using MongoDB.Driver;
using Models;

namespace Controllers
{
    [ApiController]
    [Route("import")]

    public class ImportController : ControllerBase
    {
        private readonly AppDbContext _context;
        private MongoService _mongo;
        private IMongoDatabase _mongoDatabase;

        public ImportController(AppDbContext dbContext, MongoService mongo, IMongoDatabase mDb)
        {
            _context = dbContext;
            _mongo = mongo;
            _mongoDatabase = mDb;
        }

        [HttpPost]
        public async Task<IActionResult> Import()
        {
            try
            {
                var json = await System.IO.File.ReadAllTextAsync("Data/paintings.json");

                var items = JsonSerializer.Deserialize<List<CreatePaintingDto>>(json);

                if (items == null || !items.Any())
                    return BadRequest("No data found");

                var mongoCollection = _mongoDatabase.GetCollection<ImageRecord>("images");

                int count = 0;

                foreach (var dto in items)
                {
                    // Prevent duplicates (important!)
                    if (await _context.Paintings.AnyAsync(p => p.Name == dto.Name))
                        return BadRequest("Data already imported");

                    var painting = new Paintings
                    {
                        Id = Guid.NewGuid(),
                        Heightid = dto.HeightId,
                        Widthid = dto.WidthId,
                        Categoryid = dto.CategoryId,
                        Name = dto.Name,
                        Imagelink = dto.ImageLink,
                        Price = dto.Price,
                        Sold = false
                    };

                    await _context.Paintings.AddAsync(painting);

                    await mongoCollection.InsertOneAsync(new ImageRecord
                    {
                        ImageId = painting.Id.ToString(),
                        FilePath = dto.FilePath ?? ""
                    });

                    count++;

                    // Batch save every 50 items
                    if (count % 50 == 0)
                    {
                        await _context.SaveChangesAsync();
                        Console.WriteLine($"Imported {count} items...");
                    }
                }

                await _context.SaveChangesAsync();

                return Ok($"✅ Imported {count} paintings");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"❌ Import failed: {ex.Message}");
            }
        }
    }
}