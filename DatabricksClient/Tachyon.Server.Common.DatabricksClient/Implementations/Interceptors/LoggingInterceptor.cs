namespace Tachyon.Server.Common.DatabricksClient.Implementations.Interceptors
{
    using System.Diagnostics;
    using System.Text;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Tachyon.Server.Common.DatabricksClient.Abstractions.Interceptors;
    using Tachyon.Server.Common.DatabricksClient.Models.Enums;
    using Tachyon.Server.Common.DatabricksClient.Models.Request;
    using Tachyon.Server.Common.DatabricksClient.Models.Response;

    public class LoggingInterceptor : IDatabricksInterceptor
    {
        private readonly ILogger<LoggingInterceptor> logger;
        private const string timerKey = "LoggingInterceptor.Timer";
        private const string queryIdKey = "LoggingInterceptor.StatementQueryId";

        public InterceptorPriority Priority => InterceptorPriority.Normal;

        public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
        {
            this.logger = logger;
        }

        public async Task OnBeforeRequestAsync(StatementQuery statementQuery, Dictionary<string, object> properties)
        {
            var queryId = Guid.NewGuid().ToString();
            properties[queryIdKey] = queryId;
            properties[timerKey] = Stopwatch.StartNew();

            var logMessage = new StringBuilder();
            logMessage.AppendLine($"Query Id: {queryId}");
            logMessage.AppendLine($"Query: {statementQuery.Statement}");
            logMessage.AppendLine($"Query Parameters: {JsonConvert.SerializeObject(statementQuery.Parameters)}");

            logger.LogDebug(logMessage.ToString());

            await Task.CompletedTask;
        }

        public async Task OnAfterRequestAsync(StatementResult statementResult, Dictionary<string, object> properties)
        {
            var queryId = properties.GetValueOrDefault(queryIdKey) as string;
            var timer = properties.GetValueOrDefault(timerKey) as Stopwatch;

            timer?.Stop();
            var duration = timer?.Elapsed.TotalMilliseconds ?? 0;

            var logMessage = new StringBuilder();
            logMessage.AppendLine($"Response Id: {queryId}");
            logMessage.AppendLine($"Duration: {duration:F2}ms");
            logMessage.AppendLine($"Status: {statementResult.Status.State}");

            if (statementResult.Status.State == State.Failed)
            {
                logMessage.AppendLine($"Error Code: {statementResult.Status.Error?.ErrorCode}");
                logMessage.AppendLine($"Error Message: {statementResult.Status.Error?.Message}");
            }

            logger.LogDebug(logMessage.ToString());

            await Task.CompletedTask;
        }

    }
}