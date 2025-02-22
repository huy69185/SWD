using System;

namespace AuthenticationApi.Domain.Entities
{
    public class AppUser
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty; // Google, Facebook
        public string? RefreshToken { get; set; } // Lưu Refresh Token
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
