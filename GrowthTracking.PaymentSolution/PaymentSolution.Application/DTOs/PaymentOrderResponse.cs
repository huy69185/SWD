namespace PaymentSolution.Application.DTOs
{
    public class PaymentOrderResponse
    {
        /// <summary>
        /// Order ID in your system
        /// </summary>
        public string OrderId { get; set; } = string.Empty;
        
        /// <summary>
        /// Payment ID in PayOS system
        /// </summary>
        public string PaymentId { get; set; } = string.Empty;
        
        /// <summary>
        /// Checkout URL where users should be redirected to complete payment
        /// </summary>
        public string CheckoutUrl { get; set; } = string.Empty;
        
        /// <summary>
        /// QR code data URL (base64) if available
        /// </summary>
        public string? QrCodeUrl { get; set; }
        
        /// <summary>
        /// Payment status (PENDING, COMPLETED, CANCELED, etc.)
        /// </summary>
        public string Status { get; set; } = string.Empty;
        
        /// <summary>
        /// Order amount in VND
        /// </summary>
        public long Amount { get; set; }
        
        /// <summary>
        /// Order description
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Creation timestamp
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Last updated timestamp
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
        
        
        /// <summary>
        /// True if payment has been completed, otherwise false
        /// </summary>
        public bool IsPaymentCompleted => Status.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase);
    }
}