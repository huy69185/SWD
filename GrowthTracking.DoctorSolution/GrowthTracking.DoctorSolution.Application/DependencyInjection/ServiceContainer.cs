using GrowthTracking.DoctorSolution.Application.MessageQueue;
using GrowthTracking.DoctorSolution.Application.Services;
using GrowthTracking.DoctorSolution.Application.Services.Interfaces;
using GrowthTracking.ShareLibrary.Logs;
using MassTransit;
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

            // Register MassTransit with RabbitMQ and our consumer
            services.AddMassTransit(x =>
            {

                // Configure RabbitMQ as the transport
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                });
            });
            services.AddMassTransitHostedService();

            // Add services here
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<IIdentityDocumentService, IdentityDocumentService>();
            services.AddScoped<IDoctorEventPublisher, DoctorEventPublisher>();
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
