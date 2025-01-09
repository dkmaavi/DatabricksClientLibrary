namespace Tachyon.Server.Common.DatabricksClient.Abstractions.Builders
{
    using Tachyon.Server.Common.DatabricksClient.Models.Request;
    public interface IDatabricksRequestBuilder
    {
        HttpRequestMessage BuildRequest(HttpMethod method, string endpoint, StatementQuery? query = null);
    }
}
