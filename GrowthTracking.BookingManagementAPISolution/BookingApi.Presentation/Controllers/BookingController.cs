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
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        [Authorize(Roles = "Parent")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingDTO bookingDto)
        {
            var response = await _bookingService.CreateBookingAsync(bookingDto);
            return response.Flag
                ? Ok(new ApiResponse(true, response.Message))
                : BadRequest(new ApiResponse(false, response.Message));
        }

        [HttpPut("{bookingId}")]
        [Authorize(Roles = "Parent")]
        public async Task<IActionResult> UpdateBooking(Guid bookingId, [FromBody] BookingDTO bookingDto)
        {
            bookingDto = bookingDto with { Id = bookingId };
            var response = await _bookingService.UpdateBookingAsync(bookingDto);
            return response.Flag
                ? Ok(new ApiResponse(true, response.Message))
                : BadRequest(new ApiResponse(false, response.Message));
        }

        [HttpGet("{bookingId}")]
        public async Task<IActionResult> GetBooking(Guid bookingId)
        {
            var booking = await _bookingService.GetBookingAsync(bookingId);
            if (booking == null)
                return NotFound(new ApiResponse(false, "Booking not found"));
            return Ok(new ApiResponse(true, "Booking retrieved successfully", booking));
        }

        [HttpGet("parent/{parentId}")]
        [Authorize(Roles = "Parent")]
        public async Task<IActionResult> GetBookingsByParent(Guid parentId)
        {
            var bookings = await _bookingService.GetBookingsByParentAsync(parentId);
            return Ok(new ApiResponse(true, "Bookings retrieved successfully", bookings));
        }

        [HttpGet("doctor/{doctorId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetBookingsByDoctor(Guid doctorId)
        {
            var bookings = await _bookingService.GetBookingsByDoctorAsync(doctorId);
            return Ok(new ApiResponse(true, "Bookings retrieved successfully", bookings));
        }

        [HttpDelete("{bookingId}")]
        [Authorize(Roles = "Parent")]
        public async Task<IActionResult> CancelBooking(Guid bookingId)
        {
            var response = await _bookingService.CancelBookingAsync(bookingId);
            return response.Flag
                ? Ok(new ApiResponse(true, response.Message))
                : NotFound(new ApiResponse(false, response.Message));
        }
    }
}