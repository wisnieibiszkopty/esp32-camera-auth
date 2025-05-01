using backend.Services;
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
    [BsonRepresentation(BsonType.String)]
    public DetectionResult LogType { get; set; }
    public required string PhotoPath { get; set; }
    
    private string? personName;
    public string? PersonName
    {
        get => personName;
        set
        {
            if (LogType != DetectionResult.Detected && value != null)
            {
                throw new InvalidOperationException("PersonName can only be set when LogType is Detected.");
            }
            personName = value;
        }
    }
}