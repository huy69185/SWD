using ParentManageApi.Application.DTOs;
using GrowthTracking.ShareLibrary.Response;

namespace ParentManageApi.Application.Interfaces
{
    public interface IParentRepository
    {
        Task<Response> CreateParent(ParentDTO parentDTO);
        Task<Response> UpdateParent(ParentDTO parentDTO);
        Task<ParentDTO?> GetParent(Guid parentId);
        Task<IEnumerable<ParentDTO>> GetAllParents();
        Task<Response> DeleteParent(Guid parentId);
        Task<IEnumerable<ChildDTO>> GetChildrenByParent(Guid parentId);
    }
}