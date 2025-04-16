using backend.Data;
using backend.Discord;
using backend.RabbitMQ;
using backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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

// builder.Services.AddHostedService<MqttService>();

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