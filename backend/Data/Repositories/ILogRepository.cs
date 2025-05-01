using backend.Models;
using backend.Models.Dto;
using backend.Models.Enums;
using backend.Services;

namespace backend.Data.Repositories;

public interface ILogRepository
{
    public Task<LogsMetrics> GetMetrics(Period? period = Period.All);
    public Task<Log> GetByIdAsync(string id);
    public Task<List<Log>> GetAllAsync();
    public Task<PagedResult<Log>> GetPagesAsync(
        int page = 1,
        int size = 10,
        DetectionResult? result = null,
        string? personName = null,
        DateTime? startDate = null, 
        DateTime? endDate = null);
    
    public Task SaveAsync(Log log);

    public Task<bool> DeleteAllAsync();
    public Task<bool> DeleteByIdAsync(string id);
    public Task<bool> DeleteBeforeAsync(DateTime date);
}