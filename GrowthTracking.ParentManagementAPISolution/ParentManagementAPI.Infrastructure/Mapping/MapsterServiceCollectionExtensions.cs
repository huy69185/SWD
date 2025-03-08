using ParentManageApi.Application.DTOs;
using ParentManageApi.Domain.Entities;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace ParentManageApi.Infrastructure.Mapping
{
    public static class MapsterServiceCollectionExtensions
    {
        public static IServiceCollection AddMapsterConfiguration(this IServiceCollection services)
        {
            TypeAdapterConfig.GlobalSettings
                .ForType<string, Guid>()
                .MapWith(src => Guid.Parse(src));

            TypeAdapterConfig<ParentDTO, Parent>.NewConfig()
                .Map(dest => dest.ParentId, src => Guid.Empty)
                .Map(dest => dest.CreatedAt, src => DateTime.UtcNow)
                .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow)
                .Map(dest => dest.IsDeleted, src => false);

            TypeAdapterConfig<Parent, ParentDTO>.NewConfig().MapToConstructor(true);
            TypeAdapterConfig<Child, ChildDTO>.NewConfig().MapToConstructor(true);

            services.AddSingleton(TypeAdapterConfig.GlobalSettings);
            return services;
        }
    }
}