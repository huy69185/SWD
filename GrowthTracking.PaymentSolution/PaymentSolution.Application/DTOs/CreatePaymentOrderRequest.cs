namespace PaymentSolution.Application.DTOs
{
    public class CreatePaymentOrderRequest
    {
        /// <summary>
        /// Unique order ID from your system
        /// </summary>
        public string? OrderId { get; set; } = string.Empty;
        
        /// <summary>
        /// Order amount in VND (no decimal)
        /// </summary>
        public long Amount { get; set; }
        
        /// <summary>
        /// Description of the order
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// URL to redirect after payment (success or failure)
        /// </summary>
        public string ReturnUrl { get; set; } = string.Empty;
        
        /// <summary>
        /// Callback URL for PayOS to notify about payment status
        /// </summary>
        public string CancelUrl { get; set; } = string.Empty;
        
        /// <summary>
        /// Items included in this order
        /// </summary>
        public List<PaymentItem> Items { get; set; } = new();
        
        /// <summary>
        /// Customer information (optional)
        /// </summary>
        public CustomerInfo? CustomerInfo { get; set; }
    }
    
    public class PaymentItem
    {
        /// <summary>
        /// Name of the item
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Quantity of the item
        /// </summary>
        public int Quantity { get; set; }
        
        /// <summary>
        /// Price of the item in VND
        /// </summary>
        public int Price { get; set; }
    }
    
    public class CustomerInfo
    {
        public Guid ParentId { get; set; }
        public Guid BookingId { get; set; }
        /// <summary>
        /// Customer's name
        /// </summary>
        public string? Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Customer's email
        /// </summary>
        public string? Email { get; set; } = string.Empty;
        
        /// <summary>
        /// Customer's phone number
        /// </summary>
        public string? Phone { get; set; } = string.Empty;
    }
}