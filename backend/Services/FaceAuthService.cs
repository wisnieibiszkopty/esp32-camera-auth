using backend.Models;
using backend.Models.events;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace backend.Services;

public class FaceAuthService
{
    private readonly string facesDirectory = "faces";
    
    private readonly IStorageService storageService;
    private readonly SecuritySettingsService settingsService;

    public event EventHandler<FaceRecognisedEventArgs>? FaceRecognised; 
    
    public FaceAuthService(IStorageService storageService, SecuritySettingsService settingsService)
    {
        this.storageService = storageService;
        this.settingsService = settingsService;
    }
    
    public async Task<Result<string>> RegisterFace(string personName, Stream rawStream, string fileExtension)
    {
        // check if new face can be registered
        var settings = settingsService.GetSettings();
        if (settings.Faces.Count == settings.MaxRecognizableFaces)
        {
            // failed
            return Result<string>.Failure("Achieved limit of registered faces");
        }

        using var imageStream = new MemoryStream();
        await rawStream.CopyToAsync(imageStream);
        imageStream.Position = 0;

        var image = Image.Load<Rgb24>(imageStream);
        var faceRecognition = new FaceRecognition();
        var faces = faceRecognition.DetectFaces(image);

        if (faces.Count == 0)
        {
            // failed no face on image
            return Result<string>.Failure("Didn't recognized any face on image");
        }

        if (faces.Count > 1)
        {
            // failed only 1 face can be on image
            return Result<string>.Failure("Cannot add face, because multiple were found");
        }

        imageStream.Position = 0;
        string url = await storageService.UploadImageAsync(
            facesDirectory,
            Guid.NewGuid() + fileExtension,
            imageStream
        );

        // todo handle error
        await settingsService.AddFace(new ImageData
        {
            Name = personName,
            Url = url
        });
        
        return Result<string>.Success(url);
    }

    public void UnregisterFace()
    {
        
    }

    public void VerifyFace()
    {
        // detect face on uploaded image
        // handle errors
        // load faces from azure
        // compare loaded face with ones from azure
        // if one of them is same return data about face
        // handle logs
    }
}