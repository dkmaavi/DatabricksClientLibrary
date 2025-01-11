namespace Tachyon.Server.Common.DatabricksClient.Implementations.Interceptors
{
    using Microsoft.Extensions.Logging;
    using System.Diagnostics;
    using Tachyon.Server.Common.DatabricksClient.Abstractions.Interceptors;
    using Tachyon.Server.Common.DatabricksClient.Constants;
    using Tachyon.Server.Common.DatabricksClient.Exceptions;
    using Tachyon.Server.Common.DatabricksClient.Models.Request;
    using Tachyon.Server.Common.DatabricksClient.Models.Response;

    internal class DatabricksInterceptorProcessor
    {
        private readonly IEnumerable<IDatabricksInterceptor> interceptors;
        private readonly ILogger<DatabricksInterceptorProcessor> logger;
        private readonly Dictionary<string, object> interceptorItems = new();
        public DatabricksInterceptorProcessor(IEnumerable<IDatabricksInterceptor> interceptors, ILogger<DatabricksInterceptorProcessor> logger)
        {
            this.interceptors = interceptors.ToList();
            this.logger = logger;
        }

        public async Task BeforeRequestAsync(StatementQuery statementQuery)
        {
            var requestId = Guid.NewGuid().ToString();
            interceptorItems.Add(DatabricksConstant.GlobalRequestIdKey, requestId);
            interceptorItems.Add(DatabricksConstant.GlobalRequestTimerKey, Stopwatch.StartNew());

            logger.LogDebug("Executing interceptors with global id - {RequestId}", requestId);

            foreach (var interceptor in interceptors)
            {
                try
                {
                    logger.LogDebug("Executing before request interceptor {InterceptorType} with priority {Priority}", interceptor.GetType().Name, interceptor.Priority);

                    await interceptor.BeforeRequestAsync(statementQuery, interceptorItems);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error in interceptor {InterceptorType} during BeforeRequestAsync", interceptor.GetType().Name);

                    if (ShouldRaiseException(interceptor))
                    {
                        logger.LogError(ex, "Failed to execute interceptors for global id - {RequestId}", requestId);
                        throw new DatabricksInterceptorException($"Error in interceptor {interceptor.GetType().Name} during BeforeRequestAsync", ex);
                    }
                }
            }
        }

        public async Task AfterRequestAsync(StatementResult statementResult)
        {
            var queryId = interceptorItems.GetValueOrDefault(DatabricksConstant.GlobalRequestIdKey) as string;
            var timer = interceptorItems.GetValueOrDefault(DatabricksConstant.GlobalRequestTimerKey) as Stopwatch;

            foreach (var interceptor in interceptors.Reverse())
            {
                try
                {
                    logger.LogDebug("Executing after request interceptor {InterceptorType} with priority {Priority}", interceptor.GetType().Name, interceptor.Priority);

                    await interceptor.AfterRequestAsync(statementResult, interceptorItems);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error in interceptor {InterceptorType} during AfterRequestAsync", interceptor.GetType().Name);

                    if (ShouldRaiseException(interceptor))
                    {
                        logger.LogError(ex, "Failed to execute interceptors for global id - {RequestId}", queryId);
                        throw new DatabricksInterceptorException($"Error in interceptor {interceptor.GetType().Name} during AfterRequestAsync", ex);
                    }
                }
            }

            timer?.Stop();
            var duration = timer?.Elapsed.TotalMilliseconds ?? 0;

            logger.LogDebug("Successfully executed interceptors for global request id - {queryId}, with a total duration of : {duration:F2}ms", queryId, duration);

            interceptorItems.Clear();
        }

        private bool ShouldRaiseException(IDatabricksInterceptor interceptor)
        {
            return interceptor.Priority == InterceptorPriority.Critical;
        }
    }
}
