using FaceAiSharp;

namespace backend.Models;

public class Face
{
    public string PersonName { get; set; }
    public FaceDetectorResult DetectionData { get; set; }
}