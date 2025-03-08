using GrowthTracking.ShareLibrary.Abstract;

namespace ParentManageApi.Domain.Entities
{
    public class Parent : BaseEntity
    {
        public override Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}