using GrowthTracking.ShareLibrary.Validation;
using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Application.DTOs
{
    public record AppUserDTO(
        [GuidValidation] Guid? UserAccountID,
        [Required] string FullName,
        [Required, EmailAddress] string Email,
        string? PasswordHash,
        string? PhoneNumber,
        [Required] string Role,
        DateTime? CreatedAt = null,
        DateTime? UpdatedAt = null,
        bool? IsActive = true,
        string? ProfilePictureUrl = null,
        string? Address = null,
        string? Bio = null,
        bool? EmailVerified = false,
        string? VerificationToken = null,
        string? ResetToken = null,
        DateTime? ResetTokenExpiry = null,
        string? OAuth2GoogleId = null,
        string? OAuth2FacebookId = null,
        DateTime? LastLoginAt = null
    );
}