namespace DbGenerator.Model
{
    public class DbSettings
    {
        public DbSettingItem[] Items  { get; set; }
    }

    public class DbSettingItem
    {
        public string Key { get; set; }
        public string ConnectionString { get; set; }
        public string ModelPath { get; set; }
        public string Namespace { get; set; }
        public string ContextName { get; set; }
        public string DbType { get; set; }
    }
}