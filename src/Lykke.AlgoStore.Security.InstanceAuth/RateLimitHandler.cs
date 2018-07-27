using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;
using System.Runtime.Caching;

namespace Lykke.AlgoStore.Security.InstanceAuth
{
    internal class RateLimitHandler : IActionFilter
    {
        private class CachedRateLimit
        {
            public int Requests { get; set; }
            public DateTimeOffset Expiration { get; set; }
        }

        private const string KEY_PREFIX = "instanceratelimit_";

        private static readonly MemoryCache _memoryCache = MemoryCache.Default;
        private readonly RateLimitSettings _settings;

        public RateLimitHandler(RateLimitSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (_settings.Count5xxTowardRateLimit) return;

            if(context.HttpContext.Response.StatusCode >= 500 && context.HttpContext.Response.StatusCode < 600)
            {
                var token = TokenUtils.GetToken(context.HttpContext);
                var ip = context.HttpContext.Connection.RemoteIpAddress.ToString();

                HandleServerError($"{KEY_PREFIX}{ip}");
                if (!string.IsNullOrEmpty(token))
                    HandleServerError($"{KEY_PREFIX}{token}");
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var token = TokenUtils.GetToken(context.HttpContext);
            var ip = context.HttpContext.Connection.RemoteIpAddress.ToString();

            var updateIp = CreateOrUpdateKey($"{KEY_PREFIX}{ip}");
            var updateToken = token == null ? true : CreateOrUpdateKey($"{KEY_PREFIX}{token}");

            // If either of the two should be rate limited, return status code 429 (Too Many Requests)
            // with Retry-After header indicating the cooldown time
            if (!updateIp || !updateToken)
            {
                var keyToUse = token == null ? $"{KEY_PREFIX}{ip}" : $"{KEY_PREFIX}{token}";

                context.HttpContext.Response.Headers.Add("Retry-After", GetSecondsToExpiration(keyToUse).ToString());
                context.Result = new StatusCodeResult((int)HttpStatusCode.TooManyRequests);
            }
        }

        private void HandleServerError(string key)
        {
            if (!_memoryCache.Contains(key))
                return;

            var item = (CachedRateLimit)_memoryCache.Get(key);

            item.Requests--;

            _memoryCache.Set(key, item, item.Expiration);
        }

        private bool CreateOrUpdateKey(string key)
        {
            if (!_memoryCache.Contains(key))
            {
                _memoryCache.Add(key, new CachedRateLimit
                {
                    Requests = 1,
                    Expiration = DateTimeOffset.UtcNow.AddSeconds(_settings.TimeframeDurationInSeconds)
                }, DateTimeOffset.UtcNow.AddSeconds(_settings.TimeframeDurationInSeconds));
                return true;
            }

            var item = (CachedRateLimit)_memoryCache.Get(key);

            item.Requests++;

            _memoryCache.Set(key, item, item.Expiration);

            return item.Requests <= _settings.MaximumRequestsPerTimeframe;
        }

        private int GetSecondsToExpiration(string key)
        {
            if (!_memoryCache.Contains(key))
            {
                return 0;
            }

            var item = (CachedRateLimit)_memoryCache.Get(key);
            return Math.Max(0, (int)Math.Ceiling((item.Expiration - DateTimeOffset.UtcNow).TotalSeconds));
        }
    }
}
