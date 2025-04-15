using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Discord;

[Route("api/discord/")]
[ApiController]
public class DiscordController : ControllerBase
{
    private readonly BotService botService;

    public DiscordController(BotService service)
    {
        botService = service;
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

    // [HttpPost("upload")]
    // public async Task<IActionResult> UploadImage([FromForm] IFormFile file, [FromForm] IFormFile file2)
    // {
    //     if ((file == null || file.Length == 0) || (file2 == null || file2.Length == 0))
    //     {
    //         return BadRequest("File not uploaded.");
    //     }
    //     
    //     Bitmap bitmap1;
    //     using (var memoryStream1 = new MemoryStream())
    //     {
    //         await file.CopyToAsync(memoryStream1);
    //         bitmap1 = new Bitmap(memoryStream1);
    //     }
    //     
    //     Bitmap bitmap2;
    //     using (var memoryStream2 = new MemoryStream())
    //     {
    //         await file2.CopyToAsync(memoryStream2);
    //         bitmap2 = new Bitmap(memoryStream2);
    //     }
    //
    //     var same = faceService.CompareFaces(bitmap1, bitmap2);
    //     
    //     return Ok();
    // }
    public class MessageRequest
    {
        public string Message { get; set; } = string.Empty;
    }
}