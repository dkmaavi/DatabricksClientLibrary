using Tachyon.Server.Common.DatabricksClient.Abstractions.Services;
using Tachyon.Server.Common.DatabricksClient.Models.Configuration;

namespace DatabricksApp
{
    internal class DatabricksConfigurationService : IDatabricksConfigurationService
    {
        
        public HttpClientSettings GetHttpClientSettings()
        {
            return new HttpClientSettings
            {
                
            };
        }
        public StatementApiSettings GetStatementApiSettings()
        {
            return new StatementApiSettings
            {
             
            };
        }
    }
}
