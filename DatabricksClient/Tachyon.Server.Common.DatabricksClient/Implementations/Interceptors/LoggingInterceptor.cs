using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tachyon.Server.Common.DatabricksClient.Abstractions.Interceptors;
using Tachyon.Server.Common.DatabricksClient.Models.Request;
using Tachyon.Server.Common.DatabricksClient.Models.Response;

namespace Tachyon.Server.Common.DatabricksClient.Implementations.Interceptors
{
    internal class LoggingInterceptor : IDatabricksInterceptor
    {
        private readonly ILogger<LoggingInterceptor> logger;
        private readonly InterceptorContext localContext = new();
        public InterceptorPriority Priority => InterceptorPriority.Normal;

        public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
        {
            this.logger = logger;
        }

        public async Task PreProcessAsync(StatementQuery statementQuery)
        {
            localContext.Timer.Start();

            logger.LogDebug("Executing query With Id {queryId}, Statement: {Statement} and Parameters: {Parameters}",
                    localContext.Id, statementQuery.Statement, JsonConvert.SerializeObject(statementQuery.Parameters));

            await Task.CompletedTask;
        }

        public async Task PostProcessAsync(StatementResult statementResult)
        {
            localContext.Timer?.Stop();
            var duration = localContext.Timer?.Elapsed.TotalMilliseconds ?? 0;

            var isError = statementResult.Status.Error != null;
            var logLevel = isError ? LogLevel.Error : LogLevel.Debug;

            var errorDetails = isError
                            ? $"Error Code: {statementResult.Status.Error!.ErrorCode} and Error Message: {statementResult.Status.Error.Message}"
                            : string.Empty;

            var logMessage = $"Executed query with ID {localContext.Id}. Total Duration: {duration:F2} ms, Status: {statementResult.Status.State} {errorDetails}";

            logger.Log(logLevel, logMessage);

            await Task.CompletedTask;
        }
    }
}