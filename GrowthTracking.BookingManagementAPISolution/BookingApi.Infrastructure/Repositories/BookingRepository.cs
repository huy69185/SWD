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
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingDbContext _context;

        public BookingRepository(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<Response> CreateBookingAsync(BookingDTO bookingDto)
        {
            var booking = bookingDto.Adapt<Booking>();
            if (booking.Id == Guid.Empty)
                booking.Id = Guid.NewGuid();

            booking.CreatedAt = DateTime.UtcNow;
            booking.UpdatedAt = DateTime.UtcNow;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return new Response(true, "Booking created successfully");
        }

        public async Task<Response> UpdateBookingAsync(BookingDTO bookingDto)
        {
            if (bookingDto.Id == null)
                return new Response(false, "Booking Id is required for update");

            var booking = await _context.Bookings.FindAsync(bookingDto.Id.Value);
            if (booking == null)
                return new Response(false, "Booking not found");

            bookingDto.Adapt(booking);
            booking.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return new Response(true, "Booking updated successfully");
        }

        public async Task<BookingDTO?> GetBookingAsync(Guid bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            return booking?.Adapt<BookingDTO>();
        }

        public async Task<IEnumerable<BookingDTO>> GetBookingsByParentAsync(Guid parentId)
        {
            var bookings = await _context.Bookings
                .Where(b => b.ParentId == parentId)
                .ToListAsync();
            return bookings.Adapt<IEnumerable<BookingDTO>>();
        }

        public async Task<IEnumerable<BookingDTO>> GetBookingsByDoctorAsync(Guid doctorId)
        {
            var bookings = await _context.Bookings
                .Join(_context.Schedules,
                    b => b.ScheduleId,
                    s => s.Id,
                    (b, s) => new { Booking = b, Schedule = s })
                .Where(bs => bs.Schedule.DoctorId == doctorId)
                .Select(bs => bs.Booking)
                .ToListAsync();
            return bookings.Adapt<IEnumerable<BookingDTO>>();
        }

        public async Task<Response> CancelBookingAsync(Guid bookingId)
        {
            var booking = await _context.Bookings.IgnoreQueryFilters()
                .FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking == null)
                return new Response(false, "Booking not found");

            booking.StatusDelete = true;
            booking.Status = "cancelled";
            booking.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return new Response(true, "Booking cancelled successfully");
        }

        public async Task<bool> CheckScheduleConflictAsync(Guid scheduleId, Guid bookingId = default)
        {
            return await _context.Bookings
                .Where(b => b.ScheduleId == scheduleId && b.Id != bookingId && b.Status != "cancelled")
                .AnyAsync();
        }
    }
}