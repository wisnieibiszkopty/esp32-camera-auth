using backend.Models;

namespace backend.Data.Repositories;

public interface ILogRepository
{
    public Task<Log> GetByIdAsync(string id);
    public Task<List<Log>> GetAllAsync();
    public Task<PagedResult<Log>> GetPagesAsync(int page, int size);
    public Task<List<Log>> GetByDateAsync(DateTime? startDate, DateTime? endDate);

    public Task SaveAsync(Log log);

    public Task<bool> DeleteAllAsync();
    public Task<bool> DeleteByIdAsync(string id);
    public Task<bool> DeleteBeforeAsync(DateTime date);
}