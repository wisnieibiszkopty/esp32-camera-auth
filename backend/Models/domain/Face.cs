using FaceAiSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace backend.Models;

// FaceData with loaded Image
public class Face
{
    public string? PersonName { get; set; }
    public Image<Rgb24> Image { get; set; }
    public FaceDetectorResult DetectionData { get; set; }
}