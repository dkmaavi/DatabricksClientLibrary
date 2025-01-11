namespace Tachyon.Server.Common.DatabricksClient.Exceptions
{
    using System;
    public class DatabricksInterceptorException : Exception
    {
        public DatabricksInterceptorException(string message)
            : base(message)
        {
        }

        public DatabricksInterceptorException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
