using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace backend.Services;

// TODO add some sort of routing for events
public class BotService
{
    private readonly DiscordSocketClient client;
    private readonly InteractionService interactionService;
    
    private readonly string botToken;
    private readonly ulong guildId;
    private readonly ulong channelId;
    private readonly ulong faceChannelid;
    
    private IServiceProvider? services;

    public BotService(IConfiguration config, IServiceProvider services)
    {
        botToken = config["Discord:Token"]!;
        guildId = ulong.Parse(config["Discord:GuildId"]!);
        channelId = ulong.Parse(config["Discord:ChannelId"]!);
        faceChannelid = ulong.Parse(config["Discord:FaceChannelId"]!);
        
        client = new DiscordSocketClient();
        interactionService = new InteractionService(client.Rest);
        this.services = services;
    }

    public async Task StartAsync()
    {
        client.Log += msg =>
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        };

        client.Ready += OnReadyAsync;
        client.InteractionCreated += HandleInteraction;
        client.MessageReceived += OnMessageReceivedAsync; 
        
        await client.LoginAsync(TokenType.Bot, botToken);
        await client.StartAsync();
        await Task.Delay(5000);
    }
    
    private async Task OnReadyAsync()
    {
        await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        await interactionService.RegisterCommandsToGuildAsync(guildId);
        Console.WriteLine("Slash commands registered.");
    }
    
    private async Task HandleInteraction(SocketInteraction interaction)
    {
        var context = new SocketInteractionContext(client, interaction);
        await interactionService.ExecuteCommandAsync(context, services);
    }
    
    public async Task SendMessageAsync(string message)
    {
        var channel = client.GetChannel(channelId) as IMessageChannel;
        if (channel != null)
        {
            await channel.SendMessageAsync(message);
        }
    }

    public async Task SendMessageWithFileAsync(ulong channelId, string title, string message, Stream stream, string fileName)
    {
        var channel = client.GetChannel(channelId) as IMessageChannel;
        if (channel != null)
        {
            var embed = new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(message)
                .WithImageUrl($"attachment://{fileName}")
                .Build();

            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            await channel.SendFileAsync(stream, fileName, embed: embed);
        }
    }

    private async Task OnMessageReceivedAsync(SocketMessage message)
    {
        if (message.Author.IsBot) return;
        // maybe change this if new event will arrive
        if (message.Channel.Id != faceChannelid) return;

        await SendMessageAsync("Image uploaded  📸");
    }
    
}