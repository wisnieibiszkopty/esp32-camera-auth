using backend.RabbitMQ;
using backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<BotService>();

builder.Services.AddScoped<IFaceRecognitionService, FaceRecognitionService>();
builder.Services.AddScoped<IStorageService, AzureStorageService>();

// builder.Services.AddHostedService<MqttService>();

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