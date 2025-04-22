using backend.Models;
using FaceAiSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace backend.Services;

public interface IFaceRecognition
{
    public Result<FaceDetectorResult> DetectFaces(Image<Rgb24> image);
    public float CompareFaces(Face face1, Face face2);
    // instead of just returning bool, also return info about face which was recognize on image
    public Result<string> CompareMultipleFaces(List<Face> faces, Image<Rgb24> faceToCompare);
}