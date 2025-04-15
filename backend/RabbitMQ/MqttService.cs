using System.Text;
using MQTTnet;
using Newtonsoft.Json;

namespace backend.RabbitMQ;

public class MqttService : BackgroundService
{
    private readonly string topicName = "test";
    
    private readonly IMqttClient client;
    private readonly MqttClientOptions options;
    private readonly ILogger<MqttService> logger;

    public MqttService(IConfiguration config, ILogger<MqttService> logger)
    {
        try
        {
            this.logger = logger;
            var factory = new MqttClientFactory();
            client = factory.CreateMqttClient();
            options = new MqttClientOptionsBuilder()
                .WithTcpServer(config["Rabbit:Url"], int.Parse(config["Rabbit:Port"]!))
                .WithCredentials(config["Rabbit:Login"], config["Rabbit:Password"])
                .WithCleanSession()
                .Build();
        }
        catch (Exception e)
        {
            Console.WriteLine("RabbitMQ configuration was not found " + e);
        }
    }

    private async Task ProcessMessage(string message)
    {
        try
        {
            var data = JsonConvert.DeserializeObject<RabbitData>(message);
            logger.LogInformation($"Received data: {data.Id} - {data.Message}");
        }
        catch (Exception ex)
        {
            logger.LogError("Error occured: " + ex);
        }
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await client.ConnectAsync(options, stoppingToken);
        logger.LogInformation("Connected to MQTT Broker");

        var topic = new MqttTopicFilterBuilder()
            .WithTopic(topicName)
            .Build();

        await client.SubscribeAsync(topic, stoppingToken);
        logger.LogInformation($"Subscribed to: {topicName}");

        client.ApplicationMessageReceivedAsync += async e =>
        {
            var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            await ProcessMessage(message);
        };
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
        
        await client.DisconnectAsync();
        logger.LogInformation("Disconnected with broker");
    }
}