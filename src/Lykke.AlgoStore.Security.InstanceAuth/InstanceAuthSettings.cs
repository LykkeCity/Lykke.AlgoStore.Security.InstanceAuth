namespace Lykke.AlgoStore.Security.InstanceAuth
{
    /// <summary>
    /// Instance authentication caching configuration
    /// </summary>
    public class InstanceAuthSettings
    {
        /// <summary>
        /// How long it takes for the cache to expire after the last access
        /// </summary>
        public int CacheExpiryTimeInSeconds { get; set; }

        /// <summary>
        /// Whether instances with status different from started should pass authorization
        /// </summary>
        public bool AllowNonStartedInstances { get; set; }
    }
}
