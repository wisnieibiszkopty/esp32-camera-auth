using backend.Models;
using FaceAiSharp;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace backend.Data;

public class DbContext
{
    private readonly IMongoDatabase database;

    public DbContext(IMongoClient client, IOptions<DatabaseSettings> settings)
    {
        database = client.GetDatabase(settings.Value.Database);
    }

    public IMongoCollection<T> GetCollection<T>()
    {
        return database.GetCollection<T>(typeof(T).Name);
    }
    
    public IMongoCollection<SecuritySettings> SecuritySettings => database.GetCollection<SecuritySettings>("SecuritySettings");
    public IMongoCollection<FaceData> Faces => database.GetCollection<FaceData>("Faces");
    public IMongoCollection<Log> Logs => database.GetCollection<Log>("Logs");

}