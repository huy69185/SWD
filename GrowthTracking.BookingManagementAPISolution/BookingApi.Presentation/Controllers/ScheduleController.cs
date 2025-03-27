using BookingApi.Application.DTOs;
using BookingApi.Application.Interfaces;
using BookingApi.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BookingApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.Doctor)]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSchedule([FromBody] ScheduleDTO scheduleDto)
        {
            var response = await _scheduleService.CreateScheduleAsync(scheduleDto);
            return response.Flag
                ? Ok(new ApiResponse(true, response.Message))
                : BadRequest(new ApiResponse(false, response.Message));
        }

        [HttpPut("{scheduleId}")]
        public async Task<IActionResult> UpdateSchedule(Guid scheduleId, [FromBody] ScheduleDTO scheduleDto)
        {
            scheduleDto = scheduleDto with { Id = scheduleId };
            var response = await _scheduleService.UpdateScheduleAsync(scheduleDto);
            return response.Flag
                ? Ok(new ApiResponse(true, response.Message))
                : BadRequest(new ApiResponse(false, response.Message));
        }

        [HttpGet("{scheduleId}")]
        public async Task<IActionResult> GetSchedule(Guid scheduleId)
        {
            var schedule = await _scheduleService.GetScheduleAsync(scheduleId);
            if (schedule == null)
                return NotFound(new ApiResponse(false, "Schedule not found"));
            return Ok(new ApiResponse(true, "Schedule retrieved successfully", schedule));
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetSchedulesByDoctor(Guid doctorId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var schedules = await _scheduleService.GetSchedulesByDoctorAsync(doctorId, startDate, endDate);
            return Ok(new ApiResponse(true, "Schedules retrieved successfully", schedules));
        }

        [HttpDelete("{scheduleId}")]
        public async Task<IActionResult> DeleteSchedule(Guid scheduleId)
        {
            var response = await _scheduleService.DeleteScheduleAsync(scheduleId);
            return response.Flag
                ? Ok(new ApiResponse(true, response.Message))
                : NotFound(new ApiResponse(false, response.Message));
        }
    }
}