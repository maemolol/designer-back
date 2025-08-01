using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Controllers
{
    [ApiController]
    [Route("image")]

    public class ImageController : ControllerBase
    {
        private readonly IMongoCollection<ImageRecord> _imageCollection;

        public ImageController(IMongoDatabase mongoDatabase)
        {
            _imageCollection = mongoDatabase.GetCollection<ImageRecord>("images");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetImageById(string id)
        {
            var image = await _imageCollection.Find(x => x.ImageId == id).FirstOrDefaultAsync();
            if (image == null)
            {
                return NotFound("Image metadata not found.");
            }

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), image.FilePath);
            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound("Image file not found on disk.");
            }

            // Determine MIME type
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fullPath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(fullPath);
            return File(bytes, contentType);
        }
    }
}
