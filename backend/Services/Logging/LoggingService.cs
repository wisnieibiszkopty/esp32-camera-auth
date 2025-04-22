using backend.Data.Repositories;
using backend.Services.Logging;

namespace backend.Services;

public class LoggingService: ILoggingService
{
    private readonly ILogRepository logRepository;
    private readonly SecuritySettingsService securityService;
    
    public LoggingService(ILogRepository logRepository, SecuritySettingsService securityService)
    {
        this.logRepository = logRepository;
        this.securityService = securityService;
    }

    private async Task LogToDiscord()
    {
        
    }
    
    // save image in azure
    // save log in db
    // send alert to discord
    // check logging level
    public async Task Log()
    {
        
        
        if (securityService.GetSettings().SendLogsToDiscord)
        {
            await LogToDiscord();
        }
    }
}