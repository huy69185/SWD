using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParentManageApi.Application.DTOs;
using ParentManageApi.Application.Interfaces;
using ParentManagementAPI.Application.DTOs;
using System.Security.Claims;

namespace ParentManageApi.Presentation.Controllers
{
    [Route("api/parent")]
    [ApiController]
    public class ParentController(IParentService parentService) : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateParent([FromBody] ParentDTO parentDTO)
        {
            var parentId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(parentId) || !Guid.TryParse(parentId, out var parsedParentId))
                return Unauthorized(new ApiResponse(false, "Invalid or missing user token"));

            var response = await parentService.CreateParentAsync(parentDTO, parsedParentId);
            return response.Flag ? Ok(new ApiResponse(true, response.Message, null)) : BadRequest(new ApiResponse(false, response.Message, null));
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateParent([FromBody] ParentDTO parentDTO)
        {
            var parentId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(parentId) || !Guid.TryParse(parentId, out var parsedParentId))
                return Unauthorized(new ApiResponse(false, "Invalid or missing user token"));

            var response = await parentService.UpdateParentAsync(parentDTO, parsedParentId);
            return response.Flag ? Ok(new ApiResponse(true, response.Message, null)) : BadRequest(new ApiResponse(false, response.Message, null));
        }

        [HttpGet("{parentId}")]
        [Authorize]
        public async Task<IActionResult> GetParent([FromRoute] Guid parentId)
        {
            var parent = await parentService.GetParentAsync(parentId);
            return parent == null ? NotFound(new ApiResponse(false, "Parent not found", null)) : Ok(new ApiResponse(true, "Parent found", parent));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllParents()
        {
            var parents = await parentService.GetAllParentsAsync();
            return Ok(new ApiResponse(true, "Parents retrieved successfully", parents));
        }

        [HttpDelete("{parentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteParent([FromRoute] Guid parentId)
        {
            var response = await parentService.DeleteParentAsync(parentId);
            return response.Flag ? Ok(new ApiResponse(true, response.Message, null)) : NotFound(new ApiResponse(false, response.Message, null));
        }

        [HttpGet("{parentId}/children")]
        [Authorize]
        public async Task<IActionResult> GetChildrenByParent([FromRoute] Guid parentId)
        {
            var children = await parentService.GetChildrenByParentAsync(parentId);
            return Ok(new ApiResponse(true, "Children retrieved successfully", children));
        }
    }
}