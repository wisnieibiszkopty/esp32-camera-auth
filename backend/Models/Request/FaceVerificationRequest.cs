namespace backend.Models.Dto;

public class FaceVerificationRequest
{
    public string DeviceId { get; set; }
    public DateTime Timestamp { get; set; }
    public string ImageBase64 { get; set; }
}