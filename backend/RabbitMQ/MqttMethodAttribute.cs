namespace backend.RabbitMQ;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public class MqttMethodAttribute : Attribute
{
    public string Topic { get; set; }
    public string MethodName { get; set; }

    public MqttMethodAttribute(string topic, string methodName)
    {
        Topic = topic;
        MethodName = methodName;
    }
}