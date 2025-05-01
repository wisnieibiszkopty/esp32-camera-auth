using backend.Models;
using backend.Models.Dto;
using backend.Models.Enums;

namespace backend.Services.Logging;

public interface ILoggingService
{
    public Task Log(DetectionResult detectionResult, FaceVerificationRequest request, string? personName = null);

    public Task<LogsMetrics> GetMetrics(Period? period);
    public Task<PagedResult<Log>> GetLogs(int page, int size, DetectionResult? result, string? personName, DateTime? startDate, DateTime? endDate);
}