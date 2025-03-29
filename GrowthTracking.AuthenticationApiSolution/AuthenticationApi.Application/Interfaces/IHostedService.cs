using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AuthenticationApi.Application.Services
{
    public class InactiveAccountCheckerHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public InactiveAccountCheckerHostedService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var checker = scope.ServiceProvider.GetRequiredService<InactiveAccountChecker>();
                        await checker.CheckInactiveAccountsAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    // Log lỗi nếu cần
                }
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}