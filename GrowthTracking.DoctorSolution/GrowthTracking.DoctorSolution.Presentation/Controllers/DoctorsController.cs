using GrowthTracking.DoctorSolution.Application.DTOs;
using GrowthTracking.DoctorSolution.Application.Interfaces;
using GrowthTracking.DoctorSolution.Domain.Constants;
using GrowthTracking.ShareLibrary.Response;
using GrowthTracking.ShareLibrary.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace GrowthTracking.DoctorSolution.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Consts.DoctorAndAdmin)]
    public class DoctorsController(IDoctorService doctorService) : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        [EndpointSummary("Get list doctor with paging")]
        public async Task<IActionResult> GetDoctors(int page = 1, int pageSize = 10)
        {
            var result = await doctorService.GetAllDoctors(page, pageSize);
            return Ok(new ApiResponse()
            {
                Success = true,
                Data = new
                {
                    result.TotalCount,
                    result.TotalPages,
                    result.CurrentPage,
                    result.PageSize,
                    result.HasNext,
                    result.HasPrevious,
                    doctorList = result
                }
            });
        }

        [HttpGet("{doctorId}")]
        [EndpointSummary("Get a doctor by id")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDoctorById([GuidValidation] string doctorId)
        {
            var result = await doctorService.GetDoctorById(doctorId);
            return Ok(new ApiResponse()
            {
                Success = true,
                Data = result
            });
        }

        [HttpGet("search")]
        [AllowAnonymous]
        [EndpointSummary("search doctors with paging")]
        public async Task<IActionResult> SearchDoctors(string searchTerm, int page = 1, int pageSize = 10)
        {
            var result = await doctorService.SearchDoctors(searchTerm, page, pageSize);
            return Ok(new ApiResponse()
            {
                Success = true,
                Data = new
                {
                    result.TotalCount,
                    result.TotalPages,
                    result.CurrentPage,
                    result.PageSize,
                    result.HasNext,
                    result.HasPrevious,
                    doctorList = result
                }
            });
        }

        [HttpPost]
        [EndpointSummary("Create Doctor")]
        [AllowAnonymous]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateDoctor(
            [FromForm] DoctorCreateRequest doctor)
        {
            var result = await doctorService.CreateDoctor(doctor, doctor.IdCard, doctor.ProfessionalDegree, doctor.MedicalLicense);
            return Ok(new ApiResponse()
            {
                Success = true,
                Data = result
            });
        }

        [HttpPut("{doctorId}")]
        [EndpointSummary("Update Doctor")]
        public async Task<IActionResult> UpdateDoctor([GuidValidation] string doctorId, [FromBody] DoctorUpdateRequest doctor)
        {
            var result = await doctorService.UpdateDoctor(doctorId, doctor);
            return Ok(new ApiResponse()
            {
                Success = true,
                Data = result
            });
        }

        [HttpDelete("{doctorId}")]
        [Authorize(Roles = Consts.Admin)]
        [EndpointSummary("Delete Doctor")]
        public async Task<IActionResult> DeleteDoctor([GuidValidation] string doctorId)
        {
            string currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

            await doctorService.DeleteDoctor(doctorId, currentUserId);
            return StatusCode(StatusCodes.Status204NoContent);
        }

    }
}
