using backend.Models;
using FaceAiSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace backend.Services;

public interface IFaceRecognition
{
    public Result<float[]> DetectFaces(Image<Rgb24> image);
    public float CompareFaces(float[] face1, float[] face2);
    public Result<string> CompareMultipleFaces(List<FaceData> faces, Image<Rgb24> faceToCompare);
}