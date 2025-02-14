using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Infrastructure.Data;
using AuthenticationApi.Infrastructure.Mapping;
using AuthenticationApi.Infrastructure.Repositories;
using eCommerce.ShareLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            //Add database connectivity
            //JWT Add Authentication Scheme
            ShareServiceContainer.AddSharedService(services, config, config["MySerilog:FileName"]!);
            ShareServiceContainer.AddSharedDbContext<AuthenticationDbContext>(services, config);

            //Dependency Injection
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddMapsterConfiguration();
            return services;
        }

        public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
        {
            // Register middleware such as:
            // Global Exception : Handle external errors
            // Listen Only To Api Gateway : block all outsiders call.
            ShareServiceContainer.UseSharedPolicies(app);
            return app;
        }
    }
}
