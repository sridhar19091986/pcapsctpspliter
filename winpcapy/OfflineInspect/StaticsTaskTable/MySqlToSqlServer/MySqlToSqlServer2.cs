/*
 * 
 * 生产者消费者完成mysql到sqlserver的数据转换，同时完成部分字段的转换
 * 
 * 
 * 部分字段的转换还需要修改
 * 
 * */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using GnRealTimeWork;
//using GnPlatForm.SqlServer;
using System.Configuration;
using GnPlatForm.MySql;
//using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using GnPlatForm.AnalysisOut;
using GnPlatForm.BusinessLogic;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace GnPlatForm.SqlServer
{
    public class MySqlToSqlServer2
    {
   
        private int pagesize = 0;
        private const int maxqueuelength = 10;
        private int cnt = 0;
        ILookup<decimal, bool> session_list;
        int bg = 1588;  
        //2012.5.10生成的文件
        //继续时，需要修改？
        GuangZhou_GnEntities servercontext;
        guangzhou_0410Entities mysqlcontext;
        SychronizeCache<ConcurrentBag<mpos_gn_common>> sych;
        string serverconn;
        string mysqlconn;

        public MySqlToSqlServer2(bool crnewtable)
        {
            //清楚所有的mysql连接
            CreateNewTable.KillSleepingConnections(0,ref sql_conn_num);

            pagesize = Convert.ToInt16(ConfigurationManager.AppSettings["pagesize"]);

            string mysqlconnstring = ConfigurationManager.ConnectionStrings["guangzhou_0410Entities"].ToString();

            mysqlcontext = new guangzhou_0410Entities(mysqlconnstring);

            mysqlcontext.CommandTimeout = 3600;

            string serverconnstring = ConfigurationManager.ConnectionStrings["GuangZhou_GnEntities"].ToString();

            servercontext = new GuangZhou_GnEntities(serverconnstring);

            serverconn = ConfigurationManager.ConnectionStrings["serverconn"].ToString();

            mysqlconn = mysqlcontext.Connection.ConnectionString;

            sych = new SychronizeCache<ConcurrentBag<mpos_gn_common>>(maxqueuelength);

            if (crnewtable)
                servercontext.ExecuteStoreCommand(CreateNewTable.CreateNewServerTable);

        }

 

        public void batchRun()
        {
            session_list = mysqlcontext.gn_common_201204181300
                .Where(e => e.Event_Type == 4 || e.Event_Type == 5)
                .Where(e => e.APN.Length > 1)
                //.Take(10000000)
                .Select(e => new { e.Session_ID, e.IsReassemble })
                .ToLookup(e => e.Session_ID, e => e.IsReassemble);

            //mysqlcontext.Connection.Close();

            Console.WriteLine("session_list.Count:{0}", session_list.Count);

            for (int i = bg; i < session_list.Count / pagesize + 1; i++)
            {
                Console.WriteLine("task:{0},sqlserver:{1}", i, cnt);

                Thread t = new Thread(produce);

                t.Start(i);

                while (i - (cnt + bg) > maxqueuelength)  //这1点是纯数学的方法，检测线程等待的距离，2012.5.9
                    Thread.Sleep(1000);
                //Application.DoEvents();
            }

            //Parallel.For(0, pagenum_max / pagesize + 1, (i) => produce(i, pagesize));

        }

        int sql_conn_num = 0;
        //清理mysql的连接是个问题？是影响速度的主要原因
        public void produce(object pagenum)
        {
            //CreateNewTable.KillSleepingConnections(0,out sql_conn_num);

            Console.WriteLine("produce:{0}", (int)pagenum * pagesize);

            using (guangzhou_0410Entities mysqlc = new guangzhou_0410Entities(mysqlconn))
            {
                ConcurrentBag<mpos_gn_common> gnbag = new ConcurrentBag<mpos_gn_common>();

                mysqlc.CommandTimeout = 3600;

                var mysqls_split_id = session_list.Skip((int)pagenum * pagesize)
                    .Take(pagesize).Select(e => e.Key).ToArray();

                var mysqls_split_data = mysqlc
                    .gn_common_201204181300
                    .Where(e => mysqls_split_id.Contains(e.Session_ID));

                Parallel.ForEach(mysqls_split_data,
                    (m) => Parallel.Invoke(() =>
                        LoadData(m, ref gnbag)));

                try
                {
                    Task.Factory.StartNew(() => bulkLoadToServer(gnbag));
                }
                catch { }

                mysqlc.Connection.Close();
            }
        }

        public void LoadData(gn_common_201204181300 m, ref ConcurrentBag<mpos_gn_common> gnbag)
        {
            if (m.Event_Type == 4 || m.Event_Type == 5)
                if (m.APN.Length > 1)
                    gnbag.Add(convert(m));
        }

        //private mpos_gn_common mpos;

       

        //更改bulk大小的问题？可以解决缓冲溢出的问题,stackoverload, bulk大小即分页大小，pagesize

        //更改目标数据库的问题？nvarchar的问题,可以解决的问题

        //Bulk Copy error The given value of type String from the data source cannot be converted to type nchar of the specified target column.


        private void bulkLoadToServer(ConcurrentBag<mpos_gn_common> rtlbags)
        {

            //CreateNewTable.KillSleepingConnections(0, ref sql_conn_num);

            //Application.DoEvents();

            cnt++;

            Console.WriteLine("bulkToServer-pagesize*pagenum:{0},pagenum:{1},rtlbags:{2}",
                pagesize * cnt, cnt, rtlbags.Count);
            //Data Source=192.168.4.105;Initial Catalog=GuangZhou_Gn;User ID=weihp;Password=hantele;MultipleActiveResultSets=True
            //Data Source=localhost;Initial Catalog=GuangZhou_Gn;Integrated Security=True
            //servercontext.Connection.ConnectionS
            using (SqlConnection con = new SqlConnection(serverconn))
            {
                con.Open();
                try
                {
                    using (SqlTransaction tran = con.BeginTransaction())
                    {
                        var newOrders = rtlbags;
                        SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.KeepNulls, tran);
                        bc.BulkCopyTimeout = 36000;
                        bc.BatchSize = pagesize + 20;
                        bc.DestinationTableName = "mpos_gn_common";
                        bc.WriteToServer(newOrders.AsDataReader());
                        tran.Commit();
                    }
                }
                catch
                {
                    Console.WriteLine("bulkToServer-error:{0}", cnt);

                    using (var writer = new StreamWriter(cnt + ".txt", true, Encoding.UTF8))
                    {
                        writer.WriteLine("bulkToServer-error:{0}", cnt);
                    }
                }
                finally
                {
                    con.Close();
                }
                con.Close();
            }
            //while (!rtlbags.IsEmpty) rtlbags.TryTake(out mpos);
            //GC.Collect(); GC.Collect();
        }


        public mpos_gn_common convert(gn_common_201204181300 m)
        {
            //Console.WriteLine(m.Session_ID);
            mpos_gn_common gn = new mpos_gn_common();
            gn.Abnormal_reason = (int?)m.Abnormal_reason;
            //文本解释转换
            gn.my_Abnormal_reason = MySqlConvert0.getAbnormalReason(m.Abnormal_reason);
            gn.abort = m.abort == true ? 1 : 0;
            gn.Ack = (int?)m.Ack;
            gn.APN = m.APN;
            gn.CI = (int?)m.CI;
            gn.Content_Type = m.Content_Type;//
            //文本解释转换
            gn.my_Content_Type = MySqlConvert0.getContentTypeHeader(m.Content_Type);
            gn.Count_Packet_DL = (int?)m.Count_Packet_DL;
            gn.Count_Packet_UL = (int?)m.Count_Packet_UL;
            gn.Delete_PDP = m.Delete_PDP;
            gn.Delete_PDP_Direction = m.Delete_PDP_Direction;
            gn.Delivery_Report = m.Delivery_Report;
            gn.DEST_IP = m.DEST_IP;
            //文本解释转换
            gn.my_DEST_IP = MySqlConvert0.getIpAddress((int)m.DEST_IP);
            gn.DEST_PORT = m.DEST_PORT;
            gn.Disconnect = m.Disconnect;
            gn.DNS_TTL = (int?)m.DNS_TTL;
            gn.Duration = m.Duration;
            gn.End_Date_Time = DateTime.Parse(m.End_Date_Time.ToString());
            gn.ErrorPacketLost = (int?)m.ErrorPacketLost;
            gn.ErrorPacketWired = (int?)m.ErrorPacketWired;
            gn.ErrorPacketWireless = (int?)m.ErrorPacketWireless;
            gn.Event_Type = (int?)m.Event_Type;
            //文本解释转换
            gn.my_Event_Type = MySqlConvert0.getEventType(m.Event_Type);
            gn.From = m.From;
            gn.GGSN = m.GGSN;
            gn.GGSN_IP = m.GGSN_IP;
            gn.Identifier = (int?)m.Identifier;
            gn.IMEI = m.IMEI;
            gn.IMSI = m.IMSI;
            gn.IP_LEN_DL = (int?)m.IP_LEN_DL;
            gn.IP_LEN_UL = (int?)m.IP_LEN_UL;
            gn.IpLenDlAck = (int?)m.IpLenDlAck;
            gn.IpLenUpAck = (int?)m.IpLenUpAck;
            gn.Is_UserAbnormal = (int?)m.Is_UserAbnormal;
            gn.IsReassemble = m.IsReassemble;
            gn.LAC = (int?)m.LAC;
            gn.MMS_request = m.MMS_request;
            gn.MMS_resp_status = m.MMS_resp_status;
            gn.MSISDN = (long)m.MSISDN;
            gn.Online_ID = (long)m.Online_ID;
            gn.Prefix_IMEI = m.Prefix_IMEI;
            gn.PROTOCOL = m.PROTOCOL;
            //文本解释转换
            gn.my_Protocol = MySqlConvert0.getProtocol(m.PROTOCOL);
            gn.Query_Name = m.Query_Name;//
            gn.Query_Type = m.Query_Type;
            gn.RAT_TYPE = m.RAT_TYPE;
            //文本解释转换
            gn.my_RAT_TYPE = MySqlConvert0.getRateType(m.RAT_TYPE);
            gn.Repeat_Count = (int?)m.Repeat_Count;
            gn.Resp = m.Resp;
            gn.Resp_cause = m.Resp_cause;
            gn.Resp_DelayFirst = (int?)m.Resp_DelayFirst;
            gn.Result = m.Result;
            gn.Result_DelayFirst = m.Result_DelayFirst;
            gn.SAC = (int?)m.SAC;
            gn.Service_TYPE = m.Service_TYPE;
            //文本解释转换
            gn.my_Service_TYPE = MySqlConvert0.getServiceType(m.Service_TYPE);
            gn.Session_ID = (long)m.Session_ID;
            gn.SGSN = m.SGSN;
            gn.SGSN_IP = m.SGSN_IP;
            gn.SOURCE_IP = m.SOURCE_IP;
            //文本解释转换
            gn.my_SOURCE_IP = MySqlConvert0.getIpAddress((int)m.SOURCE_IP);
            gn.SOURCE_PORT = m.SOURCE_PORT;
            gn.SP_Address = m.SP_Address;
            gn.Start_Date_Time = DateTime.Parse(m.Start_Date_Time.ToString());
            gn.Sub_Service_Type = m.Sub_Service_Type;
            //文本解释转换
            gn.my_Sub_Service_Type = MySqlConvert0.getSubServiceType(m.Sub_Service_Type);
            gn.Subject = m.Subject;//
            gn.SynDirection = (int?)m.SynDirection;
            gn.To = m.To;//
            gn.URI = m.URI;//
            //文本解释转换
            gn.my_URI_Len = MySqlConvert0.getURI_Control_N(m.URI);
            gn.my_URI_IMEI = MySqlConvert0.getURI_IMEI(m.URI);
            gn.my_URI_UA = MySqlConvert0.getURI_UA(m.URI);

            gn.my_URI_UA_MS_Type = MySqlConvert0.getURI_MS_Type(gn.my_URI_UA);
            gn.my_URI_UA_Weibo_Ver = MySqlConvert0.getURI_Weibo_Ver(gn.my_URI_UA);
            gn.my_URI_UA_OS = MySqlConvert0.getURI_OS(gn.my_URI_UA);

            gn.URI_Main = m.URI_Main;//
            //正则表达式转换
            gn.my_URI_Main = MySqlConvert0.getUriMain(m.URI_Main);//
            //获取uri的主机的二级页面，即网页内容
            gn.my_URI_Main_header = MySqlConvert0.getUriMainHeader(m.URI_Main);
            gn.User_Agent_Main = m.User_Agent_Main;//

            return gn;
        }

    }
}


/*
ALTER TABLE `gn_common_201204111000`
MODIFY COLUMN `URI`  varchar(1024) BINARY NULL AFTER `DEST_PORT`,
MODIFY COLUMN `URI_Main`  varchar(128) BINARY NULL AFTER `URI`,
MODIFY COLUMN `Content_Type`  varchar(30) BINARY NULL AFTER `URI_Main`,
MODIFY COLUMN `User_Agent_Main`  varchar(30) BINARY NULL AFTER `Content_Type`,
MODIFY COLUMN `To`  varchar(30) BINARY NULL DEFAULT NULL AFTER `From`,
MODIFY COLUMN `Subject`  varchar(40) BINARY NULL DEFAULT NULL AFTER `To`,
MODIFY COLUMN `Query_Name`  varchar(128) BINARY NULL DEFAULT NULL AFTER `Delivery_Report`;
ADD PRIMARY KEY (`Session_ID`);
 * 
 * ALTER TABLE `gn_common_201204111000`
MODIFY COLUMN `PROTOCOL`  tinyint(3) NOT NULL AFTER `RAT_TYPE`;

**/


