using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.Models;

public class Log
{
    [BsonId] 
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    public required string DeviceName { get; set; }
    public DateTime Timestamp { get; set; }
    public SecurityLevel LogType { get; set; }
    public required string PhotoPath { get; set; }
}