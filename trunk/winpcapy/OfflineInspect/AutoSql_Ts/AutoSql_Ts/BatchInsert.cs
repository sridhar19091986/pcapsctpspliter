/*
 * 
 * 
 * 同时开启多个线程，对每个表进行操作，
 * 
 * 
 * 同时，把事件写入log，或者在console显示？
 * 
 * 
 * 2012.7.27，解决投诉用户数据库入库慢的问题？
 * 
 * 多表操作，最后多表合并？？？？？同时只对一个表操作可能存在问题，
 * 
 * 也可以考虑用mongo做临时存储？  insert????
 * 
 * 几百张表合并？
 * 
 * 
 * */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace AutoSql_Ts
{

    public class BatchInsert
    {
        //c# Threadpool - limit number of threads
        //http://stackoverflow.com/questions/10342057/c-sharp-threadpool-limit-number-of-threads

        private MySqlConnection mysqlconn;

        public BatchInsert(string sqlconnstr)
        {
            this.mysqlconn = new MySqlConnection(sqlconnstr);
        }

        public void BatchInsertToMySql(List<string> batchsql)
        {
            Parallel.ForEach(batchsql, new ParallelOptions { MaxDegreeOfParallelism = 10 }, sql =>
                {
                    InsertTranscation(sql);
                });
            Console.WriteLine("finished");
        }

        public void InsertTranscation(string sql)
        {
            Console.WriteLine("start");

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = mysqlconn;
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();

            Console.WriteLine("end");
        }
    }
}