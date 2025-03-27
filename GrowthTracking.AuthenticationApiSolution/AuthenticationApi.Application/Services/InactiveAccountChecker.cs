using AuthenticationApi.Application.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthenticationApi.Application.Services
{
    public class InactiveAccountChecker : BackgroundService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<InactiveAccountChecker> _logger;

        public InactiveAccountChecker(IUserRepository userRepository, ILogger<InactiveAccountChecker> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Checking for unverified accounts...");
                    var unverifiedUsers = await _userRepository.GetUnverifiedUsersAsync();
                    foreach (var user in unverifiedUsers)
                    {
                        if (user.CreatedAt.HasValue && (DateTime.UtcNow - user.CreatedAt.Value).TotalHours > 24 && !user.EmailVerified.GetValueOrDefault())
                        {
                            var updatedUser = user with { IsActive = false };
                            await _userRepository.UpdateUser(user.UserAccountID.Value, updatedUser.FullName);
                            _logger.LogInformation($"Deactivated account for user {user.Email} due to unverified email.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while checking unverified accounts.");
                }
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken); 
            }
        }
    }
}