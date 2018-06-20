using Microsoft.AspNetCore.Authentication;

namespace Lykke.AlgoStore.Security.InstanceAuth
{
    internal class InstanceAuthOptions : AuthenticationSchemeOptions
    {
        public InstanceCacheSettings CacheSettings { get; set; }
    }
}
