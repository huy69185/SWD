using GrowthTracking.DoctorSolution.Application.DTOs;
using GrowthTracking.DoctorSolution.Application.Services.Interfaces;
using GrowthTracking.DoctorSolution.Domain.Constants;
using GrowthTracking.ShareLibrary.Response;
using GrowthTracking.ShareLibrary.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrowthTracking.DoctorSolution.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.DoctorAndAdmin)]
    public class DoctorsController(IDoctorService doctorService) : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
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
        public async Task<IActionResult> CreateDoctor([FromBody] DoctorCreateRequest doctor)
        {
            var result = await doctorService.CreateDoctor(doctor);
            return Ok(new ApiResponse()
            {
                Success = true,
                Data = result
            });
        }

        [HttpPut("{doctorId}")]
        public async Task<IActionResult> UpdateDoctor([GuidValidation] string doctorId, [FromBody] DoctorUpdateRequest doctor)
        {
            var result = await doctorService.UpdateDoctor(doctorId, doctor);
            return Ok(new ApiResponse()
            {
                Success = true,
                Data = result
            });
        }

    }
}
