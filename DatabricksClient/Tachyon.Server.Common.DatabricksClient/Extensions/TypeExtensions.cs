namespace Tachyon.Server.Common.DatabricksClient.Extensions
{
    using Tachyon.Server.Common.DatabricksClient.Models.Enums;
    internal static class TypeExtensions
    {
        public static string ToDbxType(this Type type)
        {
            return type switch
            {
                Type t when t == typeof(int) => nameof(DataType.INT),
                Type t when t == typeof(long) => nameof(DataType.LONG),
                Type t when t == typeof(string) => nameof(DataType.STRING),
                Type t when t == typeof(bool) => nameof(DataType.BOOLEAN),
                Type t when t == typeof(DateTime) => nameof(DataType.TIMESTAMP),
                Type t when t == typeof(decimal) => $"{nameof(DataType.DECIMAL)}",
                _ => nameof(DataType.STRING) // Default case
            };
        }
    }
}
