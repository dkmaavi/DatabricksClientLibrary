namespace Tachyon.Server.Common.DatabricksClient.Abstractions.Services
{
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using System.Net.Http.Headers;
    using System.Text;
    using Tachyon.Server.Common.DatabricksClient.Constants;
    using Tachyon.Server.Common.DatabricksClient.Models.Configuration;
    using Tachyon.Server.Common.DatabricksClient.Models.Request;
    using Tachyon.Server.Common.DatabricksClient.Models.Response;

    internal class DatabricksCommunicationService : IDatabricksCommunicationService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly HttpClientSettings httpSettings;
        private readonly ResilienceSettings resilienceSettings;
        private readonly StatementApiSettings apiSettings;
        private readonly ILogger<DatabricksCommunicationService> logger;

        public DatabricksCommunicationService(IHttpClientFactory httpClientFactory, HttpClientSettings httpSettings, ResilienceSettings resilienceSettings, StatementApiSettings apiSettings, ILogger<DatabricksCommunicationService> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.httpSettings = httpSettings;
            this.resilienceSettings = resilienceSettings;
            this.apiSettings = apiSettings;
            this.logger = logger;
        }

        public async Task<StatementResult> SendStatementQueryAsync(StatementQuery query, CancellationToken cancellationToken)
        {
            return await SendRequestAsync<StatementResult>(HttpMethod.Post, DatabricksConstant.ApiEndpoint, query, cancellationToken);
        }

        public async Task<StatementResult> GetStatementResultAsync(string statementId, CancellationToken cancellationToken)
        {
            return await SendRequestAsync<StatementResult>(HttpMethod.Get, $"{DatabricksConstant.ApiEndpoint}/{statementId}", null, cancellationToken);
        }

        private async Task<T> SendRequestAsync<T>(HttpMethod method, string uri, StatementQuery? query, CancellationToken cancellationToken)
        {
            using var client = BuildClient();
            using var request = BuildRequest(method, uri, query);

            var response = await client.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<T>(cancellationToken);
        }

        private HttpClient BuildClient()
        {
            var httpClient = httpClientFactory.CreateClient(DatabricksConstant.HttpClientName);

            httpClient.BaseAddress = new Uri(httpSettings.BaseUrl);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpSettings.BearerToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
        }

        private HttpRequestMessage BuildRequest(HttpMethod method, string endpoint, StatementQuery? statementQuery = null)
        {
            var request = new HttpRequestMessage(method, endpoint);

            if (statementQuery != null)
            {
                var content = CreateContent(statementQuery);
                request.Content = content;
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
