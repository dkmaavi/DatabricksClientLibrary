
namespace Tachyon.Server.Common.DatabricksClient.Extensions
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Polly;
    using Polly.Extensions.Http;
    using Tachyon.Server.Common.DatabricksClient.Configuration;
    using Tachyon.Server.Common.DatabricksClient.Helpers;
    using Tachyon.Server.Common.DatabricksClient.Services;

    public static class DatabricksDependencyExtension
    {
        public static IServiceCollection AddDatabricksDependency(this IServiceCollection services)
        {
            services.AddSingleton(sp =>
                {
                    var configurationService = sp.GetRequiredService<IDatabricksConfigurationService>();
                    return configurationService.GetHttpSettings();
                }).AddSingleton(sp =>
                {
                    var configurationService = sp.GetRequiredService<IDatabricksConfigurationService>();
                    return configurationService.GetApiSettings();
                }).AddSingleton(sp =>
                {
                    var configurationService = sp.GetRequiredService<IDatabricksConfigurationService>();
                    return configurationService.GetResilienceSettings();
                });

            services
                .AddScoped<IDatabricksService, DatabricksService>()
                .AddScoped<IDatabricksClient, DatabricksClient>();

            services.AddHttpClient(DatabricksConstant.HttpClientName)
               .ConfigureHttpClient((sp, client) =>
               {
                   var resilienceSettings = sp.GetRequiredService<ResilienceSettings>();
                   client.Timeout = TimeSpan.FromSeconds(resilienceSettings.HttpTimeout);
               })
               .AddPolicyHandler((sp, _) =>
               {
                   var resilienceSettings = sp.GetRequiredService<ResilienceSettings>();
                   var logger = sp.GetRequiredService<ILogger<DatabricksClient>>();

                   var retryPolicy = HttpPolicyExtensions
                       .HandleTransientHttpError()
                       .WaitAndRetryAsync(
                           retryCount: resilienceSettings.MaxRetry,
                           sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                           onRetry: (outcome, timespan, retryAttempt, context) =>
                           {
                               logger.LogWarning(
                                   $"Databricks service retry attempt {retryAttempt} for {context.PolicyKey}. " +
                                   $"with delay: {timespan}. " +
                                   $"with reason: {outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString()}.");
                           });


                   var circuitBreakerPolicy = HttpPolicyExtensions
                        .HandleTransientHttpError()
                        .CircuitBreakerAsync(
                            handledEventsAllowedBeforeBreaking: resilienceSettings.MaxRetry,
                            durationOfBreak: TimeSpan.FromSeconds(resilienceSettings.CircuitBreakDuration),
                            onBreak: (outcome, breakDelay, context) =>
                            {
                                logger.LogError(
                                    $"Databricks service broken for {context.PolicyKey}. " +
                                    $"with delay: {breakDelay}. " +
                                    $"with reason: {outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString()}.");
                            },
                            onReset: context =>
                            {
                                logger.LogInformation($"Databricks service reset for {context.PolicyKey}.");
                            },
                            onHalfOpen: () =>
                            {
                                logger.LogWarning("Databricks service is open for test requests only.");
                            });

                   return Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);
               });

            return services;
        }
    }
}
