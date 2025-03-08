using ParentManageApi.Application.DTOs;
using ParentManageApi.Application.Interfaces;
using ParentManageApi.Domain.Entities;
using ParentManageApi.Infrastructure.Data;
using GrowthTracking.ShareLibrary.Response;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ParentManageApi.Infrastructure.Repositories
{
    public class ParentRepository(ParentManageDbContext context) : IParentRepository
    {
        // Tạo thông tin phụ huynh mới
        public async Task<Response> CreateParent(ParentDTO parentDTO)
        {
            var existingParent = await context.Parents.IgnoreQueryFilters() // Bỏ qua query filter để kiểm tra cả bản ghi đã xóa mềm
                .FirstOrDefaultAsync(p => p.ParentId == parentDTO.ParentId);
            if (existingParent != null)
            {
                if (existingParent.IsDeleted)
                {
                    // Nếu bản ghi đã bị xóa mềm, có thể khôi phục
                    existingParent.IsDeleted = false;
                    existingParent.FullName = parentDTO.FullName;
                    existingParent.DateOfBirth = parentDTO.DateOfBirth;
                    existingParent.Gender = parentDTO.Gender;
                    existingParent.Address = parentDTO.Address;
                    existingParent.AvatarUrl = parentDTO.AvatarUrl;
                    existingParent.UpdatedAt = DateTime.Now;
                    await context.SaveChangesAsync();
                    return new Response(true, "Parent restored successfully");
                }
                return new Response(false, "Parent already exists");
            }

            var parent = parentDTO.Adapt<Parent>();
            parent.CreatedAt = DateTime.Now;
            parent.UpdatedAt = DateTime.Now;
            parent.IsDeleted = false; // Đảm bảo bản ghi mới không bị đánh dấu xóa
            context.Parents.Add(parent);
            await context.SaveChangesAsync();
            return new Response(true, "Parent created successfully");
        }

        // Cập nhật thông tin phụ huynh
        public async Task<Response> UpdateParent(ParentDTO parentDTO)
        {
            var parent = await context.Parents.FirstOrDefaultAsync(p => p.ParentId == parentDTO.ParentId);
            if (parent == null)
            {
                return new Response(false, "Parent not found");
            }

            parent.FullName = parentDTO.FullName;
            parent.DateOfBirth = parentDTO.DateOfBirth;
            parent.Gender = parentDTO.Gender;
            parent.Address = parentDTO.Address;
            parent.AvatarUrl = parentDTO.AvatarUrl;
            parent.UpdatedAt = DateTime.Now;
            await context.SaveChangesAsync();
            return new Response(true, "Parent updated successfully");
        }

        // Lấy thông tin phụ huynh theo ID
        public async Task<ParentDTO?> GetParent(Guid parentId)
        {
            var parent = await context.Parents.FirstOrDefaultAsync(p => p.ParentId == parentId);
            return parent?.Adapt<ParentDTO>();
        }

        // Lấy danh sách tất cả phụ huynh
        public async Task<IEnumerable<ParentDTO>> GetAllParents()
        {
            var parents = await context.Parents.ToListAsync();
            return parents.Adapt<IEnumerable<ParentDTO>>();
        }

        // Xóa mềm thông tin phụ huynh
        public async Task<Response> DeleteParent(Guid parentId)
        {
            var parent = await context.Parents.FirstOrDefaultAsync(p => p.ParentId == parentId);
            if (parent == null)
            {
                return new Response(false, "Parent not found");
            }

            parent.IsDeleted = true; // Đánh dấu bản ghi là đã xóa mềm
            parent.UpdatedAt = DateTime.Now;
            await context.SaveChangesAsync();
            return new Response(true, "Parent deleted successfully (soft delete)");
        }

        // Lấy danh sách trẻ em của một phụ huynh
        public async Task<IEnumerable<ChildDTO>> GetChildrenByParent(Guid parentId)
        {
            var children = await context.Children
                .Where(c => c.ParentId == parentId)
                .ToListAsync();
            return children.Adapt<IEnumerable<ChildDTO>>();
        }
    }
}