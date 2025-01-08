namespace Tachyon.Server.Common.DatabricksClient.ApiRequest
{
    using Newtonsoft.Json;
    public class StatementQueryParameter
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; } = "STRING";
    }
}
