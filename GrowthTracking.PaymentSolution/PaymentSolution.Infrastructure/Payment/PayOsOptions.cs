namespace PaymentSolution.Infrastructure.Payment
{
    /// <summary>
    /// Configuration options for PayOS integration
    /// </summary>
    public class PayOsOptions
    {
        /// <summary>
        /// The section name in configuration
        /// </summary>
        public const string SectionName = "PayOS";

        /// <summary>
        /// API key for authenticating with PayOS
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Client ID for authenticating with PayOS
        /// </summary>
        public string ClientId { get; set; } = string.Empty;

        /// <summary>
        /// Checksum key for verifying PayOS signatures
        /// </summary>
        public string ChecksumKey { get; set; } = string.Empty;

        /// <summary>
        /// Base URL for PayOS API endpoints
        /// </summary>
        public string BaseUrl { get; set; } = "https://api.payos.vn";

        /// <summary>
        /// Timeout in seconds for API requests
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// Maximum number of retry attempts for failed requests
        /// </summary>
        public int MaxRetryAttempts { get; set; } = 3;
    }
}
