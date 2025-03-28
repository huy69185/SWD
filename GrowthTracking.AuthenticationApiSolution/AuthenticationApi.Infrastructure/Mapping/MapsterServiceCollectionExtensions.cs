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
                .ForType<string, Guid>()
                .MapWith(src => Guid.Parse(src));

            TypeAdapterConfig<AppUser, AppUserDTO>.NewConfig().MapToConstructor(true);
            TypeAdapterConfig<BugReport, BugReportDTO>.NewConfig().MapToConstructor(true);
            TypeAdapterConfig<Notification, NotificationDTO>.NewConfig().MapToConstructor(true);

            services.AddSingleton(TypeAdapterConfig.GlobalSettings);
            return services;
        }
    }
}