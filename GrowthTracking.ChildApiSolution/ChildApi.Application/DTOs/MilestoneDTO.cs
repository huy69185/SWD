using System;

namespace ChildApi.Application.DTOs
{
    // DTO cho Milestone.
    public record MilestoneDTO(
        Guid? Id,        // Nullable cho record mới
        Guid ChildId,
        string MilestoneType,
        DateTime MilestoneDate,
        string? Description
    );
}
