using GrowthTracking.ShareLibrary.Abstract;
using System;

namespace BookingApi.Domain.Entities
{
    public class Booking : BaseEntity
    {
        public override Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public Guid ChildId { get; set; }
        public Guid ScheduleId { get; set; }
        public string Status { get; set; } = "pending";
        public DateTime? BookingDate { get; set; }
        public DateTime? DoctorConfirmationDeadline { get; set; }
        public string? Notes { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool StatusDelete { get; set; } = false;
    }
}