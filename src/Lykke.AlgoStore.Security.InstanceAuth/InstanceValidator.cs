using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using System;

namespace Lykke.AlgoStore.Security.InstanceAuth
{
    public static class InstanceValidator
    {
        public static bool Validate(AlgoClientInstanceData instanceData)
        {
            if (instanceData == null)
                throw new ArgumentNullException(nameof(instanceData));

            if (instanceData.AlgoInstanceStatus != CSharp.AlgoTemplate.Models.Enumerators.AlgoInstanceStatus.Started)
                return false;

            return true;
        }
    }
}
