using backend.Models;
using MongoDB.Driver;

namespace backend.Data.Repositories;

public class FaceRepository : RepositoryBase<FaceData>, IFaceRepository
{

    public FaceRepository(DbContext context) : base(context)
    {
    }

    public async Task<FaceData?> GetByPerson(string person)
    {
        return await collection
            .Find(Builders<FaceData>.Filter.Eq(f => f.Person, person))
            .FirstOrDefaultAsync();
    }

    public async Task<bool> DeleteByPersonName(string name)
    {
        var filter = Builders<FaceData>.Filter.Eq(f => f.Person, name);
        var result = await collection.DeleteOneAsync(filter);

        return result.IsAcknowledged && result.DeletedCount == 1;
    }

    public async Task<long> GetCount()
    {
        return await collection.CountDocumentsAsync(FilterDefinition<FaceData>.Empty);
    }
}