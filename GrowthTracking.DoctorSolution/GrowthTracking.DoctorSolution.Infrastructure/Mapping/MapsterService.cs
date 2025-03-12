using GrowthTracking.DoctorSolution.Application.Mapping;
using Mapster;

namespace GrowthTracking.DoctorSolution.Infrastructure.Mapping
{
    public class MapsterService : IMapperService
    {
        public TDestination Map<TSource, TDestination>(TSource source) => source.Adapt<TDestination>();
    }
}
