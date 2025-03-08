using ParentManageApi.Application.DTOs;
using ParentManageApi.Application.Interfaces;
using ParentManageApi.Domain.Entities;
using ParentManageApi.Infrastructure.Data;
using GrowthTracking.ShareLibrary.Response;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace ParentManageApi.Infrastructure.Repositories
{
    public class ParentRepository(ParentManageDbContext context) : IParentRepository
    {
        public async Task<Response> CreateParent(Parent parent)
        {
            var existingParent = await context.Parents.IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.ParentId == parent.ParentId);
            if (existingParent != null)
            {
                if (existingParent.IsDeleted)
                {
                    existingParent.IsDeleted = false;
                    existingParent.FullName = parent.FullName;
                    existingParent.DateOfBirth = parent.DateOfBirth;
                    existingParent.Gender = parent.Gender;
                    existingParent.Address = parent.Address;
                    existingParent.AvatarUrl = parent.AvatarUrl;
                    existingParent.UpdatedAt = DateTime.UtcNow;
                    await context.SaveChangesAsync();
                    return new Response(true, "Parent restored successfully");
                }
                return new Response(false, "Parent already exists");
            }

            context.Parents.Add(parent);
            await context.SaveChangesAsync();
            return new Response(true, "Parent created successfully");
        }

        public async Task<Response> UpdateParent(Parent parent)
        {
            var existingParent = await context.Parents.FirstOrDefaultAsync(p => p.ParentId == parent.ParentId);
            if (existingParent == null)
                return new Response(false, "Parent not found");

            existingParent.FullName = parent.FullName;
            existingParent.DateOfBirth = parent.DateOfBirth;
            existingParent.Gender = parent.Gender;
            existingParent.Address = parent.Address;
            existingParent.AvatarUrl = parent.AvatarUrl;
            existingParent.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
            return new Response(true, "Parent updated successfully");
        }

        public async Task<ParentDTO?> GetParent(Guid parentId)
        {
            var parent = await context.Parents.FirstOrDefaultAsync(p => p.ParentId == parentId);
            return parent?.Adapt<ParentDTO>();
        }

        public async Task<IEnumerable<ParentDTO>> GetAllParents()
        {
            var parents = await context.Parents.ToListAsync();
            return parents.Adapt<IEnumerable<ParentDTO>>();
        }

        public async Task<Response> DeleteParent(Guid parentId)
        {
            var parent = await context.Parents.FirstOrDefaultAsync(p => p.ParentId == parentId);
            if (parent == null)
                return new Response(false, "Parent not found");

            parent.IsDeleted = true;
            parent.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
            return new Response(true, "Parent deleted successfully (soft delete)");
        }

        public async Task<IEnumerable<ChildDTO>> GetChildrenByParent(Guid parentId)
        {
            var children = await context.Children
                .Where(c => c.ParentId == parentId)
                .ToListAsync();
            return children.Adapt<IEnumerable<ChildDTO>>();
        }
    }
}