namespace Tachyon.Server.Common.DatabricksClient.ApiRequest
{
    public class StatementQuery
    {
        public string Statement { get; set; }
        public List<StatementQueryParameter> Parameters { get; set; }
    }
}
