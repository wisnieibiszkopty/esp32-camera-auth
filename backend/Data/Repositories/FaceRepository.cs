using backend.Models;
using MongoDB.Driver;

namespace backend.Data.Repositories;

public class FaceRepository : RepositoryBase<FaceData>, IFaceRepository
{

    public FaceRepository(DbContext context) : base(context)
    {
    }

    public async Task<long> GetCount()
    {
        return await collection.CountDocumentsAsync(FilterDefinition<FaceData>.Empty);
    }
}