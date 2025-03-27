using BookingApi.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using GrowthTracking.ShareLibrary.Logs;

namespace BookingApi.Application.Services
{
    public class PaymentDeadlineChecker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PaymentDeadlineChecker> _logger;

        public PaymentDeadlineChecker(
            IServiceScopeFactory scopeFactory,
            ILogger<PaymentDeadlineChecker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Checking for bookings with expired payment deadlines...");
                    LogHandler.LogToConsole("PaymentDeadlineChecker: Starting check for expired payment deadlines");

                    // Tạo một scope mới để truy cập các dịch vụ Scoped
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
                        var paymentServiceClient = scope.ServiceProvider.GetRequiredService<IPaymentServiceClient>();

                        // Lấy tất cả các booking có trạng thái "confirmed"
                        var confirmedBookings = await bookingRepository.GetConfirmedBookingsAsync();

                        foreach (var booking in confirmedBookings)
                        {
                            // Kiểm tra nếu PaymentDeadline đã hết hạn
                            if (booking.PaymentDeadline.HasValue && booking.PaymentDeadline < DateTime.UtcNow)
                            {
                                // Gọi Payment Service để kiểm tra xem booking đã có giao dịch thành công chưa
                                var hasSuccessfulTransaction = await paymentServiceClient.HasSuccessfulTransactionAsync(booking.Id.Value);

                                if (!hasSuccessfulTransaction)
                                {
                                    // Nếu chưa thanh toán, hủy booking
                                    var response = await bookingRepository.CancelBookingAsync(booking.Id.Value);
                                    if (response.Flag)
                                    {
                                        _logger.LogInformation($"Booking {booking.Id} has been cancelled due to expired payment deadline.");
                                        LogHandler.LogToConsole($"PaymentDeadlineChecker: Booking {booking.Id} cancelled due to expired payment deadline");
                                    }
                                    else
                                    {
                                        _logger.LogWarning($"Failed to cancel booking {booking.Id}: {response.Message}");
                                        LogHandler.LogToDebugger($"PaymentDeadlineChecker: Failed to cancel booking {booking.Id}: {response.Message}");
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while checking payment deadlines.");
                    LogHandler.LogExceptions(ex);
                }

                // Chờ 1 giờ trước khi kiểm tra lại
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}