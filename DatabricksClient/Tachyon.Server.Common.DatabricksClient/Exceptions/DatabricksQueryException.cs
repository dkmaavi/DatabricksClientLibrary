namespace Tachyon.Server.Common.DatabricksClient.Exceptions
{
    using System;
    using Tachyon.Server.Common.DatabricksClient.Models.Enums;

    public class DatabricksQueryException : Exception
    {
        public ErrorCode ErrorCode { get; }
        public DatabricksQueryException(ErrorCode errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        public DatabricksQueryException(ErrorCode errorCode, string message, Exception inner)
            : base(message, inner)
        {
            ErrorCode = errorCode;
        }
    }
}
