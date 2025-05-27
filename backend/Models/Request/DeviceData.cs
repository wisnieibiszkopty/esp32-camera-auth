using Newtonsoft.Json;

namespace backend.Models.Dto;

public class DeviceData
{
    public string DeviceId { get; set; }
    public long Timestamp { get; set; }
}