using GrowthTracking.ShareLibrary.Abstract;
using System;

namespace BookingApi.Domain.Entities
{
    public class Schedule : BaseEntity
    {
        public override Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Location { get; set; }
        public bool IsAvailable { get; set; } = true;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool StatusDelete { get; set; } = false;
    }
}