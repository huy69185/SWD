using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Builder;
using eCommerce.ShareLibrary.Middleware;

namespace eCommerce.ShareLibrary.DependencyInjection
{
    public static class ShareServiceContainer
    {
        public static IServiceCollection AddSharedService<TContext>
            (this IServiceCollection services, IConfiguration config, string fileName) where TContext : DbContext
        {
            //Add Generic Db context
            services.AddDbContext<TContext>(option => option.UseSqlServer(
                config
                .GetConnectionString("eCommerceConnection"), sqlserverOption =>
                sqlserverOption.EnableRetryOnFailure()));
            //configure serilog logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel
                .Information()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(path: $"{fileName}-.text", 
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                outputTemplate:"Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day).CreateLogger();
            //add JWt authentication scheme
            JWTAuthencationScheme.AddJWTAuthencationScheme(services, config);
            return services;
        }
        public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
        {
            //Use global exception
            app.UseMiddleware<GlobalException>();
            //Register middleware to block all outsiders API calls
            app.UseMiddleware<ListenToOnlyApiGateway>();
            return app;
        } 
    }
}
