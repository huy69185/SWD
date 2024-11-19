using eCommerce.ShareLibrary.Logs;
using eCommerce.ShareLibrary.Response;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductAPI.DAO.Implementation;
using System.Linq.Expressions;

namespace ProductApi.Infrastructure.Repositories
{
    public class ProductRepository : IProduct
    {
        public Task<Response> AddAsync(Product entity, CancellationToken cancellationToken = default)
        {
            return ProductDAO.Instance.AddAsync(entity, cancellationToken);
        }

        public Task<Response> AddRangeAsync(IEnumerable<Product> entities, CancellationToken cancellationToken = default)
        {
            return ProductDAO.Instance.AddRangeAsync(entities, cancellationToken);
        }

        public Task<int> CountAsync(Expression<Func<Product, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
            return ProductDAO.Instance.CountAsync(filter, cancellationToken);
        }

        public Task<Response> DeleteAsync(Guid id)
        {
            return ProductDAO.Instance.DeleteAsync(id);
        }

        public Task<Response> DeleteAsync(params Product[] entities)
        {
            return ProductDAO.Instance.DeleteAsync(entities);
        }

        public Task<Response> DeleteAsync(Product entity)
        {
            return ProductDAO.Instance.DeleteAsync(entity);
        }

        public Task<IEnumerable<Product>> GetAllAsync(Expression<Func<Product, bool>>? filter = null, string? includeProperties = null, CancellationToken cancellationToken = default)
        {
            return ProductDAO.Instance.GetAllAsync(filter, includeProperties, cancellationToken);
        }

        public Task<Product?> FindByIdAsync(Guid id, string? includeProperties = null, CancellationToken cancellationToken = default)
        {
            return ProductDAO.Instance.GetAsync(id, includeProperties, cancellationToken);
        }

        public Task<Product?> GetAsync(Expression<Func<Product, bool>> filter, CancellationToken cancellationToken = default)
        {
            return ProductDAO.Instance.GetAsync(filter, cancellationToken);
        }

        public Task<IEnumerable<Product>> GetWithPaginationAsync(int pageNum = 0, int pageSize = 0, Expression<Func<Product, bool>>? filter = null, string? includeProperties = null, CancellationToken cancellationToken = default)
        {
            return ProductDAO.Instance.GetWithPaginationAsync(pageNum, pageSize, filter, includeProperties, cancellationToken);
        }

        public Task<Response> UpdateAsync(Product entity)
        {
            return ProductDAO.Instance.UpdateAsync(entity);
        }

        public Task<Response> UpdateRangeAsync(IEnumerable<Product> entities)
        {
            return ProductDAO.Instance.UpdateRangeAsync(entities);
        }
    }
}
