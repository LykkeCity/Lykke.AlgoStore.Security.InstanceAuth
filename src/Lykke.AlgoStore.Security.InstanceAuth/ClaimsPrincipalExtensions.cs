using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using System.Security.Claims;

namespace Lykke.AlgoStore.Security.InstanceAuth
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Returns the instance data associated with this principal
        /// </summary>
        /// <param name="claims">The principal to get instance data for</param>
        /// <returns>The instance data of the authenticated instance</returns>
        public static AlgoClientInstanceData GetInstanceData(this ClaimsPrincipal claims)
        {
            return InstanceAuthHandler.GetCachedIdentity(claims?.Identity?.Name)?.InstanceData;
        }

        /// <summary>
        /// Returns the auth token associated with this principal
        /// </summary>
        /// <param name="claims">The principal to get the auth token of</param>
        /// <returns>The auth token of the authenticated instance</returns>
        public static string GetAuthToken(this ClaimsPrincipal claims)
        {
            return claims?.Identity?.Name;
        }
    }
}
