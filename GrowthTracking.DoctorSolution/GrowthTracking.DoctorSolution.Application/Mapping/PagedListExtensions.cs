using GrowthTracking.ShareLibrary.Pagination;

namespace GrowthTracking.DoctorSolution.Application.Mapping
{
    public static class PagedListExtensions
    {
        public static PagedList<TDestination> MapPagedList<TSource, TDestination>(
        this PagedList<TSource> source,
        IMapperService mapper)
        {
            var mappedItems = source
                .Select(mapper.Map<TSource, TDestination>)
                .ToList();
            return new PagedList<TDestination>(
                mappedItems,
                source.TotalCount,
                source.CurrentPage,
                source.PageSize);
        }
    }
}