using Newtonsoft.Json;
using Tachyon.Server.Common.DatabricksClient.Models.Enums;



namespace Tachyon.Server.Common.DatabricksClient.Models.Response
{
    public class Manifest
    {
        
        
        
        
        
        
        public Format Format { get; set; }

        
        
        
        
        public Schema Schema { get; set; }

        
        
        
        [JsonProperty("Total_Chunk_Count")]
        public int TotalChunkCount { get; set; }

        
        
        
        public List<Chunk> Chunks { get; set; }

        
        
        
        [JsonProperty("Total_Row_Count")]
        public long TotalRowCount { get; set; }

        
        
        
        [JsonProperty("Total_Byte_Count")]
        public long TotalByteCount { get; set; }

        
        
        
        public bool Truncated { get; set; }
    }

    public class Chunk
    {
        
        
        
        [JsonProperty("Chunk_Index")]
        public int ChunkIndex { get; set; }

        
        
        
        [JsonProperty("Row_Offset")]
        public long RowOffset { get; set; }

        
        
        
        [JsonProperty("Row_Count")]
        public long RowCount { get; set; }

        
        
        
        
        [JsonProperty("Byte_Count")]
        public long ByteCount { get; set; }
    }

    public enum Format
    {
        JSON_ARRAY,
        ARROW_STREAM,
        CSV
    }

    public class Schema
    {
        [JsonProperty("Column_Count")]
        public int ColumnCount { get; set; }

        public List<Column> Columns { get; set; }
    }

    public class Column
    {
        
        
        
        public string Name { get; set; }

        
        
        
        [JsonProperty("Type_Text")]
        public string TypeText { get; set; }

        
        
        
        
        [JsonProperty("Type_Name")]
        public DataType TypeName { get; set; }


        
        
        
        [JsonProperty("Type_Precision")]
        public int? TypePrecision { get; set; }

        
        
        
        [JsonProperty("Type_Scale")]
        public int? TypeScale { get; set; }

        
        
        
        [JsonProperty("Type_Interval_Type")]
        public string TypeIntervalType { get; set; }

        
        
        
        public int Position { get; set; }
    }   

}
