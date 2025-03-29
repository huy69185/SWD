using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentSolution.Application.Interfaces;
using Net.payOS;
using Net.payOS.Types;
using PaymentSolution.Application.DTOs;
using Transaction = PaymentSolution.Domain.Entities.Transaction;
using System.Threading.Tasks;

namespace PaymentSolution.Infrastructure.Payment
{
    public class PayOsService : IPaymentService
    {
        private readonly PayOS _payOs;
        private readonly PayOsOptions _payOsConfig;
        private readonly ILogger<PayOsService> _logger;
        private readonly IGenericRepository<Transaction> _transactionRepo;

        public PayOsService(
            IOptions<PayOsOptions> options,
            ILogger<PayOsService> logger,
            IGenericRepository<Transaction> repository)
        {
            _transactionRepo = repository;
            _payOsConfig = options.Value;
            _logger = logger;

            // Initialize PayOS client
            _payOs = new PayOS(
                clientId: _payOsConfig.ClientId,
                apiKey: _payOsConfig.ApiKey,
                checksumKey: _payOsConfig.ChecksumKey
            );
        }

        public async Task<PaymentOrderResponse> CreatePaymentOrderAsync(CreatePaymentOrderRequest request)
        {
            try
            {
                _logger.LogInformation("Creating payment order for OrderId: {OrderId}, Amount: {Amount}", 
                    request.OrderId, request.Amount);

                // Convert our request to PayOS request format
                var payOsItems = request.Items.Select(i => new ItemData(i.Name, i.Quantity, i.Price)).ToList();

                var payOsRequest = new PaymentData(
                    orderCode: long.Parse(request.OrderId),
                    amount: (int)request.Amount,
                    description: request.Description,
                    items: payOsItems,
                    cancelUrl: request.CancelUrl,
                    returnUrl: request.ReturnUrl,
                    signature: null,
                    buyerName: request.CustomerInfo?.Name,
                    buyerEmail: request.CustomerInfo?.Email,
                    buyerPhone: request.CustomerInfo?.Phone,
                    buyerAddress: null,
                    expiredAt: DateTimeOffset.Now.AddHours(48).ToUnixTimeSeconds()
                );

                // Create payment with PayOS
                var payOsResponse = await _payOs.createPaymentLink(payOsRequest);

                // Save the transaction to the database
                var transaction = new Transaction
                {
                    TransactionId = Guid.NewGuid(),
                    ReferenceCode = request.OrderId,
                    Amount = request.Amount,
                    Status = payOsResponse.status,
                    CreatedAt = DateTime.Now,
                    TransactionDate = DateTime.Now,
                    PaymentMethod = "PayOS",
                    BookingId = request.CustomerInfo?.BookingId ?? Guid.Empty,
                    ParentId = request.CustomerInfo?.ParentId ?? Guid.Empty
                };

                await _transactionRepo.InsertAsync(transaction);
                await _transactionRepo.SaveAsync();

                // Map to our response model
                var response = new PaymentOrderResponse
                {
                    OrderId = request.OrderId,
                    PaymentId = payOsResponse.paymentLinkId,
                    CheckoutUrl = payOsResponse.checkoutUrl,
                    QrCodeUrl = payOsResponse.qrCode,
                    Status = payOsResponse.status,
                    Amount = request.Amount,
                    Description = request.Description,
                    CreatedAt = DateTime.Now
                };

                _logger.LogInformation("Successfully created payment order. PaymentId: {PaymentId}, CheckoutUrl: {CheckoutUrl}",
                    response.PaymentId, response.CheckoutUrl);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment order: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<object> GetPaymentOrderAsync(string orderId)
        {
            try
            {
                _logger.LogInformation("Getting payment order details for OrderId: {OrderId}", orderId);
                
                // Get payment order from PayOS
                var payOsOrder = await _payOs.getPaymentLinkInformation(long.Parse(orderId));

                if (payOsOrder == null)
                {
                    _logger.LogWarning("Payment order not found: {OrderId}", orderId);
                    throw new KeyNotFoundException($"Payment order not found: {orderId}");
                }

                _logger.LogInformation("Successfully retrieved payment order. Status: {Status}", payOsOrder.status);

                return payOsOrder;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment order: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<bool> VerifyPaymentCallback(string payload)
        {
            try
            {
                _logger.LogInformation("Verifying payment callback signature");

                // Deserialize payload to WebhookType
                var webhook = Newtonsoft.Json.JsonConvert.DeserializeObject<WebhookType>(payload);
                if (webhook == null)
                {
                    _logger.LogWarning("Failed to deserialize payload to WebhookType");
                    return false;
                }

                // Use PayOS SDK for verification
                WebhookData data = _payOs.verifyPaymentWebhookData(webhook);
                if (webhook.success)
                {
                    // Process the webhook payload
                    // TODO: Update order status in your database based on the webhook data
                    _logger.LogInformation("Received valid PayOS webhook: {Payload}", payload);
                    var transactions = await _transactionRepo.GetAsync(t => t.ReferenceCode == data.orderCode.ToString());
                    var transaction = transactions.FirstOrDefault();
                    if (transaction == null)
                    {
                        _logger.LogWarning("Transaction not found for OrderId: {OrderId}", data.orderCode);
                        return false;
                    }
                    //if (data.)
                    transaction.Status = "PAID";
                    await _transactionRepo.UpdateAsync(transaction);
                    await _transactionRepo.SaveAsync();

                    return true;

                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying payment callback: {Message}", ex.Message);
                return false;
            }
        }
    }
}