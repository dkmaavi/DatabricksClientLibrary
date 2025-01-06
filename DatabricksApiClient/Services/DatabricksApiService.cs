namespace Tachyon.Server.Common.DatabricksClient.Services
{
    using Newtonsoft.Json;
    using System.Net.Http.Headers;
    using System.Text;
    using Tachyon.Server.Common.DatabricksClient.ApiRequest;
    using Tachyon.Server.Common.DatabricksClient.Configuration;

    public class DatabricksApiService : IDatabricksApiService
    {
        private readonly DatabricksConfiguration databricksConfiguration;

        public DatabricksApiService(DatabricksConfiguration databricksConfiguration)
        {
            this.databricksConfiguration = databricksConfiguration;
        }
        public HttpClient CreateClient()
        {
            var apiUrl = new Uri(databricksConfiguration.BaseUrl);

            var httpClient = new HttpClient()
            {
                BaseAddress = apiUrl,
            };

            SetDefaultHeaders(httpClient);

            return httpClient;
        }

        public StringContent CreateContent(StatementQuery sqlStatementQuery)
        {
            var requestBody = new
            {
                warehouse_id = databricksConfiguration.WarehouseId,
                catalog = databricksConfiguration.CatalogName,
                schema = databricksConfiguration.DatabaseName,
                wait_timeout = databricksConfiguration.WaitTimeout,
                statement = sqlStatementQuery.Statement,
                parameters = sqlStatementQuery.Parameters
            };

            string jsonBody = JsonConvert.SerializeObject(requestBody);
            return new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        private void SetDefaultHeaders(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", databricksConfiguration.BeareToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
