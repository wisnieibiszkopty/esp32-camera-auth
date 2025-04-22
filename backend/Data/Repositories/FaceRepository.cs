using backend.Models;
using MongoDB.Driver;

namespace backend.Data.Repositories;

public class FaceRepository : IFaceRepository
{
    private IMongoCollection<FaceData> collection;

    public FaceRepository(DbContext context)
    {
        collection = context.Faces;
    }

    public async Task SaveAsync(FaceData face)
    {
        await collection.InsertOneAsync(face);
    }

    public async Task<List<FaceData>> GetAll()
    {
        return await collection
            .Find(_ => true)
            .ToListAsync();
    }

    public async Task<FaceData> GetById(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<long> GetCount()
    {
        return await collection.CountDocumentsAsync(FilterDefinition<FaceData>.Empty);
    }
}