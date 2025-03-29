using BookingApi.Application.DTOs;
using BookingApi.Application.Interfaces;
using GrowthTracking.ShareLibrary.Response;
using GrowthTracking.ShareLibrary.Logs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApi.Application.Services
{
    public class ConsultationService : IConsultationService
    {
        private readonly IConsultationRepository _consultationRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IEventPublisher _eventPublisher;

        public ConsultationService(IConsultationRepository consultationRepository, IBookingRepository bookingRepository, IEventPublisher eventPublisher)
        {
            _consultationRepository = consultationRepository;
            _bookingRepository = bookingRepository;
            _eventPublisher = eventPublisher;
        }

        public async Task<Response> CreateConsultationAsync(ConsultationDTO consultationDto)
        {
            LogHandler.LogToFile($"ConsultationService: Starting CreateConsultationAsync for BookingId: {consultationDto.BookingId}");

            // Validate booking
            var booking = await _bookingRepository.GetBookingAsync(consultationDto.BookingId);
            if (booking == null)
            {
                LogHandler.LogToDebugger($"ConsultationService: Booking with BookingId: {consultationDto.BookingId} not found");
                return new Response(false, "Booking not found");
            }

            if (booking.Status != "confirmed")
            {
                LogHandler.LogToDebugger($"ConsultationService: Booking with BookingId: {consultationDto.BookingId} is not confirmed");
                return new Response(false, "Booking must be confirmed to create a consultation");
            }

            var response = await _consultationRepository.CreateConsultationAsync(consultationDto);
            if (response.Flag && consultationDto.Id.HasValue)
            {
                LogHandler.LogToConsole($"ConsultationService: Consultation created successfully, publishing ConsultationScheduled event for ConsultationId: {consultationDto.Id}");
                _eventPublisher.PublishConsultationScheduled(consultationDto.Id.Value, consultationDto.BookingId, consultationDto.DoctorId);
            }
            else
            {
                LogHandler.LogToDebugger($"ConsultationService: Failed to create consultation for BookingId: {consultationDto.BookingId}. Reason: {response.Message}");
            }

            return response;
        }

        public async Task<Response> UpdateConsultationAsync(ConsultationDTO consultationDto)
        {
            LogHandler.LogToFile($"ConsultationService: Starting UpdateConsultationAsync for ConsultationId: {consultationDto.Id}");
            if (consultationDto.Id == null)
            {
                LogHandler.LogToDebugger("ConsultationService: Consultation Id is required for update");
                return new Response(false, "Consultation Id is required for update");
            }

            var existingConsultation = await _consultationRepository.GetConsultationAsync(consultationDto.Id.Value);
            if (existingConsultation == null)
            {
                LogHandler.LogToDebugger($"ConsultationService: Consultation with ConsultationId: {consultationDto.Id} not found");
                return new Response(false, "Consultation not found");
            }

            var response = await _consultationRepository.UpdateConsultationAsync(consultationDto);
            if (response.Flag)
            {
                LogHandler.LogToConsole($"ConsultationService: Consultation updated successfully for ConsultationId: {consultationDto.Id}");
            }
            else
            {
                LogHandler.LogToDebugger($"ConsultationService: Failed to update consultation with ConsultationId: {consultationDto.Id}. Reason: {response.Message}");
            }

            return response;
        }

        public async Task<ConsultationDTO?> GetConsultationAsync(Guid consultationId)
        {
            LogHandler.LogToFile($"ConsultationService: Starting GetConsultationAsync for ConsultationId: {consultationId}");
            var consultation = await _consultationRepository.GetConsultationAsync(consultationId);
            if (consultation != null)
            {
                LogHandler.LogToConsole($"ConsultationService: Successfully retrieved consultation with ConsultationId: {consultationId}");
            }
            else
            {
                LogHandler.LogToDebugger($"ConsultationService: Consultation with ConsultationId: {consultationId} not found");
            }
            return consultation;
        }

        public async Task<IEnumerable<ConsultationDTO>> GetConsultationsByDoctorAsync(Guid doctorId)
        {
            LogHandler.LogToFile($"ConsultationService: Starting GetConsultationsByDoctorAsync for DoctorId: {doctorId}");
            var consultations = await _consultationRepository.GetConsultationsByDoctorAsync(doctorId);
            LogHandler.LogToConsole($"ConsultationService: Successfully retrieved consultations for DoctorId: {doctorId}");
            return consultations;
        }

        public async Task<IEnumerable<ConsultationDTO>> GetConsultationsByBookingAsync(Guid bookingId)
        {
            LogHandler.LogToFile($"ConsultationService: Starting GetConsultationsByBookingAsync for BookingId: {bookingId}");
            var consultations = await _consultationRepository.GetConsultationsByBookingAsync(bookingId);
            LogHandler.LogToConsole($"ConsultationService: Successfully retrieved consultations for BookingId: {bookingId}");
            return consultations;
        }

        public async Task<Response> CancelConsultationAsync(Guid consultationId)
        {
            LogHandler.LogToFile($"ConsultationService: Starting CancelConsultationAsync for ConsultationId: {consultationId}");
            var consultation = await _consultationRepository.GetConsultationAsync(consultationId);
            if (consultation == null)
            {
                LogHandler.LogToDebugger($"ConsultationService: Consultation with ConsultationId: {consultationId} not found");
                return new Response(false, "Consultation not found");
            }

            var response = await _consultationRepository.CancelConsultationAsync(consultationId);
            if (response.Flag)
            {
                LogHandler.LogToConsole($"ConsultationService: Successfully cancelled consultation with ConsultationId: {consultationId}");
                _eventPublisher.PublishConsultationCancelled(consultationId, consultation.BookingId, consultation.DoctorId);
            }
            else
            {
                LogHandler.LogToDebugger($"ConsultationService: Failed to cancel consultation with ConsultationId: {consultationId}. Reason: {response.Message}");
            }
            return response;
        }
    }
}