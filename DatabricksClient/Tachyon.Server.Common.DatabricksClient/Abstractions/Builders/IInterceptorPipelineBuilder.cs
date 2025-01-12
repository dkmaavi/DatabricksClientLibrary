using Tachyon.Server.Common.DatabricksClient.Abstractions.Interceptors;

namespace Tachyon.Server.Common.DatabricksClient.Abstractions.Builders
{
    public interface IInterceptorPipelineBuilder
    {
        IInterceptorPipelineBuilder AddInterceptor<T>() where T : class, IDatabricksInterceptor;

        IInterceptorPipelineBuilder RemoveInterceptors();
    }
}