namespace Tachyon.Server.Common.DatabricksClient.Implementations.Builders
{
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Tachyon.Server.Common.DatabricksClient.Abstractions.Builders;
    using Tachyon.Server.Common.DatabricksClient.Abstractions.Interceptors;
    using Tachyon.Server.Common.DatabricksClient.Implementations.Interceptors;

    internal class InterceptorPipelineBuilder : IInterceptorPipelineBuilder
    {
        private readonly IServiceCollection services;
        private readonly List<Type> interceptorTypes = new();

        public InterceptorPipelineBuilder(IServiceCollection services)
        {
            this.services = services;
        }

        public IInterceptorPipelineBuilder AddInterceptor<T>() where T : class, IDatabricksInterceptor
        {
            services.AddScoped<T>();
            interceptorTypes.Add(typeof(T));
            return this;
        }

        public IInterceptorPipelineBuilder RemoveInterceptors()
        {
            interceptorTypes.Clear();
            return this;
        }

        public void Build()
        {
            services.AddScoped<IEnumerable<IDatabricksInterceptor>>(sp =>
            {
                return interceptorTypes.Select(t => sp.GetRequiredService(t))
                                      .Cast<IDatabricksInterceptor>().ToList();
            });

            services.AddScoped<DatabricksInterceptorProcessor>();
        }
    }
}
