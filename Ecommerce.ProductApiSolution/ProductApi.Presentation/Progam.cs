using ProductApi.Infrastructure.DependencyInjection;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructureService(builder.Configuration);
var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();

app.UseInfrastructurePolicy();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

