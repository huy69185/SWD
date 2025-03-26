using GrowthTracking.ShareLibrary.Abstract;
using System;

namespace BookingApi.Domain.Entities
{
    public class Consultation : BaseEntity
    {
        public override Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime ConsultationDate { get; set; }
        public string? ConsultationNotes { get; set; }
        public string Status { get; set; } = "scheduled";
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool StatusDelete { get; set; } = false; 
    }
}