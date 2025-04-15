namespace backend.Services;

public interface IStorageService
{
    Task UploadImageAsync(string fileName, Stream fileStream);
    void DeleteImageAsync();
    void SelectImageAsync();
    void SelectImagesAsync();
    
    // TODO Move these methods to another class
    // void UploadRecognizableFace();
    // void RemoveRecognizableFace();
    // void SelectRecognizableFaces();
    //
    // void UploadIncidentPhoto();
    // void SelectIncidentPhotos();
}