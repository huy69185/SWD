using GrowthTracking.DoctorSolution.Application.DTOs;
using GrowthTracking.DoctorSolution.Application.Interfaces;
using GrowthTracking.ShareLibrary.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrowthTracking.DoctorSolution.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IUserService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO user)
        {
            // Data has been validated by ValidationFilter
            var result = await authService.Register(user);

            var response = new ApiResponse
            {
                Success = result.Flag,
                Message = result.Message,
                Data = result.Data,
            };

            return result.Flag switch
            {
                true => Ok(response),
                _ => StatusCode(StatusCodes.Status500InternalServerError, response),
            };
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            // Data has been validated by ValidationFilter
            var result = await authService.Login(loginDTO);

            return result.Flag switch
            {
                true => Ok(new ApiResponse
                {
                    Success = result.Flag,
                    Data = new { Token = result.Message },
                }),
                _ => StatusCode(StatusCodes.Status401Unauthorized, new ApiResponse
                {
                    Success = result.Flag,
                    Message = result.Message
                }),
            };
        }

        // POST api/auth/logout
        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // In a stateless JWT authentication system, logout is usually done by removing the token on the client side.
            // So here, just a success response can be sent.
            return Ok(new ApiResponse { Success = true, Message = "User logged out successfully" });
        }
    }
}
