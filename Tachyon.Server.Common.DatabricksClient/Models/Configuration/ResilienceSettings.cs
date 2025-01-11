namespace Tachyon.Server.Common.DatabricksClient.Models.Configuration
{
    public class ResilienceSettings
    {
        public int HttpTimeout { get; set; } = 300;
        public int QueryTimeout { get; set; } = 120;
        public int PollingInterval { get; set; } = 2;
        public int MaxRetry { get; set; } = 3;
        public int CircuitBreakDuration { get; set; } = 30;
        public bool DisbleRetryPolicy { get; set; } = false;
    }
}
