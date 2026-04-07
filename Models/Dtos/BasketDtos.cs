namespace Dtos;

public class BasketRequest
{
    public string? Email { get; set; }
    public List<Guid> PaintingIds { get; set; } = [];
}