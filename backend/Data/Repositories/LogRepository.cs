using backend.Models;
using backend.Models.Dto;
using backend.Models.Enums;
using backend.Services;
using MongoDB.Driver;

namespace backend.Data.Repositories;

public class LogRepository : RepositoryBase<Log>, ILogRepository
{
    
    public LogRepository(DbContext context): base(context)
    {
    }

    private Dictionary<DetectionResult, List<DateCount>> GetMetricsFromLast24Hours(List<Log> logs)
    {
        var since24h = DateTime.UtcNow.AddHours(-24);
        var logs24h = logs.Where(log => log.Timestamp >= since24h).ToList();
        
        var logsGroupedByHour = logs24h
            .GroupBy(log => new
            {
                Hour = new DateTime(
                    log.Timestamp.Year,
                    log.Timestamp.Month,
                    log.Timestamp.Day,
                    log.Timestamp.Hour,
                    0,
                    0,
                    DateTimeKind.Utc
                ),
                log.LogType
            })
            .Select(g => new
            {
                Hour = g.Key.Hour,
                LogType = g.Key.LogType,
                Count = g.Count()
            })
            .OrderBy(x => x.Hour)
            .ToList();
        
        var logsByTypeAndHour = logsGroupedByHour
            .GroupBy(x => x.LogType)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => new DateCount
                {
                    Date = x.Hour,
                    Count = x.Count
                }).ToList()
            );

        return logsByTypeAndHour;
    }

    private Dictionary<DetectionResult, List<DateCount>> GetMetricsByPeriod(List<Log> logs, Period? period)
    {
        DateTime? fromDate = period switch
        {
            Period.Week => DateTime.Today.AddDays(-7),
            Period.Month => DateTime.Today.AddMonths(-1),
            _ => null // all
        };
        
        if (fromDate.HasValue)
        {
            logs = logs.Where(log => log.Timestamp.Date >= fromDate.Value).ToList();
        }
        
        var groupedLogs = logs
                .GroupBy(log => new { log.Timestamp.Date, log.LogType })
                .Select(g => new
                {
                    Date = g.Key.Date,
                    LogType = g.Key.LogType,
                    Count = g.Count()
                })
                .OrderBy(g => g.Date)
                .ToList()
            ;
        
        return groupedLogs
            .GroupBy(x => x.LogType)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => new DateCount
                {
                    Count = x.Count,
                    Date = x.Date
                }).ToList()
            );
    }
    
    public async Task<LogsMetrics> GetMetrics(Period? period = Period.All)
    {
        var logs = await collection
            .Find(_ => true)
            .ToListAsync();
        
        Console.WriteLine(string.Join(",", logs.Select(l => l.LogType)));
        
        return new LogsMetrics
        {
            Metrics = GetMetricsByPeriod(logs, period),
            Last24ByHour = GetMetricsFromLast24Hours(logs)
        };
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

    public async Task<PagedResult<Log>> GetPagesAsync(
        int page = 1,
        int size = 10,
        DetectionResult? result = null,
        string? personName = null,
        DateTime? startDate = null, 
        DateTime? endDate = null)
    {
        var filter = Builders<Log>.Filter.Where(log => true);

        if (result.HasValue)
        {
            filter &= Builders<Log>.Filter.Eq(log => log.LogType, result.Value);
        }

        if (personName != null)
        {
            filter &= Builders<Log>.Filter.Eq(log => log.PersonName, personName);
        }
        
        if (startDate.HasValue)
        {
            filter &= Builders<Log>.Filter.Gte(log => log.Timestamp, startDate.Value);
        }

        if (endDate.HasValue)
        {
            filter &= Builders<Log>.Filter.Lte(log => log.Timestamp, endDate.Value);
        }
        
        var logs = await collection
            .Find(filter)
            .Skip((page - 1) * size)
            .Limit(size)
            .ToListAsync();

        var total = await collection.CountDocumentsAsync(filter);

        return new PagedResult<Log>
        {
            Items = logs,
            TotalCount = (int)total,
            Page = page,
            PageSize = size
        };
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