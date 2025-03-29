using BookingApi.Application.DTOs;
using GrowthTracking.ShareLibrary.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApi.Application.Interfaces
{
    public interface IBookingRepository
    {
        Task<Response> CreateBookingAsync(BookingDTO bookingDto);
        Task<Response> UpdateBookingAsync(BookingDTO bookingDto);
        Task<BookingDTO?> GetBookingAsync(Guid bookingId);
        Task<IEnumerable<BookingDTO>> GetBookingsByParentAsync(Guid parentId);
        Task<IEnumerable<BookingDTO>> GetBookingsByDoctorAsync(Guid doctorId);
        Task<Response> CancelBookingAsync(Guid bookingId);
        Task<bool> CheckScheduleConflictAsync(Guid scheduleId, Guid bookingId = default);
        Task<IEnumerable<BookingDTO>> GetConfirmedBookingsAsync();
    }
}