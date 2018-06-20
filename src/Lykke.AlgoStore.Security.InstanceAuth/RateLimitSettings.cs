namespace Lykke.AlgoStore.Security.InstanceAuth
{
    /// <summary>
    /// Rate limiting configuration
    /// </summary>
    public class RateLimitSettings
    {
        /// <summary>
        /// Maximum number of requests allowed every minute
        /// </summary>
        public int MaximumRequestsPerMinute { get; set; }
    }
}
