using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Domain.Entities;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationApi.Infrastructure.Mapping
{
    public static class MapsterServiceCollectionExtensions
    {
        public static IServiceCollection AddMapsterConfiguration(this IServiceCollection services)
        {
            TypeAdapterConfig.GlobalSettings
                .ForType<AppUser, AppUserDTO>().MapToConstructor(true);

            services.AddSingleton(TypeAdapterConfig.GlobalSettings);
            return services;
        }
    }
}
