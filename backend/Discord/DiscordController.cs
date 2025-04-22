using backend.Data;
using backend.Models;
using backend.Models.Dto;
using backend.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace backend.Discord;

[Route("api/discord/")]
[ApiController]
public class DiscordController : ControllerBase
{
    private readonly BotService botService;
    private readonly FaceAuthService authService;

    public DiscordController(BotService service, FaceAuthService authService)
    {
        botService = service;
        this.authService = authService;
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

    [HttpPost("face-rec")]
    public async Task<IActionResult> VerifyFace([FromBody] FaceVerificationRequest request)
    {
        await authService.VerifyFace(request);
        return Ok();
    }
    
    public class MessageRequest
    {
        public string Message { get; set; } = string.Empty;
    }
}