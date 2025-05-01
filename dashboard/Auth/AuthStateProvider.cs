using System.Security.Claims;
using dashboard.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace dashboard.Auth;

public class AuthStateProvider : AuthenticationStateProvider
{
    private readonly UserService userService;
    private ClaimsPrincipal user;

    public AuthStateProvider(UserService userService)
    {
        this.userService = userService;
        user = new ClaimsPrincipal(new ClaimsIdentity());
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var username = userService.GetUsername();
        if (!string.IsNullOrEmpty(username))
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username),
            }, "apiauth");

            user = new ClaimsPrincipal(identity);
        }

        return Task.FromResult(new AuthenticationState(user));
    }
    
    public void MarkUserAsAuthenticated(string username)
    {
        userService.SetUsername(username);
        
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, username),
        }, "apiauth");

        user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }
    
    public void MarkUserAsLoggedOut()
    {
        userService.ClearUsername();
        user = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }
}