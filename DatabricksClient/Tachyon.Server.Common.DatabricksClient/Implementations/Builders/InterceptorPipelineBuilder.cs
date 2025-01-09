namespace Tachyon.Server.Common.DatabricksClient.Implementations.Builders
{
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Tachyon.Server.Common.DatabricksClient.Abstractions.Interceptors;
    using Tachyon.Server.Common.DatabricksClient.Implementations.Interceptors;

    public class InterceptorPipelineBuilder
    {
        private readonly IServiceCollection services;
        private readonly List<Type> interceptorTypes = new();

        public InterceptorPipelineBuilder(IServiceCollection services)
        {
            this.services = services;
        }

        public InterceptorPipelineBuilder AddInterceptor<T>() where T : class, IDatabricksInterceptor
        {
            services.AddScoped<T>();
            interceptorTypes.Add(typeof(T));
            return this;
        }

        public void Build()
        {
            services.AddScoped<IEnumerable<IDatabricksInterceptor>>(sp =>
            {
                return interceptorTypes.Select(t => sp.GetRequiredService(t))
                                      .Cast<IDatabricksInterceptor>().ToList();
            });

            services.AddScoped<DatabricksInterceptorExecutor>();
        }
    }
}
