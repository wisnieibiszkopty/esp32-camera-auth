using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using backend.Models.Dto;
using Microsoft.IdentityModel.Tokens;

namespace backend.Services;

public class AdminAuthService
{
    private readonly string adminUsername;
    private readonly string adminPassword;
    private readonly string jwtKey;
    
    public AdminAuthService(IConfiguration config)
    {
        adminUsername = config["Admin:Username"]!;
        adminPassword = config["Admin:Password"]!;
        jwtKey = config["Jwt:Key"]!;
    }

    public Result<string> Login(LoginRequest request)
    {
        if (request.Username != adminUsername || request.Password != adminPassword)
        {
            return Result<string>.Failure("Unauthorized"); 
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, request.Username),
            new Claim(ClaimTypes.Role, "Admin")
        };
        
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "YourAppIssuer",
            audience: "YourAppAudience",
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return Result<string>.Success(tokenString);
    }
}