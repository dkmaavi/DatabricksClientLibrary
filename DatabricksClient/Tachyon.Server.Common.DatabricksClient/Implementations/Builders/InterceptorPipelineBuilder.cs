using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tachyon.Server.Common.DatabricksClient.Abstractions.Builders;
using Tachyon.Server.Common.DatabricksClient.Abstractions.Handlers;
using Tachyon.Server.Common.DatabricksClient.Abstractions.Interceptors;
using Tachyon.Server.Common.DatabricksClient.Implementations.Handlers;

namespace Tachyon.Server.Common.DatabricksClient.Implementations.Builders
{
    internal class InterceptorPipelineBuilder : IDatabricksPipelineBuilder
    {
        private readonly IServiceCollection services;
        private readonly List<Type> interceptorTypes = new();

        public InterceptorPipelineBuilder(IServiceCollection services)
        {
            this.services = services;
        }

        public IDatabricksPipelineBuilder AddInterceptor<T>() where T : class, IDatabricksInterceptor
        {
            services.AddTransient<T>();
            interceptorTypes.Add(typeof(T));
            return this;
        }

        public IDatabricksPipelineBuilder RemoveInterceptors()
        {
            interceptorTypes.Clear();
            return this;
        }

        public void Build()
        {
            services.AddTransient<IDatabricksQueryHandler, DatabricksQueryHandler>();
            //services.AddSingleton(sp =>
            //{
            //    var databricksPipelineBuilder = new DatabricksPipelineBuilder(sp, sp.GetRequiredService<ILogger<DatabricksPipelineBuilder>>());
            //    return databricksPipelineBuilder.Build();
            //});
        }
    }
}