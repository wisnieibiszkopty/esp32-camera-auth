using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.Models;

public class SecuritySettings
{
    [BsonId] 
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    public int MaxRecognizableFaces { get; set; } = 5;
    [BsonRepresentation(BsonType.String)]
    public SecurityLevel SecurityLevel { get; set; } = SecurityLevel.Violation;
    public int MaxViolationLimit { get; set; } = 3;
    // in seconds
    public int TimeBeforeUnlockAfterViolation { get; set; }
    
    public bool SendLogsToDiscord { get; set; } = true;
    public int CommentPoolSize { get; set; } = 5;
    public List<string> CommentPool { get; set; } = new List<string>();
}