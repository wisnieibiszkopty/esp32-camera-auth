using backend.Data.Repositories;
using backend.Models;

namespace backend.Services;

public class SecuritySettingsService
{
    private ISecuritySettingsRepository _repository;
    private SecuritySettings _settings;
    
    public SecuritySettingsService(ISecuritySettingsRepository repository)
    {
        _repository = repository;
        var settings = _repository.Get();
        if (settings == null)
        {
            AddSettings();
        }
        else
        {
            _settings = settings;
        }
    }

    private void AddSettings()
    {
        var settings = new SecuritySettings
        {
            MaxRecognizableFaces = 5,
            SecurityLevel = SecurityLevel.Violation,
            SendLogsToDiscord = true,
            CommentPoolSize = 5
        };

        _repository.Add(settings);
        _settings = settings;
    }

    public SecuritySettings GetSettings()
    {
        return _settings;
    }

    public void UpdateSettings(SecuritySettings settings)
    {
        Console.WriteLine(settings.SecurityLevel);
    }
    
    public void SetSecurityLevel()
    {
        
    }

    public void SetPhotosLimit()
    {
        
    }

    public void AllowSendingLogsToDiscord()
    {
        
    }

    public void SetCommentPoolSize()
    {
        
    }

    public void GetComments()
    {
        
    }
    
    public void AddComment()
    {
        
    }

    public void RemoveComment()
    {
        
    }
}