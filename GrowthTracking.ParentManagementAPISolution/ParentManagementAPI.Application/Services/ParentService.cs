using ParentManageApi.Application.DTOs;
using ParentManageApi.Application.Interfaces;
using ParentManageApi.Domain.Entities;
using ParentManageApi.Application.Messaging;
using GrowthTracking.ShareLibrary.Response;
using Mapster;

namespace ParentManageApi.Application.Services
{
    public class ParentService(IParentRepository parentRepository, IEventPublisher eventPublisher) : IParentService
    {
        public async Task<Response> CreateParentAsync(ParentDTO parentDTO, Guid parentId)
        {
            var parent = parentDTO.Adapt<Parent>();
            parent.ParentId = parentId;
            parent.CreatedAt = DateTime.UtcNow;
            parent.UpdatedAt = DateTime.UtcNow;
            parent.IsDeleted = false;

            var response = await parentRepository.CreateParent(parent);
            if (response.Flag)
            {
                eventPublisher.PublishParentCreated(parentId, parentDTO.FullName);
            }
            return response;
        }

        public async Task<Response> UpdateParentAsync(ParentDTO parentDTO, Guid parentId)
        {
            var parent = parentDTO.Adapt<Parent>();
            parent.ParentId = parentId;
            parent.UpdatedAt = DateTime.UtcNow;

            var response = await parentRepository.UpdateParent(parent);
            if (response.Flag)
            {
                eventPublisher.PublishParentUpdated(parentId, parentDTO.FullName);
            }
            return response;
        }

        public async Task<ParentDTO?> GetParentAsync(Guid parentId)
        {
            return await parentRepository.GetParent(parentId);
        }

        public async Task<IEnumerable<ParentDTO>> GetAllParentsAsync()
        {
            return await parentRepository.GetAllParents();
        }

        public async Task<Response> DeleteParentAsync(Guid parentId)
        {
            return await parentRepository.DeleteParent(parentId);
        }

        public async Task<IEnumerable<ChildDTO>> GetChildrenByParentAsync(Guid parentId)
        {
            return await parentRepository.GetChildrenByParent(parentId);
        }
    }
}