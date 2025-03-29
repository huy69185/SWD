using ChildApi.Application.Interfaces;
using ChildApi.Application.Messaging;
using ChildApi.Infrastructure.Data;
using ChildApi.Infrastructure.Mapping;
using ChildApi.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Thêm Controllers
builder.Services.AddControllers();

// Cấu hình Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cấu hình Authentication JWT
var authenticationSettings = builder.Configuration.GetSection("Authentication");
// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
// })
// .AddJwtBearer(options =>
// {
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuer = true,
//         ValidateAudience = true,
//         ValidateLifetime = true,
//         ValidateIssuerSigningKey = true,
//         ValidIssuer = authenticationSettings["Issuer"], // http://localhost:5002
//         ValidAudience = authenticationSettings["Audience"], // http://localhost:5002
//         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings["Key"])) 
//     };
// });

// Cấu hình DbContext và các dịch vụ khác
builder.Services.AddDbContext<ChildDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IChildRepository, ChildRepository>();
builder.Services.AddScoped<IMilestoneRepository, MilestoneRepository>();
builder.Services.AddSingleton<ParentIdCache>();
builder.Services.AddHostedService<ParentEventConsumer>();
builder.Services.AddSingleton<IEventPublisher, EventPublisher>();

MapsterConfiguration.RegisterMappings();

var app = builder.Build();

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
// app.UseAuthentication(); 
// app.UseAuthorization();
app.MapControllers();

app.Run();