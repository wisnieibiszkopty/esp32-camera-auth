using System.Text;
using MQTTnet;
using MQTTnet.Protocol;
using Newtonsoft.Json;

namespace backend.RabbitMQ;

public class MqttService : BackgroundService
{
    private readonly string topicName = "test";
    
    private readonly IMqttClient client;
    private readonly MqttClientOptions options;
    private readonly ILogger<MqttService> logger;
    private readonly MqttController mqttController;

    public MqttService(IConfiguration config, ILogger<MqttService> logger, MqttController mqttController)
    {
        try
        {
            this.mqttController = mqttController;
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

    // private async Task ProcessMessage(string message)
    // {
    //     try
    //     {
    //         var data = JsonConvert.DeserializeObject<RabbitData>(message);
    //         logger.LogInformation($"Received data: {data.Id} - {data.Message}");
    //     }
    //     catch (Exception ex)
    //     {
    //         logger.LogError("Error occured: " + ex);
    //     }
    // }
    
    private async Task ProcessMessage(string topic, string message)
    {
        try
        {
            var method = mqttController.GetType()
                .GetMethods()
                .FirstOrDefault(m => m.GetCustomAttributes(typeof(MqttMethodAttribute), inherit: false)
                .Cast<MqttMethodAttribute>()
                .Any(a => a.Topic == topic));
    
            if (method != null)
            {
                var parameters = method.GetParameters();
                if (parameters.Length == 1)
                {
                    var parameterType = parameters[0].ParameterType;
                    try
                    {
                        var deserialized = JsonConvert.DeserializeObject(message, parameterType);
                        method.Invoke(mqttController, new object[] { deserialized });
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Error mapping message to method parameter: {ex}");
                    }
                }
                else
                {
                    logger.LogWarning($"Invalid parameter count for method handling topic: {topic}");
                }
            }
            else
            {
                logger.LogWarning($"No handler found for topic: {topic}");
            }
        }
        catch (Exception ex)
        {
            logger.LogError("Error processing message: " + ex);
        }
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await client.ConnectAsync(options, stoppingToken);
            logger.LogInformation("Connected to MQTT Broker");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.StackTrace);
            logger.LogError($"Failed to connect to MQTT broker: {ex.Message}");
            Environment.Exit(1);
            return;
        }

        var topics = mqttController.GetType()
            .GetMethods()
            .SelectMany(m => m.GetCustomAttributes(typeof(MqttMethodAttribute), inherit: false))
            .Cast<MqttMethodAttribute>()
            .Select(a => a.Topic)
            .Distinct()
            .Select(topic => new MqttTopicFilterBuilder()
                .WithTopic(topic)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build())
            .ToList();

        var subscriptionOptions = new MqttClientSubscribeOptions
        {
            TopicFilters = topics 
        };

        logger.LogInformation(string.Join(", ", subscriptionOptions.TopicFilters.Select(t=> t.Topic)));
        
        await client.SubscribeAsync(subscriptionOptions, stoppingToken);
        
        logger.LogInformation($"Subscribed to topics: {string.Join(", ", topics.Select(t => t.Topic))}");
        
        client.ApplicationMessageReceivedAsync += async e =>
        {
            logger.LogInformation($"Message received!");
            var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            await ProcessMessage(topicName, message);
        };
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
        
        await client.DisconnectAsync();
        logger.LogInformation("Disconnected with broker");
    }

    public async Task SendMessageAsync(string topic, string message)
    {
        try
        {
            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)               
                .WithPayload(Encoding.UTF8.GetBytes(message))  
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtMostOnce)
                .WithRetainFlag(false)              
                .Build();
            
            await client.PublishAsync(mqttMessage);
            logger.LogInformation($"Message sent: {message}");
        }
        catch (Exception ex)
        {
            logger.LogError($"Error sending message: {ex.Message}");
        }
    }
}