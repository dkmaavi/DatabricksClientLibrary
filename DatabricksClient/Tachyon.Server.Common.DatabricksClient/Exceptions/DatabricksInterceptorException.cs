using Tachyon.Server.Common.DatabricksClient.Models.Enums;

namespace Tachyon.Server.Common.DatabricksClient.Exceptions
{
    public class DatabricksInterceptorException : Exception
    {
        public ErrorCode ErrorCode { get; }

        public DatabricksInterceptorException(string message)
            : base(message)
        {
        }

        public DatabricksInterceptorException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public DatabricksInterceptorException(ErrorCode errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        public DatabricksInterceptorException(ErrorCode errorCode, string message, Exception inner)
            : base(message, inner)
        {
            ErrorCode = errorCode;
        }
    }
}