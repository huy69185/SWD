using GrowthTracking.ShareLibrary.Abstract;
using System;

namespace ChildApi.Domain.Entities
{
    // Entity Milestone: khai báo các thuộc tính theo quy ước của EF Core.
    public class Milestone : BaseEntity
    {
        // Sử dụng Guid cho MilestoneId (override từ BaseEntity)
        public override Guid Id { get; set; }

        // Khoá ngoại liên kết đến Child
        public Guid ChildId { get; set; }

        public string MilestoneType { get; set; } = null!;

        public DateTime MilestoneDate { get; set; }

        public string? Description { get; set; }
    }
}
