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
            c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            c.DocInclusionPredicate((docName, apiDesc) => true);

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

        // Configure Authentication (JWT) with debug events
        var jwtKey = builder.Configuration["Authentication:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("JWT Key is not configured in appsettings.json under 'Authentication:Key'.");
        }

        var key = Encoding.UTF8.GetBytes(jwtKey); // Thống nhất sử dụng UTF8
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

                // Thêm sự kiện debug để kiểm tra lỗi xác thực
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogError("Authentication failed: {Exception}", context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogInformation("Token validated successfully for user: {UserId}", context.Principal?.FindFirst("userId")?.Value);
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogWarning("Authentication challenge triggered: {Error}", context.ErrorDescription);
                        return Task.CompletedTask;
                    }
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
                c.RoutePrefix = "swagger";
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

        // Ensure Authentication is before Authorization
        app.UseAuthentication();
        app.UseAuthorization();

        // Apply Infrastructure Policies
        app.UseInfrastructurePolicy();

        // Map Controllers
        app.MapControllers();

        app.Run();
    }
}