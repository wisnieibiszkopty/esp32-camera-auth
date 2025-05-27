using backend.Models.Dto;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/verify")]
    [ApiController]
    public class FaceVerificationController : ControllerBase
    {
        private readonly FaceAuthService faceAuthService;
        private readonly ILogger<FaceVerificationController> logger;
        
        public FaceVerificationController(ILogger<FaceVerificationController> logger, FaceAuthService faceAuthService)
        {
            this.logger = logger;
            this.faceAuthService = faceAuthService;
        }

        [HttpGet]
        public string Test()
        {
            return "test";
        }
        
        [HttpPost]
        public async Task<ActionResult> Verify([FromForm] IFormFile image, [FromForm] DeviceData data)
        {
            if (image == null || image.Length == 0)
                return BadRequest("No image provided");

            byte[] imageBytes;
            using (var ms = new MemoryStream())
            {
                await image.CopyToAsync(ms);
                imageBytes = ms.ToArray();
            }
            
            string base64Image = Convert.ToBase64String(imageBytes);
            
            logger.LogError($"{data.DeviceId} - {data.Timestamp}");
            
            var faceVerification = new FaceVerificationRequest
            {
                DeviceId = data.DeviceId,
                TimestampUnix = data.Timestamp,
                ImageBase64 = base64Image
            };
            
            logger.LogError($"{faceVerification.DeviceId} - {faceVerification.TimestampUnix} - {faceVerification.Timestamp}");
            
            var result = await faceAuthService.VerifyFace(faceVerification);
            if (result.IsFailure)
            {
                logger.LogError(result.Error);
                return Ok(result.Error);
            }

            logger.LogInformation(result.Value);
            return Ok(result.Value);
        }
        
    }
}