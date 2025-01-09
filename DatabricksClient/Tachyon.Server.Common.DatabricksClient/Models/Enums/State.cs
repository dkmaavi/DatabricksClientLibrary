namespace Tachyon.Server.Common.DatabricksClient.Models.Enums
{
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
