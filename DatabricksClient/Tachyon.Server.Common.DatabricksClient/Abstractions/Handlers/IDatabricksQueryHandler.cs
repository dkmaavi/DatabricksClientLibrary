using Tachyon.Server.Common.DatabricksClient.Models.Request;
using Tachyon.Server.Common.DatabricksClient.Models.Response;

namespace Tachyon.Server.Common.DatabricksClient.Abstractions.Handlers
{
    public interface IDatabricksQueryHandler
    {
        Task<StatementResult> HandleQueryAsync(StatementQuery query, CancellationToken cancellationToken);
    }
}
