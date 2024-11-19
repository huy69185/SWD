using eCommerce.ShareLibrary.Abstract;
using eCommerce.ShareLibrary.Interface;
using eCommerce.ShareLibrary.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProductAPI.DAO.Interface
{
    public interface IBaseDAO<T> where T : BaseEntity
    {
        Task<Response> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task<Response> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, CancellationToken cancellationToken = default);
        Task<T?> GetAsync(Guid id, string? includeProperties = null, CancellationToken cancellationToken = default);
        Task<T?> GetAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetWithPaginationAsync(int pageNum = 0, int pageSize = 0, Expression<Func<T, bool>>? filter = null, string? includeProperties = null, CancellationToken cancellationToken = default);
        Task<Response> DeleteAsync(Guid id);
        Task<Response> UpdateAsync(T entity);
        Task<Response> DeleteAsync(params T[] entities);
        Task<Response> DeleteAsync(T entity);
        Task<Response> UpdateRangeAsync(IEnumerable<T> entities);
        
    }
}
