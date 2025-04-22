using backend.Data.Repositories;

namespace backend.Services;

public class LoggingService
{
    private readonly ILogRepository logRepository;
    
    public LoggingService(ILogRepository logRepository)
    {
        this.logRepository = logRepository;
    }
}