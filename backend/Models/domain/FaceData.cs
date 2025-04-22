using FaceAiSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace backend.Models;

public class FaceData
{
    public string Person { get; set; }
    public Image<Rgb24> Face { get; set; }
    public FaceDetectorResult DetectorResult { get; set; }
}