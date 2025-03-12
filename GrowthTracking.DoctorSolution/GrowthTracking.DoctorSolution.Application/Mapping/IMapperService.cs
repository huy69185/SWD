namespace GrowthTracking.DoctorSolution.Application.Mapping
{
    public interface IMapperService
    {
        TDestination Map<TSource, TDestination>(TSource source);
    }
}
