using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tachyon.Server.Common.DatabricksClient.Abstractions.Builders;
using Tachyon.Server.Common.DatabricksClient.Abstractions.Handlers;
using Tachyon.Server.Common.DatabricksClient.Abstractions.Interceptors;
using Tachyon.Server.Common.DatabricksClient.Models.Request;
using Tachyon.Server.Common.DatabricksClient.Models.Response;

namespace Tachyon.Server.Common.DatabricksClient.Implementations.Builders
{
    public class DatabricksPipelineBuilder : IDatabricksPipelineBuilder
    {
        private readonly IServiceCollection services;
        private readonly List<Type> interceptorTypes = new();

        public DatabricksPipelineBuilder(IServiceCollection services)
        {
            this.services = services;
        }

        public IDatabricksPipelineBuilder AddInterceptor<TInterceptor>() where TInterceptor : class, IDatabricksInterceptor
        {
            services.AddTransient<TInterceptor>();
            interceptorTypes.Add(typeof(TInterceptor));
            return this;
        }

        public IDatabricksPipelineBuilder RemoveInterceptors()
        {
            interceptorTypes.Clear();
            return this;
        }

        public IDatabricksPipelineBuilder AddHandler<THandler>() where THandler : class, IDatabricksQueryHandler
        {
            services.AddTransient<IDatabricksQueryHandler, THandler>();
            return this;
        }

        public void Build()
        {
            services.AddSingleton(sp =>
            {
                DatabricksPipelineProcessor pipelineProcessor = async (request, cancellationToken) =>
                {
                    var currentIndex = -1;

                    async Task<StatementResult> InvokeNext(StatementQuery req)
                    {
                        currentIndex++;

                        if (cancellationToken.IsCancellationRequested)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                        }

                        if (currentIndex < interceptorTypes.Count)
                        {
                            var interceptor = (IDatabricksInterceptor)sp.GetRequiredService(interceptorTypes[currentIndex]);

                            await SafeInvokeInterceptor(() => interceptor.PreProcessAsync(req), interceptor, "PreProcessAsync");

                            var response = await InvokeNext(req);

                            await SafeInvokeInterceptor(() => interceptor.PostProcessAsync(response), interceptor, "PostProcessAsync");

                            return response;
                        }

                        var handler = sp.GetRequiredService<IDatabricksQueryHandler>();
                        return await handler.HandleQueryAsync(req, cancellationToken);
                    }

                    return await InvokeNext(request);

                    Task SafeInvokeInterceptor(Func<Task> action, IDatabricksInterceptor interceptor, string phase)
                    {
                        try
                        {
                            return action();
                        }
                        catch (Exception ex)
                        {
                            var logger = sp.GetRequiredService<ILogger<DatabricksPipelineBuilder>>();
                            if (interceptor.Priority == InterceptorPriority.Critical)
                            {
                                logger.LogError(ex, "Critical error during {Phase} in interceptor {InterceptorType}. Terminating pipeline execution.", phase, interceptor.GetType().Name);
                                throw;
                            }

                            logger.LogError(ex, "Non-critical error during {Phase} in interceptor {InterceptorType}. Continuing pipeline execution.", phase, interceptor.GetType().Name);
                            return Task.CompletedTask;
                        }
                    }
                };

                return pipelineProcessor;
            });

        }
    }
}