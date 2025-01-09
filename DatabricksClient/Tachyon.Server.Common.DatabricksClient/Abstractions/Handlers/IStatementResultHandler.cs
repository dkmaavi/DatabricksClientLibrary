using Tachyon.Server.Common.DatabricksClient.Models.Response;

namespace Tachyon.Server.Common.DatabricksClient.Abstractions.Handlers
{
    public interface IStatementResultHandler
    {
        Task<List<T>> HandleQueryResultAsync<T>(StatementResult result, CancellationToken cancellationToken);
    }
}
