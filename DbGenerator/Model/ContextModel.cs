using SqlSugar;
using System.Collections.Generic;
using System.Linq;

namespace DbGenerator.Model
{
    public class ContextModel
    {
        public string Namespace { get; set; }
        public string ContextName { get; set; }
        public List<ContextTableModel> Tables { get; set; }

        public ContextModel(string _namespace, string contextName, List<DbTableInfo> tabs)
        {
            Namespace = _namespace;
            ContextName = contextName;
            Tables = new List<ContextTableModel>();
            tabs.ForEach(tab => { Tables.Add(new ContextTableModel(tab.Name, tab.Description)); });

            var temp = Tables.Select(a => a.FileString).ToList();
            var temp1 = temp.Aggregate((tab1, tab2) => tab1 + tab2);
        }


        public string FileString => $@"
using System;
using SqlSugar;
        
namespace {Namespace}
{{
    /// <summary>
    /// 
    /// </summary>
    public partial class {ContextName} : SqlSugarClient
    {{
        public {ContextName}(ConnectionConfig config) : base(config) {{ }}
        {Tables.Select(a => a.FileString).ToList().Aggregate((tab1, tab2) => tab1 + tab2)}
    }}
}}";
    }

    public class ContextTableModel
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public ContextTableModel(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }

        public string FileString => $@"
        /// <summary>
        /// {Description}
        /// </summary>
        public SimpleClient<{Name}> {Name} => base.GetSimpleClient<{Name}>();";
    }
}