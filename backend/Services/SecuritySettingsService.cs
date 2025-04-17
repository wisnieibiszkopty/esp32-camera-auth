using backend.Data.Repositories;
using backend.Models;

namespace backend.Services;

// TODO refactor in future
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
    
    public async Task<bool> UpdateSettings(SecuritySettings settings)
    {
        Console.WriteLine(settings.SecurityLevel);
        
        var result = await _repository.UpdateAsync(settings);
        _settings = _repository.Get()!;
        return result;
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

    // todo don't work
    public async Task RemoveComment(int index)
    {
        var comments = _settings.CommentPool;
        int count = comments.Count;

        if (index > count + 1)
        {
            // todo failed
            return;
        }
        
        comments.RemoveAt(index);
        _settings.CommentPool = comments;
        await _repository.UpdateAsync(_settings);
    }
}