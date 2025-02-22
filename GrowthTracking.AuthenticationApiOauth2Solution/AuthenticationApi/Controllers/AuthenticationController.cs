using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using GrowthTracking.ShareLibrary.Response;

namespace AuthenticationApi.Presentation.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("oauth")]
        public async Task<IActionResult> AuthenticateWithOAuth([FromBody] OAuthLoginDTO loginDTO)
        {
            var tokenResponse = await _authService.AuthenticateWithOAuth(loginDTO);
            return Ok(tokenResponse);
        }
    }
}
