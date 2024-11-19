using eCommerce.ShareLibrary.Abstract;
using eCommerce.ShareLibrary.Logs;
using eCommerce.ShareLibrary.Response;
using Microsoft.EntityFrameworkCore;
using ProductAPI.DAO.Data;
using ProductAPI.DAO.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProductAPI.DAO.Implementation
{
    public class BaseDAO<T> : IBaseDAO<T> where T : BaseEntity
    {
        protected ProductDbContext _context;
        internal DbSet<T> _set;

        public BaseDAO()
        {
            _context = new ProductDbContext();
            _set = _context.Set<T>();
        }

        public async Task<Response> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            try
            {
                LogHandler.LogToFile($"[BaseDAO][AddAsync] Start adding new entity to database");

                Response response = new Response(false, $"Error occured while adding new entity");

                if (entity != null)
                {
                    await _context.AddAsync(entity, cancellationToken);
                    bool isSuccess = _context.SaveChangesAsync(cancellationToken).GetAwaiter().GetResult() > 0;
                    if (isSuccess)
                    {
                        response = new Response(true, $"{entity.Id} is added to database successfully");
                    }
                }

                LogHandler.LogToFile($"[BaseDAO][AddAsync] End adding new entity to database");
                return response;
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogHandler.LogExceptions(ex);
                return new Response(false, ex.Message);
            }
        }

        public async Task<Response> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            try
            {
                LogHandler.LogToFile($"[BaseDAO][AddRangeAsync] Start adding new entities to database");
                Response response = new Response(false, $"Error occured while adding new entity");

                if (entities.Any())
                {
                    await _context.AddRangeAsync(entities, cancellationToken);
                    bool isSuccess = _context.SaveChangesAsync(cancellationToken).GetAwaiter().GetResult() == entities.Count();
                    if (isSuccess)
                    {
                        response = new Response(true, $"{entities.Count()} is added to database successfully");
                    }
                }
                
                LogHandler.LogToFile($"[BaseDAO][AddRangeAsync] End adding new entities to database");
                return response;
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogHandler.LogExceptions(ex);
                return new Response(false, ex.Message);
            }
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
            try
            {
                LogHandler.LogToFile($"[BaseDAO][CountAsync] Start counting entities in database");
                int result = 0;
                IQueryable<T> query = _set;
                if (filter != null)
                {
                    query = query.Where(filter);
                }
                result = await query.CountAsync(cancellationToken);
                LogHandler.LogToFile($"[BaseDAO][CountAsync] End counting entities in database");

                return result;
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogHandler.LogExceptions(ex);
                return 0;
            }
        }

        public async Task<Response> DeleteAsync(params T[] entities)
        {
            try
            {
                LogHandler.LogToFile($"[BaseDAO][DeleteAsync] Start deleting entities from database");
                Response response = new Response(false, $"Error occured while deleting entities"); ;
                if (entities != null)
                {
                    _context.RemoveRange(entities);
                    bool isSuccess = _context.SaveChangesAsync().GetAwaiter().GetResult() == entities.Length;
                    if (isSuccess)
                    {
                        response = new Response(true, $"{entities.Length} is deleted successfully");
                    }
                }
                
                LogHandler.LogToFile($"[BaseDAO][DeleteAsync] End deleting entities from database");
                return response;
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogHandler.LogExceptions(ex);
                return new Response(false, ex.Message);
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, CancellationToken cancellationToken = default)
        {
            try
            {
                LogHandler.LogToFile($"[BaseDAO][GetAllAsync] Start retrieving all entities from database");
                
                IQueryable<T> query = _set;
                if (filter != null)
                {
                    query = query.Where(filter);
                }
                if (!string.IsNullOrEmpty(includeProperties))
                {
                    foreach (var incluProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(incluProp);
                    }
                }
                var response = await query.ToListAsync(cancellationToken);
                LogHandler.LogToFile($"[BaseDAO][GetAllAsync] End retrieving all entities from database");
                return response;
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogHandler.LogExceptions(ex);
                return new List<T>();
            }
        }

        public async Task<T?> GetAsync(Guid id, string? includeProperties = null, CancellationToken cancellationToken = default)
        {
            try
            {
                LogHandler.LogToFile($"[BaseDAO][GetAsync] Start retrieving entity from database");

                IQueryable<T> query = _set;
                if (!string.IsNullOrEmpty(includeProperties))
                {
                    foreach (var incluProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(incluProp);
                    }
                }
                var response = await query.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

                LogHandler.LogToFile($"[BaseDAO][GetAsync] End retrieving entity from database");
                return response;
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogHandler.LogExceptions(ex);
                return null;
            }
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
        {
            try
            {
                LogHandler.LogToFile($"[BaseDAO][GetAsync] Start retrieving entity from database");

                var response = await _set.Where(filter).FirstOrDefaultAsync(cancellationToken);

                LogHandler.LogToFile($"[BaseDAO][GetAsync] End retrieving entity from database");
                return response;
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogHandler.LogExceptions(ex);
                return null;
            }
        }


        public async Task<IEnumerable<T>> GetWithPaginationAsync(int pageNum = 0, int pageSize = 0, Expression<Func<T, bool>>? filter = null, string? includeProperties = null, CancellationToken cancellationToken = default)
        {
            try
            {
                LogHandler.LogToFile($"[BaseDAO][GetWithPaginationAsync] Start retrieving entities with pagination from database");
                IQueryable<T> query = _set;

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                if (pageNum > 0 && pageSize > 0)
                {
                    query = query.Skip((pageNum - 1) * pageSize).Take(pageSize);
                }

                if (!string.IsNullOrEmpty(includeProperties))
                {
                    foreach (var incluProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(incluProp);
                    }
                }
                var response = await query.ToListAsync(cancellationToken);

                LogHandler.LogToFile($"[BaseDAO][GetWithPaginationAsync] End retrieving entities with pagination from database");
                return response;
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogHandler.LogExceptions(ex);
                return new List<T>();
            }
        }

        public async Task<Response> UpdateAsync(T entity)
        {
            try
            {
                LogHandler.LogToFile($"[BaseDAO][UpdateAsync] Start updating entity in database");
                Response response = new Response(false, $"Error occured while updating entity");
                if (entity != null)
                {
                    _context.Update(entity);
                    bool isSuccess = _context.SaveChangesAsync().GetAwaiter().GetResult() > 0;

                    if (isSuccess)
                    {
                        response = new Response(true, $"{entity.Id} is updated successfully");
                    }
                }

                LogHandler.LogToFile($"[BaseDAO][UpdateAsync] End updating entity in database");
                return response;
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogHandler.LogExceptions(ex);
                return new Response(false, ex.Message);
            }
        }

        public async Task<Response> UpdateRangeAsync(IEnumerable<T> entities)
        {
            try
            {
                LogHandler.LogToFile($"[BaseDAO][UpdateRangeAsync] Start updating entities in database");
                Response response = new Response(false, $"Error occured while updating entities");
                if (entities != null && entities.Any())
                {
                    _context.UpdateRange(entities);
                    bool isSuccess = _context.SaveChangesAsync().GetAwaiter().GetResult() == entities.Count();

                    if (isSuccess)
                    {
                        response = new Response(true, $"{entities.Count()} is updated successfully");
                    }
                }

                LogHandler.LogToFile($"[BaseDAO][UpdateRangeAsync] End updating entities in database");
                return response;
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogHandler.LogExceptions(ex);
                return new Response(false, ex.Message);
            }
        }
        public async Task<Response> DeleteAsync(Guid id)
        {
            try
            {
                LogHandler.LogToFile($"[BaseDAO][DeleteAsync] Start deleting entity from database");
                Response response = new Response(false, $"Error occured while deleting entity");
                var contest = await GetAsync(id);
                if (contest != null)
                {
                    _context.Remove(contest);
                    bool isSuccess = _context.SaveChangesAsync().GetAwaiter().GetResult() > 0;

                    if (isSuccess)
                    {
                        response = new Response(true, $"{contest.Id} is deleted successfully");
                    }
                }

                LogHandler.LogToFile($"[BaseDAO][DeleteAsync] End deleting entity from database");
                return response;
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogHandler.LogExceptions(ex);
                return new Response(false, ex.Message);
            }
        }

        public async Task<Response> DeleteAsync(T entity)
        {
            try
            {
                LogHandler.LogToFile($"[BaseDAO][DeleteAsync] Start deleting entity from database");
                Response response = new Response(false, $"Error occured while deleting entity");

                _context.Remove(entity);
                bool isSuccess = _context.SaveChangesAsync().GetAwaiter().GetResult() > 0;

                if (isSuccess)
                {
                    response = new Response(true, $"{entity.Id} is deleted successfully");
                }

                LogHandler.LogToFile($"[BaseDAO][DeleteAsync] End deleting entity from database");
                return response;
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogHandler.LogExceptions(ex);
                return new Response(false, ex.Message);
            }
        }
    }
}
