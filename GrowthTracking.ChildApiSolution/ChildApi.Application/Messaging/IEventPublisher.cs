using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildApi.Application.Messaging
{
    public interface IEventPublisher
    {
        void PublishChildCreated(Guid childId, Guid parentId, string fullName);
        void Dispose();
    }
}
