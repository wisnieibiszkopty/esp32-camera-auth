using backend.Models.Dto;

namespace backend.RabbitMQ;

public class MqttController
{
    private readonly ILogger<MqttController> _logger;

    public MqttController(ILogger<MqttController> logger)
    {
        _logger = logger;
    }

    [MqttMethod("verify-face", "Verify Face")]
    public void VerifyFace(FaceVerificationRequest request)
    {
        _logger.LogInformation("test");
    }
}