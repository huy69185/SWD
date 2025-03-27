using Microsoft.AspNetCore.Mvc;
using PaymentSolution.Application.DTOs;
using PaymentSolution.Application.Interfaces;
using PaymentSolution.Domain.Entities;
using System.Text;

namespace PaymentSolution.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController(
        IPaymentService paymentService,
        ILogger<PaymentController> logger) : ControllerBase
    {
        [HttpPost("create-order")]
        public async Task<IActionResult> CreatePaymentOrder([FromBody] CreatePaymentOrderRequest request)
        {
            // Validate the request
            if (request.Amount <= 0)
            {
                return BadRequest("Amount must be greater than zero");
            }

            if (string.IsNullOrEmpty(request.OrderId))
            {
                // Generate a unique order ID if not provided
                request.OrderId = DateTime.Now.ToString("yyyyMMddHHmmss");
            }

            // Create the payment order
            var response = await paymentService.CreatePaymentOrderAsync(request);

            return Ok(response);
        }

        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetPaymentOrder(string orderId)
        {
            var order = await paymentService.GetPaymentOrderAsync(orderId);
            return Ok(order);
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> PaymentWebhook()
        {
            // Read the request body
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var payload = await reader.ReadToEndAsync();

            // Verify the signature
            if (! await paymentService.VerifyPaymentCallback(payload))
            {
                logger.LogWarning("Invalid PayOS webhook signature received");
                return BadRequest("Invalid signature");
            }

            return Ok(new { message = "Webhook received successfully" });
        }
    }
}