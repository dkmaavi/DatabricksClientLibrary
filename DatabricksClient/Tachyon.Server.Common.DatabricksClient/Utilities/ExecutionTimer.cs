using System.Diagnostics;

namespace Tachyon.Server.Common.DatabricksClient.Utilities
{
    public class ExecutionTimer : IDisposable
    {
        private readonly Stopwatch stopwatch;

        public ExecutionTimer()
        {
            stopwatch = Stopwatch.StartNew();
        }

        public bool HasExceededTimeout(int timeoutSeconds) =>
            stopwatch.Elapsed.TotalSeconds > timeoutSeconds;

        public void Dispose()
        {
            stopwatch.Stop();
        }
    }
}