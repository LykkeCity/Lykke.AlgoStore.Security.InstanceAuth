using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using System;
using System.Security.Principal;

namespace Lykke.AlgoStore.Security.InstanceAuth
{
    internal class InstanceIdentity : IIdentity
    {
        public string AuthenticationType { get; }
        public bool IsAuthenticated { get; }
        public string Name { get; }

        public AlgoClientInstanceData InstanceData { get; }

        public InstanceIdentity(string authToken, AlgoClientInstanceData instanceData)
        {
            AuthenticationType = "Token";
            IsAuthenticated = true;
            Name = authToken ?? throw new ArgumentNullException(nameof(authToken));
            InstanceData = instanceData ?? throw new ArgumentNullException(nameof(instanceData));
        }
    }
}
