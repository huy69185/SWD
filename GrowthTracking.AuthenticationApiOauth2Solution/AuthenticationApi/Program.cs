using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GrowthTracking.ShareLibrary.DependencyInjection;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Infrastructure.Repositories;
using AuthenticationApi.Infrastructure.OAuth;
using AuthenticationApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using AuthenticationApi.Infrastructure.Firebase;

var builder = WebApplication.CreateBuilder(args);

// Load cấu hình
var configuration = builder.Configuration;

// Đăng ký Shared Services
builder.Services.AddSharedService(configuration, "AuthenticationApi");

// Đăng ký DbContext
builder.Services.AddDbContext<AuthenticationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// Đăng ký các Service cần thiết
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>(); // Thêm ITokenService
builder.Services.AddSingleton<OAuthService>(); // Đăng ký OAuthService
builder.Services.AddSingleton<FirebaseAuthService>(); // Đăng ký FirebaseAuthService

// Thêm Controller
builder.Services.AddControllers();

// Thêm Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Build ứng dụng
var app = builder.Build();

app.UseStaticFiles(); // Cho phép phục vụ file tĩnh

// Sử dụng các Middleware chung
app.UseSharedPolicies();

// Nếu đang ở môi trường Development thì bật Developer Exception Page
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Authentication API v1");
        c.RoutePrefix = string.Empty; // Thiết lập Swagger UI tại root "/"
    });
}

// Middleware Authentication và Authorization
app.UseAuthentication();
app.UseAuthorization();

// Ánh xạ Controller
app.MapControllers();

// Chạy ứng dụng
app.Run("http://localhost:5000");
