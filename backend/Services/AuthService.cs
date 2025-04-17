using backend.Models;
using backend.Models.events;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace backend.Services;

public class AuthService
{
    private readonly string facesDirectory = "faces";
    
    private readonly IStorageService storageService;
    private readonly SecuritySettingsService settingsService;

    public event EventHandler<FaceRecognisedEventArgs>? FaceRecognised; 
    
    public AuthService(IStorageService storageService, SecuritySettingsService settingsService)
    {
        this.storageService = storageService;
        this.settingsService = settingsService;
    }

    // check faces limit
    public async Task<Result<string>> RegisterFace(string personName, Stream rawStream)
    {
        try
        {
            // check if new face can be registered
            // I have to store current number of registered faces somewhere, so i won't have to count it every time

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
            string url = await storageService.UploadImageAsync(facesDirectory, personName, imageStream);
            // emit event
            // do i really need event?
            //OnFaceRegistered(personName, url);

            return Result<string>.Success("Face saved succesfully");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            return Result<string>.Failure("Chuj");
        }
        
    }

    protected virtual void OnFaceRegistered(string personName, string imagePath)
    {
        FaceRecognised?.Invoke(this, new FaceRecognisedEventArgs
        {
            PersonName = personName,
            ImagePath = imagePath
        });
    }

    public void UnregisterFace()
    {
        
    }

    private void GetRegisteredFaces()
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