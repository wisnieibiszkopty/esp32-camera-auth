using backend.Data.Repositories;
using backend.Models;
using backend.Models.Dto;
using backend.Services.Logging;

namespace backend.Services;

public class LoggingService: ILoggingService
{
    private readonly ILogRepository logRepository;
    private readonly IStorageService storageService;
    private readonly SecuritySettingsService securityService;
    private readonly BotService botService;

    private readonly Dictionary<DetectionResult, string> containersMapping = new()
    {
        { DetectionResult.None, "logs-detection-failed"},
        { DetectionResult.Multiple, "logs-detection-failed"},
        { DetectionResult.Detected, "logs-success"},
        { DetectionResult.Invalid, "logs-violations"}
    };
    
    public LoggingService(
        ILogRepository logRepository,
        IStorageService storageService,
        SecuritySettingsService securityService,
        BotService botService)
    {
        this.logRepository = logRepository;
        this.storageService = storageService;
        this.securityService = securityService;
        this.botService = botService;
    }

    private async Task LogToDiscord()
    {
        //await botService.SendMessageWithFileAsync();
        // There are 3 channels on discord -> success, detection-failed and violation
        // message should be sent to proper channel based on detectionResult
    }
    
    // check logging level
    // save image in azure
    // save log in db
    // send alert to discord
    public async Task Log(DetectionResult detectionResult, FaceVerificationRequest request, string? personName = null)
    {
        var securityLevel = securityService.GetSettings().SecurityLevel;

        bool startLogging = securityLevel == SecurityLevel.Always ||
                            (securityLevel == SecurityLevel.Violation && detectionResult == DetectionResult.Invalid);

        Console.WriteLine($"Detection result: {detectionResult}");
        Console.WriteLine($"Security level: {securityLevel}");
        Console.WriteLine($"Start logging: {startLogging}");
        
        if (!startLogging)
        {
            return;
        }
        
        string fileName = $"{Guid.NewGuid()}.jpg";
        Console.WriteLine($"filename: ${fileName}");
        byte[] imageBytes = Convert.FromBase64String(request.ImageBase64);
        using var imageStream = new MemoryStream(imageBytes);
        imageStream.Position = 0;
        var url = await storageService.UploadImageAsync(containersMapping[detectionResult], fileName, imageStream);
        Console.WriteLine($"filename: ${fileName}, url: {url}");
        Console.WriteLine($"Person name: {personName}");
        //
        // var log = new Log
        // {
        //     DeviceName = request.DeviceId,
        //     Timestamp = request.Timestamp,
        //     LogType = detectionResult,
        //     PhotoPath = url,
        //     PersonName = detectionResult == DetectionResult.Detected && personName != null ? personName : null
        // };
        //
        // await logRepository.SaveAsync(log);
        //
        // if (securityService.GetSettings().SendLogsToDiscord)
        // {
        //     await LogToDiscord();   
        // }
    }
}