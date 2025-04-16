using System.Text;
using backend.Discord.Models;
using backend.Models;
using backend.Services;
using Discord;
using Discord.Commands;
using Discord.Interactions;

namespace backend.Discord;

public class DiscordInteractions : InteractionModuleBase<SocketInteractionContext>
{
    private readonly SecuritySettingsService _settingsService;
    
    public DiscordInteractions(SecuritySettingsService service)
    {
        _settingsService = service;
    }

    // test command
    // commands should invoke some service
    [SlashCommand("test", "Testing")]
    public async Task Test(string text)
    {
        await RespondAsync($"Test: {text}");
    }

    [SlashCommand("show-settings", "Shows current security settings")]
    public async Task GetCurrentConfig()
    {
        var settings = _settingsService.GetSettings();
        await RespondAsync($"Max faces: {settings.MaxRecognizableFaces}\n" +
                           $"Current security level: {settings.SecurityLevel}\n" +
                           $"Max violations: {settings.MaxViolationLimit}\n" +
                           $"Time before unlock after violation: {settings.TimeBeforeUnlockAfterViolation}\n" +
                           $"Sending logs to Discord: {settings.SendLogsToDiscord}");
    }
    
    [SlashCommand("update-settings", "Opens modal with form for updating security settings")]
    public async Task UpdateSettings()
    {
        var settings = _settingsService.GetSettings();
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
            var settings = data.ToSecuritySettings();
            
            Console.WriteLine($"id: ${settingsId}");

            settings.Id = settingsId;
            
            await _settingsService.UpdateSettings(settings);
            await RespondAsync("Done! Your settings are now updated.");   
        }
        catch (FormatException ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            await RespondAsync($"Invalid form input: {ex.Message}");
        }
    }

    // ALE MI WJECHAŁO DEJA VU PRZECIEŻ JA TO WE ŚNIE PISAŁEM
    [SlashCommand("comments-get", "Shows list of available comments")]
    public async Task GetComments()
    {
        var comments = _settingsService.GetComments();
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
        await _settingsService.AddComment(comment);
        await RespondAsync($"Comment: '{comment}' was added to list!");
    }

    public async Task RemoveComment()
    {
        
    }
}