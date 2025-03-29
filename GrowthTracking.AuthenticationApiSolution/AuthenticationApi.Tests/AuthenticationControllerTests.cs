using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Presentation.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace AuthenticationApi.Tests
{
    public class AuthenticationControllerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly AuthenticationController _controller;

        public AuthenticationControllerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _controller = new AuthenticationController(_userRepositoryMock.Object);

            // Gi? l?p xác th?c cho các endpoint có [Authorize]
            SetupAuthenticatedUser();
        }

        #region Register
        [Fact]
        public async Task Register_Success_ReturnsOk()
        {
            // Arrange
            var userDTO = new AppUserDTO(Guid.NewGuid(), "Test User", "test@example.com", "password123", "1234567890", "Parent");
            var response = new GrowthTracking.ShareLibrary.Response.Response(true, "User registered successfully");
            _userRepositoryMock.Setup(repo => repo.Register(userDTO)).ReturnsAsync(response);

            // Act
            var result = await _controller.Register(userDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal("User registered successfully", apiResponse.Message);
        }

        [Fact]
        public async Task Register_Failure_ReturnsBadRequest()
        {
            // Arrange
            var userDTO = new AppUserDTO(Guid.NewGuid(), "Test User", "test@example.com", "password123", "1234567890", "Parent");
            var response = new GrowthTracking.ShareLibrary.Response.Response(false, "Email already registered");
            _userRepositoryMock.Setup(repo => repo.Register(userDTO)).ReturnsAsync(response);

            // Act
            var result = await _controller.Register(userDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(badRequestResult.Value);
            Assert.False(apiResponse.Success);
            Assert.Equal("Email already registered", apiResponse.Message);
        }
        #endregion

        #region Login
        [Fact]
        public async Task Login_Success_ReturnsOk()
        {
            // Arrange
            var loginDTO = new LoginDTO("test@example.com", "password123");
            var response = new GrowthTracking.ShareLibrary.Response.Response(true, "jwt-token");
            _userRepositoryMock.Setup(repo => repo.Login(loginDTO)).ReturnsAsync(response);

            // Act
            var result = await _controller.Login(loginDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal("Login successfully", apiResponse.Message);
            Assert.Equal("jwt-token", apiResponse.Data);
        }

        [Fact]
        public async Task Login_Failure_ReturnsUnauthorized()
        {
            // Arrange
            var loginDTO = new LoginDTO("test@example.com", "wrongpassword");
            var response = new GrowthTracking.ShareLibrary.Response.Response(false, "Invalid credentials");
            _userRepositoryMock.Setup(repo => repo.Login(loginDTO)).ReturnsAsync(response);

            // Act
            var result = await _controller.Login(loginDTO);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(unauthorizedResult.Value);
            Assert.False(apiResponse.Success);
            Assert.Equal("Invalid credentials", apiResponse.Message);
        }
        #endregion

        #region GetUser
        [Fact]
        public async Task GetUser_Success_ReturnsOk()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userDTO = new AppUserDTO(userId, "Test User", "test@example.com", null, "1234567890", "Parent");
            _userRepositoryMock.Setup(repo => repo.GetUser(userId)).ReturnsAsync(userDTO);

            // Act
            var result = await _controller.GetUser(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(userDTO, apiResponse.Data);
        }

        [Fact]
        public async Task GetUser_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userRepositoryMock.Setup(repo => repo.GetUser(userId)).ReturnsAsync((AppUserDTO?)null);

            // Act
            var result = await _controller.GetUser(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(notFoundResult.Value);
            Assert.False(apiResponse.Success);
            Assert.Equal("User not found", apiResponse.Message);
        }
        #endregion

        #region CreateBugReport
        [Fact]
        public async Task CreateBugReport_Success_ReturnsOk()
        {
            // Arrange
            var bugReportDTO = new BugReportDTO(Guid.NewGuid(), Guid.NewGuid(), "Error", "Description", null, "Open");
            var response = new GrowthTracking.ShareLibrary.Response.Response(true, "Bug report created successfully");
            _userRepositoryMock.Setup(repo => repo.CreateBugReport(bugReportDTO)).ReturnsAsync(response);

            // Act
            var result = await _controller.CreateBugReport(bugReportDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal("Bug report created successfully", apiResponse.Message);
        }
        #endregion

        #region GetBugReports
        [Fact]
        public async Task GetBugReports_Success_ReturnsOk()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var bugReports = new List<BugReportDTO>
            {
                new BugReportDTO(Guid.NewGuid(), userId, "Error", "Description", null, "Open")
            };
            _userRepositoryMock.Setup(repo => repo.GetBugReports(userId)).ReturnsAsync(bugReports);

            // Act
            var result = await _controller.GetBugReports(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(bugReports, apiResponse.Data);
        }
        #endregion

        #region SendNotification
        [Fact]
        public async Task SendNotification_Success_ReturnsOk()
        {
            // Arrange
            var notificationDTO = new NotificationDTO(Guid.NewGuid(), Guid.NewGuid(), "Info", "Content", "Sent");
            var response = new GrowthTracking.ShareLibrary.Response.Response(true, "Notification sent successfully");
            _userRepositoryMock.Setup(repo => repo.SendNotification(notificationDTO)).ReturnsAsync(response);

            // Act
            var result = await _controller.SendNotification(notificationDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal("Notification sent successfully", apiResponse.Message);
        }
        #endregion

        #region GetNotifications
        [Fact]
        public async Task GetNotifications_Success_ReturnsOk()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var notifications = new List<NotificationDTO>
            {
                new NotificationDTO(Guid.NewGuid(), userId, "Info", "Content", "Sent")
            };
            _userRepositoryMock.Setup(repo => repo.GetNotifications(userId)).ReturnsAsync(notifications);

            // Act
            var result = await _controller.GetNotifications(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(notifications, apiResponse.Data);
        }
        #endregion

        #region ForgotPassword
        [Fact]
        public async Task ForgotPassword_Success_ReturnsOk()
        {
            // Arrange
            var forgotPasswordDTO = new ForgotPasswordDTO { Email = "test@example.com" };
            var response = new GrowthTracking.ShareLibrary.Response.Response(true, "Password reset code sent");
            _userRepositoryMock.Setup(repo => repo.ForgotPassword(forgotPasswordDTO)).ReturnsAsync(response);

            // Act
            var result = await _controller.ForgotPassword(forgotPasswordDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal("Password reset code sent", apiResponse.Message);
        }
        #endregion

        #region ResetPassword
        [Fact]
        public async Task ResetPassword_Success_ReturnsOk()
        {
            // Arrange
            var resetPasswordDTO = new ResetPasswordDTO("123456", "oldpassword", "newpassword123");
            var response = new GrowthTracking.ShareLibrary.Response.Response(true, "Password reset successfully");
            _userRepositoryMock.Setup(repo => repo.ResetPassword(resetPasswordDTO)).ReturnsAsync(response);

            // Act
            var result = await _controller.ResetPassword(resetPasswordDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal("Password reset successfully", apiResponse.Message);
        }
        #endregion

        #region VerifyEmail
        [Fact]
        public async Task VerifyEmail_Success_ReturnsOk()
        {
            // Arrange
            var token = "valid-token";
            var response = new GrowthTracking.ShareLibrary.Response.Response(true, "Email verified successfully");
            _userRepositoryMock.Setup(repo => repo.VerifyEmail(token)).ReturnsAsync(response);

            // Act
            var result = await _controller.VerifyEmail(token);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal("Email verified successfully", apiResponse.Message);
        }
        #endregion

        #region SendOtp
        [Fact]
        public async Task SendOtp_Success_ReturnsOk()
        {
            // Arrange
            var sendOtpDTO = new SendOtpDTO("1234567890");
            var response = new GrowthTracking.ShareLibrary.Response.Response(true, "OTP sent successfully");
            _userRepositoryMock.Setup(repo => repo.SendOtp(sendOtpDTO)).ReturnsAsync(response);

            // Act
            var result = await _controller.SendOtp(sendOtpDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal("OTP sent successfully", apiResponse.Message);
        }
        #endregion

        #region VerifyPhoneNumber
        [Fact]
        public async Task VerifyPhoneNumber_Success_ReturnsOk()
        {
            // Arrange
            var verifyPhoneDTO = new VerifyPhoneDTO("1234567890", "123456");
            var response = new GrowthTracking.ShareLibrary.Response.Response(true, "Phone number verified successfully");
            _userRepositoryMock.Setup(repo => repo.VerifyPhoneNumber(verifyPhoneDTO)).ReturnsAsync(response);

            // Act
            var result = await _controller.VerifyPhoneNumber(verifyPhoneDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal("Phone number verified successfully", apiResponse.Message);
        }
        #endregion

        // Gi? l?p xác th?c cho các endpoint có [Authorize]
        private void SetupAuthenticatedUser()
        {
            var claims = new[]
            {
                new Claim("userId", Guid.NewGuid().ToString()),
                new Claim("name", "Test User"),
                new Claim("email", "test@example.com"),
                new Claim("role", "Parent")
            };
            var identity = new ClaimsIdentity(claims, "Bearer");
            var principal = new ClaimsPrincipal(identity);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.User).Returns(principal);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };
        }
    }
}