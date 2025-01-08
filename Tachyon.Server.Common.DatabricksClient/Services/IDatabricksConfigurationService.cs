namespace Tachyon.Server.Common.DatabricksClient.Services
{
    using Tachyon.Server.Common.DatabricksClient.Configuration;
    public interface IDatabricksConfigurationService
    {
        HttpSettings GetHttpSettings();
        ApiSettings GetApiSettings();
        ResilienceSettings GetResilienceSettings()
        {
            return new ResilienceSettings();
        }
    }

}