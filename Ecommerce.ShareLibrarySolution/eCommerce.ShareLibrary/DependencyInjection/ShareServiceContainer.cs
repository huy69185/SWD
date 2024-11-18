using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using eCommerce.ShareLibrary.Middleware;
using System.Threading.Tasks;

namespace eCommerce.ShareLibrary.DependencyInjection
{
    public static class ShareServiceContainer
    {
        public static IServiceCollection AddSharedService<TContext>
            (this IServiceCollection services, IConfiguration config, string fileName) where TContext : DbContext
        {
            // Get the current date and format it as 'yyyy-MM-dd'
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");

            // Define the log file path with the current date in the filename
            string logFilePath = $"{fileName}-{currentDate}.txt";

            // Configure Serilog for detailed logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug() // Log everything from Debug level and above
                .WriteTo.Debug() // Write logs to the Debug output window
                .WriteTo.Console() // Write logs to the Console
                .WriteTo.File(
                    path: logFilePath, // Use the constructed file path with date
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug, // Log everything from Debug level
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    shared: true) // Allow shared access to the log file
                .CreateLogger();

            // Log application startup
            Log.Information("Application has started.");

            // Add Generic DbContext
            services.AddDbContext<TContext>(option => option.UseSqlServer(
                config.GetConnectionString("eCommerceConnection"),
                sqlserverOption =>
                {
                    sqlserverOption.EnableRetryOnFailure();
                    Log.Debug("Configured SQL Server with retry on failure.");
                }));

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
    }

    // Middleware to log request details
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private static int _requestCounter = 0; // Counter to keep track of request number

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Increment the request counter
            int requestNumber = ++_requestCounter;

            // Log the HTTP method and request path
            Log.Information("Received {Method} request for {Path}", context.Request.Method, context.Request.Path);

            // Call the next middleware in the pipeline
            await _next(context);

            // Log when the request is completed
            Log.Information("Completed {Method} request for {Path}", context.Request.Method, context.Request.Path);

            // Add a separator line with the request number
            Log.Information("Request {RequestNumber} completed on {Date}", requestNumber, DateTime.Now.ToString("yyyy-MM-dd"));
        }
    }
}
