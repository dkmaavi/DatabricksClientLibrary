namespace Tachyon.Server.Common.DatabricksClient.Exceptions
{
    using System;
    using Tachyon.Server.Common.DatabricksClient.Models.Enums;

    public class DatabricksException : Exception
    {
        public ErrorCode ErrorCode { get; }
        public DatabricksException(ErrorCode errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        public DatabricksException(ErrorCode errorCode, string message, Exception inner)
            : base(message, inner)
        {
            ErrorCode = errorCode;
        }
    }
}
