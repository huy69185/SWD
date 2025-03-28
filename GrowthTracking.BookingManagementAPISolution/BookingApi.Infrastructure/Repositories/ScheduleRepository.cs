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
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly BookingDbContext _context;

        public ScheduleRepository(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<Response> CreateScheduleAsync(ScheduleDTO scheduleDto)
        {
            var schedule = scheduleDto.Adapt<Schedule>();
            if (schedule.Id == Guid.Empty)
                schedule.Id = Guid.NewGuid();

            schedule.CreatedAt = DateTime.UtcNow;
            schedule.UpdatedAt = DateTime.UtcNow;

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();
            return new Response(true, "Schedule created successfully");
        }

        public async Task<Response> UpdateScheduleAsync(ScheduleDTO scheduleDto)
        {
            if (scheduleDto.Id == null)
                return new Response(false, "Schedule Id is required for update");

            var schedule = await _context.Schedules.FindAsync(scheduleDto.Id.Value);
            if (schedule == null)
                return new Response(false, "Schedule not found");

            scheduleDto.Adapt(schedule);
            schedule.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return new Response(true, "Schedule updated successfully");
        }

        public async Task<ScheduleDTO?> GetScheduleAsync(Guid scheduleId)
        {
            var schedule = await _context.Schedules.FindAsync(scheduleId);
            return schedule?.Adapt<ScheduleDTO>();
        }

        public async Task<IEnumerable<ScheduleDTO>> GetSchedulesByDoctorAsync(Guid doctorId, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Schedules
                .Where(s => s.DoctorId == doctorId);

            if (startDate.HasValue)
                query = query.Where(s => s.StartTime >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(s => s.EndTime <= endDate.Value);

            var schedules = await query.ToListAsync();
            return schedules.Adapt<IEnumerable<ScheduleDTO>>();
        }

        public async Task<Response> DeleteScheduleAsync(Guid scheduleId)
        {
            var schedule = await _context.Schedules.IgnoreQueryFilters()
                .FirstOrDefaultAsync(s => s.Id == scheduleId);
            if (schedule == null)
                return new Response(false, "Schedule not found");

            schedule.StatusDelete = true;
            schedule.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return new Response(true, "Schedule deleted successfully");
        }

        public async Task<bool> CheckDoctorScheduleConflictAsync(Guid doctorId, DateTime startTime, DateTime endTime, Guid scheduleId = default)
        {
            return await _context.Schedules
                .Where(s => s.DoctorId == doctorId && s.Id != scheduleId)
                .AnyAsync(s => (startTime >= s.StartTime && startTime < s.EndTime) ||
                               (endTime > s.StartTime && endTime <= s.EndTime) ||
                               (startTime <= s.StartTime && endTime >= s.EndTime));
        }
    }
}