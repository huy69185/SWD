using GrowthTracking.ShareLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentSolution.Application.Interfaces;
using PaymentSolution.Infrastructure.DBContext;
using PaymentSolution.Infrastructure.Payment;
using PaymentSolution.Infrastructure.Repositories;

namespace PaymentSolution.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSharedDbContext<SWD_GrowthTrackingSystemDbContext>(configuration);
            // Register PayOS configuration
            services.Configure<PayOsOptions>(configuration.GetSection(PayOsOptions.SectionName));
            

            // Register PayOS service
            services.AddScoped<IPaymentService, PayOsService>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            return services;
        }

        public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
        {
            //Register middleware => global exception, listen to only api gateway
            ShareServiceContainer.UseSharedPolicies(app);
            return app;
        }
    }
}