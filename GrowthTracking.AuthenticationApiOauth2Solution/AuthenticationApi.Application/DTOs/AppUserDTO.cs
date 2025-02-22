namespace AuthenticationApi.Application.DTOs
{
    public class AppUserDTO
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
    }
}
