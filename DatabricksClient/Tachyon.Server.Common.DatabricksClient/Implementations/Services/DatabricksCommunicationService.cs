using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using Tachyon.Server.Common.DatabricksClient.Abstractions.Services;
using Tachyon.Server.Common.DatabricksClient.Constants;
using Tachyon.Server.Common.DatabricksClient.Models.Configuration;
using Tachyon.Server.Common.DatabricksClient.Models.Request;
using Tachyon.Server.Common.DatabricksClient.Models.Response;

namespace Tachyon.Server.Common.DatabricksClient.Implementations.Services
{
    internal class DatabricksCommunicationService : IDatabricksCommunicationService
    {
        private readonly HttpClient httpClient;
        private readonly StatementApiSettings apiSettings;
        private readonly ILogger<DatabricksCommunicationService> logger;
        private readonly ResilienceSettings resilienceSettings;

        public DatabricksCommunicationService(IHttpClientFactory httpClientFactory, StatementApiSettings apiSettings, ILogger<DatabricksCommunicationService> logger, ResilienceSettings resilienceSettings)
        {
            this.apiSettings = apiSettings;
            this.logger = logger;

            httpClient = httpClientFactory.CreateClient(DatabricksConstant.HttpClientName);
            this.resilienceSettings = resilienceSettings;
        }

        public async Task<StatementResult> SendStatementQueryAsync(StatementQuery query, CancellationToken cancellationToken)
        {
            return await SendRequestAsync<StatementResult>(HttpMethod.Post, DatabricksConstant.ApiEndpoint, query, cancellationToken);
        }

        public async Task<StatementResult> GetStatementResultAsync(string statementId, CancellationToken cancellationToken)
        {
            return await SendRequestAsync<StatementResult>(HttpMethod.Get, $"{DatabricksConstant.ApiEndpoint}/{statementId}", null, cancellationToken);
        }

        public async Task CancelStatementQueryAsync(string statementId, CancellationToken cancellationToken)
        {
            await SendRequestAsync<object>(HttpMethod.Post, $"{DatabricksConstant.ApiEndpoint}/{statementId}/cancel", null, cancellationToken);
        }

        private async Task<T> SendRequestAsync<T>(HttpMethod method, string uri, StatementQuery? query, CancellationToken cancellationToken)
        {
            using var request = BuildRequest(method, uri, query);

            var response = await httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<T>(cancellationToken);
        }

        private HttpRequestMessage BuildRequest(HttpMethod method, string endpoint, StatementQuery? statementQuery = null)
        {
            var request = new HttpRequestMessage(method, endpoint);

            if (statementQuery != null)
            {
                request.Content = CreateContent(statementQuery);
            }

            return request;
        }

        private StringContent CreateContent(StatementQuery query)
        {
            var requestBody = new
            {
                warehouse_id = apiSettings.WarehouseId,
                catalog = apiSettings.CatalogName,
                schema = apiSettings.DatabaseName,
                wait_timeout = apiSettings.WaitTimeout,
                statement = query.Statement,
                parameters = query.Parameters
            };

            return new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        }
    }
}