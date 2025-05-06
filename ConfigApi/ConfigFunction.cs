using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ConfigApi;

public class ConfigFunction
{
    private readonly ILogger<ConfigFunction> _logger;
    private readonly IConfiguration _config;

    public ConfigFunction(ILogger<ConfigFunction> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    [Function("ConfigFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
    {
        try
        {
            var connectionData = await req.ReadFromJsonAsync<ConnectionData>();
        
            if (connectionData == null)
            {
                return new BadRequestObjectResult("Missing ConnectionString from body");
            }

            if (!connectionData.ConnectionString.Equals(_config["Config:Endpoint"]))
            {
                return new BadRequestObjectResult("Invalid ConnectionString");
            }
        
            foreach (var key in _config.AsEnumerable())
            {
                Console.WriteLine($"Key: {key.Key}, Value: {key.Value}");
            }

            var jsonObject = new JsonObject
            {
                ["Rabbit:Login"] = _config["Rabbit:Login"],
                ["Rabbit:Password"] = _config["Rabbit:Password"],
                ["Rabbit:Port"] = _config["Rabbit:Port"],
                ["Rabbit:Url"] = _config["Rabbit:Url"]
            };
        
            return new JsonResult(jsonObject);    
        }
        
        catch (JsonException ex)
        {
            return new BadRequestObjectResult("Invalid JSON: The input does not contain valid JSON token.");
        }
    }

}