using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Runtime.Caching;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Security.InstanceAuth
{
    internal class InstanceAuthHandler : AuthenticationHandler<InstanceAuthOptions>
    {
        private const string KEY_PREFIX = "instanceauth_";

        private readonly IOptionsMonitor<InstanceAuthOptions> _options;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAlgoClientInstanceRepository _instanceRepository;
        private readonly MemoryCache _memoryCache = MemoryCache.Default;

        private readonly CacheItemPolicy _defaultCachePolicy = new CacheItemPolicy
        {
            SlidingExpiration = TimeSpan.FromMinutes(2)
        };

        public InstanceAuthHandler(
            IOptionsMonitor<InstanceAuthOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock,
            IHttpContextAccessor httpContextAccessor,
            IAlgoClientInstanceRepository instanceRepository) 
            : base(options, logger, encoder, clock)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _instanceRepository = instanceRepository ?? throw new ArgumentNullException(nameof(instanceRepository));
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var token = TokenUtils.GetToken(_httpContextAccessor.HttpContext);

            if (token == null)
                return AuthenticateResult.NoResult();

            var cachedData = _memoryCache.Get($"{KEY_PREFIX}{token}");

            if (cachedData != null)
                return AuthenticateResult.Success((AuthenticationTicket)cachedData);

            var data = await _instanceRepository.GetAlgoInstanceDataByAuthTokenAsync(token);

            if (data == null)
                return AuthenticateResult.NoResult();

            if (!InstanceValidator.Validate(data))
                return AuthenticateResult.NoResult();

            var principal = new ClaimsPrincipal(new InstanceIdentity(token));
            var ticket = new AuthenticationTicket(principal, "Bearer");

            _memoryCache.Add(new CacheItem($"{KEY_PREFIX}{token}", ticket), _defaultCachePolicy);

            return AuthenticateResult.Success(ticket);
        }
    }
}
