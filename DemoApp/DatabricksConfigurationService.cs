using Tachyon.Server.Common.DatabricksClient.Abstractions.Services;
using Tachyon.Server.Common.DatabricksClient.Models.Configuration;

namespace DemoApp
{
    internal class DatabricksConfigurationService : IDatabricksConfigurationService
    {
        public HttpClientSettings GetHttpClientSettings()
        {
            return new HttpClientSettings
            {
                BaseUrl = "https://adb-7494964805612838.18.azuredatabricks.net",
                BearerToken = "dapi697b16720dcd39639f683ff0a36642ec",
            };
        }

        public StatementApiSettings GetStatementApiSettings()
        {
            return new StatementApiSettings
            {
                WarehouseId = "fa6876c421f5a68c",
                CatalogName = "sandbox",
                DatabaseName = "axm",
                WaitTimeout = "0s"
            };
        }
    }
}