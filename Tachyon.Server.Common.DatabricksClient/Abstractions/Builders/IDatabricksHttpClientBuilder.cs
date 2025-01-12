

namespace Tachyon.Server.Common.DatabricksClient.Abstractions.Builders
{
    public interface IDatabricksHttpClientBuilder
    {
        HttpClient BuildClient();
    }
}
