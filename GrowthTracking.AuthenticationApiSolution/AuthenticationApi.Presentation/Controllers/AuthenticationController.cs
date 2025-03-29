using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationApi.Presentation.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController(IUserRepository userRepository) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AppUserDTO userDTO)
        {
            var response = await userRepository.Register(userDTO);
            return response.Flag ? Ok(new ApiResponse(true, response.Message)) : BadRequest(new ApiResponse(false, response.Message));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var response = await userRepository.Login(loginDTO);
            return response.Flag ? 
                Ok(new ApiResponse(true, "Login successfully", response.Message)) : 
                Unauthorized(new ApiResponse(false, response.Message));
        }

        [HttpGet("{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUser([FromRoute] Guid userId)
        {
            var user = await userRepository.GetUser(userId);
            return user == null ? NotFound(new ApiResponse(false, "User not found")) : Ok(new ApiResponse(true, data: user));
        }

        [HttpPost("bug-report")]
        [Authorize]
        public async Task<IActionResult> CreateBugReport([FromBody] BugReportDTO bugReportDTO)
        {
            var response = await userRepository.CreateBugReport(bugReportDTO);
            return response.Flag ? Ok(new ApiResponse(true, response.Message)) : BadRequest(new ApiResponse(false, response.Message));
        }

        [HttpGet("bug-reports/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetBugReports([FromRoute] Guid userId)
        {
            var bugReports = await userRepository.GetBugReports(userId);
            return Ok(new ApiResponse(true, data: bugReports));
        }

        [HttpPost("notification")]
        [Authorize]
        public async Task<IActionResult> SendNotification([FromBody] NotificationDTO notificationDTO)
        {
            var response = await userRepository.SendNotification(notificationDTO);
            return response.Flag ? Ok(new ApiResponse(true, response.Message)) : BadRequest(new ApiResponse(false, response.Message));
        }

        [HttpGet("notifications/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetNotifications([FromRoute] Guid userId)
        {
            var notifications = await userRepository.GetNotifications(userId);
            return Ok(new ApiResponse(true, data: notifications));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDTO)
        {
            var response = await userRepository.ForgotPassword(forgotPasswordDTO);
            return response.Flag ? Ok(new ApiResponse(true, response.Message)) : BadRequest(new ApiResponse(false, response.Message));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            var response = await userRepository.ResetPassword(resetPasswordDTO);
            return response.Flag ? Ok(new ApiResponse(true, response.Message)) : BadRequest(new ApiResponse(false, response.Message));
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            var response = await userRepository.VerifyEmail(token);
            return response.Flag ? Ok(new ApiResponse(true, response.Message)) : BadRequest(new ApiResponse(false, response.Message));
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpDTO sendOtpDTO)
        {
            var response = await userRepository.SendOtp(sendOtpDTO);
            return response.Flag ? Ok(new ApiResponse(true, response.Message)) : BadRequest(new ApiResponse(false, response.Message));
        }

        [HttpPost("verify-phone")]
        public async Task<IActionResult> VerifyPhoneNumber([FromBody] VerifyPhoneDTO verifyPhoneDTO)
        {
            var response = await userRepository.VerifyPhoneNumber(verifyPhoneDTO);
            return response.Flag ? Ok(new ApiResponse(true, response.Message)) : BadRequest(new ApiResponse(false, response.Message));
        }
    }
}