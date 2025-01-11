namespace Tachyon.Server.Common.DatabricksClient.Models.Configuration
{
    public class StatementApiSettings
    {
        public string WarehouseId { get; set; }
        public string CatalogName { get; set; }
        public string DatabaseName { get; set; }
        public string WaitTimeout { get; set; }
    }
}
