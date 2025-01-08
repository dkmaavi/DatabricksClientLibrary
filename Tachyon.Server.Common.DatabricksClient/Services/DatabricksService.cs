namespace Tachyon.Server.Common.DatabricksClient.Services
{
    using Newtonsoft.Json;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using Tachyon.Server.Common.DatabricksClient.ApiRequest;
    using Tachyon.Server.Common.DatabricksClient.Configuration;
    using Tachyon.Server.Common.DatabricksClient.Helpers;

    internal class DatabricksService : IDatabricksService
    {
        private readonly IHttpClientFactory httpClientFactory;       
        private readonly HttpSettings httpSettings;
        private readonly ApiSettings apiSettings;
        public DatabricksService( IHttpClientFactory httpClientFactory, HttpSettings httpSettings, ApiSettings apiSettings)
        {         
            this.httpClientFactory = httpClientFactory;
            this.httpSettings = httpSettings;
            this.apiSettings = apiSettings;
        }
        public HttpClient CreateClient()
        {
            var httpClient = httpClientFactory.CreateClient(DatabricksConstant.HttpClientName);

            httpClient.BaseAddress = new Uri(httpSettings.BaseUrl);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpSettings.BearerToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
        }

        public StringContent CreateContent(StatementQuery sqlStatementQuery)
        {
            var requestBody = new
            {
                warehouse_id = apiSettings.WarehouseId,
                catalog = apiSettings.CatalogName,
                schema = apiSettings.DatabaseName,
                wait_timeout = apiSettings.WaitTimeout,
                statement = sqlStatementQuery.Statement,
                parameters = sqlStatementQuery.Parameters
            };

            string jsonBody = JsonConvert.SerializeObject(requestBody);
            return new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }
    }
}
