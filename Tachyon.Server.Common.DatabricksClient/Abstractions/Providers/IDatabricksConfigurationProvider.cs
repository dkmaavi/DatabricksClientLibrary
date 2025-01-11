namespace Tachyon.Server.Common.DatabricksClient.Abstractions.Providers
{
    using Tachyon.Server.Common.DatabricksClient.Models.Configuration;

    public interface IDatabricksConfigurationProvider
    {
        StatementApiSettings GetStatementApiSettings();
        HttpClientSettings GetHttpClientSettings();
        ResilienceSettings GetResilienceSettings()
        {
            return new ResilienceSettings();
        }
    }
}
