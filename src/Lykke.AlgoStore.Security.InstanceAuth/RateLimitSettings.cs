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
        /// Set of status codes which won't be counted by the rate limiter
        /// </summary>
        public ushort[] StatusCodesToIgnore { get; set; }
    }
}
