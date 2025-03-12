using GrowthTracking.DoctorSolution.Application.Services;
using GrowthTracking.DoctorSolution.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GrowthTracking.DoctorSolution.Application.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration config)
        {
            // Add services here
            services.AddHttpClient();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<ICertificateService, CertificateService>();
            return services;
        }
    }
}
