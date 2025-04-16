using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace backend.Data;

public static class DatabaseModule
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<DatabaseSettings>(config.GetSection("Mongo"));
        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
            return new MongoClient(settings.ConnectionString);
        });
        services.AddScoped<DbContext>();

        return services;
    }
}