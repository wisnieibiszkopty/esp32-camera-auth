using backend.Models;
using backend.Models.Dto;
using backend.Models.Enums;
using backend.Services;
using backend.Services.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILoggingService loggingService;
        
        public LogController(ILoggingService loggingService)
        {
            this.loggingService = loggingService;
        }

        [Authorize]
        [HttpGet("logs")]
        public async Task<PagedResult<Log>> GetLogs(
            [FromQuery(Name = "page")] int page,
            [FromQuery(Name = "size")] int size,
            [FromQuery(Name = "result")] DetectionResult? result,
            [FromQuery(Name = "person")] string? personName,
            [FromQuery(Name = "startDate")] DateTime? startTime,
            [FromQuery(Name = "endDate")] DateTime? endTime
            )
        {
            return await loggingService.GetLogs(page, size, result, personName, startTime, endTime);
        }
        
        [Authorize]
        [HttpGet("metrics")]
        public async Task<LogsMetrics> GetMetrics([FromQuery(Name = "period")] Period? period)
        {
            return await loggingService.GetMetrics(period);
        }
    }
}
