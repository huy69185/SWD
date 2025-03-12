using GrowthTracking.ShareLibrary.Abstract;

namespace AuthenticationApi.Domain.Entities
{
    public class BugReport : BaseEntity
    {
        public Guid BugReportId { get; set; }
        public Guid UserId { get; set; }
        public string? ReportType { get; set; }
        public string? Description { get; set; }
        public string? ScreenshotUrl { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}