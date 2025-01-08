namespace Tachyon.Server.Common.DatabricksClient.Services
{
    using Tachyon.Server.Common.DatabricksClient.ApiRequest;
    public interface IDatabricksClient
    {
        Task<List<T>> GetResultAsync<T>(StatementQuery statementQuery, CancellationToken cancellationToken = default);
        Task ExecuteQueryAsync(StatementQuery statementQuery, CancellationToken cancellationToken = default);
    }
}