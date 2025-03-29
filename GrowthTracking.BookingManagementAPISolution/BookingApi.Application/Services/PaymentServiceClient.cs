using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using GrowthTracking.ShareLibrary.Logs;
using BookingApi.Application.Interfaces;

namespace BookingApi.Application.Services
{
    public class PaymentServiceClient : IPaymentServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _paymentServiceBaseUrl;

        public PaymentServiceClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _paymentServiceBaseUrl = configuration["PaymentService:BaseUrl"] ?? "http://localhost:5002"; 
        }

        public async Task<bool> HasSuccessfulTransactionAsync(Guid bookingId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_paymentServiceBaseUrl}/api/payment/booking/{bookingId}/has-successful-transaction");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<bool>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return result;
            }
            catch (Exception ex)
            {
                LogHandler.LogExceptions(ex);
                return false;
            }
        }
    }
}