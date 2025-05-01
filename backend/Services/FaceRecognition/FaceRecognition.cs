using backend.Models;
using FaceAiSharp;
using FaceAiSharp.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace backend.Services;

public class FaceRecognition : IFaceRecognition
{
    // first two variables are redundant
    private static readonly float FirstDetectionThreshold = 0.42f;
    private static readonly float SecondDetectionThreshold = 0.28f;
    private static readonly float RecognitionThreshold = 0.42f;
    
    private readonly IFaceDetector detector;
    private readonly IFaceEmbeddingsGenerator recognizer;
    
    public FaceRecognition()
    {
        detector = FaceAiSharpBundleFactory.CreateFaceDetector();
        recognizer = FaceAiSharpBundleFactory.CreateFaceEmbeddingsGenerator();
    }

    private float[] GenerateEmbedding(Image<Rgb24> image, FaceDetectorResult face)
    {
        recognizer.AlignFaceUsingLandmarks(image, face.Landmarks!);
        return recognizer.GenerateEmbedding(image);
    }
    
    public Result<float[]> DetectFaces(Image<Rgb24> image)
    {
        var faces = detector.DetectFaces(image).ToList();
        if (faces.Count == 0)
        {
            // failed no face on image
            return Result<float[]>.Failure(nameof(DetectionResult.None));
        }
        
        if (faces.Count > 1)
        {
            // failed only 1 face can be on image
            return Result<float[]>.Failure(nameof(DetectionResult.Multiple));
        }

        var embedding = GenerateEmbedding(image, faces.First());
        return Result<float[]>.Success(embedding);
    }

    public float CompareFaces(float[] face1, float[] face2)
    {
        return face1.Dot(face2);
    }
    
    // returns name of recognized person
    public Result<string> CompareMultipleFaces(List<FaceData> faces, Image<Rgb24> imageToCompare)
    {
        var faceToCompare = DetectFaces(imageToCompare);

        if (faceToCompare.IsFailure)
        {
            return Result<string>.Failure(faceToCompare.Error);
        }
        
        foreach (var face in faces)
        {
            var dot = CompareFaces(face.Embedding, faceToCompare.Value);
            Console.WriteLine(dot);
            if (dot > RecognitionThreshold)
            {
                return Result<string>.Success(face.Person);
            }
            
        }

        return Result<string>.Failure(nameof(DetectionResult.Invalid));
    }
}