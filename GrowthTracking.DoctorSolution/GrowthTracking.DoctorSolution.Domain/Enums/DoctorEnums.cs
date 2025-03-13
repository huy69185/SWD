namespace GrowthTracking.DoctorSolution.Domain.Enums
{
    public enum DoctorStatus
    {
        PendingVerification, // Waiting for admin approval of identity documents.
        Active,              // Verified and can provide consultations.
        Inactive             // Suspended or deactivated.
    }
}
