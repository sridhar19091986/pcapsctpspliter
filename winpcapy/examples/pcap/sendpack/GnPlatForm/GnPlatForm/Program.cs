/*
 * 
 * 需要修改平台的表格的字段类型设计，否则会出现linq的类型转换错误
 * 2012.4.10
 * 
 * mysql没有主键也会和linq冲突
 * 2012.4.10
 * 
 * 引入EF4 + MySql做linq的开发
 * 2012.4.10
 * 
 * 引入EF4  + SqlServer 做 linq 开发存在的问题， appconfi 文件需要做修改？
 * 
 * */


//插入数据库，更新数据库？
//#define bulkdata_ef4


//#define bulkdata_dblinq


//#define test


//手机终端
//#define apriori_ab


//#define cal_post0
//#define cal_post1
//#define cal_post2

#define cal_post3


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using GnPlatForm.BusinessLogic;
//using GnPlatForm.MySql;
using System.IO;
using GnPlatForm.KeyPerformance;
using GnPlatForm.SqlServer;
//using GnPlatForm.BusinessLogic;
using codeding.Apriori.DataStructures;
using System.Windows.Forms;
using System.Configuration;


namespace GnPlatForm
{
    class Program
    {
          [STAThread]
        static void Main(string[] args)
        {
              int[] a=new int[]{1,2,3};
              int[] b=new int[]{1,2};
              var c = a.Intersect(b);
              foreach(var m in c)
            Console.WriteLine(m);

            Console.WriteLine("0|1:{0},1|1:{1},20<<3:{2},1&~1:{3}", 0 | 1, 1 | 1, 20 << 3, 1 & ~1);


#if apriori_ab

            AprioriRules.StartLog();
            AprioriSqlServer ass = new AprioriSqlServer();
            AprioriRules am = new AprioriRules(ass);

            MicroObjectDumper.StartLog();
            ComputeKeyPerformance1 ck = new ComputeKeyPerformance1();
            ck.getKeyPerformance(am.uniqueItems);
            ck.getPostAndGetGateWay(am.uniqueItems,ass.m_dicTransactions_ex);
            //am.Solve(5, 0.8);      
#endif



#if bulkdata_ef4

            //是新建一张表？还是在原来的表中添加？
            //这样可以连续处理多个小时的数据？
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["bulkserver"]) == true)
            {
                //MySqlToSqlServer2 mysql2server = new MySqlToSqlServer2(true);

                MySqlToSqlServer2 mysql2server = new MySqlToSqlServer2(false);

                mysql2server.batchRun();

            }

#endif

#if test
            //代码测试
            ElementDumper.dumpEvnentType();     
#endif

#if  cal_post0
            //指标计算
            MicroObjectDumper.StartLog();
            ComputeKeyPerformance0 ck = new ComputeKeyPerformance0();
            ck.getKeyPerformance();
            ck.getPostAndGetGateWay();
#endif
#if  cal_post2
            //指标计算
            MicroObjectDumper.StartLog();
            ComputeKeyPerformance2 ck = new ComputeKeyPerformance2();
            ck.getKeyPerformance();
            ck.getPostAndGetGateWay();
#endif
#if  cal_post3
            //指标计算
            Main1();
#endif
#if calkpis
            //指标计算
            MicroObjectDumper.StartLog();
            ComputeKeyPerformance ck = new ComputeKeyPerformance();
            ck.getKeyPerformance(true, ass.hs);
            ck.getPostAndGetGateWay(true, ass.hs);                
#endif


            //Console.ReadKey();
        }

       [STAThread]
        static void Main1()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
}
}