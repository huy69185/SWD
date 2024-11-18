using eCommerce.ShareLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Application.Interfaces;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
        {
            //Add db connect
            //Add authentication scheme
            ShareServiceContainer.AddSharedService<ProductDbContext>(services, config, config["MySerilog:FineName"]!);

            //Create dependency
            services.AddScoped<IProduct, ProductRepository>();
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
