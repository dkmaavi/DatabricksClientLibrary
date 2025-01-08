using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Tachyon.Server.Common.DatabricksClient.Helpers;

internal sealed class QueryExecutionTimer : IDisposable
{
    private readonly Stopwatch _stopwatch;

    public QueryExecutionTimer()
    {
        _stopwatch = Stopwatch.StartNew();
    }

    public bool HasExceededTimeout(int timeoutSeconds) =>
        _stopwatch.Elapsed.TotalSeconds > timeoutSeconds;

    public void Dispose()
    {
        _stopwatch.Stop();
    }
}