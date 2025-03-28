using GrowthTracking.ShareLibrary.Validation;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace GrowthTracking.DoctorSolution.Application.DTOs
{
    public record DoctorCreateRequest(
        [Required, GuidValidation] string DoctorId,
        string FullName,
        [DateOnlyValidation, DateOfBirthValidation] 
        string? DateOfBirth,
        [RegularExpression("^(male|female|other)$", ErrorMessage = "Gender must be 'male', 'female', or 'other'.")]
        string? Gender,
        string? Address,
        [Phone] string? PhoneNumber,
        string? Specialization,
        [Range(0, 100, ErrorMessage = "Experience years must be between 0 and 100.")]
        int? ExperienceYears,
        string? Workplace,
        string? Biography,
        string? ProfilePhoto,
        [Required] IFormFile IdCard,
        [Required] IFormFile ProfessionalDegree,
        [Required] IFormFile MedicalLicense
        );
    public record DoctorUpdateRequest(
        string? FullName,
        [DateOnlyValidation, DateOfBirthValidation] string? DateOfBirth,
        [RegularExpression("^(male|female|other)$", ErrorMessage = "Gender must be 'male', 'female', or 'other'.")]
        string? Gender,
        string? Address,
        [Phone] string? PhoneNumber,
        string? Specialization,
        [Range(0, 100, ErrorMessage = "Experience years must be between 0 and 100.")]
        int? ExperienceYears,
        string? Workplace,
        string? Biography,
        string? ProfilePhoto
        );

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
