using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http.Headers;
using Tachyon.Server.Common.DatabricksClient.Abstractions;
using Tachyon.Server.Common.DatabricksClient.Abstractions.Builders;
using Tachyon.Server.Common.DatabricksClient.Abstractions.Handlers;
using Tachyon.Server.Common.DatabricksClient.Abstractions.Services;
using Tachyon.Server.Common.DatabricksClient.Constants;
using Tachyon.Server.Common.DatabricksClient.Implementations;
using Tachyon.Server.Common.DatabricksClient.Implementations.Builders;
using Tachyon.Server.Common.DatabricksClient.Implementations.Handlers;
using Tachyon.Server.Common.DatabricksClient.Implementations.Interceptors;
using Tachyon.Server.Common.DatabricksClient.Implementations.Services;
using Tachyon.Server.Common.DatabricksClient.Models.Configuration;

namespace Tachyon.Server.Common.DatabricksClient.Extensions
{
    public static class DatabricksDependencyExtension
    {
        public static IServiceCollection AddDatabricksDependency(this IServiceCollection services, Action<IDatabricksPipelineBuilder>? configureInterceptors = null)
        {
            return services
                .AddDatabricksConfiguration()
                .AddDatabricksServices()
                .AddDatabricksHttpClient()
                .AddDatabricksInterceptors(configureInterceptors);
        }

        private static IServiceCollection AddDatabricksConfiguration(this IServiceCollection services)
        {
            services.AddSingleton(sp =>
            {
                var configService = sp.GetRequiredService<IDatabricksConfigurationService>();
                return configService.GetHttpClientSettings();
            });

            services.AddSingleton(sp =>
            {
                var configService = sp.GetRequiredService<IDatabricksConfigurationService>();
                return configService.GetStatementApiSettings();
            });

            services.AddSingleton(sp =>
            {
                var configService = sp.GetRequiredService<IDatabricksConfigurationService>();
                return configService.GetResilienceSettings();
            });

            return services;
        }

        private static IServiceCollection AddDatabricksServices(this IServiceCollection services)
        {
            services.AddScoped<IDatabricksApiClient, DatabricksApiClient>();

            services.TryAddScoped<IDatabricksCommunicationService, DatabricksCommunicationService>();
            services.TryAddScoped<IDatabricksResultHandler, DatabricksResultHandler>();
            services.TryAddScoped<IDatabricksErrorHandler, DatabricksErrorHandler>();

            return services;
        }

        private static IServiceCollection AddDatabricksHttpClient(this IServiceCollection services)
        {
            services.AddHttpClient(DatabricksConstant.HttpClientName)
                .ConfigureHttpClient((sp, client) =>
                {
                    var settings = sp.GetRequiredService<HttpClientSettings>();

                    client.BaseAddress = new Uri(settings.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(settings.HttpTimeout);

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.BearerToken);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                })
                .AddPolicyHandler(CreateResiliencePolicy);

            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> CreateResiliencePolicy(IServiceProvider sp, HttpRequestMessage _)
        {
            var settings = sp.GetRequiredService<ResilienceSettings>();
            var logger = sp.GetRequiredService<ILogger<DatabricksApiClient>>();

            if (settings.DisbleRetryPolicy)
            {
                return Policy.NoOpAsync<HttpResponseMessage>();
            }

            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    retryCount: settings.MaxRetry,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        logger.LogWarning("Databricks service retry attempt {Attempt} for {PolicyKey} with interval: { Delay}. Reason: { Reason}",
                                retryAttempt, context.PolicyKey, timespan, outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString());
                    });

            var circuitBreakerPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: settings.MaxRetry,
                    durationOfBreak: TimeSpan.FromSeconds(settings.CircuitBreakDuration),
                    onBreak: (outcome, breakDelay, context) =>
                    {
                        logger.LogError("Databricks service broken for {PolicyKey} for {Delay} Seconds. Reason: {Reason}",
                            context.PolicyKey, breakDelay, outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString());
                    },
                    onReset: context =>
                    {
                        logger.LogInformation(
                            "Databricks service reset for {PolicyKey}",
                            context.PolicyKey);
                    },
                    onHalfOpen: () =>
                    {
                        logger.LogWarning(
                            "Databricks service is open for test requests only");
                    });

            return Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);
        }

        private static IServiceCollection AddDatabricksInterceptors(this IServiceCollection services, Action<IDatabricksPipelineBuilder>? configureInterceptors)
        {
            var pipelineBuilder = new DatabricksPipelineBuilder(services);

            pipelineBuilder.AddHandler<DatabricksQueryHandler>();
            pipelineBuilder.AddInterceptor<ValidationInterceptor>();
            pipelineBuilder.AddInterceptor<LoggingInterceptor>();

            configureInterceptors?.Invoke(pipelineBuilder);
            pipelineBuilder.Build();

            return services;
        }
    }
}