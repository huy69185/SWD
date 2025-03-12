using System;

namespace ChildApi.Application.DTOs
{
    // DTO cho Milestone.
    public record MilestoneDTO(
        Guid? Id,       
        Guid ChildId,
        string MilestoneType,
        DateTime MilestoneDate,
        string? Description
    );
}
