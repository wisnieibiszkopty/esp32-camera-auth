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

    public SecuritySettings? Get(string? id = null)
    {
        if (id != null)
        {
            return collection.Find(x => x.Id == id).FirstOrDefault();   
        }
        
        return collection.Find(_ => true).FirstOrDefault();
    }

    public void Add(SecuritySettings settings)
    {
        collection.InsertOne(settings);
    }
    
    public async Task<bool> UpdateAsync(SecuritySettings settings)
    {
        var result = await collection.ReplaceOneAsync(
            x => x.Id == settings.Id,
            settings
        );

        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<bool> AddComment(SecuritySettings settings)
    {
        var updatedDef = Builders<SecuritySettings>.Update
            .Set(x => x.CommentPool, settings.CommentPool);
        
        var result = await collection.UpdateOneAsync(
            x => x.Id == settings.Id,
            updatedDef
        );

        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public void DeleteComment()
    {
        
    }
    
}