using AuthenticationApi.Infrastructure.DependencyInjection;
using GrowthTracking.ShareLibrary.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Logging;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        builder.Services.AddControllers();

        // Configure Infrastructure (Database, Repositories, RabbitMQ, etc.)
        builder.Services.AddInfrastructure(builder.Configuration);

        // Add API Exploration and Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Authentication API",
                Version = "v1",
                Description = "API for Authentication in Growth Tracking System"
            });

            // Chỉ định rõ phiên bản OpenAPI
            c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First()); // Giải quyết xung đột nếu có
            c.DocInclusionPredicate((docName, apiDesc) => true); // Đảm bảo tất cả API được bao gồm

            // Cấu hình bảo mật (giữ nguyên)
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter JWT with Bearer into field",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
            });
        });

        // Configure Authentication (JWT)
        var jwtKey = builder.Configuration["Authentication:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("JWT Key is not configured in appsettings.json under 'Authentication:Key'.");
        }

        var key = Encoding.ASCII.GetBytes(jwtKey);
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Authentication:Issuer"],
                    ValidAudience = builder.Configuration["Authentication:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

        // Configure Authorization
        builder.Services.AddAuthorization();

        var app = builder.Build();

        // Configure logging
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Starting application...");

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Authentication API v1");
                c.RoutePrefix = "swagger"; // Đảm bảo route chính xác
            });
        }

        // Add Exception Handling
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("An unexpected error occurred.");
                logger.LogError("An error occurred during request handling.");
            });
        });

        // Add HTTPS Redirection
        app.UseHttpsRedirection();

        // Add Authentication before Authorization
        app.UseAuthentication();
        app.UseAuthorization();

        // Apply Infrastructure Policies
        app.UseInfrastructurePolicy();

        // Map Controllers
        app.MapControllers();

        app.Run();
    }
}