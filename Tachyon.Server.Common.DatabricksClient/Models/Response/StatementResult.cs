using Newtonsoft.Json;

namespace Tachyon.Server.Common.DatabricksClient.Models.Response
{
    public class StatementResult
    {
        [JsonProperty("Statement_Id")]
        public string StatementId { get; set; }
        public Status Status { get; set; }
        public Manifest Manifest { get; set; }
        public Result<string> Result { get; set; }
    }
}
