using Tachyon.Server.Common.DatabricksClient.Models.Request;
using Tachyon.Server.Common.DatabricksClient.Models.Response;

namespace Tachyon.Server.Common.DatabricksClient.Implementations.Builders
{
    internal delegate Task<StatementResult> DatabricksPipelineProcessor(StatementQuery query, CancellationToken cancellationToken);
}
