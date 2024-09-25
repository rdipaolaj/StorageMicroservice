using Microsoft.Extensions.DependencyInjection;
using ssptb.pe.tdlt.storage.secretsmanager.Services;

namespace ssptb.pe.tdlt.storage.secretsmanager;
public static class SecretManagerConfiguration
{
    public static IServiceCollection AddSecretManagerConfiguration(this IServiceCollection services)
    {
        services.AddSingleton<ISecretManagerService, SecretManagerService>();

        return services;
    }
}
