using backend.Data.Repositories;
using backend.Models;
using backend.Models.Dto;
using backend.Models.events;
using backend.Services.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace backend.Services;

public class FaceAuthService
{
    private readonly string facesDirectory = "faces";
    
    private readonly IStorageService storageService;
    private readonly IFaceRepository faceRepository;
    private readonly ILoggingService loggingService;
    private readonly SecuritySettingsService settingsService;
    
    public FaceAuthService(
        IStorageService storageService,
        IFaceRepository faceRepository,
        ILoggingService loggingService,
        SecuritySettingsService settingsService)
    {
        this.storageService = storageService;
        this.faceRepository = faceRepository;
        this.loggingService = loggingService;
        this.settingsService = settingsService;
    }
    
    public async Task<Result<string>> RegisterFace(string personName, Stream rawStream, string fileExtension)
    {
        // check if new face can be registered
        var settings = settingsService.GetSettings();
        if (await faceRepository.GetCount() == settings.MaxRecognizableFaces)
        {
            return Result<string>.Failure("Achieved limit of registered faces");
        }

        using var imageStream = new MemoryStream();
        await rawStream.CopyToAsync(imageStream);
        imageStream.Position = 0;

        var image = Image.Load<Rgb24>(imageStream);
        var faceRecognition = new FaceRecognition();
        var result = faceRecognition.DetectFaces(image);
        
        if (result.IsFailure)
        {
            return Result<string>.Failure(result.Error);
        }

        // todo handle error
        await faceRepository.SaveAsync(new FaceData
        {
            Person = personName,
            Embedding = result.Value
        });
        
        return Result<string>.Success("Face registered!");
    }

    public async Task<Result<string>> UnregisterFace(string personName)
    {
        var deleted = await faceRepository.DeleteByPersonName(personName);
        if (deleted)
        {
            return Result<string>.Success($"Delete {personName} data");
        }

        return Result<string>.Failure("Cannot delete face");
    }
    
    public async Task<Result<string>> VerifyFace(FaceVerificationRequest request)
    {
        try
        {
            // todo check if verification isn't blocked

            byte[] imageBytes = Convert.FromBase64String(request.ImageBase64);
            using var imageStream = new MemoryStream(imageBytes);
            var image = Image.Load<Rgb24>(imageStream);
            
            var faces = await faceRepository.GetAll();
            
            var faceRecognition = new FaceRecognition();
            var result = faceRecognition.CompareMultipleFaces(faces, image);
            
            if (result.IsFailure)
            {
                DetectionResult detectionResult;
                if (result.Error == nameof(DetectionResult.Multiple))
                {
                    detectionResult = DetectionResult.Multiple;
                }
                else
                {
                    detectionResult = DetectionResult.None;
                }
                
                Task.Run(() => loggingService.Log(detectionResult, request));
                return Result<string>.Failure(result.Error);
            }
            
            Task.Run(() => loggingService.Log(DetectionResult.Detected, request, result.Value));
            return Result<string>.Success(result.Value);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            return Result<string>.Failure(e.Message);
        }
    }
}