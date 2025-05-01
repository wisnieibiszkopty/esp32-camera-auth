using backend.Models.Dto;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Microsoft.AspNetCore.Components.Route("api/admin/auth")]
[ApiController]
public class AdminAuthController : ControllerBase
{
    private readonly AdminAuthService authService;
    
    public AdminAuthController(AdminAuthService authService)
    {
        this.authService = authService;
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        var result = authService.Login(request);
        if (result.IsFailure)
        {
            return Unauthorized();
        }
        
        return Ok(result.Value);
    }
}