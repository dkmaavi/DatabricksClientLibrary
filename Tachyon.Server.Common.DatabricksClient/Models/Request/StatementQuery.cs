

namespace Tachyon.Server.Common.DatabricksClient.Models.Request
{
    public class StatementQuery
    {
        public StatementQuery()
        {
            QueryId = Guid.NewGuid();
        }

        public Guid QueryId { get; private set; }
        public string Statement { get; set; }
        public List<StatementQueryParameter> Parameters { get; set; }
    }
}
