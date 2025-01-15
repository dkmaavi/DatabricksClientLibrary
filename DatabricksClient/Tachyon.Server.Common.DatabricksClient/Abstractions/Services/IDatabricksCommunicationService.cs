using Tachyon.Server.Common.DatabricksClient.Models.Request;
using Tachyon.Server.Common.DatabricksClient.Models.Response;

namespace Tachyon.Server.Common.DatabricksClient.Abstractions.Services
{
    public interface IDatabricksCommunicationService
    {
        Task<StatementResult> SendStatementQueryAsync(StatementQuery query, CancellationToken cancellationToken);
        Task<StatementResult> GetStatementResultAsync(string statementId, CancellationToken cancellationToken);
        Task CancelStatementQueryAsync(string statementId, CancellationToken cancellationToken);
    }
}