namespace Dtos;

public class CreatePaintingDto
{
    public int? HeightId { get; set; }
    public int? WidthId { get; set; }
    public int? CategoryId { get; set; }

    public string? Name { get; set; }
    public string? ImageLink { get; set; }
    public float? Price { get; set; }

    // Mongo-related
    public string? FilePath { get; set; }
}