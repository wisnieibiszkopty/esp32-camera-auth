using backend.Models.events;
using backend.Services;

namespace backend.Discord;

// this won't work
public class DiscordEventHandler
{
    private readonly BotService botService;
    private readonly AuthService authService;
    
    public DiscordEventHandler(BotService botService, AuthService authService)
    {
        this.botService = botService;
        this.authService = authService;

        this.authService.FaceRecognised += async (sender, args) =>
        {
            Console.WriteLine("Event occured!");
            await OnFaceRegistered(sender, args);
        };
    }
    
    private async Task OnFaceRegistered(object? sender, FaceRecognisedEventArgs e)
    {
        Console.WriteLine($"{e.PersonName} was added at path: {e.ImagePath}");
        // I have to add this to face Channel instead of general
        //await RespondAsync($"{e.PersonName} was added at path: {e.ImagePath}");
    }
}