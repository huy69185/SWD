using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using GrowthTracking.ShareLibrary.Middleware;

namespace GrowthTracking.ShareLibrary.DependencyInjection
{
    public static class ShareServiceContainer
    {
        public static IServiceCollection AddSharedService
            (this IServiceCollection services, IConfiguration config, string fileName)
        {
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            string logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Log");
            string logFilePath = Path.Combine(logDirectory, $"{fileName}-{currentDate}.txt");

            // Ensure the Log directory exists
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            // Configure Serilog for detailed logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(
                    path: logFilePath,
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    shared: true)
                .CreateLogger();
            Log.Information("Application has started.");

            // Add JWT authentication scheme
            JWTAuthencationScheme.AddJWTAuthencationScheme(services, config);
            Log.Information("JWT Authentication scheme has been added.");

            return services;
        }

        public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
        {
            // Use global exception handling middleware
            app.UseMiddleware<GlobalException>();
            Log.Information("Global exception handling middleware has been added.");

            // Register middleware to log request details
            app.UseMiddleware<RequestLoggingMiddleware>();
            Log.Information("Request logging middleware has been added.");

            // Log application shutdown
            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                Log.Information("Application is shutting down.");
                Log.CloseAndFlush();
            };

            return app;
        }

        public static IServiceCollection AddSharedDbContext<TContext>
            (this IServiceCollection services, IConfiguration config) where TContext : DbContext
        {
            // Add Generic DbContext
            services.AddDbContext<TContext>(option => option.UseSqlServer(
                config.GetConnectionString("DefaultConnection"),
                sqlserverOption =>
                {
                    sqlserverOption.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30), 
                        errorNumbersToAdd: null);
                    Log.Debug("Configured SQL Server with retry on failure.");
                }));

            return services;
        }
    }

    // Middleware to log request details
    public class RequestLoggingMiddleware(RequestDelegate next)
    {
        private static int _requestCounter = 0; // Counter to keep track of request number

        public async Task InvokeAsync(HttpContext context)
        {
            int requestNumber = ++_requestCounter;
            Log.Information("Received {Method} request for {Path}", context.Request.Method, context.Request.Path);
            await next(context);
            Log.Information("Completed {Method} request for {Path}", context.Request.Method, context.Request.Path);
            Log.Information("Request {RequestNumber} completed on {Date}", requestNumber, DateTime.Now.ToString("yyyy-MM-dd"));
        }
    }
}
