using Tachyon.Server.Common.DatabricksClient.ApiRequest;

namespace Tachyon.Server.Common.DatabricksClient.Services
{
    public interface IDatabricksService
    {
        HttpClient CreateClient();
        StringContent CreateContent(StatementQuery sqlStatementQuery);
    }
}
