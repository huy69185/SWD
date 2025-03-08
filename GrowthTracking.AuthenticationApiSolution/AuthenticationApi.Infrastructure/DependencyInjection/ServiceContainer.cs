﻿using AuthenticationApi.Application.Interfaces;
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