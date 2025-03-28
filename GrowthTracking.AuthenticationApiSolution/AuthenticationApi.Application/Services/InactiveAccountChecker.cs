using AuthenticationApi.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace AuthenticationApi.Application.Services
{
    public class InactiveAccountChecker
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<InactiveAccountChecker> _logger;

        public InactiveAccountChecker(IUserRepository userRepository, ILogger<InactiveAccountChecker> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task CheckInactiveAccountsAsync(CancellationToken cancellationToken)
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
    }
}