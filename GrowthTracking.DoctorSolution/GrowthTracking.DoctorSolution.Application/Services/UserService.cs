using GrowthTracking.DoctorSolution.Application.DTOs;
using GrowthTracking.DoctorSolution.Application.Interfaces;
using GrowthTracking.DoctorSolution.Domain.Constants;
using GrowthTracking.DoctorSolution.Domain.Enums;
using GrowthTracking.DoctorSolution.Infrastructure.DBContext;
using Microsoft.AspNetCore.Http;
using Polly.Registry;

namespace GrowthTracking.DoctorSolution.Application.Services
{
    public class UserService(
        HttpClient httpClient, 
        ResiliencePipelineProvider<string> resiliencePipeline,
        IUserRepository repo,
        ITokenService tokenService) : IUserService
    {
        public async Task<bool> CheckUserExists(string userId)
        {
            //// Get retry pipeline
            //var retryPipeline = resiliencePipeline.GetPipeline("my-retry-pipeline");

            ////Prepare response
            //var response = await retryPipeline.ExecuteAsync(
            //    async token => await httpClient.GetAsync($"/api/users/{userId}", token));
            if (!Guid.TryParse(userId, out Guid guidId))
            {
                return false;
            }
            var user = await repo.GetByIdAsync(guidId);

            return user != null;
        }

        public async Task<Response> Login(LoginDTO loginDTO)
        {
            var user = await repo.GetUserByEmail(loginDTO.Email);
            if (user == null)
            {
                return new Response(false, "Email not found");
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.PasswordHash))
            {
                return new Response(false, "Invalid password");
            }

            //Check if email is verified
            if (!user.EmailVerified.GetValueOrDefault())
            {
                return new Response(false, "Please verify your email address.");
            }

            user.LastLoginAt = DateTime.UtcNow;
            await repo.UpdateAsync(user);
            string token = tokenService.GenerateToken(user);
            return new Response(true, token);
        }

        public async Task<Response> Register(UserDTO userDTO)
        {
            var user = await repo.GetUserByEmail(userDTO.Email);
            if (user != null)
            {
                return new Response(false, "You cannot use this email for registration");
            }

            UserAccount entity = new()
            {
                UserAccountId = Guid.NewGuid(),
                FullName = userDTO.FullName,
                Email = userDTO.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDTO.Password),
                PhoneNumber = userDTO.PhoneNumber,
                Role = userDTO.Role ?? RoleEnum.Parent.ToString(),
                ProfilePictureUrl = userDTO.ProfilePictureUrl,
                Address = userDTO.Address,
                Bio = userDTO.Bio,
                //Set default
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                // EmailVerified = false
                EmailVerified = true
            };

            await repo.InsertAsync(entity);
            await repo.SaveAsync();

            //Generate email verification
            // user = await GenerateEmailVerificationToken(entity);

            //send email verification
            // await SendVerificationEmail(user.Email, user.VerificationToken!);

            return new Response(true, "User registered successfully", entity);
        }
    }
}