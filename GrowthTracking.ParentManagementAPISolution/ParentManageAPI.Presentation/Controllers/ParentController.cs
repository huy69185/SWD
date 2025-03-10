using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParentManageApi.Application.DTOs;
using ParentManageApi.Application.Interfaces;
using System.Security.Claims;
using GrowthTracking.ShareLibrary.Logs;
using ParentManagementAPI.Application.DTOs; 

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
            {
                LogHandler.LogExceptions(new UnauthorizedAccessException($"Invalid or missing user token for CreateParent"));
                return Unauthorized(new ApiResponse(false, "Invalid or missing user token"));
            }

            LogHandler.LogToFile($"Starting CreateParent for ParentId: {parsedParentId}");
            var response = await parentService.CreateParentAsync(parentDTO, parsedParentId);
            if (response.Flag)
            {
                LogHandler.LogToConsole($"Successfully created parent with ParentId: {parsedParentId}");
                return Ok(new ApiResponse(true, response.Message, null));
            }
            else
            {
                LogHandler.LogToDebugger($"Failed to create parent with ParentId: {parsedParentId}. Reason: {response.Message}");
                return BadRequest(new ApiResponse(false, response.Message, null));
            }
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateParent([FromBody] ParentDTO parentDTO)
        {
            var parentId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(parentId) || !Guid.TryParse(parentId, out var parsedParentId))
            {
                LogHandler.LogExceptions(new UnauthorizedAccessException($"Invalid or missing user token for UpdateParent"));
                return Unauthorized(new ApiResponse(false, "Invalid or missing user token"));
            }

            LogHandler.LogToFile($"Starting UpdateParent for ParentId: {parsedParentId}");
            var response = await parentService.UpdateParentAsync(parentDTO, parsedParentId);
            if (response.Flag)
            {
                LogHandler.LogToConsole($"Successfully updated parent with ParentId: {parsedParentId}");
                return Ok(new ApiResponse(true, response.Message, null));
            }
            else
            {
                LogHandler.LogToDebugger($"Failed to update parent with ParentId: {parsedParentId}. Reason: {response.Message}");
                return BadRequest(new ApiResponse(false, response.Message, null));
            }
        }

        [HttpGet("{parentId}")]
        [Authorize]
        public async Task<IActionResult> GetParent([FromRoute] Guid parentId)
        {
            LogHandler.LogToFile($"Starting GetParent for ParentId: {parentId}");
            var parent = await parentService.GetParentAsync(parentId);
            if (parent == null)
            {
                LogHandler.LogToDebugger($"Parent with ParentId: {parentId} not found");
                return NotFound(new ApiResponse(false, "Parent not found", null));
            }

            LogHandler.LogToConsole($"Successfully retrieved parent with ParentId: {parentId}");
            return Ok(new ApiResponse(true, "Parent found", parent));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllParents()
        {
            LogHandler.LogToFile("Starting GetAllParents");
            var parents = await parentService.GetAllParentsAsync();
            LogHandler.LogToConsole("Successfully retrieved all parents");
            return Ok(new ApiResponse(true, "Parents retrieved successfully", parents));
        }

        [HttpDelete("{parentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteParent([FromRoute] Guid parentId)
        {
            LogHandler.LogToFile($"Starting DeleteParent for ParentId: {parentId}");
            var response = await parentService.DeleteParentAsync(parentId);
            if (response.Flag)
            {
                LogHandler.LogToConsole($"Successfully deleted parent with ParentId: {parentId}");
                return Ok(new ApiResponse(true, response.Message, null));
            }
            else
            {
                LogHandler.LogToDebugger($"Failed to delete parent with ParentId: {parentId}. Reason: {response.Message}");
                return NotFound(new ApiResponse(false, response.Message, null));
            }
        }
    }
}