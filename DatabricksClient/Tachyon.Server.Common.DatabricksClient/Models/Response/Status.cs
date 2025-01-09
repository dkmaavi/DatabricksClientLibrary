using Newtonsoft.Json;
using Tachyon.Server.Common.DatabricksClient.Models.Enums;

namespace Tachyon.Server.Common.DatabricksClient.Models.Response
{
    public class Status
    {
        public State State { get; set; }

        public Error Error { get; set; }
    }

    public class Error
    {
        [JsonProperty("Error_code")]
        public ErrorCode ErrorCode { get; set; }
        public string Message { get; set; }
    }





}