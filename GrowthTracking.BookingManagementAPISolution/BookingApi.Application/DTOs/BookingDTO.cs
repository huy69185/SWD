using System;

namespace BookingApi.Application.DTOs
{
    public record BookingDTO(
        Guid? Id,
        Guid ParentId,
        Guid ChildId,
        Guid ScheduleId,
        string Status,
        DateTime? BookingDate,
        DateTime? DoctorConfirmationDeadline,
        string? Notes
    );
}