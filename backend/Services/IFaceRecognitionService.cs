using FaceAiSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace backend.Services;

public interface IFaceRecognitionService
{
    public List<FaceDetectorResult> FindFaces(Image<Rgb24> image);
    public bool CompareFaces(FaceDetectorResult face1, FaceDetectorResult face2);
    // instead of just returning bool, also return info about face which was recognize on image
    public bool CompareMultipleFaces(List<FaceDetectorResult> faces, FaceDetectorResult face);
}