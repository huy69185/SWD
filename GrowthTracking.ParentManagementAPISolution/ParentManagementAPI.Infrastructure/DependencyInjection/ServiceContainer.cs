using ParentManageApi.Application.Interfaces;
using ParentManageApi.Infrastructure.Data;
using ParentManageApi.Infrastructure.Repositories;
using GrowthTracking.ShareLibrary.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using ParentManageApi.Infrastructure.Mapping;

namespace ParentManageApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ParentManageDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            services.AddScoped<IParentRepository, ParentRepository>();

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