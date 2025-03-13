using GrowthTracking.DoctorSolution.Application.Interfaces;
using GrowthTracking.DoctorSolution.Application.Mapping;
using GrowthTracking.DoctorSolution.Infrastructure.Cloudinary;
using GrowthTracking.DoctorSolution.Infrastructure.DBContext;
using GrowthTracking.DoctorSolution.Infrastructure.Mapping;
using GrowthTracking.DoctorSolution.Infrastructure.Repositories;
using GrowthTracking.ShareLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GrowthTracking.DoctorSolution.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
        {
            //Add authentication scheme
            ShareServiceContainer.AddSharedService(services, config, config["MySerilog:FileName"]!);
            ShareServiceContainer.AddSharedDbContext<SWD_GrowthTrackingSystemDbContext>(services, config);

            //Add Mapster
            services.AddMapsterConfiguration();
            services.AddScoped<IMapperService, MapsterService>();

            // Register the CloudinarySettings section from appsettings.json
            services.Configure<CloudinarySettings>(config.GetSection("Cloudinary"));
            services.AddScoped<IFileStorageService, CloudinaryService>();

            // Add repositories
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IDoctorRepository, DoctorRepository>();
            services.AddScoped<ICertificateRepository, CertificateRepository>();

            return services;
        }
        public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
        {
            //Register middleware => global exception, listen to only api gateway
            ShareServiceContainer.UseSharedPolicies(app);
            return app;
        }
    }
}
