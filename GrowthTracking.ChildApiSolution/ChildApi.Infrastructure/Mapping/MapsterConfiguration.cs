using ChildApi.Application.DTOs;
using ChildApi.Domain.Entities;
using Mapster;

namespace ChildApi.Infrastructure.Mapping
{
    public static class MapsterConfiguration
    {
        // Đăng ký các mapping giữa Entity và DTO
        public static void RegisterMappings()
        {
            // Mapping từ Child sang ChildDTO và ngược lại
            TypeAdapterConfig<Child, ChildDTO>.NewConfig().MapToConstructor(true);
            TypeAdapterConfig<ChildDTO, Child>.NewConfig().MapToConstructor(true);

            // Mapping từ Milestone sang MilestoneDTO và ngược lại
            TypeAdapterConfig<Milestone, MilestoneDTO>.NewConfig().MapToConstructor(true);
            TypeAdapterConfig<MilestoneDTO, Milestone>.NewConfig().MapToConstructor(true);
        }
    }
}
