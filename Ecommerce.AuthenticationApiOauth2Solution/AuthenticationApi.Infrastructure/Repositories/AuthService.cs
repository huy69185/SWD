using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Data;
using AuthenticationApi.Infrastructure.OAuth;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AuthenticationApi.Infrastructure.Repositories
{
    public class AuthService : IAuthService
    {
        private readonly OAuthService _oauthService;
        private readonly AuthenticationDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthService(OAuthService oauthService, AuthenticationDbContext context, ITokenService tokenService)
        {
            _oauthService = oauthService;
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<ApiResponse> AuthenticateWithOAuth(OAuthLoginDTO loginDTO)
        {
            AppUser? user = null;

            if (loginDTO.Provider.ToLower() == "google")
            {
                var googleUser = await _oauthService.VerifyGoogleTokenAsync(loginDTO.IdToken);
                if (googleUser == null) return new ApiResponse(false, "Invalid Google Token");

                user = await _context.Users.FirstOrDefaultAsync(u => u.Email == googleUser.Claims["email"].ToString());

                if (user == null)
                {
                    user = new AppUser
                    {
                        Id = Guid.NewGuid(),
                        Email = googleUser.Claims["email"].ToString(),
                        Username = googleUser.Claims["name"].ToString(),
                        Provider = "Google"
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }
            }
            else if (loginDTO.Provider.ToLower() == "facebook")
            {
                var fbUser = await _oauthService.VerifyFacebookTokenAsync(loginDTO.AccessToken);
                if (fbUser == null) return new ApiResponse(false, "Invalid Facebook Token");

                user = await _context.Users.FirstOrDefaultAsync(u => u.Email == fbUser.Email);

                if (user == null)
                {
                    user = new AppUser
                    {
                        Id = Guid.NewGuid(),
                        Email = fbUser.Email,
                        Username = fbUser.Name,
                        Provider = "Facebook"
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                return new ApiResponse(false, "Unsupported provider");
            }

            var tokenResponse = _tokenService.GenerateTokens(user);
            return new ApiResponse(true, "Authentication successful", tokenResponse);
        }
    }
}
