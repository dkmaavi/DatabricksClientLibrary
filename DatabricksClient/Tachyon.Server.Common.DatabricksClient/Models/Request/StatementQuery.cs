namespace Tachyon.Server.Common.DatabricksClient.Models.Request
{
    public class StatementQuery
    {
        public string Statement { get; set; }
        public List<StatementQueryParameter> Parameters { get; set; }
    }
}
