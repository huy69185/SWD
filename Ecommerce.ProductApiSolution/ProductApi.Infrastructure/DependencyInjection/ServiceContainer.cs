using eCommerce.ShareLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Application.Interfaces;
using ProductApi.Infrastructure.Repositories;
using ProductAPI.DAO.DbInitialize;
using ProductAPI.DAO.Implementation;
using ProductAPI.DAO.Interface;
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
            //Add authentication scheme
            ShareServiceContainer.AddSharedService(services, config, config["MySerilog:FineName"]!);

            //DAO dependency
            services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddScoped<IProductDAO, ProductDAO>();

            //Repo dependency
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
