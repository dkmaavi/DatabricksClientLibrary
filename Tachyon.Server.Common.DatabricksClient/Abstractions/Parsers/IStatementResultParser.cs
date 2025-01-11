using Tachyon.Server.Common.DatabricksClient.Models.Response;

namespace Tachyon.Server.Common.DatabricksClient.Abstractions.Parsers
{
    public interface IStatementResultParser
    {
        Task<List<T>> ParseResultAsync<T>(StatementResult result, CancellationToken cancellationToken);
    }
}
