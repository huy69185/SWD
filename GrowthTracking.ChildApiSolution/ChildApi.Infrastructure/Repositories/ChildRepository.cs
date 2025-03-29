using ChildApi.Application.DTOs;
using ChildApi.Application.Interfaces;
using ChildApi.Application.Services;
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
    public class ChildRepository : IChildRepository
    {
        private readonly ChildDbContext _context;

        public ChildRepository(ChildDbContext context)
        {
            _context = context;
        }

        public async Task<(Response Response, Guid? ChildId)> CreateChildAsync(ChildDTO childDto)
        {
            var child = childDto.Adapt<Child>();
            if (child.Id == Guid.Empty)
                child.Id = Guid.NewGuid();

            _context.Children.Add(child);
            await _context.SaveChangesAsync();
            return (new Response(true, "Child created successfully"), child.Id);
        }

        public async Task<Response> UpdateChildAsync(ChildDTO childDto)
        {
            if (childDto.Id == null)
                return new Response(false, "Child Id is required for update");

            var child = await _context.Children.FindAsync(childDto.Id.Value);
            if (child == null)
                return new Response(false, "Child not found");

            childDto.Adapt(child);
            await _context.SaveChangesAsync();
            return new Response(true, "Child updated successfully");
        }

        public async Task<ChildDTO?> GetChildAsync(Guid childId)
        {
            var child = await _context.Children.FindAsync(childId);
            return child?.Adapt<ChildDTO>();
        }

        public async Task<IEnumerable<ChildDTO>> GetChildrenByParentAsync(Guid parentId)
        {
            var children = await _context.Children
                .Where(c => c.ParentId == parentId)
                .ToListAsync();
            return children.Adapt<IEnumerable<ChildDTO>>();
        }

        public async Task<Response> DeleteChildAsync(Guid childId)
        {
            var child = await _context.Children.FindAsync(childId);
            if (child == null)
                return new Response(false, "Child not found");

            _context.Children.Remove(child);
            await _context.SaveChangesAsync();
            return new Response(true, "Child deleted successfully");
        }

        public decimal? CalculateBMI(ChildDTO childDto)
        {
            if (childDto.BirthHeight == 0)
                return null;

            var heightInMeter = childDto.BirthHeight / 100;
            return Math.Round(childDto.BirthWeight / (heightInMeter * heightInMeter), 2);
        }

        public async Task<GrowthAnalysis> AnalyzeGrowthAsync(Guid childId)
        {
            var child = await _context.Children.FindAsync(childId);
            if (child == null)
                return new GrowthAnalysis { Warning = "Child not found" };

            var childDto = child.Adapt<ChildDTO>();
            return GrowthTracker.Analyze(childDto, _context);
        }
    }
}