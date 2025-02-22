namespace AuthenticationApi.Application.DTOs
{
    public class OAuthLoginDTO
    {
        public string Provider { get; set; } = string.Empty; // Google, Facebook
        public string IdToken { get; set; } = string.Empty; // Google ID Token
        public string AccessToken { get; set; } = string.Empty; // Facebook Access Token
    }
}
