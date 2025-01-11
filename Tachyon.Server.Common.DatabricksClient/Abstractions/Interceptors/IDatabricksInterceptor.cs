using Tachyon.Server.Common.DatabricksClient.Models.Request;
using Tachyon.Server.Common.DatabricksClient.Models.Response;

namespace Tachyon.Server.Common.DatabricksClient.Abstractions.Interceptors
{
    public interface IDatabricksInterceptor
    {
        InterceptorPriority Priority { get; }
        Task BeforeRequestAsync(StatementQuery statementQuery, Dictionary<string, object> interceptorItems);
        Task AfterRequestAsync(StatementResult statementResult, Dictionary<string, object> interceptorItems);
    }
}
