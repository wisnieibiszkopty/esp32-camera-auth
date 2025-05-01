using backend.Data.Repositories;
using backend.Models;
using backend.Models.Dto;
using backend.Services.Logging;

namespace backend.Services;

public class LoggingService: ILoggingService
{
    private readonly ulong violationChannelId;
    private readonly ulong detectionFailedChannelId;
    private readonly ulong successChannelId;
    
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
        IConfiguration config,
        ILogRepository logRepository,
        IStorageService storageService,
        SecuritySettingsService securityService,
        BotService botService)
    {
        this.logRepository = logRepository;
        this.storageService = storageService;
        this.securityService = securityService;
        this.botService = botService;
        
        violationChannelId = ulong.Parse(config["Discord:Violation"]!);
        detectionFailedChannelId = ulong.Parse(config["Discord:Detection-Failed"]!);
        successChannelId = ulong.Parse(config["Discord:Success"]!);
    }
    
    // when face don't match ones in db, instead of getting Invalid i get None
    private async Task LogToDiscord(DetectionResult detectionResult, DateTime time, string deviceName, Stream image, string? personName = null)
    {
        (ulong channelId, string messageInfo) = detectionResult switch
        {
            DetectionResult.None => (detectionFailedChannelId, "Face wasn't detected"),
            DetectionResult.Multiple => (detectionFailedChannelId, "Multiple faces detected"),
            DetectionResult.Detected => (successChannelId, $"Detected person with name: {personName}"),
            DetectionResult.Invalid => (violationChannelId, "Detected unknown face"),
            _ => (detectionFailedChannelId, "Unknown error occured")
        };

        var message = messageInfo;

        if (detectionResult == DetectionResult.Invalid)
        {
            // getting random comment from pool
            var comments = securityService.GetSettings().CommentPool;

            if (comments == null || comments.Count == 0)
            {
                throw new InvalidOperationException("Comment pool is empty.");
            }
        
            var random = new Random();
            int index = random.Next(comments.Count);
            var randomComment = comments[index];

            message += $"\n{randomComment}";
        }
        
        Console.WriteLine(message);
        
        await botService.SendMessageWithFileAsync(
            channelId,
            $"{deviceName} | {time}",
            message,
            image,
            $"{Guid.NewGuid()}-{time}.jpg"
        );
    }

    public async Task Log(DetectionResult detectionResult, FaceVerificationRequest request, string? personName = null)
    {
        var securityLevel = securityService.GetSettings().SecurityLevel;
        bool startLogging = securityLevel == SecurityLevel.Always ||
                            (securityLevel == SecurityLevel.Violation && detectionResult == DetectionResult.Invalid);
        
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
        
        var log = new Log
        {
            DeviceName = request.DeviceId,
            Timestamp = request.Timestamp,
            LogType = detectionResult,
            PhotoPath = url,
            PersonName = detectionResult == DetectionResult.Detected && personName != null ? personName : null
        };
        
        await logRepository.SaveAsync(log);
        
        if (securityService.GetSettings().SendLogsToDiscord)
        {
            imageStream.Position = 0;
            try
            {
                await LogToDiscord(detectionResult, request.Timestamp, request.DeviceId, imageStream, personName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Message);
            }
        }
    }
}