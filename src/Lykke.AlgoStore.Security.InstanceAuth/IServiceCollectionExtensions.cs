using Microsoft.Extensions.DependencyInjection;
using System;

namespace Lykke.AlgoStore.Security.InstanceAuth
{
    public static class IServiceCollectionExtensions
    {
        private const string AUTH_SCHEME = "Bearer";

        /// <summary>
        /// Adds algo instance authentication through bearer token
        /// </summary>
        /// <param name="services">The service collection to register the authentication in</param>
        /// <param name="cacheSettings">Instance data cache settings</param>
        public static void AddInstanceAuthentication(this IServiceCollection services, InstanceCacheSettings cacheSettings)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (cacheSettings == null)
                throw new ArgumentNullException(nameof(cacheSettings));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = AUTH_SCHEME;
                options.DefaultChallengeScheme = AUTH_SCHEME;
            }).AddScheme<InstanceAuthOptions, InstanceAuthHandler>(AUTH_SCHEME, o => o.CacheSettings = cacheSettings);
        }
    }
}
