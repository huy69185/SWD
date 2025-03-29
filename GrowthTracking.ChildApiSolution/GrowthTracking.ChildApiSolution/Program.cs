using ChildApi.Infrastructure.Data;
using ChildApi.Infrastructure.Mapping;
using ChildApi.Infrastructure.Repositories;
using ChildApi.Application.Interfaces;
using ChildApi.Application.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Thêm Controllers vào container
builder.Services.AddControllers();

// Thêm Swagger cho tài liệu API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Đăng ký IEventPublisher với implementation EventPublisher
builder.Services.AddSingleton<IEventPublisher, EventPublisher>();

// Cấu hình DbContext với connection string từ file cấu hình (appsettings.json)
builder.Services.AddDbContext<ChildDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Đăng ký Mapster configuration
MapsterConfiguration.RegisterMappings();

// Đăng ký repository interfaces và implementations
builder.Services.AddScoped<IChildRepository, ChildRepository>();
builder.Services.AddScoped<IMilestoneRepository, MilestoneRepository>();

// Đăng ký ParentIdCache với interface IParentIdCache và RabbitMQ Consumer (BackgroundService)
builder.Services.AddSingleton<IParentIdCache, ParentIdCache>();
builder.Services.AddHostedService<ParentEventConsumer>();

// Cấu hình xác thực JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        ValidAudience = builder.Configuration["Authentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authentication:Key"]))
    };
});

var app = builder.Build();

// Cấu hình Swagger cho môi trường Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChildApi v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

// Kích hoạt Authentication và Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();