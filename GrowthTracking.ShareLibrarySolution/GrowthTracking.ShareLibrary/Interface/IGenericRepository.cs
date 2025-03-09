namespace GrowthTracking.ShareLibrary.Interface
{
    using GrowthTracking.ShareLibrary.Response;
    using System.Linq.Expressions;

    public interface IGenericRepository<T> where T : class
    {
        Task<Response> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task<Response> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, CancellationToken cancellationToken = default);
        Task<T?> FindByIdAsync<TKey>(TKey id, string? includeProperties = null, CancellationToken cancellationToken = default);
        Task<T?> GetAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetWithPaginationAsync(int pageNum = 0, int pageSize = 0, Expression<Func<T, bool>>? filter = null, string? includeProperties = null, CancellationToken cancellationToken = default);
        Task<Response> DeleteAsync(Guid id);
        Task<Response> UpdateAsync(T entity);
        Task<Response> DeleteAsync(params T[] entities);
        Task<Response> DeleteAsync(T entity);
        Task<Response> UpdateRangeAsync(IEnumerable<T> entities);
    }
}
