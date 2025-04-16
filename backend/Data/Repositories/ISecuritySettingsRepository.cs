using backend.Models;

namespace backend.Data.Repositories;

public interface ISecuritySettingsRepository
{
    public Task<SecuritySettings?> GetAsync(string id);
    public Task<bool> UpdateAsync(SecuritySettings settings);
}