/*
 * 
 * 指标计算的集合？ ④ 定位异常SP或WAP网关
⑥ 网络失败-无响应分析（多接口数据分析）
⑧ Gb异常信令分析（投诉分析）

 * */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GnPlatForm.BusinessLogic;
using GnPlatForm.SqlServer;
using System.Configuration;

namespace GnPlatForm.KeyPerformance
{
    public class ComputeKeyPerformance3
    {
        private GuangZhou_GnEntities servercontext;
        private IEnumerable<mpos_gn_common> sqlserver_gn_table;
        public ComputeKeyPerformance3()
        {
            string serverconnstring = ConfigurationManager.ConnectionStrings["GuangZhou_GnEntities"].ToString();
            servercontext = new GuangZhou_GnEntities(serverconnstring);
            sqlserver_gn_table = servercontext.mpos_gn_common;
        }

        //Gn接口各指标统计分析,按照终端分类，按照uri分类，维度？？？？
        public dynamic getKeyPerformance()
        {
            var db = from p in sqlserver_gn_table
                     where p.SynDirection == 0
                     where p.Event_Type == 4
                     group p by new { a = p.my_DEST_IP == "10.0.0.172", b = p.my_URI_Main_header == "10.0.0.172" } into ttt
                     select new
                     {
                         content = ttt.Key.a,
                         contentb = ttt.Key.b,
                         cnt = ttt.Count(),
                         suc = ttt.Where(e => e.Resp != null).Count(),
                         percent = 1.0 * ttt.Where(e => e.Resp != null).Count() / ttt.Count(),
                         uri_size = ttt.Average(e => e.my_URI_Len),
                         size = ttt.Average(e => e.IP_LEN_DL),  
                         rate = ttt.Average(e => e.Duration) == null ? 0 : 1.0 * ttt.Average(e => e.IP_LEN_DL) / ttt.Average(e => (double)e.Duration),
                         delay = ttt.Average(e => e.Duration) 
                     };

            var dborder = db.Where(e => e.cnt > 0).OrderByDescending(e => e.cnt);
            MicroObjectDumper.Write(dborder); return dborder;
        }

        //计算各URI通过网关的成功率和时延,也可以指定uri
        public dynamic getPostAndGetGateWay()
        {
            var gn = from p in sqlserver_gn_table
                     //where p.my_DEST_IP == "10.0.0.172" || p.my_URI_Main_header == "10.0.0.172"
                     where p.SynDirection == 0
                     where p.Event_Type == 4
                     //group p by p.my_URI_Main into tttt
                     //计算各URI通过网关的成功率和时延,也可以指定uri
                     //from p in tttt
                     group p by new { q = p.my_URI_Main } into tttt
                     select new
                     {
                         contentq = tttt.Key.q,

                         cnt = tttt.Count(),
                         r = from q in tttt
                             where q.my_URI_Main == tttt.Key.q
                             group q by new { a = q.my_DEST_IP == "10.0.0.172", b = q.my_URI_Main_header == "10.0.0.172" }
                                 into ttt
                                 select new
                                 {
                                     content = ttt.Key.a,
                                     contentb = ttt.Key.b,
                                     //uri = ttt.Key,
                                     cnt = ttt.Count(),
                                     suc = ttt.Where(e => e.Resp != null).Count(),
                                     percent = 1.0 * ttt.Where(e => e.Resp != null).Count() / ttt.Count(),
                                     uri_size = ttt.Average(e => e.my_URI_Len),
                                     size = ttt.Average(e => e.IP_LEN_DL),
                                     rate = ttt.Average(e => e.Duration) == null ? 0 :
                                     1.0 * ttt.Average(e => e.IP_LEN_DL) / ttt.Average(e => (double)e.Duration),
                                     delay = ttt.Average(e => e.Duration)
                                 }
                     };
            MicroObjectDumper.Write(gn.OrderByDescending(e => e.cnt)); return gn;
        }
    }
}
