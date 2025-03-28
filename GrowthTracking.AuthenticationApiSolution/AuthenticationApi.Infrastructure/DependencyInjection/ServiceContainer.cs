using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Application.Messaging;
using AuthenticationApi.Application.Services;
using AuthenticationApi.Infrastructure.Data;
using AuthenticationApi.Infrastructure.Mapping;
using AuthenticationApi.Infrastructure.Repositories;
using GrowthTracking.ShareLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AuthenticationDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddSingleton<ParentEventConsumer>();
            services.AddHostedService<ParentEventConsumerHostedService>();
            services.AddScoped<InactiveAccountChecker>();
            services.AddHostedService<InactiveAccountCheckerHostedService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ISmsService, SmsService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ITokenService, JWTokenService>();
            services.AddHttpContextAccessor();

            services.AddMapsterConfiguration();
            return services;
        }

        public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
        {
            app.UseSharedPolicies();
            return app;
        }
    }
}