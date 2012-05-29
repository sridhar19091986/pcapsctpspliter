

//2012.5.18,完成数据库脚本的生成


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace AutoSql_Ts
{
    class Program
    {
        const int time_size = 15;
        static string conf_file = "config.xml";
        static int gb_pre_time = 24;
        static string gb_ts_xcd_script;
        static string gb_ts_table;
        static string gb_ts_in;

        static Dictionary<string, DateTime> imsi_ts_dic = new Dictionary<string, DateTime>();

        static List<Tuple<string, string>> imsi_ts_tb = new List<Tuple<string, string>>();


        //生成投诉信息库，读取文件信息？建表时的engine？不然报错？
        static void Main(string[] args)
        {
            XElement config = XElement.Load(conf_file);
            gb_ts_in = config.Element("gb_ts_in").Value.ToString();
            gb_ts_table = config.Element("gb_ts_table").Value.ToString();
            gb_ts_xcd_script = config.Element("gb_ts_xcd_script").Value.ToString();
            gb_pre_time = int.Parse(config.Element("gb_pre_time").Value.ToString());

            if (File.Exists(gb_ts_xcd_script))
                File.Delete(gb_ts_xcd_script);

            StreamReader reader = new StreamReader(gb_ts_in, System.Text.Encoding.Default);

            string create_ts = string.Format(@"
                    drop table if exists {0};
                    create table {0} engine=myisam  select * from gb_common_201205151200 where 1<>1;"
                    , gb_ts_table);
            WriteToFile(create_ts);

            while (!reader.EndOfStream)
            {
                string m = reader.ReadLine();
                string[] ar = m.Split(Convert.ToChar(','));
                if (ar[0] == string.Empty) break;
                read_ts_line(ar[0], ar[1]);
            }

            string[] mm = new string[] { "00", "15", "30", "45" };
            foreach (var m in imsi_ts_dic)
            {
                for (int i = 0; i < gb_pre_time * mm.Length; i++)
                {
                    var dt1 = RoundUp(m.Value.AddMinutes(-1 * i * time_size), TimeSpan.FromMinutes(time_size));
                    string ts_table = dt1.ToString("yyyyMMddHHmm");
                    Tuple<string, string> ts = new Tuple<string, string>(m.Key, "gb_common_" + ts_table);
                    imsi_ts_tb.Add(ts);
                }
            }

            int line=0;
            var look_ts = imsi_ts_tb.ToLookup(e => e.Item2);
            foreach (var m in look_ts)
            {
                string imsilist = m.Select(e => e.Item1).Aggregate((a, b) => a + "," + b);

                string sql = string.Format("INSERT into {0} select * from  {1} where IMSI in ({2});",
                    gb_ts_table, m.Key, imsilist);

                sql = contact_sql(m.Key, sql);

                line++;

               

                WriteToFile(sql);
            }

            WriteToFile("select '第" + line.ToString() + "行';");

            WriteToFile("select '脚本运行完成!';");

            Console.WriteLine("gb_ts_xcd_script.txt，脚本文件生成完毕！");

            Console.ReadKey();
        }


        //解析投诉信息，生成查询语句，根据投诉时间确定在哪几张表进行查询？
        private static void read_ts_line(string imsi, string ttime)
        {
            string ts_imsi = imsi.Trim();
            DateTime ts_time = DateTime.Parse(ttime);
            if (!imsi_ts_dic.ContainsKey(ts_imsi))
                if (ts_imsi.Length == 15)
                    imsi_ts_dic.Add(ts_imsi, ts_time);
        }

        /*
        http://stackoverflow.com/questions/7029353/c-sharp-round-up-time-to-nearest-x-minutes
         * Is there a simple function for rounding UP a DateTime in C#, to the nearest 15 minutes?
        E.g.
        2011-08-11 16:59 becomes 2011-08-11 17:00
        2011-08-11 17:00 stays as 2011-08-11 17:00
        2011-08-11 17:01 becomes 2011-08-11 17:15
        **/
        private static DateTime RoundUp(DateTime dt, TimeSpan d)
        {
            return new DateTime(((dt.Ticks + d.Ticks - 1) / d.Ticks) * d.Ticks);
        }


        //判断表是否存在最好是用存储过程，调用该存储过程执行 投诉数据的汇聚？
        private static string contact_sql(string ts_table, string sql)
        {

            string procedure = string.Format(@"

                    DROP PROCEDURE IF EXISTS `sp_Check_Table`; 
                    create procedure sp_check_table (IN p_tablename varchar(50))
                    begin
                      select count(1) from information_schema.tables where table_name = p_tablename into @cnt;
                      if @cnt != 0 then 
                        {0}
                      else 
                        select 'Table not exists!';
                      end if;
                    end;
                    call sp_check_table('{1}');", sql, ts_table);
            return procedure;
        }

        //写文件，同时显示在控制台窗口？
        private static void WriteToFile(string strs)  //这里增加些文件的方法
        {
            using (var writer = new StreamWriter(gb_ts_xcd_script, true, Encoding.UTF8))
            {
                writer.WriteLine(strs);
                Console.WriteLine(strs);
            }
        }
    }
}
