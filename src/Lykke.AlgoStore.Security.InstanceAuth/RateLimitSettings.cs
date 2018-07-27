namespace Lykke.AlgoStore.Security.InstanceAuth
{
    /// <summary>
    /// Rate limiting configuration
    /// </summary>
    public class RateLimitSettings
    {
        /// <summary>
        /// Maximum number of requests allowed every timeframe
        /// </summary>
        public int MaximumRequestsPerTimeframe { get; set; }

        /// <summary>
        /// The duration of each timeframe of requests
        /// </summary>
        public uint TimeframeDurationInSeconds { get; set; }

        /// <summary>
        /// Whether 5xx errors returned from the server should be counted towards the rate limit
        /// </summary>
        public bool Count5xxTowardRateLimit { get; set; }
    }
}
