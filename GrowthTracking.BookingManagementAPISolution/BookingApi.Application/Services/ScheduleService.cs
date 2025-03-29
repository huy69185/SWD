using BookingApi.Application.DTOs;
using BookingApi.Application.Interfaces;
using GrowthTracking.ShareLibrary.Response;
using GrowthTracking.ShareLibrary.Logs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApi.Application.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;

        public ScheduleService(IScheduleRepository scheduleRepository)
        {
            _scheduleRepository = scheduleRepository;
        }

        public async Task<Response> CreateScheduleAsync(ScheduleDTO scheduleDto)
        {
            LogHandler.LogToFile($"ScheduleService: Starting CreateScheduleAsync for DoctorId: {scheduleDto.DoctorId}");

            // Validate schedule conflicts
            var hasConflict = await _scheduleRepository.CheckDoctorScheduleConflictAsync(scheduleDto.DoctorId, scheduleDto.StartTime, scheduleDto.EndTime);
            if (hasConflict)
            {
                LogHandler.LogToDebugger($"ScheduleService: Schedule conflict detected for DoctorId: {scheduleDto.DoctorId}");
                return new Response(false, "Schedule conflict detected");
            }

            if (scheduleDto.StartTime >= scheduleDto.EndTime)
            {
                LogHandler.LogToDebugger($"ScheduleService: Invalid schedule time range for DoctorId: {scheduleDto.DoctorId}");
                return new Response(false, "Start time must be before end time");
            }

            var response = await _scheduleRepository.CreateScheduleAsync(scheduleDto);
            if (response.Flag)
            {
                LogHandler.LogToConsole($"ScheduleService: Schedule created successfully for DoctorId: {scheduleDto.DoctorId}");
            }
            else
            {
                LogHandler.LogToDebugger($"ScheduleService: Failed to create schedule for DoctorId: {scheduleDto.DoctorId}. Reason: {response.Message}");
            }

            return response;
        }

        public async Task<Response> UpdateScheduleAsync(ScheduleDTO scheduleDto)
        {
            LogHandler.LogToFile($"ScheduleService: Starting UpdateScheduleAsync for ScheduleId: {scheduleDto.Id}");
            if (scheduleDto.Id == null)
            {
                LogHandler.LogToDebugger("ScheduleService: Schedule Id is required for update");
                return new Response(false, "Schedule Id is required for update");
            }

            var existingSchedule = await _scheduleRepository.GetScheduleAsync(scheduleDto.Id.Value);
            if (existingSchedule == null)
            {
                LogHandler.LogToDebugger($"ScheduleService: Schedule with ScheduleId: {scheduleDto.Id} not found");
                return new Response(false, "Schedule not found");
            }

            var hasConflict = await _scheduleRepository.CheckDoctorScheduleConflictAsync(scheduleDto.DoctorId, scheduleDto.StartTime, scheduleDto.EndTime, scheduleDto.Id.Value);
            if (hasConflict)
            {
                LogHandler.LogToDebugger($"ScheduleService: Schedule conflict detected for ScheduleId: {scheduleDto.Id}");
                return new Response(false, "Schedule conflict detected");
            }

            var response = await _scheduleRepository.UpdateScheduleAsync(scheduleDto);
            if (response.Flag)
            {
                LogHandler.LogToConsole($"ScheduleService: Schedule updated successfully for ScheduleId: {scheduleDto.Id}");
            }
            else
            {
                LogHandler.LogToDebugger($"ScheduleService: Failed to update schedule with ScheduleId: {scheduleDto.Id}. Reason: {response.Message}");
            }

            return response;
        }

        public async Task<ScheduleDTO?> GetScheduleAsync(Guid scheduleId)
        {
            LogHandler.LogToFile($"ScheduleService: Starting GetScheduleAsync for ScheduleId: {scheduleId}");
            var schedule = await _scheduleRepository.GetScheduleAsync(scheduleId);
            if (schedule != null)
            {
                LogHandler.LogToConsole($"ScheduleService: Successfully retrieved schedule with ScheduleId: {scheduleId}");
            }
            else
            {
                LogHandler.LogToDebugger($"ScheduleService: Schedule with ScheduleId: {scheduleId} not found");
            }
            return schedule;
        }

        public async Task<IEnumerable<ScheduleDTO>> GetSchedulesByDoctorAsync(Guid doctorId, DateTime? startDate, DateTime? endDate)
        {
            LogHandler.LogToFile($"ScheduleService: Starting GetSchedulesByDoctorAsync for DoctorId: {doctorId}");
            var schedules = await _scheduleRepository.GetSchedulesByDoctorAsync(doctorId, startDate, endDate);
            LogHandler.LogToConsole($"ScheduleService: Successfully retrieved schedules for DoctorId: {doctorId}");
            return schedules;
        }

        public async Task<Response> DeleteScheduleAsync(Guid scheduleId)
        {
            LogHandler.LogToFile($"ScheduleService: Starting DeleteScheduleAsync for ScheduleId: {scheduleId}");
            var response = await _scheduleRepository.DeleteScheduleAsync(scheduleId);
            if (response.Flag)
            {
                LogHandler.LogToConsole($"ScheduleService: Successfully deleted schedule with ScheduleId: {scheduleId}");
            }
            else
            {
                LogHandler.LogToDebugger($"ScheduleService: Failed to delete schedule with ScheduleId: {scheduleId}. Reason: {response.Message}");
            }
            return response;
        }
    }
}