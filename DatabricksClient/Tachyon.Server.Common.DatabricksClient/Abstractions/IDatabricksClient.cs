namespace Tachyon.Server.Common.DatabricksClient.Abstractions
{
    using Tachyon.Server.Common.DatabricksClient.Models.Request;
    using Tachyon.Server.Common.DatabricksClient.Models.Response;

    public interface IDatabricksClient
    {
        Task<List<T>> GetResultAsync<T>(StatementQuery statementQuery, CancellationToken cancellationToken = default);
        Task<StatementResult> ExecuteQueryAsync(StatementQuery statementQuery, CancellationToken cancellationToken = default);
    }
}