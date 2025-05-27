using Newtonsoft.Json;

namespace backend.Models.Dto;

public class DeviceData
{
    public string DeviceId { get; set; }
    [JsonProperty("timestamp")]
    public long TimestampUnix { get; set; }
}