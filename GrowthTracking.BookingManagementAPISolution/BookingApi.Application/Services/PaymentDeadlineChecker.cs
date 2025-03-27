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
        private const int BaseRetryDelaySeconds = 5; // Thời gian chờ 
        private const int MaxConsecutiveFailures = 5; // Số lần thất bại liên tiếp tối đa trước khi dừng

        private int _consecutiveFailures = 0; //Couunt để tính tổng số lần thất bại

        public PaymentDeadlineChecker(
            IServiceScopeFactory scopeFactory,
            ILogger<PaymentDeadlineChecker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PaymentDeadlineChecker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Checking for bookings with expired payment deadlines...");
                    LogHandler.LogToConsole("PaymentDeadlineChecker: Starting check for expired payment deadlines");

                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
                        var paymentServiceClient = scope.ServiceProvider.GetRequiredService<IPaymentServiceClient>();

                        // Lấy tất cả các booking có trạng thái "confirmed"
                        var confirmedBookings = await bookingRepository.GetConfirmedBookingsAsync();

                        if (confirmedBookings == null || !confirmedBookings.Any())
                        {
                            _logger.LogInformation("No confirmed bookings found to check.");
                            LogHandler.LogToConsole("PaymentDeadlineChecker: No confirmed bookings found.");
                        }
                        else
                        {
                            foreach (var booking in confirmedBookings)
                            {
                                try
                                {
                                    // Kiểm tra nếu PaymentDeadline đã hết hạn chưa
                                    if (booking.PaymentDeadline.HasValue && booking.PaymentDeadline < DateTime.UtcNow)
                                    {
                                        _logger.LogInformation($"Processing booking {booking.Id} with PaymentDeadline {booking.PaymentDeadline}.");

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
                                        else
                                        {
                                            _logger.LogInformation($"Booking {booking.Id} has a successful transaction, skipping cancellation.");
                                            LogHandler.LogToConsole($"PaymentDeadlineChecker: Booking {booking.Id} has a successful transaction, skipping.");
                                        }
                                    }
                                    else
                                    {
                                        _logger.LogInformation($"Booking {booking.Id} has not reached PaymentDeadline yet or PaymentDeadline is null.");
                                    }
                                }
                                catch (Exception bookingEx)
                                {
                                    _logger.LogError(bookingEx, $"Error processing booking {booking.Id}.");
                                    LogHandler.LogExceptions(bookingEx);
                                    continue;
                                }
                            }
                        }

                        // Reset số lần thất bại liên tiếp nếu không có lỗi
                        _consecutiveFailures = 0;
                    }
                }
                catch (Exception ex)
                {
                    _consecutiveFailures++;
                    _logger.LogError(ex, $"Error while checking payment deadlines. Consecutive failures: {_consecutiveFailures}/{MaxConsecutiveFailures}");
                    LogHandler.LogExceptions(ex);

                    // Nếu số lần thất bại liên tiếp vượt quá ngưỡng, dừng service
                    if (_consecutiveFailures >= MaxConsecutiveFailures)
                    {
                        _logger.LogCritical($"PaymentDeadlineChecker stopped due to {MaxConsecutiveFailures} consecutive failures.");
                        LogHandler.LogToConsole($"PaymentDeadlineChecker: Stopped due to {MaxConsecutiveFailures} consecutive failures.");
                        return;
                    }

                    // Retry với exponential backoff
                    int delaySeconds = BaseRetryDelaySeconds * (int)Math.Pow(2, _consecutiveFailures - 1);
                    _logger.LogInformation($"Retrying after {delaySeconds} seconds...");
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds), stoppingToken);
                    continue;
                }

                // Chờ 1 giờ trước khi kiểm tra lại
                _logger.LogInformation("Completed checking payment deadlines. Waiting for 1 hour before the next check...");
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }

            _logger.LogInformation("PaymentDeadlineChecker stopped due to cancellation request.");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("PaymentDeadlineChecker is stopping...");
            await base.StopAsync(cancellationToken);
            _logger.LogInformation("PaymentDeadlineChecker stopped.");
        }
    }
}