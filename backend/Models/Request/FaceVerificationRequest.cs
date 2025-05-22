using Newtonsoft.Json;

namespace backend.Models.Dto;

public class FaceVerificationRequest
{
    public string DeviceId { get; set; }
    [JsonProperty("timestamp")]
    public long TimestampUnix { get; set; }
    [JsonIgnore]
    public DateTime Timestamp => DateTimeOffset.FromUnixTimeSeconds(TimestampUnix).UtcDateTime;
    public string ImageBase64 { get; set; }
}