using ParentManageApi.Application.DTOs;
using ParentManageApi.Application.Interfaces;
using ParentManageApi.Domain.Entities;
using ParentManageApi.Application.Messaging;
using GrowthTracking.ShareLibrary.Response;
using Mapster;
using GrowthTracking.ShareLibrary.Logs;

namespace ParentManageApi.Application.Services
{
    public class ParentService(IParentRepository parentRepository, IEventPublisher eventPublisher) : IParentService
    {
        public async Task<Response> CreateParentAsync(ParentDTO parentDTO, Guid parentId)
        {
            LogHandler.LogToFile($"ParentService: Starting CreateParentAsync for ParentId: {parentId}");
            var parent = parentDTO.Adapt<Parent>();
            parent.ParentId = parentId;
            parent.CreatedAt = DateTime.UtcNow;
            parent.UpdatedAt = DateTime.UtcNow;
            parent.IsDeleted = false;

            var response = await parentRepository.CreateParent(parent);
            if (response.Flag)
            {
                LogHandler.LogToConsole($"ParentService: Parent created successfully, publishing ParentCreated event for ParentId: {parentId}");
                eventPublisher.PublishParentCreated(parentId, parentDTO.FullName);
            }
            else
            {
                LogHandler.LogToDebugger($"ParentService: Failed to create parent with ParentId: {parentId}. Reason: {response.Message}");
            }
            return response;
        }

        public async Task<Response> UpdateParentAsync(ParentDTO parentDTO, Guid parentId)
        {
            LogHandler.LogToFile($"ParentService: Starting UpdateParentAsync for ParentId: {parentId}");
            var parent = parentDTO.Adapt<Parent>();
            parent.ParentId = parentId;
            parent.UpdatedAt = DateTime.UtcNow;

            var response = await parentRepository.UpdateParent(parent);
            if (response.Flag)
            {
                LogHandler.LogToConsole($"ParentService: Parent updated successfully, publishing ParentUpdated event for ParentId: {parentId}");
                eventPublisher.PublishParentUpdated(parentId, parentDTO.FullName);
            }
            else
            {
                LogHandler.LogToDebugger($"ParentService: Failed to update parent with ParentId: {parentId}. Reason: {response.Message}");
            }
            return response;
        }

        public async Task<ParentDTO?> GetParentAsync(Guid parentId)
        {
            LogHandler.LogToFile($"ParentService: Starting GetParentAsync for ParentId: {parentId}");
            var parent = await parentRepository.GetParent(parentId);
            if (parent != null)
            {
                LogHandler.LogToConsole($"ParentService: Successfully retrieved parent with ParentId: {parentId}");
            }
            else
            {
                LogHandler.LogToDebugger($"ParentService: Parent with ParentId: {parentId} not found");
            }
            return parent;
        }

        public async Task<IEnumerable<ParentDTO>> GetAllParentsAsync()
        {
            LogHandler.LogToFile("ParentService: Starting GetAllParentsAsync");
            var parents = await parentRepository.GetAllParents();
            LogHandler.LogToConsole("ParentService: Successfully retrieved all parents");
            return parents;
        }

        public async Task<Response> DeleteParentAsync(Guid parentId)
        {
            LogHandler.LogToFile($"ParentService: Starting DeleteParentAsync for ParentId: {parentId}");
            var response = await parentRepository.DeleteParent(parentId);
            if (response.Flag)
            {
                LogHandler.LogToConsole($"ParentService: Successfully deleted parent with ParentId: {parentId}");
            }
            else
            {
                LogHandler.LogToDebugger($"ParentService: Failed to delete parent with ParentId: {parentId}. Reason: {response.Message}");
            }
            return response;
        }
    }
}