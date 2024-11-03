using Couchbase.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ssptb.pe.tdlt.storage.common.Settings;
using ssptb.pe.tdlt.storage.data.Repositories.Interfaces;
using ssptb.pe.tdlt.storage.data.Repositories;
using ssptb.pe.tdlt.storage.data.Helpers;
using MongoDB.Driver;

namespace ssptb.pe.tdlt.storage.data;
public static class DataConfiguration
{
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceProvider = services.BuildServiceProvider();
        var mongoDbSettings = serviceProvider.GetService<IOptions<MongoDbSettings>>()?.Value;

        if (mongoDbSettings == null)
        {
            throw new InvalidOperationException("MongoDBSettings not configured properly.");
        }

        // Configurar MongoDB
        services.AddSingleton<IMongoClient, MongoClient>(sp =>
            new MongoClient(mongoDbSettings.ConnectionString));
        services.AddScoped(sp => sp.GetRequiredService<IMongoClient>().GetDatabase("Storage"));

        services.AddSingleton<MongoDBInitializer>();

        return services;
    }

    public static IServiceCollection AddDataServicesConfiguration(this IServiceCollection services)
    {
        services.AddTransient<IMongoDBHelper, MongoDBHelper>();
        services.AddTransient<IFileRepository, FileRepository>();

        return services;
    }

    public static void InitializeMongoDatabase(this IServiceProvider services)
    {
        var mongoInitializer = services.GetRequiredService<MongoDBInitializer>();
        mongoInitializer.InitializeDatabase();
    }
}
