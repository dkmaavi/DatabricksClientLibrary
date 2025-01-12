using Tachyon.Server.Common.DatabricksClient.Abstractions.Handlers;
using Tachyon.Server.Common.DatabricksClient.Models.Response;

namespace DemoApp
{
    internal class CustomStatementResultParser : IDatabricksResultHandler
    {
        public Task<List<T>> HandleResultAsync<T>(StatementResult result, CancellationToken cancellationToken)
        {
            return Task.FromResult(new List<T>());
        }
    }
}