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
    [SlashCommand("show-comments", "Shows list of available comments")]
    public async Task ShowComments()
    {
        var comments = settingsService.GetSettings().CommentPool;
        var sb = new StringBuilder("List of available comments: ");

        int index = 1;
        foreach (var comment in comments)
        {
            sb.Append($"\n{index}. {comment}");
            index++;
        }
        
        await RespondAsync(sb.ToString());
    }

    [SlashCommand("add-comment", "Adds new comment, which will be attached to violation photo")]
    public async Task AddComment(string comment)
    {
        var result = await settingsService.AddComment(comment);
        if (result.IsSuccess)
        {
            await RespondAsync($"Comment: '{result.Value}' was added to list!");   
        }
        else
        {
            await RespondAsync(result.Error);
        }
    }
    
    [SlashCommand("remove-comment", "Remove comment from list (indexed from 1)")]
    public async Task RemoveComment(int index)
    {
        var result = await settingsService.RemoveComment(index);
        if (result.IsFailure)
        {
            await RespondAsync(result.Error);   
        }
        else
        {
            await RespondAsync($"Deleted comment: {result.Value}");
        }
    }

    [SlashCommand("show-faces", "Shows list of currently registered faces")]
    public async Task ShowFaces()
    {
        var faces = await faceAuthService.GetFaces();
        var sb = new StringBuilder("List of registered faces: ");
        foreach (var face in faces)
        {
            sb.Append($"\n{face}");
        }

        await RespondAsync(sb.ToString());
    }
    
    [SlashCommand("register-face", "Registers new face which can be detected")]
    public async Task RegisterFace(string personName, Attachment image)
    {
        if (image == null || string.IsNullOrWhiteSpace(image.Url))
        {
            await RespondAsync("You have to add image", ephemeral: true);
            return;
        }
        
        using var httpClient = new HttpClient();
        using var stream = await httpClient.GetStreamAsync(image.Url);
        
        string extenstion = Path.GetExtension(image.Filename);
        var result = await faceAuthService.RegisterFace(personName, stream, extenstion);
        
        if (result.IsSuccess)
        {
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
    
}