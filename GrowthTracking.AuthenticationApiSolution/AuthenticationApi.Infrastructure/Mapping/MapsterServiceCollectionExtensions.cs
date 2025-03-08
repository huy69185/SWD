using AuthenticationApi.Application.DTOs; // Thêm directive này
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

            // Cấu hình ánh xạ cho AppUser
            TypeAdapterConfig<AppUser, AppUserDTO>.NewConfig().MapToConstructor(true);

            // Cấu hình ánh xạ cho BugReport
            TypeAdapterConfig<BugReport, BugReportDTO>.NewConfig().MapToConstructor(true);

            // Cấu hình ánh xạ cho Notification
            TypeAdapterConfig<Notification, NotificationDTO>.NewConfig().MapToConstructor(true);

            services.AddSingleton(TypeAdapterConfig.GlobalSettings);
            return services;
        }
    }
}