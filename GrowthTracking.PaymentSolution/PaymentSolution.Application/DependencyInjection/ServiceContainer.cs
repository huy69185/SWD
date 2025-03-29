using Microsoft.Extensions.DependencyInjection;
using PaymentSolution.Application.Interfaces;

namespace PaymentSolution.Application.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            return services;
        }
    }
}
