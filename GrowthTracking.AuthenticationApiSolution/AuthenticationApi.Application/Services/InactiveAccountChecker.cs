using AuthenticationApi.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace AuthenticationApi.Application.Services
{
    public class InactiveAccountChecker(IUserRepository userRepository, ILogger<InactiveAccountChecker> logger)
    {
        public async Task CheckInactiveAccountsAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Checking for unverified accounts...");
            var unverifiedUsers = await userRepository.GetUnverifiedUsersAsync();
            foreach (var user in unverifiedUsers)
            {
                if (user.CreatedAt.HasValue && (DateTime.UtcNow - user.CreatedAt.Value).TotalHours > 24 && !user.EmailVerified.GetValueOrDefault())
                {
                    user.IsActive = false;
                    await userRepository.UpdateUser(user.UserAccountID, user.FullName ?? string.Empty);
                    logger.LogInformation($"Deactivated account for user {user.Email} due to unverified email.");
                }
            }
        }
    }
}