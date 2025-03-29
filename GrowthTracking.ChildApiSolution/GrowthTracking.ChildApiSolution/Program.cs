using ChildApi.Infrastructure.Data;
using ChildApi.Infrastructure.Mapping;
using ChildApi.Infrastructure.Repositories;
using ChildApi.Application.Interfaces;
using ChildApi.Application.Messaging;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Thêm Controllers vào container
builder.Services.AddControllers();

// Thêm Swagger cho tài liệu API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IEventPublisher, EventPublisher>();

// Cấu hình DbContext với connection string từ file cấu hình (appsettings.json)
builder.Services.AddDbContext<ChildDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Đăng ký Mapster configuration
MapsterConfiguration.RegisterMappings();

// Đăng ký repository interfaces và implementations
builder.Services.AddScoped<IChildRepository, ChildRepository>();
builder.Services.AddScoped<IMilestoneRepository, MilestoneRepository>();

// Đăng ký ParentIdCache và RabbitMQ Consumer (BackgroundService)
builder.Services.AddSingleton<ParentIdCache>();
builder.Services.AddHostedService<ParentEventConsumer>();

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

// Nếu có xác thực thì kích hoạt Authentication
// app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
