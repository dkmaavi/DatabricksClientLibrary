using Tachyon.Server.Common.DatabricksClient.Models.Request;
using Tachyon.Server.Common.DatabricksClient.Models.Response;

namespace Tachyon.Server.Common.DatabricksClient.Abstractions.Interceptors
{
    public interface IDatabricksInterceptor
    {
        InterceptorPriority Priority { get; }
        Task BeforeRequestAsync(StatementQuery statementQuery);
        Task AfterRequestAsync(StatementResult statementResult);
    }
}
