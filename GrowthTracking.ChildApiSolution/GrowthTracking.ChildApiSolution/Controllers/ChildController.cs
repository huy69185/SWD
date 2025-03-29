using ChildApi.Application.DTOs;
using ChildApi.Application.Interfaces;
using ChildApi.Application.Messaging; // Thêm namespace cho IEventPublisher
using ChildApi.Application.Services;
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
    [AllowAnonymous]
    public class ChildController : ControllerBase
    {
        private readonly IChildRepository _childRepository;
        private readonly ParentIdCache _parentIdCache;
        private readonly IEventPublisher _eventPublisher;

        public ChildController(IChildRepository childRepository, ParentIdCache parentIdCache, IEventPublisher eventPublisher)
        {
            _childRepository = childRepository;
            _parentIdCache = parentIdCache;
            _eventPublisher = eventPublisher;
        }

        // POST: api/Child
        [HttpPost]
        public async Task<IActionResult> CreateChild([FromBody] ChildDTO childDto)
        {
            // Ghi đè ParentId trong DTO bằng giá trị từ cache (đã được cập nhật qua RabbitMQ)
            childDto = childDto with { ParentId = _parentIdCache.ParentId };

            var result = await _childRepository.CreateChildAsync(childDto);
            if (result.Flag && childDto.Id.HasValue)
            {
                _eventPublisher.PublishChildCreated(childDto.Id.Value, childDto.ParentId, childDto.FullName);
            }
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

        // GET: api/Child/growth/{childId}
        [HttpGet("growth/{childId}")]
        public async Task<IActionResult> GetGrowthAnalysis(Guid childId)
        {
            var analysis = await _childRepository.AnalyzeGrowthAsync(childId);
            if (string.IsNullOrEmpty(analysis.Warning))
                analysis.Warning = "No issues detected";
            return Ok(new ApiResponse { Success = true, Data = analysis });
        }
    }
}