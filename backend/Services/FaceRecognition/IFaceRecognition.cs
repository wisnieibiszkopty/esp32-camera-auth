using backend.Models;
using FaceAiSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace backend.Services;

public interface IFaceRecognition
{
    public Result<FaceDetectorResult> DetectFaces(Image<Rgb24> image);
    public float CompareFaces(FaceData face1, FaceData face2);
    // instead of just returning bool, also return info about face which was recognize on image
    public Result<string> CompareMultipleFaces(List<FaceData> faces, Image<Rgb24> faceToCompare);
}