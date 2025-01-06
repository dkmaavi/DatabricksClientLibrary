namespace Tachyon.Server.Common.DatabricksClient
{
    using Tachyon.Server.Common.DatabricksClient.ApiRequest;
    using Tachyon.Server.Common.DatabricksClient.ApiResponse;
    using Tachyon.Server.Common.DatabricksClient.Services;

    public class DatabricksApi
    {
        private readonly IDatabricksApiService httpClientService;
        private readonly HttpClient httpClient;
        private const string apiEndpoint = "api/2.0/sql/statements";
        private const int pollingInterval = 1000;

        public DatabricksApi(IDatabricksApiService httpClientService)
        {
            this.httpClientService = httpClientService;
            this.httpClient = httpClientService.CreateClient();
        }

        public async Task<StatementResult> SendQueryAsync(StatementQuery sqlStatementQuery)
        {
            var result = await SendRequestAsync<StatementResult>(HttpMethod.Post, apiEndpoint, sqlStatementQuery);

            while (result.Status.State is State.Running or State.Pending) // need to handle paginated response
            {
                result = await SendRequestAsync<StatementResult>(HttpMethod.Get, $"{apiEndpoint}/{result.StatementId}");

                await Task.Delay(pollingInterval);
            }

            return result;
        }

        private async Task<T> SendRequestAsync<T>(HttpMethod method, string uri, StatementQuery? sqlStatementQuery = null)
        {
            using (var request = new HttpRequestMessage(method, uri))
            {
                if (sqlStatementQuery != null)
                {
                    var requestBody = httpClientService.CreateContent(sqlStatementQuery);
                    request.Content = requestBody;
                }

                var response = await httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsAsync<T>();
                return result;
            }
        }
    }
}
