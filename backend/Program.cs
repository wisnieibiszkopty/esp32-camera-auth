using System.Text.Json.Serialization;
using backend.Data;
using backend.Discord;
using backend.RabbitMQ;
using backend.Services;
using backend.Services.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddAzureAppConfiguration(options =>
{
    var endpoint = builder.Configuration["Config:Endpoint"];
    options.Connect(endpoint);
});

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
    );
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IStorageService, AzureStorageService>();

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddRepositories();

builder.Services.AddScoped<SecuritySettingsService>();
builder.Services.AddScoped<ILoggingService, LoggingService>();
builder.Services.AddScoped<FaceAuthService>();

// rabbitmq-plugins enable rabbitmq_mqtt
//builder.Services.AddSingleton<MqttController>();
//builder.Services.AddHostedService<MqttService>();

builder.Services.AddSingleton<BotService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

var discordService = app.Services.GetRequiredService<BotService>();
await discordService.StartAsync();

app.MapControllers();

app.Run();