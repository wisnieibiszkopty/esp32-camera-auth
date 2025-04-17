using backend.Models;

namespace backend.Data.Repositories;

public interface ISecuritySettingsRepository
{
    
    public SecuritySettings? Get(string? id = null);
    public void Add(SecuritySettings settings);
    public Task<bool> UpdateAsync(SecuritySettings settings);
    public Task<bool> AddComment(SecuritySettings settings);
}