using Newtonsoft.Json;

namespace Tachyon.Server.Common.DatabricksClient.ApiResponse
{
    public class Status
    {
        public State State { get; set; }

        public Error Error { get; set; }
    }

    public class Error
    {
        [JsonProperty("Error_code")]
        public ErrorCode ErrorCode { get; set; }
        public string Message { get; set; }
    }

    public enum ErrorCode
    {
        // Generic error codes
        TIMEOUT,
        PARSE_ERROR,

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

    public enum State
    {
        /// <summary>
        /// Waiting for warehouse.
        /// </summary>
        Pending,
        /// <summary>
        /// Running.
        /// </summary>
        Running,
        /// <summary>
        /// Execution successful, result data available for fetch.
        /// </summary>
        Succeeded,
        /// <summary>
        /// Execution failed; reason for failure described in accompanying error message.
        /// </summary>
        Failed,
        /// <summary>
        /// User canceled; can come from explicit cancel call, or timeout with on_wait_timeout=CANCEL.
        /// </summary>
        Canceled,
        /// <summary>
        /// Execution successful, and statement closed; result no longer available for fetch.
        /// </summary>
        Closed
    }

}