using System.Diagnostics.Metrics;

namespace DemoApp
{
    public class Counters
    {
        public long ApiRequestsTotal { get; set; }
        public long ApiSuccessfulRequestsTotal { get; set; }
        public long ApiFailedRequestsTotal { get; set; }
    }
}
