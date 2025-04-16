using backend.Models;
using MongoDB.Driver;

namespace backend.Data.Repositories;

public class SecuritySettingsRepository : ISecuritySettingsRepository
{
    private IMongoCollection<SecuritySettings> collection;
    
    public SecuritySettingsRepository(DbContext context)
    {
        collection = context.SecuritySettings;
    }

    public async Task<SecuritySettings?> GetAsync(string id)
    {
        return await collection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateAsync(SecuritySettings settings)
    {
        var result = await collection.ReplaceOneAsync(
            x => x.Id == settings.Id,
            settings
        );

        return result.IsAcknowledged && result.ModifiedCount > 0;
    }
}