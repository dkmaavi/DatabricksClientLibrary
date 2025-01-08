namespace Tachyon.Server.Common.DatabricksClient.Services
{
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json.Linq;
    using System.Net.Http;
    using Tachyon.Server.Common.DatabricksClient.ApiRequest;
    using Tachyon.Server.Common.DatabricksClient.ApiResponse;
    using Tachyon.Server.Common.DatabricksClient.Configuration;
    using Tachyon.Server.Common.DatabricksClient.Exceptions;
    using Tachyon.Server.Common.DatabricksClient.Helpers;

    internal class DatabricksClient : IDatabricksClient, IDisposable
    {
        private readonly IDatabricksService databricksService;
        private readonly HttpClient httpClient;
        private readonly ResilienceSettings resilienceSettings;
        private readonly ILogger<DatabricksClient> logger;

        public DatabricksClient(IDatabricksService databricksService, ResilienceSettings resilienceSettings, ILogger<DatabricksClient> logger)
        {
            this.databricksService = databricksService;
            this.httpClient = databricksService.CreateClient();
            this.resilienceSettings = resilienceSettings;
            this.logger = logger;
        }

        public async Task<List<T>> GetResultAsync<T>(StatementQuery statementQuery, CancellationToken cancellationToken = default)
        {
            var response = await SendQueryAsync(statementQuery, cancellationToken);

            if (response.Status.State != State.Failed)
            {
                return ParseResult<T>(response);
            }
            else
            {
                throw new DatabricksException(response.Status.Error.ErrorCode, response.Status.Error.Message);
            }
        }

        public async Task ExecuteQueryAsync(StatementQuery statementQuery, CancellationToken cancellationToken = default)
        {
            var response = await SendQueryAsync(statementQuery, cancellationToken);

            if (response.Status.State == State.Failed)
            {
                throw new DatabricksException(response.Status.Error.ErrorCode, response.Status.Error.Message);
            }
        }

        private async Task<StatementResult> SendQueryAsync(StatementQuery sqlStatementQuery, CancellationToken cancellationToken)
        {
            try
            {
                using var queryExecutionTimer = new QueryExecutionTimer();
                var result = await SendRequestAsync<StatementResult>(HttpMethod.Post, DatabricksConstant.ApiEndpoint, sqlStatementQuery: sqlStatementQuery, cancellationToken: cancellationToken);

                while (result.Status.State is State.Running or State.Pending) //TODO -  need to handle response with multiple chunks
                {
                    logger.LogDebug("Running databrciks query for statement - {Statement} with status - {State}", result.StatementId, result.Status.State);

                    if (queryExecutionTimer.HasExceededTimeout(resilienceSettings.QueryTimeout))
                    {
                        throw new DatabricksException(ErrorCode.TIMEOUT, $"Databricks query execution exceeded timeout of {resilienceSettings.QueryTimeout} seconds");
                    }

                    result = await SendRequestAsync<StatementResult>(HttpMethod.Get, $"{DatabricksConstant.ApiEndpoint}/{result.StatementId}", cancellationToken);

                    await Task.Delay(TimeSpan.FromSeconds(resilienceSettings.PollingInterval), cancellationToken);
                }

                return result;
            }
            catch (Exception ex) when (ex is not DatabricksException)
            {
                throw new DatabricksException(ErrorCode.UNKNOWN,
                    "An unexpected error occurred while executing the databricks query, please refer the inner exception for more details", ex);
            }
        }

        private async Task<T> SendRequestAsync<T>(HttpMethod method, string uri, CancellationToken cancellationToken, StatementQuery? sqlStatementQuery = null)
        {
            using (var request = new HttpRequestMessage(method, uri))
            {
                if (sqlStatementQuery != null)
                {
                    var requestBody = databricksService.CreateContent(sqlStatementQuery);
                    request.Content = requestBody;
                }

                var response = await httpClient.SendAsync(request, cancellationToken);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsAsync<T>(cancellationToken);
                return result;
            }
        }

        private static List<T> ParseResult<T>(StatementResult result)
        {
            var columnMap = result.Manifest.Schema.Columns
                .ToDictionary(column => column.Position, column => column.Name);

            return (result.Result.Data ?? Enumerable.Empty<List<string>>())
                .Select(row => CreateObject<T>(row, columnMap))
                .ToList();
        }

        private static T CreateObject<T>(IReadOnlyList<string> row, IReadOnlyDictionary<int, string> columnMap)
        {
            var obj = new JObject();
            for (var i = 0; i < row.Count; i++)
            {
                obj[columnMap[i]] = row[i];
            }
            return obj.ToObject<T>() ?? throw new DatabricksException(ErrorCode.PARSE_ERROR, $"Failed to parse row to {typeof(T).Name}");
        }

        public void Dispose()
        {
            httpClient?.Dispose();
        }

    }
}
