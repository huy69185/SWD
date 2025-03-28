using System;

namespace BookingApi.Application.DTOs
{
    public record ConsultationDTO(
        Guid? Id,
        Guid BookingId,
        Guid DoctorId,
        DateTime ConsultationDate,
        string? ConsultationNotes,
        string Status
    );
}