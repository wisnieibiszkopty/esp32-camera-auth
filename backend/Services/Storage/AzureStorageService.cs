using Azure.Storage.Blobs;
using backend.Models;

namespace backend.Services;

public class AzureStorageService : IStorageService
{
    private readonly string connectionString;

    private readonly BlobServiceClient client;
    
    public AzureStorageService(IConfiguration config)
    {
        connectionString = config["Azure:StorageConnection"]!;
        client = new BlobServiceClient(connectionString);
    }

    // saving it in public directory is not safe at all
    public async Task<string> UploadImageAsync(string directory, string fileName, Stream fileStream)
    {
        var blobContainerClient = client.GetBlobContainerClient(directory);

        await blobContainerClient.CreateIfNotExistsAsync();

        var blobClient = blobContainerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(fileStream, overwrite: true);
        
        return blobClient.Uri.ToString();
    }
    
    public Task DeleteImageAsync()
    {
        throw new NotImplementedException();
    }

    public Task SelectImageAsync(string path)
    {
        throw new NotImplementedException();
    }

    public async Task<List<FileData>> SelectImagesAsync(string directory)
    {
        // maybe move client to class field
        var containerClient = client.GetBlobContainerClient(directory);
        
        var files = new List<FileData>();
        await foreach (var blobItem in containerClient.GetBlobsAsync())
        {
            var blobClient = containerClient.GetBlobClient(blobItem.Name);
            using var stream = new MemoryStream();
            await blobClient.DownloadToAsync(stream);
            files.Add(new FileData
            {
                File = stream.ToArray(),
                Name = blobItem.Name
            });
        }

        return files;
    }
}