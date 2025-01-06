using Tachyon.Server.Common.DatabricksClient.ApiRequest;

namespace Tachyon.Server.Common.DatabricksClient.Services
{
    public interface IDatabricksApiService
    {
        HttpClient CreateClient();
        StringContent CreateContent(StatementQuery sqlStatementQuery);
    }
}
