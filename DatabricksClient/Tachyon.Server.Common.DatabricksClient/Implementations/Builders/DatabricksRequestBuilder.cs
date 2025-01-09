namespace Tachyon.Server.Common.DatabricksClient.Implementations.Builders
{
    using Newtonsoft.Json;
    using System.Text;
    using Tachyon.Server.Common.DatabricksClient.Abstractions.Builders;
    using Tachyon.Server.Common.DatabricksClient.Models.Configuration;
    using Tachyon.Server.Common.DatabricksClient.Models.Request;
    public class DatabricksRequestBuilder : IDatabricksRequestBuilder
    {
        private readonly StatementApiSettings apiSettings;

        public DatabricksRequestBuilder(StatementApiSettings apiSettings)
        {
            this.apiSettings = apiSettings;
        }

        public HttpRequestMessage BuildRequest(HttpMethod method, string endpoint, StatementQuery? statementQuery = null)
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
