using Microsoft.Extensions.DependencyInjection;

namespace BugStore.Application.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<BugStore.Application.Handlers.Customers.Handler>();
            services.AddScoped<BugStore.Application.Handlers.Products.Handler>();
            services.AddScoped<BugStore.Application.Handlers.Orders.Handler>();
            return services;
        }
    }
}