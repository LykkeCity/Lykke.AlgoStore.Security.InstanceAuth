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

        private static readonly MemoryCache _memoryCache = MemoryCache.Default;

        private readonly IOptionsMonitor<InstanceAuthOptions> _optionsMonitor;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAlgoClientInstanceRepository _instanceRepository;
        private readonly CacheItemPolicy _defaultCachePolicy;

        public InstanceAuthHandler(
            IOptionsMonitor<InstanceAuthOptions> optionsMonitor, 
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock,
            IHttpContextAccessor httpContextAccessor,
            IAlgoClientInstanceRepository instanceRepository,
            IConfigureOptions<InstanceAuthOptions> configureOptions) 
            : base(optionsMonitor, logger, encoder, clock)
        {
            if (configureOptions == null)
                throw new ArgumentNullException(nameof(configureOptions));

            _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _instanceRepository = instanceRepository ?? throw new ArgumentNullException(nameof(instanceRepository));

            var authOptions = optionsMonitor.CurrentValue;

            var configureNamedOptions = (IConfigureNamedOptions<InstanceAuthOptions>)configureOptions;
            configureNamedOptions.Configure(InstanceAuthConstants.AUTH_SCHEME, authOptions);

            _defaultCachePolicy = new CacheItemPolicy
            {
                SlidingExpiration = TimeSpan.FromSeconds(authOptions.AuthSettings.CacheExpiryTimeInSeconds)
            };
        }

        /// <summary>
        /// Handles instance authentication based on the bearer token
        /// </summary>
        /// <returns>Authentication result</returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var token = TokenUtils.GetToken(_httpContextAccessor.HttpContext);

            if (token == null)
                return AuthenticateResult.NoResult();

            // Check if the token already exists in the cache
            var cachedData = _memoryCache.Get($"{KEY_PREFIX}{token}");

            if (cachedData != null)
                return AuthenticateResult.Success(MakeTicket((InstanceIdentity)cachedData));

            var data = await _instanceRepository.GetAlgoInstanceDataByAuthTokenAsync(token);

            if (data == null)
                return AuthenticateResult.NoResult();

            if (!InstanceValidator.Validate(data, !_optionsMonitor.CurrentValue.AuthSettings.AllowNonStartedInstances))
                return AuthenticateResult.NoResult();

            var identity = new InstanceIdentity(token, data);
            
            _memoryCache.Add(new CacheItem($"{KEY_PREFIX}{token}", identity), _defaultCachePolicy);

            return AuthenticateResult.Success(MakeTicket(identity));
        }

        private AuthenticationTicket MakeTicket(InstanceIdentity identity)
        {
            var principal = new ClaimsPrincipal(identity);
            return new AuthenticationTicket(principal, InstanceAuthConstants.AUTH_SCHEME);
        }

        internal static InstanceIdentity GetCachedIdentity(string token)
        {
            if (token == null)
                return null;

            var cachedData = _memoryCache.Get($"{KEY_PREFIX}{token}");

            return cachedData as InstanceIdentity;
        }
    }
}
