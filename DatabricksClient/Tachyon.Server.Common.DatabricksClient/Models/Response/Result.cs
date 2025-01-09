using Newtonsoft.Json;

namespace Tachyon.Server.Common.DatabricksClient.Models.Response
{
    public class Result<T>
    {
        /// <summary>
        /// Gets or sets the position within the sequence of result set chunks.
        /// </summary>
        [JsonProperty("Chunk_Index")]
        public int ChunkIndex { get; set; }

        /// <summary>
        /// Gets or sets the starting row offset within the result set.
        /// </summary>
        [JsonProperty("Row_Offset")]
        public long RowOffset { get; set; }

        /// <summary>
        /// Gets or sets the number of rows within the result chunk.
        /// </summary>
        [JsonProperty("Row_Count")]
        public long RowCount { get; set; }

        /// <summary>
        /// Gets or sets the result data array.
        /// </summary>
        [JsonProperty("Data_Array")]
        public List<List<T>> Data { get; set; }

        /// <summary>
        /// Gets or sets the chunk index for the next chunk when fetching.
        /// If absent, indicates there are no more chunks.
        /// </summary>
        [JsonProperty("Next_Chunk_Index")]
        public int? NextChunkIndex { get; set; }

        /// <summary>
        /// Gets or sets a link to fetch the next chunk when fetching.
        /// If absent, indicates there are no more chunks.
        /// </summary>
        [JsonProperty("Next_Chunk_Internal_Link")]
        public string NextChunkInternalLink { get; set; }

        /// <summary>
        /// Gets or sets the list of external links providing presigned URLs
        /// to the result data in cloud storage.
        /// </summary>
        [JsonProperty("External_Links")]
        public List<ExternalLink> ExternalLinks { get; set; }
    }

    public class ExternalLink
    {
        /// <summary>
        /// Gets or sets the position within the sequence of result set chunks.
        /// </summary>
        [JsonProperty("Chunk-Index")]
        public int ChunkIndex { get; set; }

        /// <summary>
        /// Gets or sets the starting row offset within the result set.
        /// </summary>
        [JsonProperty("Row_Offset")]
        public long RowOffset { get; set; }

        /// <summary>
        /// Gets or sets the number of rows within the result chunk.
        /// </summary>
        [JsonProperty("Row_Count")]
        public long RowCount { get; set; }

        /// <summary>
        /// Gets or sets the number of bytes in the result chunk. This field is not available when using INLINE disposition.
        /// </summary>
        [JsonProperty("Byte_Count")]
        public long ByteCount { get; set; }

        /// <summary>
        /// Gets or sets the chunk index for the next chunk. If absent, indicates there are no more chunks.
        /// </summary>
        [JsonProperty("Next_Chunk_Index")]
        public int? NextChunkIndex { get; set; }

        /// <summary>
        /// Gets or sets the link to fetch the next chunk. If absent, indicates there are no more chunks.
        /// </summary>
        [JsonProperty("Next_Chunk_Internal_Link")]
        public string NextChunkInternalLink { get; set; }

        /// <summary>
        /// Gets or sets the presigned URL pointing to a chunk of result data, hosted by an external service.
        /// </summary>
        [JsonProperty("External_Link")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the date-time that the given external link will expire and becomes invalid.
        /// </summary>
        [JsonProperty("Expiration")]
        public DateTime Expiration { get; set; }
    }

}