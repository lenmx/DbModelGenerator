using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using DbGenerator;

namespace DbGenerator.Model
{
    public class TableModel
    {
        public string Namespace { get; set; }
        public string TableName { get; set; }
        public string Description { get; set; }
        public DbObjectType DbObjectType { get; set; }
        public List<ColumnModel> Columns { get; set; }

        public TableModel(string _namespace, DbTableInfo table, List<DbColumnInfo> cols)
        {
            Namespace = _namespace;
            TableName = table.Name;
            Description = table.Description;
            DbObjectType = table.DbObjectType;
            Columns = new List<ColumnModel>();
            cols.ForEach(col => { Columns.Add(new ColumnModel(col)); });
        }

        public string FileString => $@"
using System;
using SqlSugar;

namespace {Namespace}
{{
    /// <summary>
    /// {Description}
    /// </summary>
    [SugarTable(""{TableName}"")]
    public partial class {TableName}
    {{
        public {TableName}() {{ }}
        {Columns.Select(a => a.FileString).Aggregate((col1, col2) => col1 + col2)}
    }}
}}";
    }
}

public class ColumnModel
{
    public string TableName { get; set; }
    public int TableId { get; set; }
    public string DbColumnName { get; set; }
    public string PropertyName { get; set; }
    public string DataType { get; set; }
    public Type PropertyType { get; set; }
    public int Length { get; set; }
    public string ColumnDescription { get; set; }
    public string DefaultValue { get; set; }
    public bool IsNullable { get; set; }
    public bool IsIdentity { get; set; }
    public bool IsPrimarykey { get; set; }
    public object Value { get; set; }
    public int DecimalDigits { get; set; }
    public int Scale { get; set; }

    public string DotNetDataType
        => ConstDataType.Map.ContainsKey(DataType) ? ConstDataType.Map[DataType] : DataType;

    public string NullableFlag
        => IsNullable && !ConstDataType.NullableFlags.Any(a => a == DotNetDataType) ? "?" : "";

    public ColumnModel(DbColumnInfo col)
    {
        ColumnDescription = col.ColumnDescription;
        DataType = col.DataType;
        DbColumnName = col.DbColumnName;
        DecimalDigits = col.DecimalDigits;
        DefaultValue = col.DefaultValue;
        IsIdentity = col.IsIdentity;
        IsNullable = col.IsNullable;
        IsPrimarykey = col.IsPrimarykey;
        Length = col.Length;
        PropertyName = col.PropertyName;
        PropertyType = col.PropertyType;
        Scale = col.Scale;
        TableId = col.TableId;
        TableName = col.TableName;
        Value = col.Value;
    }

    public string FileString => $@"
        /// <summary>
        /// Desc: {ColumnDescription}
        /// Default: {DefaultValue}
        /// Nullable: {IsNullable}
        /// </summary> {(IsPrimarykey ? $@"
        [SugarColumn(IsPrimaryKey = true, IsIdentity = {IsIdentity.ToString().ToLower()})]" : "")}
        public {DotNetDataType}{NullableFlag} {DbColumnName} {{ get; set; }}";
}