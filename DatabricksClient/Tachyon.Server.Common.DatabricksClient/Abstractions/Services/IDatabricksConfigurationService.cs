using Tachyon.Server.Common.DatabricksClient.Models.Configuration;

namespace Tachyon.Server.Common.DatabricksClient.Abstractions.Services
{
    public interface IDatabricksConfigurationService
    {
        StatementApiSettings GetStatementApiSettings();

        HttpClientSettings GetHttpClientSettings();

        ResilienceSettings GetResilienceSettings()
        {
            return new ResilienceSettings();
        }
    }
}