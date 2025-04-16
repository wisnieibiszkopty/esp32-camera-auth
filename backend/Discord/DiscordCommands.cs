using Discord.Interactions;

namespace backend.Discord;

public class DiscordCommands : InteractionModuleBase<SocketInteractionContext>
{
    public DiscordCommands()
    {
        
    }

    // test command
    // commands should invoke some service
    [SlashCommand("test", "Testing")]
    public async Task TestAsync(string text)
    {
        await RespondAsync($"Test: {text}");
    }
}