using backend.Models;
using MongoDB.Driver;

namespace backend.Data.Repositories;

public class LogRepository : ILogRepository
{
    private IMongoCollection<Log> collection;
    
    public LogRepository(DbContext context)
    {
        collection = context.Logs;
    }

    public async Task<Log> GetByIdAsync(string id)
    {
        return await collection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Log>> GetAllAsync()
    {
        return await collection
            .Find(_ => true)
            .ToListAsync();
    }

    public async Task<PagedResult<Log>> GetPagesAsync(int page, int size)
    {
        var logs = await collection
            .Find(_ => true)
            .Skip((page - 1) * size)
            .Limit(size)
            .ToListAsync();

        var total = await collection.CountDocumentsAsync(_ => true);

        return new PagedResult<Log>
        {
            Items = logs,
            TotalCount = (int)total,
            Page = page,
            PageSize = size
        };
    }

    public async Task<List<Log>> GetByDateAsync(DateTime? startDate, DateTime? endDate)
    {
        throw new NotImplementedException();
    }

    public async Task SaveAsync(Log log)
    {
        await collection.InsertOneAsync(log);
    }

    public async Task<bool> DeleteAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteBeforeAsync(DateTime date)
    {
        throw new NotImplementedException();
    }
}