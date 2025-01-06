namespace Tachyon.Server.Common.DatabricksClient.Configuration
{
    public class DatabricksConfiguration
    {
        public string BaseUrl { get; set; }
        public string BeareToken { get; set; }
        public string WarehouseId { get; set; }
        public string CatalogName { get; set; }
        public string DatabaseName { get; set; }
        public string WaitTimeout { get; set; }
    }
}