using Microsoft.Extensions.DependencyInjection;
using ssptb.pe.tdlt.storage.internalservices.Base;
using ssptb.pe.tdlt.storage.internalservices.Cloudinary;
using ssptb.pe.tdlt.storage.internalservices.Helpers;
using ssptb.pe.tdlt.storage.internalservices.Helpers.Interfaces;

namespace ssptb.pe.tdlt.storage.internalservices;
public static class InternalServicesConfiguration
{
    public static IServiceCollection AddInternalServicesConfiguration(this IServiceCollection services)
    {
        services.AddTransient<IBaseService, BaseService>();
        services.AddTransient<IJsonValidator, JsonValidator>();
        services.AddTransient<IFileNameGenerator, FileNameGenerator>();
        services.AddTransient<ICloudinaryService, CloudinaryService>();

        return services;
    }
}
