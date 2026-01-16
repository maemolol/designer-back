using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class ImageRecord
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string ImageId { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
}