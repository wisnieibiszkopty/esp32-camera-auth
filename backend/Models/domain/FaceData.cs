using FaceAiSharp;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace backend.Models;

public class FaceData
{
    [BsonId] 
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    public string Person { get; set; }
    public float[] Embedding { get; set; }
    //public string Url { get; set; }
    //public FaceDetectorResult DetectorResult { get; set; }
}