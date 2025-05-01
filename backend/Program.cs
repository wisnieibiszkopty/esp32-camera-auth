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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// FaceRecognition probably shouldn't be service
//builder.Services.AddScoped<IFaceRecognition, FaceRecognition>();
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

foreach (var key in app.Configuration.AsEnumerable())
{
    Console.WriteLine($"Key: {key.Key}, Value: {key.Value}");
}

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