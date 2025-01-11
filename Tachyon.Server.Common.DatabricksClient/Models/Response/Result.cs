using Newtonsoft.Json;

namespace Tachyon.Server.Common.DatabricksClient.Models.Response
{
    public class Result<T>
    {
        
        
        
        [JsonProperty("Chunk_Index")]
        public int ChunkIndex { get; set; }

        
        
        
        [JsonProperty("Row_Offset")]
        public long RowOffset { get; set; }

        
        
        
        [JsonProperty("Row_Count")]
        public long RowCount { get; set; }

        
        
        
        [JsonProperty("Data_Array")]
        public List<List<T>> Data { get; set; }

        
        
        
        
        [JsonProperty("Next_Chunk_Index")]
        public int? NextChunkIndex { get; set; }

        
        
        
        
        [JsonProperty("Next_Chunk_Internal_Link")]
        public string NextChunkInternalLink { get; set; }

        
        
        
        
        [JsonProperty("External_Links")]
        public List<ExternalLink> ExternalLinks { get; set; }
    }

    public class ExternalLink
    {
        
        
        
        [JsonProperty("Chunk-Index")]
        public int ChunkIndex { get; set; }

        
        
        
        [JsonProperty("Row_Offset")]
        public long RowOffset { get; set; }

        
        
        
        [JsonProperty("Row_Count")]
        public long RowCount { get; set; }

        
        
        
        [JsonProperty("Byte_Count")]
        public long ByteCount { get; set; }

        
        
        
        [JsonProperty("Next_Chunk_Index")]
        public int? NextChunkIndex { get; set; }

        
        
        
        [JsonProperty("Next_Chunk_Internal_Link")]
        public string NextChunkInternalLink { get; set; }

        
        
        
        [JsonProperty("External_Link")]
        public string Url { get; set; }

        
        
        
        [JsonProperty("Expiration")]
        public DateTime Expiration { get; set; }
    }

}
