using Discord;
using Discord.WebSocket;

namespace backend.Services;

public class BotService
{
    private readonly DiscordSocketClient client;
    private readonly string botToken;
    private readonly ulong channelId;
    private readonly ulong faceChannelid;

    public BotService(IConfiguration config)
    {
        botToken = config["Discord:Token"]!;
        channelId = ulong.Parse(config["Discord:ChannelId"]!);
        faceChannelid = ulong.Parse(config["Discord:FaceChannelId"]!); 
        client = new DiscordSocketClient();
    }

    public async Task StartAsync()
    {
        client.Log += msg =>
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        };

        client.MessageReceived += OnMessageReceivedAsync; 
        
        await client.LoginAsync(TokenType.Bot, botToken);
        await client.StartAsync();
        await Task.Delay(5000);
    }
    
    public async Task SendMessageAsync(string message)
    {
        var channel = client.GetChannel(channelId) as IMessageChannel;
        if (channel != null)
        {
            await channel.SendMessageAsync(message);
        }
    }

    private async Task OnMessageReceivedAsync(SocketMessage message)
    {
        if (message.Author.IsBot) return;
        // maybe change this if new event will arrive
        if (message.Channel.Id != faceChannelid) return;

        await SendMessageAsync("Image uploaded  📸");
    }
    
    private async Task DownloadImageAsync(string url, string filename)
    {
        
    }
}