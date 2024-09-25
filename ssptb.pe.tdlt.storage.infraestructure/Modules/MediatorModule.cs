using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using ssptb.pe.tdlt.storage.infraestructure.Behaviors;
using ssptb.pe.tdlt.storage.internalservices;
using ssptb.pe.tdlt.storage.secretsmanager;
using ssptb.pe.tdlt.storage.redis;
using ssptb.pe.tdlt.storage.commandhandler.Upload;
using ssptb.pe.tdlt.storage.commandvalidator.Upload;

namespace ssptb.pe.tdlt.storage.infraestructure.Modules;
public static class MediatorModule
{
    public static IServiceCollection AddMediatRAssemblyConfiguration(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblyContaining(typeof(UploadJsonCommandHandler));

            configuration.AddOpenBehavior(typeof(ValidatorBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(UploadJsonCommandValidator).Assembly);

        return services;
    }
    public static IServiceCollection AddCustomServicesConfiguration(this IServiceCollection services)
    {
        services.AddInternalServicesConfiguration();
        services.AddSecretManagerConfiguration();
        services.AddRedisServiceConfiguration();

        return services;
    }
}

