using eCommerce.ShareLibrary.Logs;
using eCommerce.ShareLibrary.Response;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using System.Linq.Expressions;


namespace ProductApi.Infrastructure.Repositories
{
    internal class ProductRepository(ProductDbContext context) : IProduct
    {
        public async Task<Response> CreateAsync(Product entity)
        {
            try
            {
                //Check if product is already exist
                var getProduct = await GetByAsync(_ => _.Name!.Equals(entity.Name));
                if (getProduct is not null && !string.IsNullOrEmpty(getProduct.Name))
                    return new Response(false, $"{entity.Name} is already added");
                var currentEntity = context.Products.Add(entity).Entity;
                await context.SaveChangesAsync();
                if (currentEntity is not null && currentEntity.Id > 0)
                    return new Response(true, $"{entity.Name} is added to database successfully");
                else return new Response(false, $"Error occured while adding {entity.Name}");
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptions(ex);
                //Display the scary message to client
                return new Response(false, "Error occurred adding new product");
            }
        }

        public async Task<Response> DeleteAsync(Product entity)
        {
            try
            {
                //check if product is not found
                var product = FindByIdAsync(entity.Id);
                if(product is null ) return new Response(false,$"{entity.Name} not found");

                context.Products.Remove(entity);
                await context.SaveChangesAsync();
                return new Response(true,$"{entity.Name} is deleted successfully");
            }
            catch(Exception ex)
            {
                //Log the original exception
                LogException.LogExceptions(ex);
                //Display the scary message to client
                return new Response(false, "Error occurred deleting product");
            }
        }

        public async Task<Product> FindByIdAsync(int id)
        {
            try
            {
                var product = await context.Products.FindAsync(id);
                return product is not null ? product : null!;
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptions(ex);
                //Display the scary message to client
                throw new Exception("Error occurred retrieving product");
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                var products = await context.Products.AsNoTracking().ToListAsync();
                return products is not null ? products : null!;
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptions(ex);
                //Display the scary message to client
                throw new InvalidOperationException("Error occurred retrieving product");
            }
        }

        public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
        {
            try 
            {
                var product = await context.Products.Where(predicate).FirstOrDefaultAsync()!;
                return product is not null ? product : null!;
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptions(ex);
                //Display the scary message to client
                throw new InvalidOperationException("Error occurred retrieving product");
            }
        }

        public async Task<Response> UpdateAsync(Product entity)
        {
            try
            {
                // Find product by id
                var existingProduct = await FindByIdAsync(entity.Id);
                if (existingProduct is null)
                {
                    return new Response(false, $"{entity.Name} not found");
                }

                // check duplicate name
                var duplicateProduct = await GetByAsync(p => p.Name == entity.Name && p.Id != entity.Id);
                if (duplicateProduct is not null)
                {
                    return new Response(false, $"A product with the name '{entity.Name}' already exists.");
                }

                // update product
                context.Entry(existingProduct).State = EntityState.Detached;
                context.Products.Update(entity);
                await context.SaveChangesAsync();
                return new Response(true, $"{entity.Name} is updated successfully");
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptions(ex);
                //Display the scary message to client
                return new Response(false, "Error occurred while updating product");
            }
        }

    }
}
