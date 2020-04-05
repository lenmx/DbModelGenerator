using System.Collections.Generic;

namespace DbGenerator
{
    public static class ConstDataType
    {
        public static readonly string[] NullableFlags = new string[] { "String", "Byte[]" };
        public static readonly Dictionary<string, string> Map = new Dictionary<string, string>
        {
            {  "bigint", "Int64" },
            {  "tinyint", "Byte" },
            {  "binary", "Byte[]" },
            {  "image", "Byte[]" },
            {  "varbinary", "Byte[]" },
            {  "timestamp", "Byte[]" },
            {  "bit", "Boolean" },
            {  "char", "String" },
            {  "nchar", "String" },
            {  "ntext", "String" },
            {  "nvarchar", "String" },
            {  "varchar", "String" },
            {  "text", "String" },
            {  "date", "DateTime" },
            {  "datetime", "DateTime" },
            {  "datetime2", "DateTime" },
            {  "smalldatetime", "DateTime" },
            {  "datetimeoffset", "DateTimeOffset" },
            {  "time", "TimeSpan" },
            {  "decimal", "Decimal" },
            {  "money", "Decimal" },
            {  "numeric", "Decimal" },
            {  "smallmoney", "Decimal" },
            {  "float", "Double" },
            {  "int", "Int32" },
            {  "real", "Single" },
            {  "smallint", "Int16" },
        };
    }
}