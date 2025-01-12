using Tachyon.Server.Common.DatabricksClient.Models.Response;

namespace Tachyon.Server.Common.DatabricksClient.Abstractions.Handlers
{
    public interface IDatabricksResultHandler
    {
        Task<List<T>> HandleResultAsync<T>(StatementResult result, CancellationToken cancellationToken);
    }
}