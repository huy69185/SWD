using GrowthTracking.ShareLibrary.Filter;
using System.Text.Json.Serialization;

namespace GrowthTracking.DoctorSolution.Presentation.Extensions
{
    public static class ApiConfigurationExtensions
    {
        public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
        {
            // Configure Controllers
            services.AddControllers(options =>
            {
                // Add filters globally
                options.Filters.Add<ValidationFilter>();
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                // Suppress ModelState Invalid Filter to avoid automatic BadRequest responses
                options.SuppressModelStateInvalidFilter = true;
            })
            .AddJsonOptions(options =>
            {
                // Avoid circular references in JSON serialization
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            return services;
        }
    }
}
