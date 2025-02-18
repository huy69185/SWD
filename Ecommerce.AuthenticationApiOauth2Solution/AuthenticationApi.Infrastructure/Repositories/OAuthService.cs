using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace AuthenticationApi.Infrastructure.OAuth
{
    public class OAuthService
    {
        private readonly FirebaseApp _firebaseApp;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public OAuthService(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();

            var firebaseConfigPath = configuration["Firebase:CredentialPath"];
            _firebaseApp = FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(firebaseConfigPath)
            });
        }

        public async Task<FirebaseToken?> VerifyGoogleTokenAsync(string idToken)
        {
            var auth = FirebaseAuth.GetAuth(_firebaseApp);
            return await auth.VerifyIdTokenAsync(idToken);
        }

        public async Task<FacebookUserInfoDTO?> VerifyFacebookTokenAsync(string accessToken)
        {
            try
            {
                // Gọi API kiểm tra token hợp lệ
                var appId = _configuration["Facebook:AppId"];
                var appSecret = _configuration["Facebook:AppSecret"];
                var debugTokenUrl = $"https://graph.facebook.com/debug_token?input_token={accessToken}&access_token={appId}|{appSecret}";

                var response = await _httpClient.GetAsync(debugTokenUrl);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var tokenInfo = JsonSerializer.Deserialize<FacebookTokenDebugResponse>(json);

                if (tokenInfo?.Data?.IsValid != true)
                {
                    return null; // Token không hợp lệ
                }

                // Lấy thông tin người dùng từ Facebook
                var userInfoUrl = $"https://graph.facebook.com/me?fields=id,name,email&access_token={accessToken}";
                var userResponse = await _httpClient.GetAsync(userInfoUrl);
                userResponse.EnsureSuccessStatusCode();
                var userJson = await userResponse.Content.ReadAsStringAsync();
                var userInfo = JsonSerializer.Deserialize<FacebookUserInfoDTO>(userJson);

                return userInfo;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi xác thực Facebook: {ex.Message}");
                return null;
            }
        }
    }
    public class FacebookUserInfoDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class FacebookTokenDebugResponse
    {
        public TokenData Data { get; set; }
    }

    public class TokenData
    {
        public bool IsValid { get; set; }
    }
    public class FacebookUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
