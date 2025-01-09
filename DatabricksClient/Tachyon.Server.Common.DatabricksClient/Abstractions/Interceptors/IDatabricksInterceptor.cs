using Tachyon.Server.Common.DatabricksClient.Models.Request;
using Tachyon.Server.Common.DatabricksClient.Models.Response;

namespace Tachyon.Server.Common.DatabricksClient.Abstractions.Interceptors
{
    public interface IDatabricksInterceptor
    {
        InterceptorPriority Priority { get; }
        Task OnBeforeRequestAsync(StatementQuery statementQuery, Dictionary<string, object> properties);
        Task OnAfterRequestAsync(StatementResult statementResult, Dictionary<string, object> properties);
    }
}
