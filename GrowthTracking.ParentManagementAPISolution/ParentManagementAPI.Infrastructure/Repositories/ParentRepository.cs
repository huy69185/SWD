using ParentManageApi.Application.DTOs;
using ParentManageApi.Application.Interfaces;
using ParentManageApi.Domain.Entities;
using ParentManageApi.Infrastructure.Data;
using GrowthTracking.ShareLibrary.Response;
using Mapster;
using Microsoft.EntityFrameworkCore;
using GrowthTracking.ShareLibrary.Logs;

namespace ParentManageApi.Infrastructure.Repositories
{
    public class ParentRepository(ParentManageDbContext context) : IParentRepository
    {
        public async Task<Response> CreateParent(Parent parent)
        {
            LogHandler.LogToFile($"ParentRepository: Starting CreateParent for ParentId: {parent.ParentId}");
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
                    LogHandler.LogToConsole($"ParentRepository: Parent with ParentId: {parent.ParentId} restored successfully");
                    return new Response(true, "Parent restored successfully");
                }
                LogHandler.LogToDebugger($"ParentRepository: Parent with ParentId: {parent.ParentId} already exists");
                return new Response(false, "Parent already exists");
            }

            context.Parents.Add(parent);
            await context.SaveChangesAsync();
            LogHandler.LogToConsole($"ParentRepository: Parent with ParentId: {parent.ParentId} created successfully");
            return new Response(true, "Parent created successfully");
        }

        public async Task<Response> UpdateParent(Parent parent)
        {
            LogHandler.LogToFile($"ParentRepository: Starting UpdateParent for ParentId: {parent.ParentId}");
            var existingParent = await context.Parents.FirstOrDefaultAsync(p => p.ParentId == parent.ParentId);
            if (existingParent == null)
            {
                LogHandler.LogToDebugger($"ParentRepository: Parent with ParentId: {parent.ParentId} not found");
                return new Response(false, "Parent not found");
            }

            existingParent.FullName = parent.FullName;
            existingParent.DateOfBirth = parent.DateOfBirth;
            existingParent.Gender = parent.Gender;
            existingParent.Address = parent.Address;
            existingParent.AvatarUrl = parent.AvatarUrl;
            existingParent.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
            LogHandler.LogToConsole($"ParentRepository: Parent with ParentId: {parent.ParentId} updated successfully");
            return new Response(true, "Parent updated successfully");
        }

        public async Task<ParentDTO?> GetParent(Guid parentId)
        {
            LogHandler.LogToFile($"ParentRepository: Starting GetParent for ParentId: {parentId}");
            var parent = await context.Parents.FirstOrDefaultAsync(p => p.ParentId == parentId);
            if (parent == null)
            {
                LogHandler.LogToDebugger($"ParentRepository: Parent with ParentId: {parentId} not found");
            }
            else
            {
                LogHandler.LogToConsole($"ParentRepository: Successfully retrieved parent with ParentId: {parentId}");
            }
            return parent?.Adapt<ParentDTO>();
        }

        public async Task<IEnumerable<ParentDTO>> GetAllParents()
        {
            LogHandler.LogToFile("ParentRepository: Starting GetAllParents");
            var parents = await context.Parents.ToListAsync();
            LogHandler.LogToConsole("ParentRepository: Successfully retrieved all parents");
            return parents.Adapt<IEnumerable<ParentDTO>>();
        }

        public async Task<Response> DeleteParent(Guid parentId)
        {
            LogHandler.LogToFile($"ParentRepository: Starting DeleteParent for ParentId: {parentId}");
            var parent = await context.Parents.FirstOrDefaultAsync(p => p.ParentId == parentId);
            if (parent == null)
            {
                LogHandler.LogToDebugger($"ParentRepository: Parent with ParentId: {parentId} not found");
                return new Response(false, "Parent not found");
            }

            parent.IsDeleted = true;
            parent.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
            LogHandler.LogToConsole($"ParentRepository: Parent with ParentId: {parentId} deleted successfully (soft delete)");
            return new Response(true, "Parent deleted successfully (soft delete)");
        }
    }
}