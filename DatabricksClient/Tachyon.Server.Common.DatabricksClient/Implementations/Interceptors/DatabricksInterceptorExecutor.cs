namespace Tachyon.Server.Common.DatabricksClient.Implementations.Interceptors
{
    using Microsoft.Extensions.Logging;
    using Tachyon.Server.Common.DatabricksClient.Abstractions.Interceptors;
    using Tachyon.Server.Common.DatabricksClient.Exceptions;
    using Tachyon.Server.Common.DatabricksClient.Models.Request;
    using Tachyon.Server.Common.DatabricksClient.Models.Response;

    internal class DatabricksInterceptorExecutor
    {
        private readonly IEnumerable<IDatabricksInterceptor> interceptors;
        private readonly ILogger<DatabricksInterceptorExecutor> logger;

        public DatabricksInterceptorExecutor(IEnumerable<IDatabricksInterceptor> interceptors, ILogger<DatabricksInterceptorExecutor> logger)
        {
            this.interceptors = interceptors.ToList();
            this.logger = logger;
        }

        public async Task OnBeforeRequestAsync(StatementQuery statementQuery, Dictionary<string, object> properties)
        {
            foreach (var interceptor in interceptors)
            {
                try
                {
                    logger.LogDebug("Executing before request interceptor {InterceptorType} with priority {Priority}", interceptor.GetType().Name, interceptor.Priority);

                    await interceptor.OnBeforeRequestAsync(statementQuery, properties);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error in interceptor {InterceptorType} during OnBeforeRequestAsync", interceptor.GetType().Name);

                    if (ShouldRethrowException(interceptor))
                    {
                        throw new DatabricksInterceptorException($"Error in interceptor {interceptor.GetType().Name} during OnBeforeRequestAsync", ex);
                    }
                }
            }
        }

        public async Task OnAfterRequestAsync(StatementResult statementResult, Dictionary<string, object> properties)
        {
            foreach (var interceptor in interceptors.Reverse())
            {
                try
                {
                    logger.LogDebug("Executing after request interceptor {InterceptorType} with priority {Priority}", interceptor.GetType().Name, interceptor.Priority);

                    await interceptor.OnAfterRequestAsync(statementResult, properties);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error in interceptor {InterceptorType} during OnAfterRequestAsync", interceptor.GetType().Name);

                    if (ShouldRethrowException(interceptor))
                    {
                        throw new DatabricksInterceptorException($"Error in interceptor {interceptor.GetType().Name} during OnAfterRequestAsync", ex);
                    }
                }
            }
        }

        private bool ShouldRethrowException(IDatabricksInterceptor interceptor)
        {
            return interceptor.Priority == InterceptorPriority.Critical;
        }
    }
}
