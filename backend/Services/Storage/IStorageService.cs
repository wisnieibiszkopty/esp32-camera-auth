using backend.Models;

namespace backend.Services;

public interface IStorageService
{
    Task<string> UploadImageAsync(string directory, string fileName, Stream fileStream);
    Task DeleteImageAsync();
    Task SelectImageAsync(string path);
    Task<List<FileData>> SelectImagesAsync(string directory);
    
    // TODO Move these methods to another class
    // void RemoveRecognizableFace();
    //
    // void UploadIncidentPhoto();
    // void SelectIncidentPhotos();
}