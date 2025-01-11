namespace Tachyon.Server.Common.DatabricksClient.Abstractions.Services
{
    using Tachyon.Server.Common.DatabricksClient.Models.Request;
    using Tachyon.Server.Common.DatabricksClient.Models.Response;

    public interface IDatabricksCommunicationService
    {
        Task<StatementResult> SendStatementQueryAsync(StatementQuery query, CancellationToken cancellationToken);
        Task<StatementResult> GetStatementResultAsync(string statementId, CancellationToken cancellationToken);

    }
}
