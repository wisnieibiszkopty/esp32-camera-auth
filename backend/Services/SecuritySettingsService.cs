using backend.Data.Repositories;
using backend.Models;

namespace backend.Services;

public class SecuritySettingsService
{
    private static readonly int _commentsLimit = 20;
    
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
            SendLogsToDiscord = true
        };

        _repository.Add(settings);
        _settings = settings;
    }

    public SecuritySettings GetSettings()
    {
        return _settings;
    }

    // todo update them fr
    public async Task<bool> UpdateSettings(SecuritySettings settings)
    {
        Console.WriteLine(settings.SecurityLevel);
        
        return await _repository.UpdateAsync(settings);
    }

    public List<string> GetComments()
    {
        return _settings.CommentPool;
    }
    
    // TODO handle limit
    public async Task AddComment(string comment)
    {
        if (_commentsLimit != _settings.CommentPool.Count)
        {
            _settings.CommentPool.Add(comment);
            await _repository.AddComment(_settings);
        }
    }

    public void RemoveComment()
    {
        
    }
}