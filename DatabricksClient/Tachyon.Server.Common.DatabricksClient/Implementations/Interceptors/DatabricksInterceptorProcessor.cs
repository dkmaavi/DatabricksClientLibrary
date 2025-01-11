namespace Tachyon.Server.Common.DatabricksClient.Implementations.Interceptors
{
    using Microsoft.Extensions.Logging;
    using Tachyon.Server.Common.DatabricksClient.Abstractions.Interceptors;
    using Tachyon.Server.Common.DatabricksClient.Models.Request;
    using Tachyon.Server.Common.DatabricksClient.Models.Response;

    internal class DatabricksInterceptorProcessor
    {
        private readonly IEnumerable<IDatabricksInterceptor> interceptors;
        private readonly ILogger<DatabricksInterceptorProcessor> logger;
        private readonly InterceptorContext globalContext = new();
        public DatabricksInterceptorProcessor(IEnumerable<IDatabricksInterceptor> interceptors, ILogger<DatabricksInterceptorProcessor> logger)
        {
            this.interceptors = interceptors.ToList();
            this.logger = logger;
        }

        public InterceptorContext GlobalContext { get { return globalContext; } }

        public async Task BeforeRequestAsync(StatementQuery statementQuery)
        {
            globalContext.Timer.Start();

            logger.LogDebug("Executing interceptors with global id - {RequestId}", globalContext.Id);

            foreach (var interceptor in interceptors)
            {
                try
                {
                    logger.LogDebug("Executing before request interceptor {InterceptorType} with priority {Priority}", interceptor.GetType().Name, interceptor.Priority);

                    await interceptor.BeforeRequestAsync(statementQuery);
                }
                catch (Exception ex)
                {
                    if (interceptor.Priority == InterceptorPriority.Critical)
                    {
                        logger.LogError("Failed to execute before interceptors for global id - {RequestId}", globalContext.Id);
                        throw;
                    }
                    else
                    {
                        logger.LogError(ex, "Error in interceptor {InterceptorType} during BeforeRequestAsync", interceptor.GetType().Name);
                    }
                }
            }
        }

        public async Task AfterRequestAsync(StatementResult statementResult)
        {
            foreach (var interceptor in interceptors.Reverse())
            {
                try
                {
                    logger.LogDebug("Executing after request interceptor {InterceptorType} with priority {Priority}", interceptor.GetType().Name, interceptor.Priority);

                    await interceptor.AfterRequestAsync(statementResult);
                }
                catch (Exception ex)
                {
                    if (interceptor.Priority == InterceptorPriority.Critical)
                    {
                        logger.LogError("Failed to execute after interceptors for global id - {RequestId}", globalContext.Id);
                        throw;
                    }
                    else
                    {
                        logger.LogError(ex, "Error in interceptor {InterceptorType} during AfterRequestAsync", interceptor.GetType().Name);
                    }
                }
            }

            globalContext.Timer?.Stop();
            var duration = globalContext.Timer?.Elapsed.TotalMilliseconds ?? 0;

            logger.LogDebug("Successfully executed interceptors for global request id - {queryId}, with a total duration of : {duration:F2}ms", globalContext.Id, duration);

        }
    }
}
