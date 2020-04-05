using System;
using System.Diagnostics;
using System.Linq;

namespace DbGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("请输入要生成的数据库并以回车键结束(多数据库用“,”隔开): ");
            
            Stopwatch st = Stopwatch.StartNew();
            st.Start();
            string[] dbs = Console.ReadLine().Split(new char[] { ',' }).ToArray();
            var generator = new Generator(dbs);
            generator.Create();
            
            Console.WriteLine($"生成成功，耗时 {st.ElapsedMilliseconds} ms");
            st.Stop();
            Console.ReadKey();
        }
    }
}