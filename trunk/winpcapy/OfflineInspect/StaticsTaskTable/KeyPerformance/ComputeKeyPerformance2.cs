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

namespace GnPlatForm.KeyPerformance
{
    public class ComputeKeyPerformance2
    {
        private GuangZhou_GnEntities servercontext;
        private IEnumerable<mpos_gn_common> sqlserver_gn_table;
        public ComputeKeyPerformance2()
        {
            string serverconnstring = ConfigurationManager.ConnectionStrings["GuangZhou_GnEntities"].ToString();
            //servercontext = new GuangZhou_GnEntities1(serverconnstring);
            servercontext = new GuangZhou_GnEntities(serverconnstring);
            sqlserver_gn_table = servercontext.mpos_gn_common;
        }

        //Gn接口各指标统计分析,按照终端分类，按照uri分类，维度？？？？
        public void getKeyPerformance()
        {
            var db = from p in sqlserver_gn_table
                     where p.my_URI_Main == "weibo.cn"
                     where p.SynDirection == 0
                     where p.Event_Type == 4
                     group p by p.my_DEST_IP into ttt
                     select new
                     {
                         content = ttt.Key,
                         cnt = ttt.Count(),
                         suc = ttt.Where(e => e.Resp != null).Count(),
                         percent = 1.0 * ttt.Where(e => e.Resp != null).Count() / ttt.Count(),
                         uri_size = ttt.Average(e => e.my_URI_Len),
                         size = ttt.Average(e => e.IP_LEN_DL),  //这里只去下行
                         rate = ttt.Average(e => e.Duration) == null ? 0 : 1.0 * ttt.Average(e => e.IP_LEN_DL) / ttt.Average(e => (double)e.Duration),
                         delay = ttt.Average(e => e.Duration)  //这里不取第1个包
                     };

            var dborder = db.Where(e => e.cnt > 0).OrderByDescending(e => e.cnt);
            MicroObjectDumper.Write(dborder);
        }

        //计算各URI通过网关的成功率和时延,也可以指定uri
        public void getPostAndGetGateWay()
        {
            var gn = from p in sqlserver_gn_table
                     //.Where(e=>e.my_Content_Type !=null)
                     //where p.my_URI_Main == "weibo.cn"
                     where p.SynDirection == 0
                     //where p.my_DEST_IP=="10.0.0.172"
                     where p.Event_Type == 4
                     // where p.my_Content_Type.IndexOf("image/gif") > -1
                     //group p by new { my_URI_Main = p.my_URI_Main == "weibo.cn" } into ttt
                     group p by p.my_URI_Main into ttt
                     select new
                     {
                         uri = ttt.Key,
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
