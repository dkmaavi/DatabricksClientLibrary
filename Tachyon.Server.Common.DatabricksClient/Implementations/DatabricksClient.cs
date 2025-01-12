

namespace Tachyon.Server.Common.DatabricksClient.Implementations
{
    using Microsoft.Extensions.Logging;
    using Tachyon.Server.Common.DatabricksClient.Abstractions.Builders;
    using Tachyon.Server.Common.DatabricksClient.Abstractions.Parsers;
    using Tachyon.Server.Common.DatabricksClient.Abstractions;
    using Tachyon.Server.Common.DatabricksClient.Constants;
    using Tachyon.Server.Common.DatabricksClient.Exceptions;
    using Tachyon.Server.Common.DatabricksClient.Implementations.Interceptors;
    using Tachyon.Server.Common.DatabricksClient.Models.Configuration;
    using Tachyon.Server.Common.DatabricksClient.Models.Enums;
    using Tachyon.Server.Common.DatabricksClient.Models.Request;
    using Tachyon.Server.Common.DatabricksClient.Models.Response;
    using Tachyon.Server.Common.DatabricksClient.Utilities;
    using System;

    internal class DatabricksClient : IDatabricksClient
    {
        private readonly IDatabricksHttpClientBuilder databricksHttpClientBuilder;
        private readonly IDatabricksHttpRequestBuilder databricksRequestBuilder;
        private readonly IStatementResultParser statementResultParser;
        private readonly DatabricksInterceptorProcessor interceptorProcessor;
        private readonly ResilienceSettings resilienceSettings;
        private readonly ILogger<DatabricksClient> logger;
        public DatabricksClient(IDatabricksHttpClientBuilder databricksHttpClientBuilder,
            IDatabricksHttpRequestBuilder databricksRequestBuilder, IStatementResultParser statementResultParser,
            DatabricksInterceptorProcessor interceptorProcessor, ResilienceSettings resilienceSettings, ILogger<DatabricksClient> logger)
        {
            this.databricksHttpClientBuilder = databricksHttpClientBuilder;
            this.databricksRequestBuilder = databricksRequestBuilder;
            this.statementResultParser = statementResultParser;
            this.interceptorProcessor = interceptorProcessor;
            this.resilienceSettings = resilienceSettings;
            this.logger = logger;
        }

        public async Task<List<T>> GetResultAsync<T>(StatementQuery statementQuery, CancellationToken cancellationToken = default)
        {
            var response = await ExecuteQueryAsync(statementQuery, cancellationToken);
            return await statementResultParser.ParseResultAsync<T>(response, cancellationToken);
        }

        public async Task<StatementResult> ExecuteQueryAsync(StatementQuery statementQuery, CancellationToken cancellationToken = default)
        {
            try
            {
                logger.LogInformation("Executing databricks query with Id - {QueryId}", statementQuery.QueryId);

                await interceptorProcessor.BeforeRequestAsync(statementQuery);

                var result = await ExecuteWithPollingAsync(statementQuery, cancellationToken);

                await interceptorProcessor.AfterRequestAsync(result);

                logger.LogInformation("Successfully executed databricks query with Id - {QueryId}", statementQuery.QueryId);

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to execute databricks query with Id - {QueryId}", statementQuery.QueryId);
                await RaiseExceptionAsync(ex);
                throw;
            }
        }

        private async Task<StatementResult> ExecuteWithPollingAsync(StatementQuery statementQuery, CancellationToken cancellationToken)
        {
            using var executionTimer = new ExecutionTimer();

            var result = await SendRequestAsync<StatementResult>(HttpMethod.Post, DatabricksConstant.ApiEndpoint, statementQuery, cancellationToken);

            while (result.Status.State is State.Running or State.Pending) //TODO -  need to handle response with multiple chunks
            {
                if (executionTimer.HasExceededTimeout(resilienceSettings.QueryTimeout))
                {
                    throw new DatabricksException(ErrorCode.TIMEOUT, $"Databricks query execution exceeded timeout of {resilienceSettings.QueryTimeout} seconds");
                }

                logger.LogDebug("Running databricks query for statement - {StatementId} with status - {State}",
                    result.StatementId, result.Status.State);

                await Task.Delay(TimeSpan.FromSeconds(resilienceSettings.PollingInterval), cancellationToken);

                result = await SendRequestAsync<StatementResult>(HttpMethod.Get, $"{DatabricksConstant.ApiEndpoint}/{result.StatementId}", null, cancellationToken);
            }

            if (result.Status.State == State.Failed)
            {
                throw new DatabricksException(result.Status.Error.ErrorCode, result.Status.Error.Message);
            }

            return result;
        }

        private async Task<T> SendRequestAsync<T>(HttpMethod method, string uri, StatementQuery? query, CancellationToken cancellationToken)
        {
            using var client = databricksHttpClientBuilder.BuildClient();
            using var request = databricksRequestBuilder.BuildRequest(method, uri, query);

            var response = await client.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<T>(cancellationToken);
        }

        private async Task RaiseExceptionAsync(Exception ex)
        {
            if (ex is DatabricksException or DatabricksInterceptorException)
            {
                throw ex;
            }

            await Task.CompletedTask;
            var (errorCode, message) = ex switch
            {
                HttpRequestException => (ErrorCode.NETWORK_ERROR, "Failed to communicate with Databricks API"),
                OperationCanceledException => (ErrorCode.OPERATION_CANCELED, "Operation was cancelled"),
                _ => (ErrorCode.UNKNOWN, "An unexpected error occurred while executing the databricks query")
            };

            throw new DatabricksException(errorCode, message, ex);
        }
    }
}
