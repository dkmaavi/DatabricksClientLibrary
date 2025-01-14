using Tachyon.Server.Common.DatabricksClient.Abstractions.Interceptors;

namespace Tachyon.Server.Common.DatabricksClient.Abstractions.Builders
{
    public interface IDatabricksPipelineBuilder
    {
      
        IDatabricksPipelineBuilder AddInterceptor<TInterceptor>() where TInterceptor : class, IDatabricksInterceptor;

        IDatabricksPipelineBuilder RemoveInterceptors();
    }
}