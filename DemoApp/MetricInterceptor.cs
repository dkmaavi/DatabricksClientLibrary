using Microsoft.Extensions.Logging;
using Tachyon.Server.Common.DatabricksClient.Abstractions.Interceptors;
using Tachyon.Server.Common.DatabricksClient.Models.Enums;
using Tachyon.Server.Common.DatabricksClient.Models.Request;
using Tachyon.Server.Common.DatabricksClient.Models.Response;

namespace DemoApp
{
    public class MetricInterceptor : IDatabricksInterceptor
    {
        private readonly Counters counters;
        private readonly ILogger<MetricInterceptor> logger;

        private int counter = 0;

        public InterceptorPriority Priority => InterceptorPriority.Normal;

        public MetricInterceptor(Counters counters, ILogger<MetricInterceptor> logger)
        {
            this.counters = counters;
            this.logger = logger;
        }

        public async Task PreProcessAsync(StatementQuery query)
        {
             counter++;
            await Task.CompletedTask;
        }

        public async Task PostProcessAsync(StatementResult statementResult)
        {
            counter++;
            counters.ApiRequestsTotal++;
            if (statementResult.Status.State == State.Failed)
            {
                counters.ApiFailedRequestsTotal++;
            }
            else
            {
                counters.ApiSuccessfulRequestsTotal++;
            }

            logger.LogDebug($"ApiRequestsTotal - {counters.ApiRequestsTotal}");
            logger.LogDebug($"ApiSuccessfulRequestsTotal - {counters.ApiSuccessfulRequestsTotal}");
            logger.LogDebug($"ApiFailedRequestsTotal - {counters.ApiFailedRequestsTotal}");

            logger.LogDebug($"Counter - {counter}");
            await Task.CompletedTask;
        }
     
    }
}