

namespace Tachyon.Server.Common.DatabricksClient.Constants
{
    public class DatabricksConstant
    {
        public const string HttpClientName = "DatabricksHttpClient";
        public const string ApiEndpoint = "api/2.0/sql/statements";
        public const string InsertQueryToken = "$InsertQuery$";
        public const string GlobalRequestIdKey = "GlobalInterceptor.RequestIdKey";
        public const string GlobalRequestTimerKey = "GlobalInterceptor.RequestTimerKey";
    }
}
