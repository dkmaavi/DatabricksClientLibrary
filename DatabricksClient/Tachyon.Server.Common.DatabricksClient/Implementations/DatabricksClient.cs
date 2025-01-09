namespace Tachyon.Server.Common.DatabricksClient.Implementations
{
    using Microsoft.Extensions.Logging;
    using System.Net.Http;
    using Tachyon.Server.Common.DatabricksClient.Abstractions;
    using Tachyon.Server.Common.DatabricksClient.Abstractions.Builders;
    using Tachyon.Server.Common.DatabricksClient.Abstractions.Handlers;
    using Tachyon.Server.Common.DatabricksClient.Constants;
    using Tachyon.Server.Common.DatabricksClient.Exceptions;
    using Tachyon.Server.Common.DatabricksClient.Implementations.Interceptors;
    using Tachyon.Server.Common.DatabricksClient.Models.Configuration;
    using Tachyon.Server.Common.DatabricksClient.Models.Enums;
    using Tachyon.Server.Common.DatabricksClient.Models.Request;
    using Tachyon.Server.Common.DatabricksClient.Models.Response;
    using Tachyon.Server.Common.DatabricksClient.Utilities;

    internal class DatabricksClient : IDatabricksClient, IDisposable
    {
        private readonly IDatabricksRequestBuilder databricksRequestBuilder;
        private readonly IStatementResultHandler statementResultHandler;
        private readonly DatabricksInterceptorExecutor databricksInterceptorExecutor;
        private readonly HttpClient httpClient;
        private readonly ResilienceSettings resilienceSettings;
        private readonly ILogger<DatabricksClient> logger;
        private bool disposed;
        public DatabricksClient(IDatabricksHttpClientBuilder databricksHttpClientBuilder,
            IDatabricksRequestBuilder databricksRequestBuilder, IStatementResultHandler statementResultHandler,
            DatabricksInterceptorExecutor databricksInterceptorExecutor, ResilienceSettings resilienceSettings, ILogger<DatabricksClient> logger)
        {
            this.databricksRequestBuilder = databricksRequestBuilder;
            this.statementResultHandler = statementResultHandler;
            this.databricksInterceptorExecutor = databricksInterceptorExecutor;
            this.resilienceSettings = resilienceSettings;
            this.logger = logger;
            httpClient = databricksHttpClientBuilder.BuildClient();
        }

        public async Task<List<T>> GetResultAsync<T>(StatementQuery statementQuery, CancellationToken cancellationToken = default)
        {
            var response = await ExecuteQueryAsync(statementQuery, cancellationToken);
            return await statementResultHandler.HandleQueryResultAsync<T>(response, cancellationToken);
        }

        public async Task<StatementResult> ExecuteQueryAsync(StatementQuery statementQuery, CancellationToken cancellationToken = default)
        {
            try
            {
                var properties = new Dictionary<string, object>();
                await databricksInterceptorExecutor.OnBeforeRequestAsync(statementQuery, properties);

                using var queryExecutionTimer = new QueryExecutionTimer();
                var statementResult = await SendRequestAsync<StatementResult>(HttpMethod.Post, ApiConstants.ApiEndpoint, statementQuery: statementQuery, cancellationToken: cancellationToken);

                while (statementResult.Status.State is State.Running or State.Pending) //TODO -  need to handle response with multiple chunks
                {
                    logger.LogDebug("Running databrciks query for statement - {Statement} with status - {State}", statementResult.StatementId, statementResult.Status.State);

                    if (queryExecutionTimer.HasExceededTimeout(resilienceSettings.QueryTimeout))
                    {
                        throw new DatabricksQueryException(ErrorCode.TIMEOUT, $"Databricks query execution exceeded timeout of {resilienceSettings.QueryTimeout} seconds");
                    }

                    statementResult = await SendRequestAsync<StatementResult>(HttpMethod.Get, $"{ApiConstants.ApiEndpoint}/{statementResult.StatementId}", cancellationToken);

                    await Task.Delay(TimeSpan.FromSeconds(resilienceSettings.PollingInterval), cancellationToken);
                }

                await databricksInterceptorExecutor.OnAfterRequestAsync(statementResult, properties);

                if (statementResult.Status.State == State.Failed)
                {
                    throw new DatabricksQueryException(statementResult.Status.Error.ErrorCode, statementResult.Status.Error.Message);
                }
                return statementResult;
            }
            catch (HttpRequestException ex)
            {
                throw new DatabricksHttpException(ErrorCode.NETWORK_ERROR,
                    "Failed to communicate with Databricks API", ex);
            }
            catch (OperationCanceledException ex)
            {
                throw new DatabricksHttpException(ErrorCode.OPERATION_CANCELED,
                    "Opertaion was cancelled", ex);
            }
            catch (Exception ex) when (ex is not DatabricksQueryException)
            {
                throw new DatabricksHttpException(ErrorCode.UNKNOWN,
                    "An unexpected error occurred while executing the databricks query, please refer the inner exception for more details", ex);
            }
        }

        private async Task<T> SendRequestAsync<T>(HttpMethod method, string uri, CancellationToken cancellationToken, StatementQuery? statementQuery = null)
        {
            using (var request = databricksRequestBuilder.BuildRequest(method, uri, statementQuery))
            {
                var response = await httpClient.SendAsync(request, cancellationToken);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsAsync<T>(cancellationToken);
                return result;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                httpClient?.Dispose();
            }

            disposed = true;
        }

    }
}
