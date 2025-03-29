using MassTransit;

namespace GrowthTracking.DoctorSolution.Application.MessageQueue
{
    public interface IDoctorEventPublisher
    {
        Task PublishDoctorCreatedAsync(Guid doctorId, string fullName);
    }

    public class DoctorEventPublisher(IPublishEndpoint publishEndpoint) : IDoctorEventPublisher
    {
        public async Task PublishDoctorCreatedAsync(Guid doctorId, string fullName)
        {
            var doctorEvent = new DoctorEvent
            {
                DoctorId = doctorId,
                FullName = fullName,
                EventType = "DoctorCreated"
            };

            await publishEndpoint.Publish(doctorEvent);
        }
    }
}
