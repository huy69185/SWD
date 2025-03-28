using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationApi.Application.Services
{
    public class JWTokenService(IConfiguration config) : ITokenService
    {
        public string GenerateToken(AppUser user)
        {
            var key = Encoding.UTF8.GetBytes(config.GetSection("Authentication:Key").Value!);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new ("userId", user.UserAccountID.ToString()),
                new ("name", user.FullName!),
                new ("email", user.Email!),
            };
            if (!string.IsNullOrEmpty(user.Role))
            {
                claims.Add(new Claim("role", user.Role!));
            }

            var token = new JwtSecurityToken(
                issuer: config.GetSection("Authentication:Issuer").Value,
                audience: config.GetSection("Authentication:Audience").Value,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
