namespace Tachyon.Server.Common.DatabricksClient.Exceptions
{
    using System;
    using Tachyon.Server.Common.DatabricksClient.Models.Enums;

    public class DatabricksHttpException : Exception
    {
        public ErrorCode ErrorCode { get; }
        public DatabricksHttpException(ErrorCode errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        public DatabricksHttpException(ErrorCode errorCode, string message, Exception inner)
            : base(message, inner)
        {
            ErrorCode = errorCode;
        }
    }
}
