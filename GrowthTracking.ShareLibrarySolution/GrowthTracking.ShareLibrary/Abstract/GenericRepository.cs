using GrowthTracking.ShareLibrary.Interface;
using System.Linq.Expressions;

namespace GrowthTracking.ShareLibrary.Abstract
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        public Task<Response.Response> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Response.Response> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Response.Response> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Response.Response> DeleteAsync(params T[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<Response.Response> DeleteAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task<T?> FindByIdAsync<TKey>(TKey id, string? includeProperties = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<T?> GetAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> GetWithPaginationAsync(int pageNum = 0, int pageSize = 0, Expression<Func<T, bool>>? filter = null, string? includeProperties = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Response.Response> UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task<Response.Response> UpdateRangeAsync(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }
    }
}
