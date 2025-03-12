using GrowthTracking.DoctorSolution.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace GrowthTracking.DoctorSolution.Application.Services
{
    public class UserService(HttpClient httpClient, IConfiguration configuration) : IUserService
    {
        public async Task<bool> CheckUserExists(string userId)
        {
            // The gateway abstracts the endpoint for the User service.
            var gatewayUrl = configuration["ApiGatewayUrl"];
            var response = await httpClient.GetAsync($"{gatewayUrl}/api/users/{userId}");
            return response.IsSuccessStatusCode;
        }
    }
}
