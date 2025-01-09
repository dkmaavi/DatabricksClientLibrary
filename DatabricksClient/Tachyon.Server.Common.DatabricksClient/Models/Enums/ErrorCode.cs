namespace Tachyon.Server.Common.DatabricksClient.Models.Enums
{
    public enum ErrorCode
    {
        // Generic error codes
        TIMEOUT,
        PARSE_ERROR,
        INTERCEPTOR,
        OPERATION_CANCELED,
        NETWORK_ERROR,

        // Databricks error codes
        UNKNOWN,
        INTERNAL_ERROR,
        TEMPORARILY_UNAVAILABLE,
        IO_ERROR,
        BAD_REQUEST,
        SERVICE_UNDER_MAINTENANCE,
        WORKSPACE_TEMPORARILY_UNAVAILABLE,
        DEADLINE_EXCEEDED,
        CANCELLED,
        RESOURCE_EXHAUSTED,
        ABORTED,
        NOT_FOUND,
        ALREADY_EXISTS,
        UNAUTHENTICATED
    }
}
