using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Discord;

[Route("api/discord/")]
[ApiController]
public class DiscordController : ControllerBase
{
    private readonly BotService botService;
    private readonly IStorageService storageService;

    public DiscordController(BotService service, IStorageService storageService)
    {
        botService = service;
        this.storageService = storageService;
    }
        
    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] MessageRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest("Message is required.");   
        }

        await botService.SendMessageAsync(request.Message);
        return Ok("Message sent.");
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        using (var stream = file.OpenReadStream())
        {
            await storageService.UploadImageAsync(file.FileName, stream);
        }

        return Ok("File uploaded successfully.");
    }
    public class MessageRequest
    {
        public string Message { get; set; } = string.Empty;
    }
}