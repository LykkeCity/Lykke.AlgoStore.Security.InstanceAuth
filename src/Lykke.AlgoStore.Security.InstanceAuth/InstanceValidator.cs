using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using System;

namespace Lykke.AlgoStore.Security.InstanceAuth
{
    public static class InstanceValidator
    {
        /// <summary>
        /// Checks whether a given instance data is valid and can be used for authentication
        /// </summary>
        /// <param name="instanceData">The instance data to validate</param>
        /// <param name="checkStatus">Whether the instance status should be validated</param>
        /// <returns>True when the instance data is valid, false otherwise</returns>
        public static bool Validate(AlgoClientInstanceData instanceData, bool checkStatus = true)
        {
            if (instanceData == null)
                throw new ArgumentNullException(nameof(instanceData));

            if (checkStatus && 
                instanceData.AlgoInstanceStatus != CSharp.AlgoTemplate.Models.Enumerators.AlgoInstanceStatus.Started)
                return false;

            return true;
        }
    }
}
