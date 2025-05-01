using System.Text.Json.Serialization;
using backend.Data;
using backend.Discord;
using backend.RabbitMQ;
using backend.Services;
using backend.Services.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddAzureAppConfiguration(options =>
{
    var endpoint = builder.Configuration["Config:Endpoint"];
    options.Connect(endpoint);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = "YourAppIssuer",
            ValidAudience = "YourAppAudience",
            IssuerSigningKey =
                new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
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
builder.Services.AddScoped<AdminAuthService>();
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

app.UseAuthentication();
app.UseAuthorization();

var discordService = app.Services.GetRequiredService<BotService>();
await discordService.StartAsync();

app.MapControllers();

app.Run();