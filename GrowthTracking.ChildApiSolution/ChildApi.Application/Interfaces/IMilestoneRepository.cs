using ChildApi.Application.DTOs;
using GrowthTracking.ShareLibrary.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChildApi.Application.Interfaces
{
    public interface IMilestoneRepository
    {
        Task<Response> CreateMilestoneAsync(MilestoneDTO milestoneDto);
        Task<Response> UpdateMilestoneAsync(MilestoneDTO milestoneDto);
        Task<MilestoneDTO?> GetMilestoneAsync(Guid milestoneId);
        Task<IEnumerable<MilestoneDTO>> GetMilestonesByChildAsync(Guid childId);
        Task<Response> DeleteMilestoneAsync(Guid milestoneId);
    }
}
