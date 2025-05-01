using backend.Services;

namespace backend.Models.Dto;

public class LogsMetrics
{
    public Dictionary<DetectionResult, List<DateCount>> Metrics { get; set; }
    public Dictionary<DetectionResult, List<DateCount>> Last24ByHour { get; set; }
}