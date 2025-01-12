using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tachyon.Server.Common.DatabricksClient.Abstractions.Services;
using Tachyon.Server.Common.DatabricksClient.Extensions;

namespace DemoApp
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var demoApp = host.Services.GetRequiredService<DatabricksApp>();
            await demoApp.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<Counters>();
                    services.AddScoped<DatabricksApp>();

                    /******************* configure databricks settings *********************************/

                    services.AddScoped<IDatabricksConfigurationService, DatabricksConfigurationService>();
                    //services.AddScoped<IStatementResultParser, CustomStatementResultParser>();

                    services.AddDatabricksDependency(
                        configureInterceptors: pipeline =>
                        {
                           // pipeline.RemoveInterceptors();
                            pipeline.AddInterceptor<MetricInterceptor>();
                        });

                    /***********************************************************************************/
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Debug);
                });
    }
}