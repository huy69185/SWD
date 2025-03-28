using ParentManageApi.Application.DTOs;
using ParentManageApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParentManagementAPI.Application.DTOs;

namespace ParentManageApi.Presentation.Controllers
{
    [Route("api/parent")]
    [ApiController]
    public class ParentController(IParentRepository parentRepository) : ControllerBase
    {
        [HttpPost]
        // Tạo thông tin phụ huynh mới
        public async Task<IActionResult> CreateParent([FromBody] ParentDTO parentDTO)
        {
            var response = await parentRepository.CreateParent(parentDTO);
            return response.Flag ? Ok(new ApiResponse(true, response.Message, null)) : BadRequest(new ApiResponse(false, response.Message, null));
        }

        [HttpPut]
        // Cập nhật thông tin phụ huynh
        public async Task<IActionResult> UpdateParent([FromBody] ParentDTO parentDTO)
        {
            var response = await parentRepository.UpdateParent(parentDTO);
            return response.Flag ? Ok(new ApiResponse(true, response.Message, null)) : BadRequest(new ApiResponse(false, response.Message, null));
        }

        [HttpGet("{parentId}")]
        [Authorize]
        // Lấy thông tin phụ huynh theo ID
        public async Task<IActionResult> GetParent([FromRoute] Guid parentId)
        {
            var parent = await parentRepository.GetParent(parentId);
            return parent == null ? NotFound(new ApiResponse(false, "Parent not found", null)) : Ok(new ApiResponse(true, "Parent found", parent));
        }

        [HttpGet]
        [Authorize]
        // Lấy danh sách tất cả phụ huynh
        public async Task<IActionResult> GetAllParents()
        {
            var parents = await parentRepository.GetAllParents();
            return Ok(new ApiResponse(true, "Parents retrieved successfully", parents));
        }

        [HttpDelete("{parentId}")]
        [Authorize]
        // Xóa mềm thông tin phụ huynh
        public async Task<IActionResult> DeleteParent([FromRoute] Guid parentId)
        {
            var response = await parentRepository.DeleteParent(parentId);
            return response.Flag ? Ok(new ApiResponse(true, response.Message, null)) : NotFound(new ApiResponse(false, response.Message, null));
        }

        [HttpGet("{parentId}/children")]
        [Authorize]
        // Lấy danh sách trẻ em của một phụ huynh
        public async Task<IActionResult> GetChildrenByParent([FromRoute] Guid parentId)
        {
            var children = await parentRepository.GetChildrenByParent(parentId);
            return Ok(new ApiResponse(true, "Children retrieved successfully", children));
        }
    }
}