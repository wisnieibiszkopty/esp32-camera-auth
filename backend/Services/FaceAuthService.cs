using backend.Data.Repositories;
using backend.Models;
using backend.Models.Dto;
using backend.Services.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace backend.Services;

public class FaceAuthService
{
    private readonly string facesDirectory = "faces";
    
    private readonly IFaceRepository faceRepository;
    private readonly ILoggingService loggingService;
    private readonly SecuritySettingsService settingsService;
    
    public FaceAuthService(
        IFaceRepository faceRepository,
        ILoggingService loggingService,
        SecuritySettingsService settingsService)
    {
        this.faceRepository = faceRepository;
        this.loggingService = loggingService;
        this.settingsService = settingsService;
    }

    public async Task<List<string>> GetFaces()
    {
        var faces = await faceRepository.GetAll();
        return faces.Select(f => f.Person).ToList();
    }
    
    // face was registered, however discord bot didn't send message to channel
    public async Task<Result<string>> RegisterFace(string personName, Stream rawStream, string fileExtension)
    {
        var settings = settingsService.GetSettings();
        if (await faceRepository.GetCount() == settings.MaxRecognizableFaces)
        {
            return Result<string>.Failure("Achieved limit of registered faces");
        }

        var face = await faceRepository.GetByPerson(personName);

        if (face != null)
        {
            return Result<string>.Failure("Person with that name already exists");
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
        // also delete from storage
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
            byte[] imageBytes = Convert.FromBase64String(request.ImageBase64);
            using var imageStream = new MemoryStream(imageBytes);
            var image = Image.Load<Rgb24>(imageStream);
            
            var faces = await faceRepository.GetAll();
            
            var faceRecognition = new FaceRecognition();
            var result = faceRecognition.CompareMultipleFaces(faces, image);
            
            if (result.IsFailure)
            {
                DetectionResult detectionResult = result.Error switch
                {
                    nameof(DetectionResult.Multiple) => DetectionResult.Multiple,
                    nameof(DetectionResult.None) => DetectionResult.None,
                    nameof(DetectionResult.Invalid) => DetectionResult.Invalid,
                    _ => DetectionResult.None
                };
                
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