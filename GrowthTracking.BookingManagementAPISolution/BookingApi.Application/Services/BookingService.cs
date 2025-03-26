using BookingApi.Application.DTOs;
using BookingApi.Application.Interfaces;
using GrowthTracking.ShareLibrary.Response;
using GrowthTracking.ShareLibrary.Logs;
using Mapster;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApi.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IEventPublisher _eventPublisher;

        public BookingService(IBookingRepository bookingRepository, IScheduleRepository scheduleRepository, IEventPublisher eventPublisher)
        {
            _bookingRepository = bookingRepository;
            _scheduleRepository = scheduleRepository;
            _eventPublisher = eventPublisher;
        }

        public async Task<Response> CreateBookingAsync(BookingDTO bookingDto)
        {
            LogHandler.LogToFile($"BookingService: Starting CreateBookingAsync for BookingId: {bookingDto.Id}");

            // Validate schedule availability and conflicts
            var schedule = await _scheduleRepository.GetScheduleAsync(bookingDto.ScheduleId);
            if (schedule == null)
            {
                LogHandler.LogToDebugger($"BookingService: Schedule with ScheduleId: {bookingDto.ScheduleId} not found");
                return new Response(false, "Schedule not found");
            }

            if (!schedule.IsAvailable)
            {
                LogHandler.LogToDebugger($"BookingService: Schedule with ScheduleId: {bookingDto.ScheduleId} is not available");
                return new Response(false, "Schedule is not available");
            }

            var hasConflict = await _bookingRepository.CheckScheduleConflictAsync(bookingDto.ScheduleId);
            if (hasConflict)
            {
                LogHandler.LogToDebugger($"BookingService: Schedule conflict detected for ScheduleId: {bookingDto.ScheduleId}");
                return new Response(false, "Schedule conflict detected");
            }

            // Set default values
            bookingDto = bookingDto with
            {
                Status = "pending",
                BookingDate = DateTime.UtcNow,
                DoctorConfirmationDeadline = DateTime.UtcNow.AddDays(1) // 1 day to confirm
            };

            var response = await _bookingRepository.CreateBookingAsync(bookingDto);
            if (response.Flag && bookingDto.Id.HasValue)
            {
                LogHandler.LogToConsole($"BookingService: Booking created successfully, publishing BookingCreated event for BookingId: {bookingDto.Id}");
                _eventPublisher.PublishBookingCreated(bookingDto.Id.Value, bookingDto.ParentId, bookingDto.ChildId, schedule.DoctorId);
            }
            else
            {
                LogHandler.LogToDebugger($"BookingService: Failed to create booking. Reason: {response.Message}");
            }

            return response;
        }

        public async Task<Response> UpdateBookingAsync(BookingDTO bookingDto)
        {
            LogHandler.LogToFile($"BookingService: Starting UpdateBookingAsync for BookingId: {bookingDto.Id}");
            if (bookingDto.Id == null)
            {
                LogHandler.LogToDebugger("BookingService: Booking Id is required for update");
                return new Response(false, "Booking Id is required for update");
            }

            var existingBooking = await _bookingRepository.GetBookingAsync(bookingDto.Id.Value);
            if (existingBooking == null)
            {
                LogHandler.LogToDebugger($"BookingService: Booking with BookingId: {bookingDto.Id} not found");
                return new Response(false, "Booking not found");
            }

            var response = await _bookingRepository.UpdateBookingAsync(bookingDto);
            if (response.Flag)
            {
                LogHandler.LogToConsole($"BookingService: Booking updated successfully for BookingId: {bookingDto.Id}");
            }
            else
            {
                LogHandler.LogToDebugger($"BookingService: Failed to update booking with BookingId: {bookingDto.Id}. Reason: {response.Message}");
            }

            return response;
        }

        public async Task<BookingDTO?> GetBookingAsync(Guid bookingId)
        {
            LogHandler.LogToFile($"BookingService: Starting GetBookingAsync for BookingId: {bookingId}");
            var booking = await _bookingRepository.GetBookingAsync(bookingId);
            if (booking != null)
            {
                LogHandler.LogToConsole($"BookingService: Successfully retrieved booking with BookingId: {bookingId}");
            }
            else
            {
                LogHandler.LogToDebugger($"BookingService: Booking with BookingId: {bookingId} not found");
            }
            return booking;
        }

        public async Task<IEnumerable<BookingDTO>> GetBookingsByParentAsync(Guid parentId)
        {
            LogHandler.LogToFile($"BookingService: Starting GetBookingsByParentAsync for ParentId: {parentId}");
            var bookings = await _bookingRepository.GetBookingsByParentAsync(parentId);
            LogHandler.LogToConsole($"BookingService: Successfully retrieved bookings for ParentId: {parentId}");
            return bookings;
        }

        public async Task<IEnumerable<BookingDTO>> GetBookingsByDoctorAsync(Guid doctorId)
        {
            LogHandler.LogToFile($"BookingService: Starting GetBookingsByDoctorAsync for DoctorId: {doctorId}");
            var bookings = await _bookingRepository.GetBookingsByDoctorAsync(doctorId);
            LogHandler.LogToConsole($"BookingService: Successfully retrieved bookings for DoctorId: {doctorId}");
            return bookings;
        }

        public async Task<Response> CancelBookingAsync(Guid bookingId)
        {
            LogHandler.LogToFile($"BookingService: Starting CancelBookingAsync for BookingId: {bookingId}");
            var booking = await _bookingRepository.GetBookingAsync(bookingId);
            if (booking == null)
            {
                LogHandler.LogToDebugger($"BookingService: Booking with BookingId: {bookingId} not found");
                return new Response(false, "Booking not found");
            }

            var response = await _bookingRepository.CancelBookingAsync(bookingId);
            if (response.Flag)
            {
                LogHandler.LogToConsole($"BookingService: Successfully cancelled booking with BookingId: {bookingId}");
                var schedule = await _scheduleRepository.GetScheduleAsync(booking.ScheduleId);
                if (schedule != null)
                {
                    _eventPublisher.PublishBookingCancelled(bookingId, booking.ParentId, booking.ChildId, schedule.DoctorId);
                }
            }
            else
            {
                LogHandler.LogToDebugger($"BookingService: Failed to cancel booking with BookingId: {bookingId}. Reason: {response.Message}");
            }
            return response;
        }
    }
}