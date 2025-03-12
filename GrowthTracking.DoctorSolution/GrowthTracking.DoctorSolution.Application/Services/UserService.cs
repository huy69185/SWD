using GrowthTracking.DoctorSolution.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Polly.Registry;

namespace GrowthTracking.DoctorSolution.Application.Services
{
    public class UserService(
        HttpClient httpClient, 
        ResiliencePipelineProvider<string> resiliencePipeline) : IUserService
    {
        public async Task<bool> CheckUserExists(string userId)
        {
            // Get retry pipeline
            var retryPipeline = resiliencePipeline.GetPipeline("my-retry-pipeline");

            //Prepare response
            var response = await retryPipeline.ExecuteAsync(
                async token => await httpClient.GetAsync($"/api/users/{userId}", token));

            return response.IsSuccessStatusCode;
        }
    }
}
