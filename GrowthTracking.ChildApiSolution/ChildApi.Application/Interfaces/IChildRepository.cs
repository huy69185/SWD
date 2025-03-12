using ChildApi.Application.DTOs;
using GrowthTracking.ShareLibrary.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChildApi.Application.Interfaces
{
    public interface IChildRepository
    {
        Task<Response> CreateChildAsync(ChildDTO childDto);
        Task<Response> UpdateChildAsync(ChildDTO childDto);
        Task<ChildDTO?> GetChildAsync(Guid childId);
        Task<IEnumerable<ChildDTO>> GetChildrenByParentAsync(Guid parentId);
        Task<Response> DeleteChildAsync(Guid childId);

        // Tính toán BMI dựa trên cân nặng và chiều cao
        decimal? CalculateBMI(ChildDTO childDto);
    }
}
