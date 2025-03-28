using BookingApi.Infrastructure.Data;
using BookingApi.Infrastructure.DependencyInjection;
using BookingApi.Infrastructure.Mapping;
using BookingApi.Presentation.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApiConfiguration();

builder.Services.AddInfrastructure(builder.Configuration);

// Register Mapster mappings
MapsterConfiguration.RegisterMappings();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();