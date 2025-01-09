namespace Tachyon.Server.Common.DatabricksClient.Exceptions
{
    using System;
    using Tachyon.Server.Common.DatabricksClient.Models.Enums;

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
