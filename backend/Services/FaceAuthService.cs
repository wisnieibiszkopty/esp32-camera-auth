using backend.Data.Repositories;
using backend.Models;
using backend.Models.Dto;
using backend.Models.events;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace backend.Services;

public class FaceAuthService
{
    private readonly string facesDirectory = "faces";
    
    private readonly IStorageService storageService;
    private readonly IFaceRepository faceRepository;
    private readonly SecuritySettingsService settingsService;
    
    public FaceAuthService(
        IStorageService storageService,
        IFaceRepository faceRepository,
        SecuritySettingsService settingsService)
    {
        this.storageService = storageService;
        this.faceRepository = faceRepository;
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
        
        imageStream.Position = 0;
        string url = await storageService.UploadImageAsync(
            facesDirectory,
            Guid.NewGuid() + fileExtension,
            imageStream
        );

        // todo handle error
        await faceRepository.SaveAsync(new FaceData
        {
            Url = url,
            Person = personName,
            DetectorResult = result.Value
            // i don't want to store image data in db
        });
        
        return Result<string>.Success("Face registered!");
    }

    public void UnregisterFace()
    {
        
    }

    // I should add more complex response
    public async Task<Result<string>> VerifyFace(FaceVerificationRequest request)
    {
        try
        {
            // todo check if verification isn't blocked

            byte[] imageBytes = Convert.FromBase64String(request.ImageBase64);
            using var imageStream = new MemoryStream(imageBytes);
            var image = Image.Load<Rgb24>(imageStream);

            // load faces from azure
            //var registeredFacesData = settingsService.GetSettings().Faces;
            var registeredFacesData = await faceRepository.GetAll();
            
            Console.WriteLine(registeredFacesData.Count);

            var faces = new List<Face>();

            foreach (var face in registeredFacesData)
            {
                Console.WriteLine(face.Person);

                var storedFile = await storageService.SelectImageAsync(face.Url);
                
                faces.Add(new Face
                {
                    PersonName = face.Person,
                    DetectionData = face.DetectorResult,
                    Image = storedFile.AsImage()
                });
            }
            
            // compare loaded face with ones from azure
            var faceRecognition = new FaceRecognition();
            var result = faceRecognition.CompareMultipleFaces(faces, image);

            if (result.IsFailure)
            {
                return Result<string>.Failure(result.Error);
            }
            
            // if one of them is same return data about face
            // handle logs   
            
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