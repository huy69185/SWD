public record BookingDTO
{
    public Guid? Id { get; set; }
    public Guid ParentId { get; init; }
    public Guid ChildId { get; init; }
    public Guid ScheduleId { get; init; }
    public string Status { get; init; }
    public DateTime? BookingDate { get; init; }
    public DateTime? DoctorConfirmationDeadline { get; init; }
    public DateTime? PaymentDeadline { get; init; }
    public string Notes { get; init; }
    public string CancelledBy { get; init; }
    public DateTime? CancellationTime { get; init; }

    public BookingDTO(
        Guid? Id,
        Guid ParentId,
        Guid ChildId,
        Guid ScheduleId,
        string Status,
        DateTime? BookingDate,
        DateTime? DoctorConfirmationDeadline,
        DateTime? PaymentDeadline,
        string Notes,
        string CancelledBy,
        DateTime? CancellationTime)
    {
        this.Id = Id;
        this.ParentId = ParentId;
        this.ChildId = ChildId;
        this.ScheduleId = ScheduleId;
        this.Status = Status;
        this.BookingDate = BookingDate;
        this.DoctorConfirmationDeadline = DoctorConfirmationDeadline;
        this.PaymentDeadline = PaymentDeadline;
        this.Notes = Notes;
        this.CancelledBy = CancelledBy;
        this.CancellationTime = CancellationTime;
    }
}