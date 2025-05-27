using backend.Models;

namespace backend.Services.Storage;

public class LocalStorageService : IStorageService
{
    private readonly string basePath;

    public LocalStorageService(IConfiguration config)
    {
        basePath = config["Storage:BasePath"] ?? throw new ArgumentNullException("Storage:BasePath not set");

        if (!Directory.Exists(basePath))
            Directory.CreateDirectory(basePath);

        if (!Directory.Exists(basePath))
            Directory.CreateDirectory(basePath);
    }

    public async Task<string> UploadImageAsync(string directory, string fileName, Stream fileStream)
    {
        string dirPath = Path.Combine(basePath, directory);
        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);

        string filePath = Path.Combine(dirPath, fileName);
        
        using (var outputStream = File.Create(filePath))
        {
            await fileStream.CopyToAsync(outputStream);
        }
        
        return Path.GetFullPath(filePath);
    }

    public Task DeleteImageAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<FileData> SelectImageAsync(string fullPath)
    {
        if (!File.Exists(fullPath))
            throw new FileNotFoundException("File not found", fullPath);

        byte[] fileBytes = await File.ReadAllBytesAsync(fullPath);

        return new FileData
        {
            File = fileBytes,
            Name = Path.GetFileName(fullPath)
        };
    }

    public async Task<List<FileData>> SelectImagesAsync(string directory)
    {
        string dirPath = Path.Combine(basePath, directory);
        if (!Directory.Exists(dirPath))
            return new List<FileData>();

        var files = new List<FileData>();
        var filePaths = Directory.GetFiles(dirPath);

        foreach (var filePath in filePaths)
        {
            var fileBytes = await File.ReadAllBytesAsync(filePath);
            files.Add(new FileData
            {
                File = fileBytes,
                Name = Path.GetFileName(filePath)
            });
        }

        return files;
    }
}