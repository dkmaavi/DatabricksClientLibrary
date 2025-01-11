namespace Tachyon.Server.Common.DatabricksClient.Abstractions.Services
{
    using Tachyon.Server.Common.DatabricksClient.Models.Configuration;

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
