using Discord.Interactions;

namespace backend.Discord;

public class DiscordCommands : InteractionModuleBase<SocketInteractionContext>
{
    // test command
    // commands should invoke some service
    [SlashCommand("test", "Testing")]
    public async Task TestAsync(string text)
    {
        await RespondAsync($"Test: {text}");
    }
}