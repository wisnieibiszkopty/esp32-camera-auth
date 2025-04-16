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
    public async Task TestAsync(string text)
    {
        await RespondAsync($"Test: {text}");
    }

    [SlashCommand("show-settings", "Shows current security settings")]
    public async Task GetCurrentConfigAsync()
    {
        var settings = _settingsService.GetSettings();
        await RespondAsync($"Max faces: {settings.MaxRecognizableFaces}\n" +
                           $"Current security level: {settings.SecurityLevel}\n" +
                           $"Max violations: {settings.MaxViolationLimit}\n" +
                           $"Time before unlock after violation: {settings.TimeBeforeUnlockAfterViolation}\n" +
                           $"Sending logs to Discord: {settings.SendLogsToDiscord}");
    }

    // TODO receive this modal and change settings based on form fields
    // maybe send also settings id somehow?
    [SlashCommand("update-settings", "Opens modal with form for updating security settings")]
    public async Task UpdateSettings()
    {
        var settings = _settingsService.GetSettings();
        Console.WriteLine(settings.Id);
        var modal = new ModalBuilder()
            .WithTitle("Security Settings")
            .WithCustomId("security_settings")
            .AddTextInput("Recognizable faces limit (1-10)", "max_faces", placeholder: "5", value: settings.MaxRecognizableFaces.ToString())
            .AddTextInput("Security level","security_level", placeholder: "Violation", value: settings.SecurityLevel.ToString())
            .AddTextInput("Violations limit (0-10)", "violations_limit", placeholder: "3", value: settings.MaxViolationLimit.ToString())
            // TODO add parsing to correct format
            .AddTextInput("Reset time (hh:mm:ss)","reset_time", placeholder: "1hour", value: settings.TimeBeforeUnlockAfterViolation.ToString())
            // TODO add parsing to correct format
            .AddTextInput("Send logs to discord (t | f)","send_logs_to_discord", placeholder: "t", value: settings.SendLogsToDiscord.ToString());
        await Context.Interaction.RespondWithModalAsync(modal.Build());
    }
    
    [ModalInteraction("security_settings")]
    public async Task SubmitSecuritySettingModal(SettingsModalData data)
    {
        Console.WriteLine($"Modal data: {data.SecurityLevel}");
        try
        {
            var settings = data.ToSecuritySettings();
            _settingsService.UpdateSettings(settings);
        }
        catch (FormatException ex)
        {
            await RespondAsync($"Invalid form input: {ex}");
        }
    }
}