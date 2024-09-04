using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StarportBot.Models;

[Serializable]
public class LostUser
{
    [BsonId]
    public ObjectId Id { get; set; }
    
    public required ulong BannedUser { get; set; }
    public required ulong BanningAdmin { get; set; }
    public DateTime Time { get; set; } = DateTime.UtcNow;
}