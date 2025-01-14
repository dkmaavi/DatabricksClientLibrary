using Tachyon.Server.Common.DatabricksClient.Models.Request;
using Tachyon.Server.Common.DatabricksClient.Models.Response;

namespace Tachyon.Server.Common.DatabricksClient.Abstractions.Interceptors
{
    public interface IDatabricksInterceptor
    {
        InterceptorPriority Priority { get; }
        Task PreProcessAsync(StatementQuery query);
        Task PostProcessAsync(StatementResult result);

    }
}