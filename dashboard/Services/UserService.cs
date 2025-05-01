namespace dashboard.Services;

// To save username in localstorage I have to call javascript interlope
// No way I'm doing that
public class UserService
{
    private string? username;

    public void SetUsername(string name)
    {
        this.username = name;
    }

    public string? GetUsername()
    {
        return username;
    }

    public void ClearUsername()
    {
        username = null;
    }
}