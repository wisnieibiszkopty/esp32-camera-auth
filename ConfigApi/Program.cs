using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Configuration.AddAzureAppConfiguration(options =>
{
    var endpoint = builder.Configuration["Endpoint"];
    options.Connect(endpoint);
});

builder.ConfigureFunctionsWebApplication();
builder.Build().Run();