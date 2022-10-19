using Forge.Security.Jwt.Client.Storage.Browser.Models;
using Forge.Security.Jwt.Shared.Client.Models;
using Forge.Security.Jwt.Shared.Client.Services;
using Forge.Security.Jwt.Shared.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Forge.Security.Jwt.Client.Storage.Browser
{

    /// <summary>Service Collection Extension methods</summary>
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Registers the LocalStorage as scoped for Forge Jwt Client side security services.
        /// </summary>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddForgeJwtClientAuthenticationCoreWithLocalStorage(this IServiceCollection services)
            => services.AddForgeJwtClientAuthenticationCoreWithLocalStorage(null);

        /// <summary>Registers the LocalStorage as scoped for Forge Jwt Client side security services.</summary>
        /// <param name="services">The services.</param>
        /// <param name="configure">The configure.</param>
        /// <returns>
        ///   IServiceCollection
        /// </returns>
        public static IServiceCollection AddForgeJwtClientAuthenticationCoreWithLocalStorage(this IServiceCollection services, Action<BrowserStorageOptions> configure)
        {
            return services
                .Replace(new ServiceDescriptor(typeof(IStorage<ParsedTokenData>),
                    typeof(Forge.Security.Jwt.Client.Storage.Browser.LocalStorage.Storage),
                    ServiceLifetime.Scoped))
                .Replace(new ServiceDescriptor(typeof(IRefreshTokenService),
                    typeof(Forge.Security.Jwt.Client.Storage.Browser.LocalStorage.JwtTokenRefreshHostedService),
                    ServiceLifetime.Scoped))
                .Configure<BrowserStorageOptions>(configureOptions =>
                {
                    configure?.Invoke(configureOptions);
                });
        }

        /// <summary>
        /// Registers the LocalStorage as singleton for Forge Jwt Client side security services.
        /// </summary>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddForgeJwtClientAuthenticationCoreAsSingletonWithLocalStorage(this IServiceCollection services)
            => services.AddForgeJwtClientAuthenticationCoreAsSingletonWithLocalStorage(null);

        /// <summary>Registers the LocalStorage as singleton for Forge Jwt Client side security services.</summary>
        /// <param name="services">The services.</param>
        /// <param name="configure">The configure.</param>
        /// <returns>
        ///   IServiceCollection
        /// </returns>
        public static IServiceCollection AddForgeJwtClientAuthenticationCoreAsSingletonWithLocalStorage(this IServiceCollection services, Action<BrowserStorageOptions> configure)
        {
            return services
                .Replace(new ServiceDescriptor(typeof(IStorage<ParsedTokenData>),
                    typeof(Forge.Security.Jwt.Client.Storage.Browser.LocalStorage.Storage),
                    ServiceLifetime.Singleton))
                .Replace(new ServiceDescriptor(typeof(IRefreshTokenService),
                    typeof(Forge.Security.Jwt.Client.Storage.Browser.LocalStorage.JwtTokenRefreshHostedService),
                    ServiceLifetime.Singleton))
                .Configure<BrowserStorageOptions>(configureOptions =>
                {
                    configure?.Invoke(configureOptions);
                });
        }

        /// <summary>
        /// Registers the SessionStorage as scoped for Forge Jwt Client side security services.
        /// </summary>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddForgeJwtClientAuthenticationCoreWithSessionStorage(this IServiceCollection services)
            => services.AddForgeJwtClientAuthenticationCoreWithSessionStorage(null);

        /// <summary>Registers the SessionStorage as scoped for Forge Jwt Client side security services.</summary>
        /// <param name="services">The services.</param>
        /// <param name="configure">The configure.</param>
        /// <returns>
        ///   IServiceCollection
        /// </returns>
        public static IServiceCollection AddForgeJwtClientAuthenticationCoreWithSessionStorage(this IServiceCollection services, Action<BrowserStorageOptions> configure)
        {
            return services
                .Replace(new ServiceDescriptor(typeof(IStorage<ParsedTokenData>),
                    typeof(Forge.Security.Jwt.Client.Storage.Browser.SessionStorage.Storage),
                    ServiceLifetime.Scoped))
                .Replace(new ServiceDescriptor(typeof(IRefreshTokenService),
                    typeof(Forge.Security.Jwt.Client.Storage.Browser.SessionStorage.JwtTokenRefreshHostedService),
                    ServiceLifetime.Scoped))
                .Configure<BrowserStorageOptions>(configureOptions =>
                {
                    configure?.Invoke(configureOptions);
                });
        }

        /// <summary>
        /// Registers the SessionStorage as singleton for Forge Jwt Client side security services.
        /// </summary>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddForgeJwtClientAuthenticationCoreAsSingletonWithSessionStorage(this IServiceCollection services)
            => services.AddForgeJwtClientAuthenticationCoreAsSingletonWithSessionStorage(null);

        /// <summary>Registers the SessionStorage as singleton for Forge Jwt Client side security services.</summary>
        /// <param name="services">The services.</param>
        /// <param name="configure">The configure.</param>
        /// <returns>
        ///   IServiceCollection
        /// </returns>
        public static IServiceCollection AddForgeJwtClientAuthenticationCoreAsSingletonWithSessionStorage(this IServiceCollection services, Action<BrowserStorageOptions> configure)
        {
            return services
                .Replace(new ServiceDescriptor(typeof(IStorage<ParsedTokenData>),
                    typeof(Forge.Security.Jwt.Client.Storage.Browser.SessionStorage.Storage),
                    ServiceLifetime.Singleton))
                .Replace(new ServiceDescriptor(typeof(IRefreshTokenService),
                    typeof(Forge.Security.Jwt.Client.Storage.Browser.SessionStorage.JwtTokenRefreshHostedService),
                    ServiceLifetime.Singleton))
                .Configure<BrowserStorageOptions>(configureOptions =>
                {
                    configure?.Invoke(configureOptions);
                });
        }

    }

}
