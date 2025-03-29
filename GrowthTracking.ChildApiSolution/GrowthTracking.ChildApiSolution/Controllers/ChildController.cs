using ChildApi.Application.DTOs;
using ChildApi.Application.Interfaces;
using ChildApi.Application.Messaging;
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
    [Authorize]
    public class ChildController : ControllerBase
    {
        private readonly IChildRepository _childRepository;
        private readonly IParentIdCache _parentIdCache;
        private readonly IEventPublisher _eventPublisher;

        public ChildController(IChildRepository childRepository, IParentIdCache parentIdCache, IEventPublisher eventPublisher)
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

            // Gọi repository để tạo child và lấy thông tin đã tạo (bao gồm Id)
            var (result, newChildId) = await _childRepository.CreateChildAsync(childDto);
            if (result.Flag)
            {
                // Lấy thông tin child vừa tạo để đảm bảo có Id
                var createdChild = await _childRepository.GetChildAsync(newChildId ?? Guid.Empty);
                if (createdChild != null && createdChild.Id.HasValue)
                {
                    _eventPublisher.PublishChildCreated(createdChild.Id.Value, createdChild.ParentId, createdChild.FullName);
                    return Ok(new ApiResponse { Success = true, Message = result.Message });
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { Success = false, Message = "Failed to retrieve created child" });
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { Success = false, Message = result.Message });
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
            return Ok(new ApiResponse { Success = true, Data = new BmiResult { BMI = bmi } });
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

    // Class mới để thay thế anonymous type trong GetChildBMI
    public class BmiResult
    {
        public decimal? BMI { get; set; }
    }
}