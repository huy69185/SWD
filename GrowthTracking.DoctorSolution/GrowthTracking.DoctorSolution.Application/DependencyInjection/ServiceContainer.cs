using GrowthTracking.DoctorSolution.Application.Services;
using GrowthTracking.DoctorSolution.Application.Services.Interfaces;
using GrowthTracking.ShareLibrary.Logs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Retry;

namespace GrowthTracking.DoctorSolution.Application.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration config)
        {
            services.AddRetryStrategyForClient(config);

            // Add services here
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<ICertificateService, CertificateService>();
            return services;
        }

        private static void AddRetryStrategyForClient(this IServiceCollection services, IConfiguration config)
        {
            services.AddHttpClient<IUserService, UserService>(option =>
            {
                option.BaseAddress = new Uri(config["ApiGatewayUrl"]);
                option.Timeout = TimeSpan.FromSeconds(5);
            });
            // Create a Retry Strategy
            var retryStrategy = new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<TaskCanceledException>(),
                BackoffType = DelayBackoffType.Constant,
                UseJitter = true,
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromMilliseconds(500),
                OnRetry = args =>
                {
                    string message = $"OnRetry: Attempts={args.AttemptNumber} Outcome={args.Outcome}";
                    LogHandler.LogToConsole(message);
                    LogHandler.LogToDebugger(message);
                    return ValueTask.CompletedTask;
                }
            };

            // Use the Retry Strategy
            services.AddResiliencePipeline("my-retry-pipeline", builder =>
            {
                builder.AddRetry(retryStrategy);
            });
        }
    }
}
