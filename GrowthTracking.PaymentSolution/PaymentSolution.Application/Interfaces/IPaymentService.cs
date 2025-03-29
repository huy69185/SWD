using PaymentSolution.Application.DTOs;
using System.Threading.Tasks;

namespace PaymentSolution.Application.Interfaces
{
    public interface IPaymentService
    {
        /// <summary>
        /// Creates a payment order
        /// </summary>
        /// <param name="request">Payment order request</param>
        /// <returns>Payment order details including checkout URL</returns>
        Task<PaymentOrderResponse> CreatePaymentOrderAsync(CreatePaymentOrderRequest request);
        
        /// <summary>
        /// Gets information about an existing payment order
        /// </summary>
        /// <param name="orderId">The order ID to retrieve</param>
        /// <returns>Payment order details</returns>
        Task<object> GetPaymentOrderAsync(string orderId);

        /// <summary>
        /// Verifies a payment callback from PayOS
        /// </summary>
        /// <param name="payload">The callback payload</param>
        /// <returns>True if the signature is valid, false otherwise</returns>
        Task<bool> VerifyPaymentCallback(string payload);
    }
}