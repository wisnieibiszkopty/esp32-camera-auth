using backend.Data.Repositories;
using backend.Models;
using backend.Models.Dto;
using Mapster;

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
    
    public async Task<bool> UpdateBaseSettings(SecuritySettingsBaseDto settingsDto)
    {
        var config = new TypeAdapterConfig();
        config.NewConfig<SecuritySettingsBaseDto, SecuritySettings>()
            .IgnoreNullValues(true);

        settingsDto.Adapt(_settings, config);
        
        var result = await _repository.UpdateAsync(_settings);
        return result;
    }
    
    public async Task<Result<string>> AddComment(string comment)
    {
        if (_commentsLimit == _settings.CommentPool.Count)
        {
            return Result<string>.Failure("Achieved comments limit");
        }
        
        _settings.CommentPool.Add(comment);
        await _repository.UpdateAsync(_settings);
        return Result<string>.Success(comment);
    }

    // todo don't work
    public async Task<Result<string>> RemoveComment(int index)
    {
        var comments = GetSettings().CommentPool;
        int count = comments.Count;
        if (index > count)
        {
            return Result<string>.Failure("Index is incorrect - goes beyond the length of the comment list");
        }

        var deletedComment = comments[index - 1];
        comments.RemoveAt(index-1);
        
        _settings.CommentPool = comments;
        await _repository.UpdateAsync(_settings);
        
        return Result<string>.Success(deletedComment);
    }
}