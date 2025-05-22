using System.Text;
using backend.Models.Dto;
using backend.Services;
using MQTTnet;
using Newtonsoft.Json;

namespace backend.RabbitMQ;

public class MqttService : BackgroundService
{
    private readonly string topicName = "verify/face";
    private readonly string resultTopicName = "verify/result";
    
    private readonly IMqttClient client;
    private readonly MqttClientOptions options;
    
    private readonly ILogger<MqttService> logger;
    private readonly IServiceProvider serviceProvider;

    public MqttService(IConfiguration config, ILogger<MqttService> logger, IServiceProvider serviceProvider)
    {
        this.logger = logger;
        this.serviceProvider = serviceProvider;
        
        var factory = new MqttClientFactory();
        client = factory.CreateMqttClient();

        client.ApplicationMessageReceivedAsync += async e =>
        {
            logger.LogInformation("New event!");
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            logger.LogInformation(payload);
            await HandleEvent(payload);
        };
        
        options = new MqttClientOptionsBuilder()
            .WithTcpServer(config["Rabbit:Url"], int.Parse(config["Rabbit:Port"]))
            .WithCredentials(config["Rabbit:Login"], config["Rabbit:Password"])
            .WithCleanSession()
            .Build();
    }

    private async Task SendMessage(string topic, string message)
    {
        var mqttMessage = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(Encoding.UTF8.GetBytes(message))
            .WithRetainFlag(false)
            .Build();

        if (client.IsConnected)
        {
            await client.PublishAsync(mqttMessage);
        }
        else
        {
            throw new InvalidOperationException("MQTT client is not connected");
        }
    }
    
    // TODO abstract it to controller or something like that in future
    private async Task HandleEvent(string payload)
    {
        try
        {
            var faceData = JsonConvert.DeserializeObject<FaceVerificationRequest>(payload);
            if (faceData == null)
            {
                throw new InvalidOperationException("Cannot deserialize FaceVerificationRequest");
            }

            logger.LogInformation("git gud");
            await SendMessage(resultTopicName, "Git gut");

            using var scope = serviceProvider.CreateScope();
            var faceAuthService = scope.ServiceProvider.GetRequiredService<FaceAuthService>();
            
            var result = await faceAuthService.VerifyFace(faceData);
            if (result.IsFailure)
            {
                logger.LogInformation("Verification failure");
                await SendMessage(resultTopicName, "Failure");
            }
            else
            {
                logger.LogInformation("Verification succeeded");
                await SendMessage(resultTopicName, "Success");
            }

        }
        catch (Exception ex)
        {
            logger.LogError($"Serialization error {ex.Message}");
            await SendMessage(resultTopicName, "Error");
        }
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await client.ConnectAsync(options, stoppingToken);

        await client.SubscribeAsync(new MqttTopicFilterBuilder()
            .WithTopic(topicName)
            .WithAtLeastOnceQoS()
            .Build());
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
    
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        var disconnectOptions = new MqttClientDisconnectOptionsBuilder()
            .WithReason(MqttClientDisconnectOptionsReason.NormalDisconnection)
            .Build();
        
        await client.DisconnectAsync(disconnectOptions, cancellationToken);
    }
}