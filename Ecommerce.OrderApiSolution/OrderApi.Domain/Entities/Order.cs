using System;
using System.ComponentModel.DataAnnotations;

namespace OrderApi.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product ID is required.")]
        public string ProductId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Client ID is required.")]
        public int ClientId { get; set; }

        [Required, Range(1, int.MaxValue, ErrorMessage = "Purchase quantity must be at least 1.")]
        public int PurchaseQuantity { get; set; }

        [Required(ErrorMessage = "Order date is required.")]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    }
}
