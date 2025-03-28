using ParentManageApi.Application.DTOs;
using GrowthTracking.ShareLibrary.Response;

namespace ParentManageApi.Application.Interfaces
{
    public interface IParentService
    {
        Task<Response> CreateParentAsync(ParentDTO parentDTO, Guid parentId);
        Task<Response> UpdateParentAsync(ParentDTO parentDTO, Guid parentId);
        Task<ParentDTO?> GetParentAsync(Guid parentId);
        Task<IEnumerable<ParentDTO>> GetAllParentsAsync();
        Task<Response> DeleteParentAsync(Guid parentId);
    }
}