using backend.Models;
using backend.Models.Dto;

namespace backend.Services.Logging;

public interface ILoggingService
{
    public Task Log(DetectionResult detectionResult, FaceVerificationRequest request, string? personName = null);
}