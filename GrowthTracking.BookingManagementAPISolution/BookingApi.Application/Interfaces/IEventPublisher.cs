using System;

namespace BookingApi.Application.Interfaces
{
    public interface IEventPublisher
    {
        void PublishBookingCreated(Guid bookingId, Guid parentId, Guid childId, Guid doctorId);
        void PublishConsultationScheduled(Guid consultationId, Guid bookingId, Guid doctorId);
        void PublishBookingCancelled(Guid bookingId, Guid parentId, Guid childId, Guid doctorId); // Thêm sự kiện mới
        void PublishConsultationCancelled(Guid consultationId, Guid bookingId, Guid doctorId); // Thêm sự kiện mới
        void Dispose();
    }
}