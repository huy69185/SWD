using GrowthTracking.DoctorSolution.Application.Interfaces;
using GrowthTracking.DoctorSolution.Infrastructure.DBContext;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GrowthTracking.DoctorSolution.Application.Services
{
    public class JWTokenService(IConfiguration config) : ITokenService
    {
        public string GenerateToken(UserAccount user)
        {
            var key = Encoding.UTF8.GetBytes(config.GetSection("Authentication:Key").Value!);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new ("userId", user.UserAccountId.ToString()),
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
