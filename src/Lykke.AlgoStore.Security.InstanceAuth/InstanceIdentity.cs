using System;
using System.Security.Principal;

namespace Lykke.AlgoStore.Security.InstanceAuth
{
    internal class InstanceIdentity : IIdentity
    {
        public string AuthenticationType { get; }
        public bool IsAuthenticated { get; }
        public string Name { get; }

        public InstanceIdentity(string authToken)
        {
            AuthenticationType = "Token";
            IsAuthenticated = true;
            Name = authToken ?? throw new ArgumentNullException(nameof(authToken));
        }
    }
}
