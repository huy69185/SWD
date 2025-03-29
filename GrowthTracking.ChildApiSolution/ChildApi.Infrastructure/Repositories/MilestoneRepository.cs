using ChildApi.Application.DTOs;
using ChildApi.Application.Interfaces;
using ChildApi.Domain.Entities;
using ChildApi.Infrastructure.Data;
using GrowthTracking.ShareLibrary.Response;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChildApi.Infrastructure.Repositories
{
    public class MilestoneRepository : IMilestoneRepository
    {
        private readonly ChildDbContext _context;

        public MilestoneRepository(ChildDbContext context)
        {
            _context = context;
        }

        public async Task<Response> CreateMilestoneAsync(MilestoneDTO milestoneDto)
        {
            var milestone = milestoneDto.Adapt<Milestone>();
            if (milestone.Id == Guid.Empty)
                milestone.Id = Guid.NewGuid();
            _context.Milestones.Add(milestone);
            await _context.SaveChangesAsync();
            return new Response(true, "Milestone created successfully");
        }

        public async Task<Response> UpdateMilestoneAsync(MilestoneDTO milestoneDto)
        {
            if (milestoneDto.Id == null)
                return new Response(false, "Milestone Id is required for update");

            var milestone = await _context.Milestones.FindAsync(milestoneDto.Id.Value);
            if (milestone == null)
                return new Response(false, "Milestone not found");

            milestoneDto.Adapt(milestone);
            await _context.SaveChangesAsync();
            return new Response(true, "Milestone updated successfully");
        }

        public async Task<MilestoneDTO?> GetMilestoneAsync(Guid milestoneId)
        {
            var milestone = await _context.Milestones.FindAsync(milestoneId);
            return milestone?.Adapt<MilestoneDTO>();
        }

        public async Task<IEnumerable<MilestoneDTO>> GetMilestonesByChildAsync(Guid childId)
        {
            var milestones = await _context.Milestones
                .Where(m => m.ChildId == childId)
                .ToListAsync();
            return milestones.Adapt<IEnumerable<MilestoneDTO>>();
        }

        public async Task<Response> DeleteMilestoneAsync(Guid milestoneId)
        {
            var milestone = await _context.Milestones.FindAsync(milestoneId);
            if (milestone == null)
                return new Response(false, "Milestone not found");

            _context.Milestones.Remove(milestone);
            await _context.SaveChangesAsync();
            return new Response(true, "Milestone deleted successfully");
        }
    }
}