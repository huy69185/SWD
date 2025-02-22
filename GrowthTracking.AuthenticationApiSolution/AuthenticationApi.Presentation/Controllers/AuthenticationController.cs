using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using GrowthTracking.ShareLibrary.Response;
using GrowthTracking.ShareLibrary.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController(IUserRepository userRepository) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AppUserDTO user)
        {
            // Data has been validated by ValidationFilter
            var result = await userRepository.Register(user);
            
            return result.Flag switch
            {
                true => Ok(new ApiResponse
                {
                    Success = result.Flag,
                    Message = result.Message,
                }),
                _ => StatusCode(StatusCodes.Status500InternalServerError , new ApiResponse
                {
                    Success = result.Flag,
                    Message = result.Message,
                }),
            };
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            // Data has been validated by ValidationFilter
            var result = await userRepository.Login(loginDTO);

            return result.Flag switch
            {
                true => Ok(new ApiResponse
                {
                    Success = result.Flag,
                    Message = result.Message,
                }),
                _ => StatusCode(StatusCodes.Status401Unauthorized, new ApiResponse
                {
                    Success = result.Flag,
                    Message = result.Message,
                }),
            };
        }

        [HttpGet("{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUser([FromRoute, GuidValidation] string userId)
        {
            var result = await userRepository.GetUserDTO(Guid.Parse(userId));

            return result == null
                ? NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "User not found",
                })
                : Ok(new ApiResponse
                {
                    Success = true,
                    Data = result,
                });
        }
    }
}
