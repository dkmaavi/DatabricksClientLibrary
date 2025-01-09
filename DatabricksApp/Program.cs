using DatabricksApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tachyon.Server.Common.DatabricksClient.Abstractions.Configuration;
using Tachyon.Server.Common.DatabricksClient.Extensions;
using Tachyon.Server.Common.DatabricksClient.Implementations.Interceptors;

class Program
{
    static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        var demoApp = host.Services.GetRequiredService<DemoApp>();
        await demoApp.RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddDatabricksDependency(
                    configureInterceptors: pipeline =>
                    {
                        pipeline.AddInterceptor<LoggingInterceptor>();
                    });

                services.AddScoped<IDatabricksConfigurationProvider, DatabricksConfigurationProvider>();
                services.AddScoped<DemoApp>();
            })
            .ConfigureLogging((context, logging) =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Debug);
            });
}


