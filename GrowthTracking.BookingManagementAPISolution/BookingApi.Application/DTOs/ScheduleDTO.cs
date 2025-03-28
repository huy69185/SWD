using System;

namespace BookingApi.Application.DTOs
{
    public record ScheduleDTO(
        Guid? Id,
        Guid DoctorId,
        DateTime StartTime,
        DateTime EndTime,
        string? Location,
        bool IsAvailable
    );
}