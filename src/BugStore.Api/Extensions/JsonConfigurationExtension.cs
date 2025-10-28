using System.Text.Json;

namespace BugStore.Api.Extensions
{
    public static class JsonConfigurationExtension
    {public static IServiceCollection AddJsonConfiguration(this IServiceCollection services)
        {
            services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.SerializerOptions.PropertyNameCaseInsensitive = true;
            });
            return services;
        }
    }
}