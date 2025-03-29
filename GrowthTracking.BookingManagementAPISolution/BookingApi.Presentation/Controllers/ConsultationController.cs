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
    [Authorize(Roles = Constants.ParentAndDoctor)]
    public class ConsultationController : ControllerBase
    {
        private readonly IConsultationService _consultationService;

        public ConsultationController(IConsultationService consultationService)
        {
            _consultationService = consultationService;
        }

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> CreateConsultation([FromBody] ConsultationDTO consultationDto)
        {
            var response = await _consultationService.CreateConsultationAsync(consultationDto);
            return response.Flag
                ? Ok(new ApiResponse(true, response.Message))
                : BadRequest(new ApiResponse(false, response.Message));
        }

        [HttpPut("{consultationId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> UpdateConsultation(Guid consultationId, [FromBody] ConsultationDTO consultationDto)
        {
            consultationDto = consultationDto with { Id = consultationId };
            var response = await _consultationService.UpdateConsultationAsync(consultationDto);
            return response.Flag
                ? Ok(new ApiResponse(true, response.Message))
                : BadRequest(new ApiResponse(false, response.Message));
        }

        [HttpGet("{consultationId}")]
        public async Task<IActionResult> GetConsultation(Guid consultationId)
        {
            var consultation = await _consultationService.GetConsultationAsync(consultationId);
            if (consultation == null)
                return NotFound(new ApiResponse(false, "Consultation not found"));
            return Ok(new ApiResponse(true, "Consultation retrieved successfully", consultation));
        }

        [HttpGet("doctor/{doctorId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetConsultationsByDoctor(Guid doctorId)
        {
            var consultations = await _consultationService.GetConsultationsByDoctorAsync(doctorId);
            return Ok(new ApiResponse(true, "Consultations retrieved successfully", consultations));
        }

        [HttpGet("booking/{bookingId}")]
        public async Task<IActionResult> GetConsultationsByBooking(Guid bookingId)
        {
            var consultations = await _consultationService.GetConsultationsByBookingAsync(bookingId);
            return Ok(new ApiResponse(true, "Consultations retrieved successfully", consultations));
        }

        [HttpDelete("{consultationId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> CancelConsultation(Guid consultationId)
        {
            var response = await _consultationService.CancelConsultationAsync(consultationId);
            return response.Flag
                ? Ok(new ApiResponse(true, response.Message))
                : NotFound(new ApiResponse(false, response.Message));
        }
    }
}