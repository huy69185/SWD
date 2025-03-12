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
        // Đăng ký người dùng mới
        public async Task<IActionResult> Register([FromBody] AppUserDTO userDTO)
        {
            var response = await userRepository.Register(userDTO);
            return response.Flag ? Ok(new ApiResponse(true, response.Message)) : BadRequest(new ApiResponse(false, response.Message));
        }

        [HttpPost("login")]
        // Đăng nhập người dùng
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var response = await userRepository.Login(loginDTO);
            return response.Flag ? Ok(new ApiResponse(true, response.Message)) : Unauthorized(new ApiResponse(false, response.Message));
        }

        [HttpGet("{userId}")]
        [Authorize]
        // Lấy thông tin người dùng theo ID
        public async Task<IActionResult> GetUser([FromRoute] Guid userId)
        {
            var user = await userRepository.GetUser(userId);
            return user == null ? NotFound(new ApiResponse(false, "User not found")) : Ok(new ApiResponse(true, data: user));
        }

        [HttpPost("bug-report")]
        [Authorize]
        // Tạo báo cáo lỗi
        public async Task<IActionResult> CreateBugReport([FromBody] BugReportDTO bugReportDTO)
        {
            var response = await userRepository.CreateBugReport(bugReportDTO);
            return response.Flag ? Ok(new ApiResponse(true, response.Message)) : BadRequest(new ApiResponse(false, response.Message));
        }

        [HttpGet("bug-reports/{userId}")]
        [Authorize]
        // Lấy danh sách báo cáo lỗi theo userId
        public async Task<IActionResult> GetBugReports([FromRoute] Guid userId)
        {
            var bugReports = await userRepository.GetBugReports(userId);
            return Ok(new ApiResponse(true, data: bugReports));
        }

        [HttpPost("notification")]
        [Authorize]
        // Gửi thông báo
        public async Task<IActionResult> SendNotification([FromBody] NotificationDTO notificationDTO)
        {
            var response = await userRepository.SendNotification(notificationDTO);
            return response.Flag ? Ok(new ApiResponse(true, response.Message)) : BadRequest(new ApiResponse(false, response.Message));
        }

        [HttpGet("notifications/{userId}")]
        [Authorize]
        // Lấy danh sách thông báo theo userId
        public async Task<IActionResult> GetNotifications([FromRoute] Guid userId)
        {
            var notifications = await userRepository.GetNotifications(userId);
            return Ok(new ApiResponse(true, data: notifications));
        }
    }
}