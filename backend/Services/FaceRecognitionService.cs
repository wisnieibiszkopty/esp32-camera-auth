using FaceAiSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace backend.Services;

public class FaceRecognitionService : IFaceRecognitionService
{
    public FaceRecognitionService()
    {
 
    }

    public List<FaceDetectorResult> FindFaces(Image<Rgb24> image)
    {
        throw new NotImplementedException();
    }

    public bool CompareFaces(FaceDetectorResult face1, FaceDetectorResult face2)
    {
        throw new NotImplementedException();
    }

    public bool CompareMultipleFaces(List<FaceDetectorResult> faces, FaceDetectorResult face)
    {
        throw new NotImplementedException();
    }
}