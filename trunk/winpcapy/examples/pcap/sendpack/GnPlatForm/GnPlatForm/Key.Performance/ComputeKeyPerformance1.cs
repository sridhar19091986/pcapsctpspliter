/*
 * 
 * 指标计算的集合？
 * 
 * */



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using GnPlatForm.MySql;
using GnPlatForm.BusinessLogic;
using GnPlatForm.SqlServer;
using System.Configuration;
using codeding.Apriori.DataStructures;

namespace GnPlatForm.KeyPerformance
{
    public class ComputeKeyPerformance1
    {
        private GuangZhou_GnEntities servercontext;
        private IEnumerable<mpos_gn_common> sqlserver_gn_table;
        public ComputeKeyPerformance1()
        {
            string serverconnstring = ConfigurationManager.ConnectionStrings["GuangZhou_GnEntities"].ToString();
            //servercontext = new GuangZhou_GnEntities1(serverconnstring);
            servercontext = new GuangZhou_GnEntities(serverconnstring);
            sqlserver_gn_table = servercontext.mpos_gn_common;

        }
        //Gn接口各指标统计分析,按照终端分类，按照uri分类，维度？？？？
        public void getKeyPerformance(Itemset hs)
        {
            var db = from p in sqlserver_gn_table
                     //where detail == true ? p.my_URI_Main == "10086.cn" : p.my_URI_Main != null
                     where p.Event_Type == 4
                     where p.SynDirection == 0
                     //where hs.Contains(p.Prefix_IMEI)
                     group p by hs.Contains(p.Prefix_IMEI) into ttt  //23g业务类型区分
                     select new
                     {
                         //rat = ttt.Key.RAT_TYPE,
                         event_type = ttt.Key,
                         cnt = ttt.Count(),
                         suc = ttt.Where(e => e.Resp != null).Count(),
                         percent = 1.0 * ttt.Where(e => e.Resp != null).Count() / ttt.Count(),
                         uri_size = ttt.Average(e => e.my_URI_Len),
                         size = ttt.Average(e => e.IP_LEN_DL),  //这里只去下行
                         rate = ttt.Average(e => e.Duration) == null ? 0 :
                         1.0 * ttt.Average(e => e.IP_LEN_DL) / ttt.Average(e => (double)e.Duration),
                         delay = ttt.Average(e => e.Duration)  //这里不取第1个包
                     };

            var dborder = db.Where(e => e.cnt > 0).OrderByDescending(e => e.cnt);
            MicroObjectDumper.Write(dborder);
        }
        //计算各URI通过网关的成功率和时延,也可以指定uri
        public void getPostAndGetGateWay(Itemset hs, ILookup<string, string> m_dicTransactions)
        {
            var gn = from p in sqlserver_gn_table
                     where p.Event_Type == 4
                     where p.SynDirection == 0
                     //where p.my_URI_Main=="qq.com" || p.my_URI_Main==string.Empty
                     //group p by new { ABC = p.DEST_IP == 2885681162 } into ttt
                     //group p by p.my_DEST_IP into ttt
                     //where detail == true ? p.my_URI_Main == "10086.cn" : p.my_URI_Main != null
                     where hs.Contains(p.Prefix_IMEI)
                     group p by p.Prefix_IMEI into ttt
                     select new
                     {
                         //abc = ttt.Key.ABC.ToString(),
                         abc = ttt.Key.ToString(),
                         type = m_dicTransactions[ttt.Key.ToString()].Where(e => e.IndexOf("iphone") != -1|e.IndexOf("iPhone") != -1).FirstOrDefault(),
                         cnt = ttt.Count(),
                         suc = ttt.Where(e => e.Resp != null).Count(),
                         percent = 1.0 * ttt.Where(e => e.Resp != null).Count() / ttt.Count(),
                         uri_size = ttt.Average(e => e.my_URI_Len),
                         size = ttt.Average(e => e.IP_LEN_DL),  //这里只去下行
                         rate = ttt.Average(e => e.Duration) == null ? 0 :
                         1.0 * ttt.Average(e => e.IP_LEN_DL) / ttt.Average(e => (double)e.Duration),
                         delay = ttt.Average(e => e.Duration)  //这里不取第1个包
                     };
            MicroObjectDumper.Write(gn.OrderByDescending(e => e.cnt));
        }
    }
}
