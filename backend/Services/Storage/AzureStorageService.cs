using Azure.Storage.Blobs;

namespace backend.Services;

public class AzureStorageService : IStorageService
{
    private readonly string connectionString;
    // I will add more containers later?
    private readonly string containerName = "faces";

    public AzureStorageService(IConfiguration config)
    {
        connectionString = config["Azure:StorageConnection"]!;
    }

    // saving it in public directory is not safe at all
    public async Task<string> UploadImageAsync(string directory, string fileName, Stream fileStream)
    {
        var blobServiceClient = new BlobServiceClient(connectionString);
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(directory);

        await blobContainerClient.CreateIfNotExistsAsync();

        var blobClient = blobContainerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(fileStream, overwrite: true);
        return blobClient.Uri.ToString();
    }
    
    public void DeleteImageAsync()
    {
        throw new NotImplementedException();
    }

    public void SelectImageAsync()
    {
        throw new NotImplementedException();
    }

    public void SelectImagesAsync()
    {
        throw new NotImplementedException();
    }
}