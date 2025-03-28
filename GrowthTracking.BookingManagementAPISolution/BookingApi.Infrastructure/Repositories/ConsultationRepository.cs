using BookingApi.Application.DTOs;
using BookingApi.Application.Interfaces;
using BookingApi.Domain.Entities;
using BookingApi.Infrastructure.Data;
using GrowthTracking.ShareLibrary.Response;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApi.Infrastructure.Repositories
{
    public class ConsultationRepository : IConsultationRepository
    {
        private readonly BookingDbContext _context;

        public ConsultationRepository(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<Response> CreateConsultationAsync(ConsultationDTO consultationDto)
        {
            var consultation = consultationDto.Adapt<Consultation>();
            if (consultation.Id == Guid.Empty)
                consultation.Id = Guid.NewGuid();

            consultation.CreatedAt = DateTime.UtcNow;
            consultation.UpdatedAt = DateTime.UtcNow;

            _context.Consultations.Add(consultation);
            await _context.SaveChangesAsync();
            return new Response(true, "Consultation created successfully");
        }

        public async Task<Response> UpdateConsultationAsync(ConsultationDTO consultationDto)
        {
            if (consultationDto.Id == null)
                return new Response(false, "Consultation Id is required for update");

            var consultation = await _context.Consultations.FindAsync(consultationDto.Id.Value);
            if (consultation == null)
                return new Response(false, "Consultation not found");

            consultationDto.Adapt(consultation);
            consultation.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return new Response(true, "Consultation updated successfully");
        }

        public async Task<ConsultationDTO?> GetConsultationAsync(Guid consultationId)
        {
            var consultation = await _context.Consultations.FindAsync(consultationId);
            return consultation?.Adapt<ConsultationDTO>();
        }

        public async Task<IEnumerable<ConsultationDTO>> GetConsultationsByDoctorAsync(Guid doctorId)
        {
            var consultations = await _context.Consultations
                .Where(c => c.DoctorId == doctorId)
                .ToListAsync();
            return consultations.Adapt<IEnumerable<ConsultationDTO>>();
        }

        public async Task<IEnumerable<ConsultationDTO>> GetConsultationsByBookingAsync(Guid bookingId)
        {
            var consultations = await _context.Consultations
                .Where(c => c.BookingId == bookingId)
                .ToListAsync();
            return consultations.Adapt<IEnumerable<ConsultationDTO>>();
        }

        public async Task<Response> CancelConsultationAsync(Guid consultationId)
        {
            var consultation = await _context.Consultations.IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == consultationId);
            if (consultation == null)
                return new Response(false, "Consultation not found");

            consultation.StatusDelete = true;
            consultation.Status = "cancelled";
            consultation.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return new Response(true, "Consultation cancelled successfully");
        }
    }
}