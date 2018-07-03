using Microsoft.AspNetCore.Authentication;

namespace Lykke.AlgoStore.Security.InstanceAuth
{
    internal class InstanceAuthOptions : AuthenticationSchemeOptions
    {
        public InstanceAuthSettings AuthSettings { get; set; }
    }
}
