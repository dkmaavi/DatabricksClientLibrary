namespace Tachyon.Server.Common.DatabricksClient.Implementations.Interceptors
{
    using Microsoft.Extensions.Logging;
    using Tachyon.Server.Common.DatabricksClient.Abstractions.Interceptors;
    using Tachyon.Server.Common.DatabricksClient.Exceptions;
    using Tachyon.Server.Common.DatabricksClient.Models.Enums;
    using Tachyon.Server.Common.DatabricksClient.Models.Request;
    using Tachyon.Server.Common.DatabricksClient.Models.Response;

    internal class ValidationInterceptor : IDatabricksInterceptor
    {
        private readonly ILogger<LoggingInterceptor> logger;

        public InterceptorPriority Priority => InterceptorPriority.Critical;

        public ValidationInterceptor(ILogger<LoggingInterceptor> logger)
        {
            this.logger = logger;
        }

        public async Task BeforeRequestAsync(StatementQuery statementQuery)
        {
            if (string.IsNullOrEmpty(statementQuery.Statement))
                throw new DatabricksInterceptorException("Statement query is null", new ArgumentNullException(nameof(statementQuery.Statement)));

            await Task.CompletedTask;
        }

        public async Task AfterRequestAsync(StatementResult statementResult)
        {
            if (statementResult.Status.State == State.Failed)
            {
                throw new DatabricksInterceptorException(statementResult.Status.Error.ErrorCode, statementResult.Status.Error.Message);
            }

            await Task.CompletedTask;
        }

    }
}
