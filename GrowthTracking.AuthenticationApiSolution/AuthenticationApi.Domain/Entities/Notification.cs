using GrowthTracking.ShareLibrary.Abstract;

namespace AuthenticationApi.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public Guid NotificationId { get; set; }
        public Guid UserId { get; set; }
        public string? NotificationType { get; set; }
        public string? Content { get; set; }
        public string? Status { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}