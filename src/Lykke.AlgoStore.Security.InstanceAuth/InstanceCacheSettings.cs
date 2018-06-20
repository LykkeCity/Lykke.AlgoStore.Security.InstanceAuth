namespace Lykke.AlgoStore.Security.InstanceAuth
{
    /// <summary>
    /// Instance authentication caching configuration
    /// </summary>
    public class InstanceCacheSettings
    {
        /// <summary>
        /// How long it takes for the cache to expire after the last access
        /// </summary>
        public int CacheExpiryTimeInSeconds { get; set; }
    }
}
