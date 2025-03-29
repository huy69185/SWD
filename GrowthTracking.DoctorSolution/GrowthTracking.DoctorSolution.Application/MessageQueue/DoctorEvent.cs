namespace GrowthTracking.DoctorSolution.Application.MessageQueue
{
    public class DoctorEvent
    {
        public Guid DoctorId { get; set; }
        public string FullName { get; set; }
        public string EventType { get; set; }
    }
}
