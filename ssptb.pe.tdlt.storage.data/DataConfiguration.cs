﻿using Couchbase.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ssptb.pe.tdlt.storage.common.Settings;
using ssptb.pe.tdlt.storage.data.Repositories.Interfaces;
using ssptb.pe.tdlt.storage.data.Repositories;
using ssptb.pe.tdlt.storage.data.Helpers;

namespace ssptb.pe.tdlt.storage.data;
public static class DataConfiguration
{
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        // Configura la cadena de conexión obtenida de los secretos de AWS o directamente de la configuración
        var serviceProvider = services.BuildServiceProvider();
        var couchBaseSettings = serviceProvider.GetService<IOptions<CouchBaseSettings>>()?.Value;

        if (couchBaseSettings == null)
        {
            throw new InvalidOperationException("CouchBaseSettings not configured properly.");
        }

        // Configurar Couchbase
        services.AddCouchbase(options =>
        {
            options.ConnectionString = couchBaseSettings.ConnectionString;
            options.UserName = couchBaseSettings.UserName;
            options.Password = couchBaseSettings.Password;
            options.ApplyProfile("wan-development");
        });

        // Registrar el bucket
        services.AddCouchbaseBucket<INamedBucketProvider>(couchBaseSettings.BucketName);

        return services;
    }

    public static IServiceCollection AddDataServicesConfiguration(this IServiceCollection services)
    {
        services.AddTransient<ICouchbaseHelper, CouchbaseHelper>();
        services.AddTransient<IFileRepository, FileRepository>();

        return services;
    }
}