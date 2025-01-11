namespace Tachyon.Server.Common.DatabricksClient.Implementations.Handlers
{
    using Tachyon.Server.Common.DatabricksClient.Abstractions.Handlers;
    using Tachyon.Server.Common.DatabricksClient.Exceptions;
    using Tachyon.Server.Common.DatabricksClient.Models.Enums;

    internal class DatabricksErrorHandler : IDatabricksErrorHandler
    {
        public Task HandleErrorAsync(Exception ex, CancellationToken cancellationToken)
        {
            if (ex is DatabricksException or DatabricksInterceptorException)
            {
                throw ex;
            }

            var (errorCode, message) = ex switch
            {
                HttpRequestException => (ErrorCode.NETWORK_ERROR, "Failed to communicate with Databricks API"),
                OperationCanceledException => (ErrorCode.OPERATION_CANCELED, "Operation was cancelled"),
                _ => (ErrorCode.UNKNOWN, "An unexpected error occurred while executing the databricks query")
            };

            throw new DatabricksException(errorCode, message, ex);
        }
    }
}
