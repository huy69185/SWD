using BookingApi.Application.DTOs;
using GrowthTracking.ShareLibrary.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApi.Application.Interfaces
{
    public interface IConsultationService
    {
        Task<Response> CreateConsultationAsync(ConsultationDTO consultationDto);
        Task<Response> UpdateConsultationAsync(ConsultationDTO consultationDto);
        Task<ConsultationDTO?> GetConsultationAsync(Guid consultationId);
        Task<IEnumerable<ConsultationDTO>> GetConsultationsByDoctorAsync(Guid doctorId);
        Task<IEnumerable<ConsultationDTO>> GetConsultationsByBookingAsync(Guid bookingId);
        Task<Response> CancelConsultationAsync(Guid consultationId);
    }
}