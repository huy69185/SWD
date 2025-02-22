using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Data;
using GrowthTracking.ShareLibrary.Response;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationApi.Infrastructure.Repositories
{
    public class UserRepository(AuthenticationDbContext context, IConfiguration config) : IUserRepository
    {
        public async Task<AppUserDTO?> GetUserDTO<TKey>(TKey userId)
        {
            var user = await context.Users.FindAsync(userId);
            return user?.Adapt<AppUserDTO>();
        }

        public async Task<Response> Login(LoginDTO loginDTO)
        {
            var user = await GetUserByEmail(loginDTO.Email);
            if (user == null)
            {
                return new Response(false, "Invalid credentials");
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password))
            {
                return new Response(false, "Invalid credentials");
            }

            string token = GenerateToken(user);
            return new Response(true, token);
        }

        private string GenerateToken(AppUser user)
        {
            var key = Encoding.UTF8.GetBytes(config.GetSection("Authentication:Key").Value!);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var  claims = new List<Claim>
            {
                new (ClaimTypes.Name, user.Name!),
                new (ClaimTypes.Email, user.Email!),
            };
            if (!string.IsNullOrEmpty(user.Role))
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Role!));
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

        public async Task<Response> Register(AppUserDTO appUserDTO)
        {
            var user = await GetUserByEmail(appUserDTO.Email);
            if (user != null)
            {
                return new Response(false, "You cannot use this email for registration");
            }

            var result = context.Users.Add(new AppUser
            {
                Name = appUserDTO.Name,
                Email = appUserDTO.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(appUserDTO.Password),
                PhoneNumber = appUserDTO.PhoneNumber,
                Address = appUserDTO.Address,
                Role = appUserDTO.Role
            });
            await context.SaveChangesAsync();
            return result != null ? 
                new Response(true, "User registered successfully") : new Response(false, "User registration failed");
        }

        private async Task<AppUser?> GetUserByEmail(string email)
        {
            return await context.Users.FirstOrDefaultAsync(user => user.Email == email);
        }
    }
}
