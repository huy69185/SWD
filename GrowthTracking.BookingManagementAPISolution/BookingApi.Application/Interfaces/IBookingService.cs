using BookingApi.Application.DTOs;
using GrowthTracking.ShareLibrary.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApi.Application.Interfaces
{
    public interface IBookingService
    {
        Task<Response> CreateBookingAsync(BookingDTO bookingDto);
        Task<Response> UpdateBookingAsync(BookingDTO bookingDto);
        Task<BookingDTO?> GetBookingAsync(Guid bookingId);
        Task<IEnumerable<BookingDTO>> GetBookingsByParentAsync(Guid parentId);
        Task<IEnumerable<BookingDTO>> GetBookingsByDoctorAsync(Guid doctorId);
        Task<Response> CancelBookingAsync(Guid bookingId);
    }
}