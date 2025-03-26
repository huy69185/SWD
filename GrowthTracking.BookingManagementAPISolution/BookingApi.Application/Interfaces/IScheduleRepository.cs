using BookingApi.Application.DTOs;
using GrowthTracking.ShareLibrary.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApi.Application.Interfaces
{
    public interface IScheduleRepository
    {
        Task<Response> CreateScheduleAsync(ScheduleDTO scheduleDto);
        Task<Response> UpdateScheduleAsync(ScheduleDTO scheduleDto);
        Task<ScheduleDTO?> GetScheduleAsync(Guid scheduleId);
        Task<IEnumerable<ScheduleDTO>> GetSchedulesByDoctorAsync(Guid doctorId, DateTime? startDate, DateTime? endDate);
        Task<Response> DeleteScheduleAsync(Guid scheduleId);
        Task<bool> CheckDoctorScheduleConflictAsync(Guid doctorId, DateTime startTime, DateTime endTime, Guid scheduleId = default);
    }
}