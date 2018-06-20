using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Security.InstanceAuth
{
    internal static class RateLimitMiddleware
    {
        private class CachedRateLimit
        {
            public int Requests { get; set; }
            public DateTimeOffset Expiration { get; set; }
        }

        private const string KEY_PREFIX = "instanceratelimit_";

        private static readonly MemoryCache _memoryCache = MemoryCache.Default;

        public static int MaxRequestsPerInterval { get; set; } = 3;

        public static async Task RateLimit(HttpContext context, Func<Task> next)
        {
            var token = TokenUtils.GetToken(context);
            var ip = context.Connection.RemoteIpAddress.ToString();

            var updateIp = CreateOrUpdateKey($"{KEY_PREFIX}{ip}");
            var updateToken = token == null ? true : CreateOrUpdateKey($"{KEY_PREFIX}{token}");

            // If either of the two should be rate limited, return status code 429 (Too Many Requests)
            // with Retry-After header indicating the cooldown time
            if (!updateIp || !updateToken)
            {
                context.Response.StatusCode = 429;
                context.Response.Headers.Add("Retry-After", "60");

                context.Response.Body = new MemoryStream();
                return;
            }

            await next();
        }

        private static bool CreateOrUpdateKey(string key)
        {
            if(!_memoryCache.Contains(key))
            {
                _memoryCache.Add(key, new CachedRateLimit
                {
                    Requests = 1,
                    Expiration = DateTimeOffset.UtcNow.AddMinutes(1)
                }, DateTimeOffset.UtcNow.AddMinutes(1));
                return true;
            }

            var item = (CachedRateLimit)_memoryCache.Get(key);

            item.Requests++;

            _memoryCache.Set(key, item, item.Expiration);

            return item.Requests <= MaxRequestsPerInterval;
        }
    }
}
