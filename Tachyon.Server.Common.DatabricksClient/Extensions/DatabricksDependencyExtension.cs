

namespace Tachyon.Server.Common.DatabricksClient.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Polly;
    using Polly.Extensions.Http;
    using Tachyon.Server.Common.DatabricksClient.Abstractions;
    using Tachyon.Server.Common.DatabricksClient.Abstractions.Builders;
    using Tachyon.Server.Common.DatabricksClient.Abstractions.Parsers;
    using Tachyon.Server.Common.DatabricksClient.Abstractions.Providers;
    using Tachyon.Server.Common.DatabricksClient.Constants;
    using Tachyon.Server.Common.DatabricksClient.Implementations;
    using Tachyon.Server.Common.DatabricksClient.Implementations.Builders;
    using Tachyon.Server.Common.DatabricksClient.Implementations.Interceptors;
    using Tachyon.Server.Common.DatabricksClient.Implementations.Parsers;
    using Tachyon.Server.Common.DatabricksClient.Models.Configuration;

    public static class DatabricksDependencyExtension
    {
        public static IServiceCollection AddDatabricksDependency(this IServiceCollection services, Action<IInterceptorPipelineBuilder>? configureInterceptors = null)
        {
            RegisterConfiguration(services);

            RegisterServices(services);

            RegisterHttpClient(services);

            RegisterInterceptorPipeline(services, configureInterceptors);

            return services;
        }
        private static void RegisterConfiguration(IServiceCollection services)
        {
            services.AddSingleton(sp =>
            {
                var configurationService = sp.GetRequiredService<IDatabricksConfigurationProvider>();
                return configurationService.GetHttpClientSettings();
            }).AddSingleton(sp =>
            {
                var configurationService = sp.GetRequiredService<IDatabricksConfigurationProvider>();
                return configurationService.GetStatementApiSettings();
            }).AddSingleton(sp =>
            {
                var configurationService = sp.GetRequiredService<IDatabricksConfigurationProvider>();
                return configurationService.GetResilienceSettings();
            });
        }
        private static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IDatabricksClient, DatabricksClient>();

            services.TryAddScoped<IDatabricksHttpClientBuilder, DatabricksHttpClientBuilder>();
            services.TryAddScoped<IDatabricksHttpRequestBuilder, DatabricksHttpRequestBuilder>();
            services.TryAddScoped<IStatementResultParser, StatementResultParser>();
            
        }
        private static void RegisterHttpClient(IServiceCollection services)
        {
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

                    return !resilienceSettings.DisbleRetryPolicy
                        ? Policy.WrapAsync(retryPolicy, circuitBreakerPolicy)
                        : Policy.NoOpAsync<HttpResponseMessage>();
                });
        }
        private static void RegisterInterceptorPipeline(IServiceCollection services, Action<IInterceptorPipelineBuilder>? configureInterceptors)
        {
            var pipelineBuilder = new InterceptorPipelineBuilder(services);

            pipelineBuilder
                .AddInterceptor<LoggingInterceptor>();

            configureInterceptors?.Invoke(pipelineBuilder);

            pipelineBuilder.Build();
        }
    }
}
