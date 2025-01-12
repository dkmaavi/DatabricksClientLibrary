using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using Tachyon.Server.Common.DatabricksClient.Abstractions.Interceptors;
using Tachyon.Server.Common.DatabricksClient.Models.Request;
using Tachyon.Server.Common.DatabricksClient.Models.Response;

namespace Tachyon.Server.Common.DatabricksClient.Implementations.Interceptors
{
    internal sealed class DatabricksInterceptorProcessor
    {
        private readonly ImmutableList<IDatabricksInterceptor> interceptors;
        private readonly InterceptorContext globalContext;
        private readonly ILogger<DatabricksInterceptorProcessor> logger;

        public DatabricksInterceptorProcessor(IEnumerable<IDatabricksInterceptor> interceptors, ILogger<DatabricksInterceptorProcessor> logger)
        {
            this.interceptors = interceptors.ToImmutableList();
            globalContext = new InterceptorContext();
            this.logger = logger;
        }

        public async Task BeforeRequestAsync(StatementQuery statementQuery)
        {
            logger.LogInformation("Starting interceptor execution. Request ID: {RequestId}",
                globalContext.Id);

            globalContext.Timer.Start();
            await ExecuteInterceptorsAsync(interceptors, i => i.BeforeRequestAsync(statementQuery), nameof(this.BeforeRequestAsync));
        }

        public async Task AfterRequestAsync(StatementResult statementResult)
        {
            try
            {
                await ExecuteInterceptorsAsync(interceptors.Reverse(), i => i.AfterRequestAsync(statementResult), nameof(this.AfterRequestAsync));
            }
            finally
            {
                await LogExecutionDetailAsync();
            }
        }

        private async Task ExecuteInterceptorsAsync(IEnumerable<IDatabricksInterceptor> interceptors, Func<IDatabricksInterceptor, Task> executeInterceptor, string operationName)
        {
            foreach (var interceptor in interceptors)
            {
                try
                {
                    logger.LogDebug("Executing {OperationName} for interceptor {InterceptorType} (Priority: {Priority})",
                        operationName, interceptor.GetType().Name, interceptor.Priority);

                    await executeInterceptor(interceptor);
                }
                catch (Exception ex)
                {
                    await HandleInterceptorExceptionAsync(interceptor, operationName, ex);
                }
            }
        }

        private async Task HandleInterceptorExceptionAsync(IDatabricksInterceptor interceptor, string operation, Exception exception)
        {
            if (interceptor.Priority == InterceptorPriority.Critical)
            {
                logger.LogError(exception, "Critical interceptor {InterceptorType} failed during {Operation}. Request ID: {RequestId}",
                    interceptor.GetType().Name, operation, globalContext.Id);

                throw exception;
            }

            logger.LogError(exception, "Non-critical interceptor {InterceptorType} failed during {Operation}. Continuing execution. Request ID: {RequestId}",
                interceptor.GetType().Name, operation, globalContext.Id);

            await Task.CompletedTask;
        }

        private async Task LogExecutionDetailAsync()
        {
            globalContext.Timer?.Stop();
            var duration = globalContext.Timer?.Elapsed.TotalMilliseconds ?? 0;

            logger.LogDebug("Completed interceptor execution. Request ID: {RequestId}, Duration: {Duration:F2}ms",
                globalContext.Id, duration);

            await Task.CompletedTask;
        }
    }
}