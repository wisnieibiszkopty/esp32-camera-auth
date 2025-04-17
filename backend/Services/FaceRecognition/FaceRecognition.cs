using FaceAiSharp;
using FaceAiSharp.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace backend.Services;

public class FaceRecognition : IFaceRecognition
{
    private readonly IFaceDetector detector;
    private readonly IFaceEmbeddingsGenerator recognizer;
    
    public FaceRecognition()
    {
        detector = FaceAiSharpBundleFactory.CreateFaceDetector();
        recognizer = FaceAiSharpBundleFactory.CreateFaceEmbeddingsGenerator();
    }
    
    public List<FaceDetectorResult> DetectFaces(Image<Rgb24> image)
    {
        return detector.DetectFaces(image).ToList();
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