using GrowthTracking.ShareLibrary.Pagination;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace GrowthTracking.DoctorSolution.Infrastructure.Mapping
{
    public static class MapsterConfiguration
    {
        public static IServiceCollection AddMapsterConfiguration(this IServiceCollection services)
        {
            TypeAdapterConfig.GlobalSettings
                .ForType<string, Guid>()
                .MapWith(src => Guid.Parse(src));

            services.AddSingleton(TypeAdapterConfig.GlobalSettings);
            return services;
        }
    }
}
