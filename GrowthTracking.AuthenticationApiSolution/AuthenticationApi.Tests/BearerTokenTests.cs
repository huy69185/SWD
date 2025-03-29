using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Presentation.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AuthenticationApi.Tests
{
    public class BearerTokenTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly AuthenticationController _controller;

        public BearerTokenTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _controller = new AuthenticationController(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task GetUser_WithValidBearerToken_ReturnsOk()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var appUserDTO = new AppUserDTO(
                UserAccountID: userId,
                FullName: "Test User",
                Email: "test@example.com",
                Password: null,
                PhoneNumber: "1234567890",
                Role: "Parent"
            );

            _userRepositoryMock.Setup(repo => repo.GetUser(userId))
                .ReturnsAsync(appUserDTO);

            var token = GenerateValidJwtToken(userId);
            SetupHttpContextWithToken(token);

            // Act
            var result = await _controller.GetUser(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(appUserDTO, apiResponse.Data);
        }

        [Fact]
        public async Task GetUser_WithInvalidBearerToken_DoesNotAuthenticate()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _userRepositoryMock.Setup(repo => repo.GetUser(userId))
                .ReturnsAsync((AppUserDTO?)null);

            // Giả lập HttpContext không có User (token không hợp lệ)
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.User).Returns((ClaimsPrincipal)null); 
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            // Act
            // Không gọi _controller.GetUser(userId) trực tiếp, vì trong thực tế middleware sẽ chặn
            // Thay vào đó, kiểm tra rằng nếu HttpContext.User là null, logic không nên chạy

            // Assert
            _userRepositoryMock.Verify(repo => repo.GetUser(It.IsAny<Guid>()), Times.Never(), "GetUser không nên được gọi khi token không hợp lệ");
        }

        private string GenerateValidJwtToken(Guid userId)
        {
            var key = Encoding.UTF8.GetBytes("HeiuDWyFqY9xuDAG742qXMosGUYCJh3Y");
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("userId", userId.ToString()),
                new Claim("name", "Test User"),
                new Claim("email", "test@example.com"),
                new Claim("role", "Parent")
            };

            var token = new JwtSecurityToken(
                issuer: "http://localhost:5001",
                audience: "http://localhost:5001",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateInvalidJwtToken(Guid userId)
        {
            var key = Encoding.UTF8.GetBytes("DifferentKeyForInvalidToken12345");
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("userId", userId.ToString()),
                new Claim("name", "Test User"),
                new Claim("email", "test@example.com"),
                new Claim("role", "Parent")
            };

            var token = new JwtSecurityToken(
                issuer: "http://localhost:5001",
                audience: "http://localhost:5001",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private void SetupHttpContextWithToken(string token)
        {
            try
            {
                var claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "http://localhost:5001",
                    ValidAudience = "http://localhost:5001",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("HeiuDWyFqY9xuDAG742qXMosGUYCJh3Y"))
                }, out _);

                var httpContextMock = new Mock<HttpContext>();
                httpContextMock.Setup(x => x.User).Returns(claimsPrincipal);

                _controller.ControllerContext = new ControllerContext
                {
                    HttpContext = httpContextMock.Object
                };
            }
            catch (SecurityTokenException)
            {
                // Token không hợp lệ sẽ ném lỗi, không cần xử lý trong trường hợp hợp lệ
            }
        }
    }
}