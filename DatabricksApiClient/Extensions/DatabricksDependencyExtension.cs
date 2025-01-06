
namespace Tachyon.Server.Common.DatabricksClient.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using Tachyon.Server.Common.DatabricksClient.Configuration;
    using Tachyon.Server.Common.DatabricksClient.Services;

    public static class DatabricksDependencyExtension
    {
        public static IServiceCollection AddDatabricksDependency(this IServiceCollection services)
        {
            services
                .AddScoped<IDatabricksApiService, DatabricksApiService>()
                .AddScoped<DatabricksApi>();

            return services;
        }
    }
}
