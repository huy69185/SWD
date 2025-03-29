using GrowthTracking.ShareLibrary.Abstract;

namespace AuthenticationApi.Domain.Entities
{
    public class AppUser
    {
        public Guid UserAccountID { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Role { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool? IsActive { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Address { get; set; }
        public string? Bio { get; set; }
        public bool? EmailVerified { get; set; }
        public bool? PhoneVerified { get; set; }
        public string? VerificationToken { get; set; }
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }
        public string? OAuth2GoogleId { get; set; }
        public string? OAuth2FacebookId { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }
}