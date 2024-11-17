using System.Linq.Expressions;
using eCommerce.ShareLibrary.Response;
namespace eCommerce.ShareLibrary.Interface
{
    using eCommerce.ShareLibrary.Response;
    using System.Linq.Expressions;

    public interface IGenericInterface<T> where T : class
    {
        Task<Response> CreateAsync(T entity);
        Task<Response> UpdateAsync(T entity);
        Task<Response> DeleteAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> FindByIdAsync(int id);
        Task<T> GetByAsync(Expression<Func<T, bool>> predicate);
    }
}
