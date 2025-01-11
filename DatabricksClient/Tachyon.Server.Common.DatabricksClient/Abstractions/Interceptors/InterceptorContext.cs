using System.Diagnostics;

namespace Tachyon.Server.Common.DatabricksClient.Abstractions.Interceptors
{
    public class InterceptorContext
    {
        public Guid Id { get; } = Guid.NewGuid();
        public Stopwatch Timer { get; } = new Stopwatch();
        public Dictionary<string, object> DataItems { get; } = [];   
    }
}
