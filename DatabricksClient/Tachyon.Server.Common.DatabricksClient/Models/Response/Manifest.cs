using Newtonsoft.Json;

namespace Tachyon.Server.Common.DatabricksClient.Models.Response
{
    public class Manifest
    {
        /// <summary>
        /// The format of the data in the result set.
        /// </summary>
        /// <typeparam name="Format">
        /// An enumeration specifying the format. Possible values include JSON_ARRAY, ARROW_STREAM, and CSV.
        /// </typeparam>
        public Format Format { get; set; }

        /// <summary>
        /// The schema of the data in the result set. 
        /// The schema is an ordered list of column descriptions.
        /// </summary>
        public Schema Schema { get; set; }

        /// <summary>
        /// The total number of chunks that the result set has been divided into.
        /// </summary>
        [JsonProperty("Total_Chunk_Count")]
        public int TotalChunkCount { get; set; }

        /// <summary>
        /// An array of objects containing the metadata for each chunk in the result set.
        /// </summary>
        public List<Chunk> Chunks { get; set; }

        /// <summary>
        /// The total number of rows in the result set.
        /// </summary>
        [JsonProperty("Total_Row_Count")]
        public long TotalRowCount { get; set; }

        /// <summary>
        /// The total number of bytes in the result set. This field is not available when using INLINE disposition..
        /// </summary>
        [JsonProperty("Total_Byte_Count")]
        public long TotalByteCount { get; set; }

        /// <summary>
        /// Indicates whether the result is truncated due to row_limit or byte_limit.
        /// </summary>
        public bool Truncated { get; set; }
    }

    public class Chunk
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
        /// Gets or sets the number of bytes in the result chunk. 
        /// This field is not available when using INLINE disposition.
        /// </summary>
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
        /// <summary>
        /// The name of the column.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The full SQL type specification.
        /// </summary>
        [JsonProperty("Type_Text")]
        public string TypeText { get; set; }

        /// <summary>
        /// The name of the base data type. This doesn't include details for complex types such as STRUCT, MAP or ARRAY.
        /// Possible values include: BOOLEAN, BYTE, SHORT, INT, LONG, FLOAT, DOUBLE, DATE, TIMESTAMP, STRING, BINARY, DECIMAL, INTERVAL, ARRAY, STRUCT, MAP, CHAR, NULL, USER_DEFINED_TYPE.
        /// </summary>
        [JsonProperty("Type_Name")]
        public DataType TypeName { get; set; }


        /// <summary>
        /// Specifies the number of digits in a number. This applies to the DECIMAL type.
        /// </summary>
        [JsonProperty("Type_Precision")]
        public int? TypePrecision { get; set; }

        /// <summary>
        /// Specifies the number of digits to the right of the decimal point in a number. This applies to the DECIMAL type.
        /// </summary>
        [JsonProperty("Type_Scale")]
        public int? TypeScale { get; set; }

        /// <summary>
        /// The format of the interval type.
        /// </summary>
        [JsonProperty("Type_Interval_Type")]
        public string TypeIntervalType { get; set; }

        /// <summary>
        /// The ordinal position of the column (starting at position 0).
        /// </summary>
        public int Position { get; set; }
    }

    public enum DataType
    {
        BOOLEAN,
        BYTE,
        SHORT,
        INT,
        LONG,
        FLOAT,
        DOUBLE,
        DATE,
        TIMESTAMP,
        STRING,
        BINARY,
        DECIMAL,
        INTERVAL,
        ARRAY,
        STRUCT,
        MAP,
        CHAR,
        NULL,
        USER_DEFINED_TYPE
    }

}