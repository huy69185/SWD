using System;

namespace ParentManageApi.Application.Messaging
{
    public interface IEventPublisher
    {
        void PublishParentCreated(Guid parentId, string fullName);
        void PublishParentUpdated(Guid parentId, string fullName);
        void Dispose();
    }
}