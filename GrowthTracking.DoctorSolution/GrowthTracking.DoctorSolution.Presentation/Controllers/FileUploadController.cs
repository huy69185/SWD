using GrowthTracking.DoctorSolution.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using GrowthTracking.ShareLibrary.Response;

namespace GrowthTracking.DoctorSolution.Presentation.Controllers
{
    [Route("api/upload")]
    [ApiController]
    public class FileUploadController(IFileStorageService service) : ControllerBase
    {
        [HttpPost]
        [EndpointSummary("Test API for uploading file")]
        [Route("file")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {

            var uploadResult = await service.UploadFileAsync(file);

            if (uploadResult.Success)
            {
                return Ok(new ApiResponse()
                {
                    Success = true,
                    Data = new
                    {
                        uploadResult.Url,
                        uploadResult.PublicId
                    }
                });
            }

            return StatusCode(500, uploadResult.ErrorMessage);
        }
    }
}
