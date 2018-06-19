using Microsoft.AspNetCore.Builder;
using System;

namespace Lykke.AlgoStore.Security.InstanceAuth
{
    public static class IApplicationBuilderExtensions
    {
        public static void UseRateLimiting(
            this IApplicationBuilder applicationBuilder, 
            RateLimitSettings settings)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            RateLimitMiddleware.MaxRequestsPerInterval = settings.MaximumRequestsPerMinute;

            applicationBuilder.Use(RateLimitMiddleware.RateLimit);
        }
    }
}
