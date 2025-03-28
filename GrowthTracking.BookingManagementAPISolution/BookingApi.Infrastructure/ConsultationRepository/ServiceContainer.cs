using BookingApi.Application.Interfaces;
using BookingApi.Application.Messaging;
using BookingApi.Application.Services;
using BookingApi.Infrastructure.Data;
using BookingApi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookingApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<BookingDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IScheduleRepository, ScheduleRepository>();
            services.AddScoped<IConsultationRepository, ConsultationRepository>();

            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IConsultationService, ConsultationService>();

            services.AddSingleton<IEventPublisher, EventPublisher>();
            services.AddHostedService<ParentEventConsumer>();
            services.AddHostedService<DoctorEventConsumer>();

            // Đăng ký PaymentServiceClient
            services.AddHttpClient<IPaymentServiceClient, PaymentServiceClient>();

            // Đăng ký PaymentDeadlineChecker
            services.AddHostedService<PaymentDeadlineChecker>();

            return services;
        }
    }
}