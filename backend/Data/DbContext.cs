using backend.Models;
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

    public IMongoCollection<Settings> Settings => database.GetCollection<Settings>("Settings");
    public IMongoCollection<Log> Logs => database.GetCollection<Log>("Logs");

}