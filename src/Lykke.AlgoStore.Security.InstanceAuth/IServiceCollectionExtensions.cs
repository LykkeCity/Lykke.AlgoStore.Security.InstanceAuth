using Microsoft.Extensions.DependencyInjection;
using System;

namespace Lykke.AlgoStore.Security.InstanceAuth
{
    public static class IServiceCollectionExtensions
    {
        private const string AUTH_SCHEME = "Bearer";

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
