using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class ImageRecord
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string? ImageId { get; set; }
    public string? FilePath { get; set; }
}