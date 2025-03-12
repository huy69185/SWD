using ParentManageApi.Infrastructure.DependencyInjection;
using GrowthTracking.ShareLibrary.DependencyInjection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Parent Manage API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Parent Manage API v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseInfrastructurePolicy();
app.MapControllers();

app.Run("http://localhost:5002");