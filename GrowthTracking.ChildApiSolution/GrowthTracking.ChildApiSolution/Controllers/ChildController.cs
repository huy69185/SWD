using ChildApi.Application.DTOs;
using ChildApi.Application.Interfaces;
using ChildApi.Application.Messaging;
using GrowthTracking.ShareLibrary.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ChildApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Yêu cầu xác thực nếu cần
    public class ChildController : ControllerBase
    {
        private readonly IChildRepository _childRepository;
        private readonly ParentIdCache _parentIdCache;

        public ChildController(IChildRepository childRepository, ParentIdCache parentIdCache)
        {
            _childRepository = childRepository;
            _parentIdCache = parentIdCache;
        }

        // POST: api/Child
        [HttpPost]
        public async Task<IActionResult> CreateChild([FromBody] ChildDTO childDto)
        {
            // Ghi đè ParentId trong DTO bằng giá trị từ cache (đã được cập nhật qua RabbitMQ)
            childDto = childDto with { ParentId = _parentIdCache.ParentId };

            var result = await _childRepository.CreateChildAsync(childDto);
            return result.Flag
                ? Ok(new ApiResponse { Success = true, Message = result.Message })
                : StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { Success = false, Message = result.Message });
        }

        // GET: api/Child/{childId}
        [HttpGet("{childId}")]
        public async Task<IActionResult> GetChild(Guid childId)
        {
            var child = await _childRepository.GetChildAsync(childId);
            if (child == null)
                return NotFound(new ApiResponse { Success = false, Message = "Child not found" });
            return Ok(new ApiResponse { Success = true, Data = child });
        }

        // PUT: api/Child/{childId}
        [HttpPut("{childId}")]
        public async Task<IActionResult> UpdateChild(Guid childId, [FromBody] ChildDTO childDto)
        {
            // Gán Id từ route cho DTO
            childDto = childDto with { Id = childId };
            var result = await _childRepository.UpdateChildAsync(childDto);
            return result.Flag
                ? Ok(new ApiResponse { Success = true, Message = result.Message })
                : NotFound(new ApiResponse { Success = false, Message = result.Message });
        }

        // DELETE: api/Child/{childId}
        [HttpDelete("{childId}")]
        public async Task<IActionResult> DeleteChild(Guid childId)
        {
            var result = await _childRepository.DeleteChildAsync(childId);
            return result.Flag
                ? Ok(new ApiResponse { Success = true, Message = result.Message })
                : NotFound(new ApiResponse { Success = false, Message = result.Message });
        }

        // GET: api/Child/parent/{parentId}
        [HttpGet("parent/{parentId}")]
        public async Task<IActionResult> GetChildrenByParent(Guid parentId)
        {
            var children = await _childRepository.GetChildrenByParentAsync(parentId);
            return Ok(new ApiResponse { Success = true, Data = children });
        }

        // GET: api/Child/bmi/{childId}
        [HttpGet("bmi/{childId}")]
        public async Task<IActionResult> GetChildBMI(Guid childId)
        {
            var child = await _childRepository.GetChildAsync(childId);
            if (child == null)
                return NotFound(new ApiResponse { Success = false, Message = "Child not found" });
            var bmi = _childRepository.CalculateBMI(child);
            return Ok(new ApiResponse { Success = true, Data = new { BMI = bmi } });
        }
    }
}
