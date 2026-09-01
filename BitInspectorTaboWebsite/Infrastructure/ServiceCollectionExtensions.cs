// ------------------------------------------------------------------------
// This file is licensed to you under the MIT License.
// ------------------------------------------------------------------------

using BitInspectorTaboWebsite.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace BitInspectorTaboWebsite;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add common client services required by the Fluent UI Web Components for Blazor library
    /// </summary>
    /// <param name="services">Service collection</param>
    public static IServiceCollection AddFluentUIClientServices(this IServiceCollection services)
    {
        services.AddSingleton<IAppVersionService, AppVersionService>();
        services.AddScoped<CacheStorageAccessor>();
        services.AddHttpClient<IStaticAssetService, HttpBasedStaticAssetService>();

        return services;
    }

    /// <summary>
    /// Add common server services required by the Fluent UI Web Components for Blazor library
    /// </summary>
    /// <param name="services">Service collection</param>
    public static IServiceCollection AddFluentUIDemoServerServices(this IServiceCollection services)
    {
        services.AddScoped<IAppVersionService, AppVersionService>();
        services.AddScoped<CacheStorageAccessor>();

        return services;
    }
}
