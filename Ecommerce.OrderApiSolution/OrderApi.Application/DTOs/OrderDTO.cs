using System;
using System.ComponentModel.DataAnnotations;

namespace OrderApi.Application.DTOs
{
    public record OrderDTO(
        int Id,

        [Required(ErrorMessage = "Product ID is required.")]
        string ProductId,

        [Required, Range(1, int.MaxValue, ErrorMessage = "Client ID must be at least 1.")]
        int ClientId,

        [Required, Range(1, int.MaxValue, ErrorMessage = "Purchase quantity must be at least 1.")]
        int PurchaseQuantity,

        [Required(ErrorMessage = "Order date is required.")]
        DateTime OrderDate
    );
}
