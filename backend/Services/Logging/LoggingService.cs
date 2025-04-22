using backend.Data.Repositories;
using backend.Services.Logging;

namespace backend.Services;

public class LoggingService: ILoggingService
{
    private readonly ILogRepository logRepository;
    
    public LoggingService(ILogRepository logRepository)
    {
        this.logRepository = logRepository;
    }

    public async Task Log()
    {
        throw new NotImplementedException();
    }
}