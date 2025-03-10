using ParentManageApi.Infrastructure.DependencyInjection;
using GrowthTracking.ShareLibrary.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Diagnostics;
using GrowthTracking.ShareLibrary.Logs; // Thêm namespace cho LogHandler
using Serilog; // Thêm namespace cho Serilog

var builder = WebApplication.CreateBuilder(args);

// Kiểm tra cấu hình trước khi gọi AddSharedService
var authConfig = builder.Configuration.GetSection("Authentication");
if (string.IsNullOrEmpty(authConfig["Key"]) || string.IsNullOrEmpty(authConfig["Issuer"]) || string.IsNullOrEmpty(authConfig["Audience"]))
{
    throw new InvalidOperationException("Authentication configuration is missing. Please provide Authentication:Key, Authentication:Issuer, and Authentication:Audience in appsettings.json.");
}

// Thêm ShareLibrary service với tên file log cụ thể
builder.Services.AddSharedService(builder.Configuration, "ParentManageAPI");

// Thêm các service từ ParentManageAPI
builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);

// Thêm Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Parent Manage API", Version = "v1" });
});

// Cấu hình Serilog cho ứng dụng
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .MinimumLevel.Debug()
        .WriteTo.Debug()
        .WriteTo.Console()
        .WriteTo.File(
            path: Path.Combine(Directory.GetCurrentDirectory(), "Log", $"ParentManageAPI-{DateTime.Now:yyyy-MM-dd}.txt"),
            restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
            shared: true);
    Log.Information("Application has started.");
});

var app = builder.Build();

// Cấu hình pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Parent Manage API v1"));
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Parent Manage API v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseInfrastructurePolicy(); // Bao gồm middleware ghi log và xử lý exception từ ShareLibrary
app.MapControllers();

// Mở Swagger khi chạy trong môi trường Development
var swaggerUrl = "http://localhost:5002/swagger";
if (app.Environment.IsDevelopment())
{
    try
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = swaggerUrl,
            UseShellExecute = true
        });
        LogHandler.LogToConsole($"Automatically opened Swagger at: {swaggerUrl}");
    }
    catch (Exception ex)
    {
        LogHandler.LogExceptions(ex);
        LogHandler.LogToDebugger($"Failed to open browser automatically: {ex.Message}");
    }
}

// Ghi log khi ứng dụng dừng
AppDomain.CurrentDomain.ProcessExit += (s, e) =>
{
    LogHandler.LogToFile("Application is shutting down.");
    Log.CloseAndFlush();
};

// Chạy ứng dụng trên port 5002
app.Run("http://localhost:5002");