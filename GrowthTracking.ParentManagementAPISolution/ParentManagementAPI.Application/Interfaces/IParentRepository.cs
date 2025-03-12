using ParentManageApi.Application.DTOs;
using ParentManageApi.Domain.Entities;
using GrowthTracking.ShareLibrary.Response;

namespace ParentManageApi.Application.Interfaces
{
    public interface IParentRepository
    {
        Task<Response> CreateParent(Parent parent);
        Task<Response> UpdateParent(Parent parent);
        Task<ParentDTO?> GetParent(Guid parentId);
        Task<IEnumerable<ParentDTO>> GetAllParents();
        Task<Response> DeleteParent(Guid parentId);
    }
}