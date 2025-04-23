using System.Text;
using backend.Discord.Models;
using backend.Models;
using backend.Models.events;
using backend.Services;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace backend.Discord;

public class DiscordInteractions : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ulong faceChannelId;

    private readonly ILogger<DiscordInteractions> logger;
    private readonly SecuritySettingsService settingsService;
    private readonly FaceAuthService faceAuthService;
    
    public DiscordInteractions(
        IConfiguration config,
        ILogger<DiscordInteractions> logger,
        SecuritySettingsService settingsService,
        FaceAuthService faceAuthService)
    {
        faceChannelId = ulong.Parse(config["Discord:ChannelId"]!);
        this.logger = logger;
        this.settingsService = settingsService;
        this.faceAuthService = faceAuthService;
    }

    [SlashCommand("show-settings", "Shows current security settings")]
    public async Task GetCurrentConfig()
    {
        var settings = settingsService.GetSettings();
        await RespondAsync($"Max faces: {settings.MaxRecognizableFaces}\n" +
                           $"Current security level: {settings.SecurityLevel}\n" +
                           $"Max violations: {settings.MaxViolationLimit}\n" +
                           $"Time before unlock after violation: {settings.TimeBeforeUnlockAfterViolation}\n" +
                           $"Sending logs to Discord: {settings.SendLogsToDiscord}");
    }
    
    [SlashCommand("update-settings", "Opens modal with form for updating security settings")]
    public async Task UpdateSettings()
    {
        var settings = settingsService.GetSettings();
        Console.WriteLine(settings.Id);
        var modal = new ModalBuilder()
            .WithTitle("Security Settings")
            .WithCustomId($"security_settings:settingsId={settings.Id}")
            .AddTextInput("Recognizable faces limit (1-10)", "max_faces", placeholder: "5",
                value: settings.MaxRecognizableFaces.ToString())
            .AddTextInput("Security level", "security_level", placeholder: "Violation",
                value: settings.SecurityLevel.ToString())
            .AddTextInput("Violations limit (1-10)", "violations_limit", placeholder: "3",
                value: settings.MaxViolationLimit.ToString())
            .AddTextInput("Reset time (hh:mm:ss)", "reset_time", placeholder: "1hour",
                value: settings.TimeBeforeUnlockAfterViolationAsString())
            .AddTextInput("Send logs to discord (True | False)", "send_logs_to_discord", placeholder: "True",
                value: settings.SendLogsToDiscord.ToString());
        await Context.Interaction.RespondWithModalAsync(modal.Build());
    }
    
    [ModalInteraction("security_settings:settingsId=*", ignoreGroupNames: true)]
    public async Task SubmitSecuritySettingModal(string settingsId, SettingsModalData data)
    {
        try
        {
            var settings = data.ToSecuritySettingsDto();
            await settingsService.UpdateBaseSettings(settings);
            await RespondAsync("Done! Your settings are now updated.");   
        }
        catch (FormatException ex)
        {
            logger.LogError(ex.Message);
            await RespondAsync($"Invalid form input: {ex.Message}");
        }
    }

    // ALE MI WJECHAŁO DEJA VU PRZECIEŻ JA TO WE ŚNIE PISAŁEM
    [SlashCommand("comments-get", "Shows list of available comments")]
    public async Task GetComments()
    {
        var comments = settingsService.GetSettings().CommentPool;
        var sb = new StringBuilder("List of available comments: ");
        
        foreach (var comment in comments)
        {
            sb.Append($"\n{comment}");
        }
        
        await RespondAsync(sb.ToString());
    }

    [SlashCommand("comments-add", "Adds new comment, which will be attached to violation photo")]
    public async Task AddComment(string comment)
    {
        await settingsService.AddComment(comment);
        await RespondAsync($"Comment: '{comment}' was added to list!");
    }

    // don't work
    [SlashCommand("comments-remove", "Remove comment from list")]
    public async Task RemoveComment(int index)
    {
        await settingsService.RemoveComment(index);
        await RespondAsync($"Comment with index: {index}");
    }
    
    // Maybe i shouldn't upload face via discord?
    [SlashCommand("register-face", "Registers new face which can be detected")]
    public async Task RegisterFace(string personName, Attachment image)
    {
        if (image == null || string.IsNullOrWhiteSpace(image.Url))
        {
            await RespondAsync("You have to add image", ephemeral: true);
            return;
        }
        
        Console.WriteLine(image.ContentType);
        Console.WriteLine(image.Filename);
        
        using var httpClient = new HttpClient();
        using var stream = await httpClient.GetStreamAsync(image.Url);
        
        Console.WriteLine(image.Url);
        
        string extenstion = Path.GetExtension(image.Filename);
        var result = await faceAuthService.RegisterFace(personName, stream, extenstion);
        
        if (result.IsSuccess)
        {
            //await SendMessageToChannel(personName, result.Data);
            await RespondAsync("Registered new face!");
        }
        else
        {
            await RespondAsync(result.Error);
        }
    }

    [SlashCommand("remove-face", "Remove face from list of recognizable faces")]
    public async Task RemoveFace(string personName)
    {
        var result = await faceAuthService.UnregisterFace(personName);
        if (result.IsSuccess)
        {
            await RespondAsync(result.Value);
        }
        else
        {
            await RespondAsync(result.Error);
        }
    }
    
    private async Task SendMessageToChannel(string message, string url)
    {
        var channel = Context.Guild.GetTextChannel(faceChannelId);
        if (channel != null)
        {
            logger.LogDebug(channel.Id.ToString());
            await channel.SendMessageAsync(message + "\n" + url);
        }
        else
        {
            logger.LogError("Cannot access faces channel");
        }
    }
    
}