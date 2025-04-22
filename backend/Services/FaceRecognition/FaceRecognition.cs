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
    
    public Result<FaceDetectorResult> DetectFaces(Image<Rgb24> image)
    {
        var faces = detector.DetectFaces(image).ToList();
        if (faces.Count == 0)
        {
            // failed no face on image
            return Result<FaceDetectorResult>.Failure("Didn't recognized any face on image");
        }
        
        if (faces.Count > 1)
        {
            // failed only 1 face can be on image
            return Result<FaceDetectorResult>.Failure("Cannot add face, because multiple were found");
        }

        return Result<FaceDetectorResult>.Success(faces.First());
    }

    public float CompareFaces(FaceData face1, FaceData face2)
    {
        recognizer.AlignFaceUsingLandmarks(face1.Face, face1.DetectorResult.Landmarks!);
        recognizer.AlignFaceUsingLandmarks(face2.Face, face2.DetectorResult.Landmarks!);
        var embedding1 = recognizer.GenerateEmbedding(face1.Face);
        var embedding2 = recognizer.GenerateEmbedding(face2.Face);
        var dot = embedding1.Dot(embedding2);
        
        Console.WriteLine($"Dot product: {dot}");
        if (dot >= FirstDetectionThreshold)
        {
            Console.WriteLine("Assessment: Both pictures show the same person.");
        }
        else if (dot > SecondDetectionThreshold && dot < FirstDetectionThreshold)
        {
            Console.WriteLine("Assessment: Hard to tell if the pictures show the same person.");
        }
        else if (dot <= SecondDetectionThreshold)
        {
            Console.WriteLine("Assessment: These are two different people.");
        }

        return dot;
    }
    
    // returns name of recognized person
    public Result<string> CompareMultipleFaces(List<FaceData> faces, Image<Rgb24> imageToCompare)
    {
        var result = DetectFaces(imageToCompare);

        if (result.IsFailure)
        {
            return Result<string>.Failure("There is no faces on uploaded image");
        }
        
        var faceToCompare = new FaceData
        {
            DetectorResult = result.Value,
            Face = imageToCompare
        };

        foreach (var face in faces)
        {
            var dot = CompareFaces(face, faceToCompare);
            if (dot > RecognitionThreshold)
            {
                return Result<string>.Success(face.Person);
            }
        }

        return Result<string>.Failure("Face doesn't match ones in db");
    }
}