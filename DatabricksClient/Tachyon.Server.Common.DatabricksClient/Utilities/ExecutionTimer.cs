namespace Tachyon.Server.Common.DatabricksClient.Utilities
{
    using System.Diagnostics;
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
