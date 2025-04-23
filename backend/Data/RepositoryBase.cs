using MongoDB.Driver;

namespace backend.Data;

public abstract class RepositoryBase<T> where T : class
{
    protected readonly IMongoCollection<T> collection;

    public RepositoryBase(DbContext context)
    {
        collection = context.GetCollection<T>();
    }
    
    public async Task SaveAsync(T entity)
    {
        await collection.InsertOneAsync(entity);
    }

    public async Task<List<T>> GetAll()
    {
        return await collection.Find(_ => true).ToListAsync();
    }

    public async Task<T> GetById(string id)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        return await collection.Find(filter).FirstOrDefaultAsync();
    }
}