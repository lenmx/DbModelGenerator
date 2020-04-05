using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbGenerator.Model;
using Microsoft.Extensions.Configuration;
using SqlSugar;

namespace DbGenerator
{
    public class Generator
    {
        public DbSettings dbSettings = new DbSettings();

        public Generator(string[] dbs)
        {
            Console.WriteLine($"配置项校验");

            if (dbs == null || dbs.Length <= 0)
                throw new Exception("请输入要生成到数据库");

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            config.GetSection("DbSettings").Bind(dbSettings);

            foreach (var db in dbs)
            {
                if (!dbSettings.Items.Any(a => a.Key.ToLower() == db.ToLower()))
                    throw new Exception($"未找到{db}配置项");
            }

            dbSettings.Items = dbSettings.Items.Where(a => dbs.Select(b => b.ToLower()).Contains(a.Key.ToLower()))
                .ToArray();
        }

        public void Create()
        {
            var tasks = new Task[dbSettings.Items.Length];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Factory.StartNew((index) =>
                {
                    var setting = dbSettings.Items[(int) index];

                    CreateTable(setting);

                    CreateContext(setting);
                }, i);
            }

            Task.WaitAll(tasks);
        }

        void CreateTable(DbSettingItem setting)
        {
            ConcurrentDictionary<string, string> files = new ConcurrentDictionary<string, string>();
            using (var db = Db(setting))
            {
                // get file string
                var tables = db.Context.DbMaintenance.GetTableInfoList(true);
                tables.ForEach(table =>
                {
                    var cols = db.Context.DbMaintenance.GetColumnInfosByTableName(table.Name);
                    var tab = new TableModel(setting.Namespace, table, cols);
                    files.TryAdd(tab.TableName, tab.FileString);
                });
            }

            WriteFiles(setting.ModelPath, files);
        }

        void CreateContext(DbSettingItem setting)
        {
            List<DbTableInfo> tables = new List<DbTableInfo>();
            using (var db = Db(setting))
                tables = db.Context.DbMaintenance.GetTableInfoList(true);

            var files = new ConcurrentDictionary<string, string>();
            var fileString = new ContextModel(setting.Namespace, setting.ContextName, tables).FileString;
            files.TryAdd(setting.ContextName, fileString);

            WriteFiles(setting.ModelPath, files);
        }

        SqlSugarClient Db(DbSettingItem setting)
            => new SqlSugarClient(new ConnectionConfig
            {
                ConnectionString = setting.ConnectionString,
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true,
            });

        void WriteFiles(string path, ConcurrentDictionary<string, string> files)
        {
            // write file 
            foreach (var file in files)
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string filepath = Path.Combine(path, file.Key+".cs");
                using (var fs = new FileStream(filepath, FileMode.Create))
                {
                    byte[] data = Encoding.UTF8.GetBytes(file.Value);
                    fs.Write(data, 0, data.Length);
                    Console.WriteLine($"{path} {file.Key} done");
                }
            }
        }
    }
}