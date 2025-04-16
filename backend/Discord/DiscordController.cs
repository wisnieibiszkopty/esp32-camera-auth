using backend.Data;
using backend.Models;
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
    private readonly IStorageService storageService;
    private readonly DbContext context;

    public DiscordController(BotService service, IStorageService storageService, DbContext context)
    {
        botService = service;
        this.storageService = storageService;
        this.context = context;
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

    // temporary endpoints for testing database connection
    // [HttpPost("add")]
    // public async Task<IActionResult> AddLog()
    // {
    //     await context.Logs.InsertOneAsync(new Log{Message = "test"});
    //     return Ok("Added");
    // }
    //
    // [HttpGet("get")]
    // public async Task<List<Log>> GetLogs()
    // {
    //     var logs = await context.Logs.Find(_ => true).ToListAsync();
    //     return logs;
    // }
    //
    // [HttpDelete]
    // public async Task<IActionResult> DeleteLogs()
    // {
    //     await context.Logs.DeleteManyAsync(_ => true);
    //     return Ok();
    // }
    
    public class MessageRequest
    {
        public string Message { get; set; } = string.Empty;
    }
}