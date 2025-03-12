using GrowthTracking.ShareLibrary.Validation;
using System.ComponentModel.DataAnnotations;

namespace GrowthTracking.DoctorSolution.Application.DTOs
{
    public record DoctorCreateRequest(
        [Required, GuidValidation] string DoctorId,
        string FullName,
        string? DateOfBirth,
        string? Gender,
        string? Address,
        string? PhoneNumber,
        string? Specialization,
        int? ExperienceYears,
        string? Workplace,
        string? Biography,
        string? ProfilePhoto,
        decimal? AverageRating
        );
    public record DoctorUpdateRequest();

    public record DoctorResponse(
        string DoctorId,
        string FullName,
        string? DateOfBirth,
        string? Gender,
        string? Address,
        string? PhoneNumber,
        string? Specialization,
        int? ExperienceYears,
        string? Workplace,
        string? Biography,
        string? ProfilePhoto,
        decimal? AverageRating,
        string? CreatedAt,
        string? UpdatedAt
        );

}
