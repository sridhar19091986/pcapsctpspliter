using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;
using System.Data.Objects;
using System.Data.Entity;
using System.Data.Entity.Design;
using EntityFramework.Extensions;
using EntityFramework.Caching;
using EntityFramework.Batch;
//using MutliInterface_Gi;
//using MutliInterface_Giw;
//using MutliInterface_Gn;

//using System.Data.DataSetExtensions;

using MutliInterfaceGnGi.ServerEntity.Gi;
using MutliInterfaceGnGi.ServerEntity.Gw;
using MutliInterfaceGnGi.ServerEntity.Gn;
using MutliInterfaceGnGi.ServerEntity.Other;
using MutliInterfaceGnGi.ServerEntity.Gb;


//syn指标统计分析

namespace MutliInterfaceGnGi
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public partial class Service1 : IService1
    {


        //单接口指标统计
        private DataTable viewKeyKPI_gn_FromCache()
        {
  

            var gngi = from p in gz_gn.GnGiGw_Syn.FromCache()
                       group p by p.Syn into ttt
                       select new
                       {
                           ttt.Key,
                           min_time = ttt.Min(e => e.PacketTime),
                           max_time = ttt.Max(e => e.PacketTime),
                           itface = "Gn",
                           get_cnt = ttt.Count(),
                           reponse_cnt = ttt.Where(e => decimal.Parse(e.acksyn_ack==null?"0":e.acksyn_ack) - decimal.Parse(e.tcp_seq) == 1).Count(),
                           reponse_rate = 1.0 * ttt.Where(e => decimal.Parse(e.acksyn_ack==null?"0":e.acksyn_ack) - decimal.Parse(e.tcp_seq) == 1).Count() / ttt.Count(),
                           reponse_delay = ttt.Where(e => decimal.Parse(e.acksyn_ack==null?"0":e.acksyn_ack) - decimal.Parse(e.tcp_seq) == 1).Average(e => e.Ack_Syn_delayFirst),

                       };

            var dborder = gngi.OrderByDescending(e => e.get_cnt);
            return dborder.ToDataTable();
        }

        //单接口指标统计
        private DataTable viewKeyKPI_gi_FromCache()
        {


            var gngi = from p in gz_gi.GnGiGw_Syn.FromCache()
                       group p by p.Syn into ttt
                       select new
                       {
                           ttt.Key,
                           min_time = ttt.Min(e => e.PacketTime),
                           max_time = ttt.Max(e => e.PacketTime),
                           itface = "Gi_neikou",

                           get_cnt = ttt.Count(),
                           reponse_cnt = ttt.Where(e => decimal.Parse(e.acksyn_ack==null?"0":e.acksyn_ack) - decimal.Parse(e.tcp_seq) == 1).Count(),
                           reponse_rate = 1.0 * ttt.Where(e => decimal.Parse(e.acksyn_ack==null?"0":e.acksyn_ack) - decimal.Parse(e.tcp_seq) == 1).Count() / ttt.Count(),
                           reponse_delay = ttt.Where(e => decimal.Parse(e.acksyn_ack==null?"0":e.acksyn_ack) - decimal.Parse(e.tcp_seq) == 1).Average(e => e.Ack_Syn_delayFirst),

                       };

            var dborder = gngi.OrderByDescending(e => e.get_cnt);
            return dborder.ToDataTable();
        }

        //单接口指标统计
        private DataTable viewKeyKPI_giw_FromCache()
        {


            var gi = from p in gz_giw.GnGiGw_Syn.FromCache()
                     group p by p.Syn into ttt
                     select new
                     {
                         ttt.Key,
                         min_time = ttt.Min(e => e.PacketTime),
                         max_time = ttt.Max(e => e.PacketTime),
                         itface = "Gi_waikou",
                         get_cnt = ttt.Count(),
                         reponse_cnt = ttt.Where(e => decimal.Parse(e.acksyn_ack==null?"0":e.acksyn_ack) - decimal.Parse(e.tcp_seq) == 1).Count(),
                         reponse_rate = 1.0 * ttt.Where(e => decimal.Parse(e.acksyn_ack==null?"0":e.acksyn_ack) - decimal.Parse(e.tcp_seq) == 1).Count() / ttt.Count(),
                         reponse_delay = ttt.Where(e => decimal.Parse(e.acksyn_ack==null?"0":e.acksyn_ack) - decimal.Parse(e.tcp_seq) == 1).Average(e => e.Ack_Syn_delayFirst),

                     };
            var gngi = gi;
            var dborder = gngi.OrderByDescending(e => e.get_cnt);
            return dborder.ToDataTable();
        }

        //gi内口成功率的计算
        //在Gi外口中过滤不成功的？
        private DataTable viewKeyKPI_gi_from_giw_FromCache(string tcp)
        {
     

            if (tcp == "TCP")
            {
                var gngi = from n in gz_gn.GnGiGw_Syn.FromCache()
                           join w in gz_giw.GnGiGw_Syn.FromCache()
                           on new { ip = n.ip2_dst_host, id = n.ip2_id, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack }
                           equals new { ip = w.ip_dst_host, id = w.ip_id, w.tcp_seq, w.tcp_nxtseq, w.tcp_ack }
                           join i in gz_gi.GnGiGw_Syn.FromCache() 
                           on new { ip = n.ip2_dst_host, id = n.ip2_id ,n.tcp_seq, n.tcp_nxtseq, n.tcp_ack }
                           equals new { ip = i.ip_dst_host, id = i.ip_id, i.tcp_seq, i.tcp_nxtseq, i.tcp_ack }
                           where n.BeginFileNum != null && w.BeginFileNum != null && i.BeginFileNum != null
                           where w.Ack_Syn != null  //gw不丢包
                           where decimal.Parse(w.acksyn_ack == null ? "0" : w.acksyn_ack) - decimal.Parse(w.tcp_seq) == 1
                           group i by i.Syn into ttt
                           select new
                           {
                               ttt.Key,
                               min_time = ttt.Min(e => e.PacketTime),
                               max_time = ttt.Max(e => e.PacketTime),
                               itface = "Gi_neikou",
                               get_cnt = ttt.Count(),
                               reponse_cnt = ttt.Where(e => decimal.Parse(e.acksyn_ack==null?"0":e.acksyn_ack) - decimal.Parse(e.tcp_seq) == 1).Count(),
                               reponse_rate = 1.0 * ttt.Where(e => decimal.Parse(e.acksyn_ack==null?"0":e.acksyn_ack) - decimal.Parse(e.tcp_seq) == 1).Count() / ttt.Count(),
                               reponse_delay = ttt.Where(e => decimal.Parse(e.acksyn_ack==null?"0":e.acksyn_ack) - decimal.Parse(e.tcp_seq) == 1).Average(e => e.Ack_Syn_delayFirst),

                           };

                var dborder = gngi.OrderByDescending(e => e.get_cnt);
                return dborder.ToDataTable();
            }
            else
            {
                var gngi = from n in gz_gn.GnGiGw_Syn.FromCache()
                           join w in gz_giw.GnGiGw_Syn.FromCache()
                           on new { ip = n.ip2_dst_host, id = n.ip2_id, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack }
                           equals new { ip = w.ip2_dst_host, id = w.ip2_id, w.tcp_seq, w.tcp_nxtseq, w.tcp_ack }
                           join i in gz_gi.GnGiGw_Syn.FromCache()
                           on new { ip = n.ip2_dst_host, id = n.ip2_id, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack }
                           equals new { ip = i.ip2_dst_host, id = i.ip2_id, i.tcp_seq, i.tcp_nxtseq, i.tcp_ack }
                           where n.BeginFileNum != null && w.BeginFileNum != null && i.BeginFileNum != null
                           where w.Ack_Syn != null  //gw不丢包
                           where decimal.Parse(w.acksyn_ack == null ? "0" : w.acksyn_ack) - decimal.Parse(w.tcp_seq) == 1
                           group i by i.Syn into ttt
                           select new
                           {
                               ttt.Key,
                               min_time = ttt.Min(e => e.PacketTime),
                               max_time = ttt.Max(e => e.PacketTime),
                               itface = "Gi_neikou",
                               get_cnt = ttt.Count(),
                               reponse_cnt = ttt.Where(e => decimal.Parse(e.acksyn_ack==null?"0":e.acksyn_ack) - decimal.Parse(e.tcp_seq) == 1).Count(),
                               reponse_rate = 1.0 * ttt.Where(e => e.Ack_Syn!= null).Count() / ttt.Count(),
                               reponse_delay = ttt.Where(e => decimal.Parse(e.acksyn_ack==null?"0":e.acksyn_ack) - decimal.Parse(e.tcp_seq) == 1).Average(e => e.Ack_Syn_delayFirst),

                           };

                var dborder = gngi.OrderByDescending(e => e.get_cnt);
                return dborder.ToDataTable();
            }
        }

        //gi内口成功率的计算
        //在Gi外口中过滤不成功的？
        private DataTable viewKeyKPI_giw_from_giw_FromCache(string tcp)
        {


            if (tcp == "TCP")
            {
                var gngi = from n in gz_gn.GnGiGw_Syn.FromCache()
                           join w in gz_giw.GnGiGw_Syn.FromCache()
                           on new { ip = n.ip2_dst_host, id = n.ip2_id, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack }
                           equals new { ip = w.ip_dst_host, id = w.ip_id, w.tcp_seq, w.tcp_nxtseq, w.tcp_ack }
                           join i in gz_gi.GnGiGw_Syn.FromCache()
                           on new { ip = n.ip2_dst_host, id = n.ip2_id, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack }
                           equals new { ip = i.ip_dst_host, id = i.ip_id, i.tcp_seq, i.tcp_nxtseq, i.tcp_ack }
                           where n.BeginFileNum != null && w.BeginFileNum != null && i.BeginFileNum != null
                           where w.Ack_Syn != null  //gw不丢包
                           where decimal.Parse(w.acksyn_ack == null ? "0" : w.acksyn_ack) - decimal.Parse(w.tcp_seq) == 1
                           group w by w.Syn into ttt
                           select new
                           {
                               ttt.Key,
                               min_time = ttt.Min(e => e.PacketTime),
                               max_time = ttt.Max(e => e.PacketTime),
                               itface = "Gi_waikou",
                               get_cnt = ttt.Count(),
                               reponse_cnt = ttt.Where(e => decimal.Parse(e.acksyn_ack==null?"0":e.acksyn_ack) - decimal.Parse(e.tcp_seq) == 1).Count(),
                               reponse_rate = 1.0 * ttt.Where(e => decimal.Parse(e.acksyn_ack==null?"0":e.acksyn_ack) - decimal.Parse(e.tcp_seq) == 1).Count() / ttt.Count(),
                               reponse_delay = ttt.Where(e => decimal.Parse(e.acksyn_ack==null?"0":e.acksyn_ack) - decimal.Parse(e.tcp_seq) == 1).Average(e => e.Ack_Syn_delayFirst),

                           };

                var dborder = gngi.OrderByDescending(e => e.get_cnt);
                return dborder.ToDataTable();
            }
            else
            {
                var gngi = from n in gz_gn.GnGiGw_Syn.FromCache()
                           join w in gz_giw.GnGiGw_Syn.FromCache()
                           on new { ip = n.ip2_dst_host, id = n.ip2_id, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack }
                           equals new { ip = w.ip2_dst_host, id = w.ip2_id, w.tcp_seq, w.tcp_nxtseq, w.tcp_ack }
                           join i in gz_gi.GnGiGw_Syn.FromCache()
                           on new { ip = n.ip2_dst_host, id = n.ip2_id, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack }
                           equals new { ip = i.ip2_dst_host, id = i.ip2_id, i.tcp_seq, i.tcp_nxtseq, i.tcp_ack }
                           where n.BeginFileNum != null && w.BeginFileNum != null && i.BeginFileNum != null
                           where w.Ack_Syn != null  //gw不丢包
                           where decimal.Parse(w.acksyn_ack == null ? "0" : w.acksyn_ack) - decimal.Parse(w.tcp_seq) == 1
                           group w by w.Syn into ttt
                           select new
                           {
                               ttt.Key,
                               min_time = ttt.Min(e => e.PacketTime),
                               max_time = ttt.Max(e => e.PacketTime),
                               itface = "Gi_waikou",
                               get_cnt = ttt.Count(),
                               reponse_cnt = ttt.Where(e => decimal.Parse(e.acksyn_ack==null?"0":e.acksyn_ack) - decimal.Parse(e.tcp_seq) == 1).Count(),
                               reponse_rate = 1.0 * ttt.Where(e => decimal.Parse(e.acksyn_ack==null?"0":e.acksyn_ack) - decimal.Parse(e.tcp_seq) == 1).Count() / ttt.Count(),
                               reponse_delay = ttt.Where(e => decimal.Parse(e.acksyn_ack==null?"0":e.acksyn_ack) - decimal.Parse(e.tcp_seq) == 1).Average(e => e.Ack_Syn_delayFirst),

                           };

                var dborder = gngi.OrderByDescending(e => e.get_cnt);
                return dborder.ToDataTable();
            }
        }

        //gn成功率的计算
        //在Gi外口中过滤不成功的？
        private DataTable viewKeyKPI_gn_from_giw_FromCache(string tcp)
        {

            if (tcp == "TCP")
            {
                var gngi = from n in gz_gn.GnGiGw_Syn.FromCache()
                           join w in gz_giw.GnGiGw_Syn.FromCache()
                           on new { ip = n.ip2_dst_host, id = n.ip2_id, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack }
                           equals new { ip = w.ip_dst_host, id = w.ip_id, w.tcp_seq, w.tcp_nxtseq, w.tcp_ack }
                           join i in gz_gi.GnGiGw_Syn.FromCache()
                           on new { ip = n.ip2_dst_host, id = n.ip2_id, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack }
                           equals new { ip = i.ip_dst_host, id = i.ip_id, i.tcp_seq, i.tcp_nxtseq, i.tcp_ack }
                           where n.BeginFileNum != null && w.BeginFileNum != null && i.BeginFileNum != null
                           where w.Ack_Syn != null  //gw不丢包
                           where decimal.Parse(w.acksyn_ack == null ? "0" : w.acksyn_ack) - decimal.Parse(w.tcp_seq) == 1
                           group n by n.Syn into ttt
                           select new
                           {
                               ttt.Key,
                               min_time = ttt.Min(e => e.PacketTime),
                               max_time = ttt.Max(e => e.PacketTime),
                               itface = "Gn",
                               get_cnt = ttt.Count(),
                               reponse_cnt = ttt.Where(e => decimal.Parse(e.acksyn_ack==null?"0":e.acksyn_ack) - decimal.Parse(e.tcp_seq) == 1).Count(),
                               reponse_rate = 1.0 * ttt.Where(e => decimal.Parse(e.acksyn_ack==null?"0":e.acksyn_ack) - decimal.Parse(e.tcp_seq) == 1).Count() / ttt.Count(),
                               reponse_delay = ttt.Where(e => decimal.Parse(e.acksyn_ack==null?"0":e.acksyn_ack) - decimal.Parse(e.tcp_seq) == 1).Average(e => e.Ack_Syn_delayFirst),

                           };

                var dborder = gngi.OrderByDescending(e => e.get_cnt);
                return dborder.ToDataTable();
            }
            else
            {
                var gngi = from n in gz_gn.GnGiGw_Syn.FromCache()
                           join w in gz_giw.GnGiGw_Syn.FromCache()
                           on new { ip = n.ip2_dst_host, id = n.ip2_id, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack }
                           equals new { ip = w.ip2_dst_host, id = w.ip2_id, w.tcp_seq, w.tcp_nxtseq, w.tcp_ack }
                           join i in gz_gi.GnGiGw_Syn.FromCache()
                           on new { ip = n.ip2_dst_host, id = n.ip2_id, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack }
                           equals new { ip = i.ip2_dst_host, id = i.ip2_id, i.tcp_seq, i.tcp_nxtseq, i.tcp_ack }
                           where n.BeginFileNum != null && w.BeginFileNum != null && i.BeginFileNum != null
                           where w.Ack_Syn != null  //gw不丢包
                           where decimal.Parse(w.acksyn_ack==null?"0":w.acksyn_ack) - decimal.Parse(w.tcp_seq) == 1
                           group n by n.Syn into ttt
                           select new
                           {
                               ttt.Key,
                               min_time = ttt.Min(e => e.PacketTime),
                               max_time = ttt.Max(e => e.PacketTime),
                               itface = "Gn",
                               get_cnt = ttt.Count(),
                               reponse_cnt = ttt.Where(e => decimal.Parse(e.acksyn_ack==null?"0":e.acksyn_ack) - decimal.Parse(e.tcp_seq) == 1).Count(),
                               reponse_rate = 1.0 * ttt.Where(e => decimal.Parse(e.acksyn_ack==null?"0":e.acksyn_ack) - decimal.Parse(e.tcp_seq) == 1).Count() / ttt.Count(),
                               reponse_delay = ttt.Where(e => decimal.Parse(e.acksyn_ack==null?"0":e.acksyn_ack) - decimal.Parse(e.tcp_seq) == 1).Average(e => e.Ack_Syn_delayFirst),

                           };

                var dborder = gngi.OrderByDescending(e => e.get_cnt);
                return dborder.ToDataTable();
            }
        }

        //在Gn中出现丢包的例子
        private DataTable viewLost_gn_from_giw_FromCache(string tcp)
        {
    

            if (tcp == "TCP")
            {
                var gngi = from n in gz_gn.GnGiGw_Syn.FromCache()
                           join w in gz_giw.GnGiGw_Syn.FromCache()
                           on new { ip = n.ip2_dst_host, id = n.ip2_id, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack }
                           equals new { ip = w.ip_dst_host, id = w.ip_id, w.tcp_seq, w.tcp_nxtseq, w.tcp_ack }
                           join i in gz_gi.GnGiGw_Syn.FromCache()
                           on new { ip = n.ip2_dst_host, id = n.ip2_id, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack }
                           equals new { ip = i.ip_dst_host, id = i.ip_id, i.tcp_seq, i.tcp_nxtseq, i.tcp_ack }
                           where n.BeginFileNum != null && w.BeginFileNum != null && i.BeginFileNum != null
                           where n.Ack_Syn == null  //gn丢包
                           where i.Ack_Syn != null  //gi不丢包
                           where decimal.Parse(i.acksyn_ack == null ? "0" : i.acksyn_ack) - decimal.Parse(i.tcp_seq) == 1
                           where w.Ack_Syn != null  //gw不丢包
                           where decimal.Parse(w.acksyn_ack == null ? "0" : w.acksyn_ack) - decimal.Parse(w.tcp_seq) == 1
                           select new
                           {
                               n_BeginFileNum = n.BeginFileNum ,
                               n_BeginFrameNum = n.BeginFrameNum,
                               n_PacketNum = n.PacketNum,
                               n_FileNum =  n.FileNum,
                               i_BeginFileNum = i.BeginFileNum,
                               i_BeginFrameNum = i.BeginFrameNum,
                               i_PacketNum = i.PacketNum,
                               i_FileNum = i.FileNum,
                               w_BeginFileNum =  w.BeginFileNum,
                               w_BeginFrameNum = w.BeginFrameNum,
                               w_PacketNum = w.PacketNum,
                               w_FileNum =  w.FileNum,

                           };

                var dborder = gngi.OrderBy(e=>e.n_PacketNum).Take(100).ToList();
                return dborder.ToDataTable();
            }
            else
            {
                var gngi = from n in gz_gn.GnGiGw_Syn.FromCache()
                           join w in gz_giw.GnGiGw_Syn.FromCache()
                           on new { ip = n.ip2_dst_host, id = n.ip2_id, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack }
                           equals new { ip = w.ip2_dst_host, id = w.ip2_id, w.tcp_seq, w.tcp_nxtseq, w.tcp_ack }
                           join i in gz_gi.GnGiGw_Syn.FromCache()
                           on new { ip = n.ip2_dst_host, id = n.ip2_id, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack }
                           equals new { ip = i.ip2_dst_host, id = i.ip2_id, i.tcp_seq, i.tcp_nxtseq, i.tcp_ack }
                           where n.BeginFileNum != null && w.BeginFileNum != null && i.BeginFileNum != null
                           where n.Ack_Syn == null  //gn丢包
                           where i.Ack_Syn != null  //gi不丢包
                           where decimal.Parse(i.acksyn_ack == null ? "0" : i.acksyn_ack) - decimal.Parse(i.tcp_seq) == 1
                           where w.Ack_Syn != null  //gw不丢包
                           where decimal.Parse(w.acksyn_ack == null ? "0" : w.acksyn_ack) - decimal.Parse(w.tcp_seq) == 1
                           select new
                           {
                               n_BeginFileNum = n.BeginFileNum ,
                               n_BeginFrameNum = n.BeginFrameNum,
                               n_PacketNum = n.PacketNum,
                               n_FileNum =  n.FileNum,
                               i_BeginFileNum = i.BeginFileNum,
                               i_BeginFrameNum = i.BeginFrameNum,
                               i_PacketNum = i.PacketNum,
                               i_FileNum = i.FileNum,
                               w_BeginFileNum =  w.BeginFileNum,
                               w_BeginFrameNum = w.BeginFrameNum,
                               w_PacketNum = w.PacketNum,
                               w_FileNum =  w.FileNum,

                           };

                var dborder = gngi.OrderBy(e=>e.n_PacketNum).Take(100).ToList();
                return dborder.ToDataTable();
            }

        }

        //在Gi内口中出现丢包的例子
        private DataTable viewLost_gi_from_giw_FromCache(string tcp)
        {


            if (tcp == "TCP")
            {
                var gngi = from n in gz_gn.GnGiGw_Syn.FromCache()
                           join w in gz_giw.GnGiGw_Syn.FromCache()
                           on new { ip = n.ip2_dst_host, id = n.ip2_id, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack }
                           equals new { ip = w.ip_dst_host, id = w.ip_id, w.tcp_seq, w.tcp_nxtseq, w.tcp_ack }
                           join i in gz_gi.GnGiGw_Syn.FromCache()
                           on new { ip = n.ip2_dst_host, id = n.ip2_id, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack }
                           equals new { ip = i.ip_dst_host, id = i.ip_id, i.tcp_seq, i.tcp_nxtseq, i.tcp_ack }
                           where n.BeginFileNum != null && w.BeginFileNum != null && i.BeginFileNum != null
                           where n.Ack_Syn == null  //gn丢包
                           where i.Ack_Syn == null  //gi丢包
                           where w.Ack_Syn != null  //gw不丢包
                           where decimal.Parse(w.acksyn_ack == null ? "0" : w.acksyn_ack) - decimal.Parse(w.tcp_seq) == 1
                           select new
                           {
                               n_BeginFileNum = n.BeginFileNum ,
                               n_BeginFrameNum = n.BeginFrameNum,
                               n_PacketNum = n.PacketNum,
                               n_FileNum =  n.FileNum,
                               i_BeginFileNum = i.BeginFileNum,
                               i_BeginFrameNum = i.BeginFrameNum,
                               i_PacketNum = i.PacketNum,
                               i_FileNum = i.FileNum,
                               w_BeginFileNum =  w.BeginFileNum,
                               w_BeginFrameNum = w.BeginFrameNum,
                               w_PacketNum = w.PacketNum,
                               w_FileNum =  w.FileNum,

                           };

                var dborder = gngi.OrderBy(e=>e.n_PacketNum).Take(100).ToList();
                return dborder.ToDataTable();
            }
            else
            {
                var gngi = from n in gz_gn.GnGiGw_Syn.FromCache()
                           join w in gz_giw.GnGiGw_Syn.FromCache()
                           on new { ip = n.ip2_dst_host, id = n.ip2_id, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack }
                           equals new { ip = w.ip2_dst_host, id = w.ip2_id, w.tcp_seq, w.tcp_nxtseq, w.tcp_ack }
                           join i in gz_gi.GnGiGw_Syn.FromCache()
                           on new { ip = n.ip2_dst_host, id = n.ip2_id, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack }
                           equals new { ip = i.ip2_dst_host, id = i.ip2_id, i.tcp_seq, i.tcp_nxtseq, i.tcp_ack }
                           where n.BeginFileNum != null && w.BeginFileNum != null && i.BeginFileNum != null
                           where n.Ack_Syn == null  //gn丢包
                           where i.Ack_Syn == null  //gi丢包
                           where w.Ack_Syn != null  //gw不丢包
                           where decimal.Parse(w.acksyn_ack == null ? "0" : w.acksyn_ack) - decimal.Parse(w.tcp_seq) == 1
                           select new
                           {
                               n_BeginFileNum = n.BeginFileNum ,
                               n_BeginFrameNum = n.BeginFrameNum,
                               n_PacketNum = n.PacketNum,
                               n_FileNum =  n.FileNum,
                               i_BeginFileNum = i.BeginFileNum,
                               i_BeginFrameNum = i.BeginFrameNum,
                               i_PacketNum = i.PacketNum,
                               i_FileNum = i.FileNum,
                               w_BeginFileNum =  w.BeginFileNum,
                               w_BeginFrameNum = w.BeginFrameNum,
                               w_PacketNum = w.PacketNum,
                               w_FileNum =  w.FileNum,

                           };

                var dborder = gngi.OrderBy(e=>e.n_PacketNum).Take(100).ToList();
                return dborder.ToDataTable();
            }

        }

        //在Gi内口出现重复发包的例子

        //计算方法有待进一步研究

        private DataTable viewAll_gigngw_from_giw_FromCache()
        {


            var gngi = from n in gz_gn.GnGiGw_Syn.FromCache()
                       join w in gz_giw.GnGiGw_Syn.FromCache()
                       on new { ip = n.ip2_dst_host, id = n.ip2_id, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack }
                       equals new { ip = w.ip2_dst_host, id = w.ip2_id, w.tcp_seq, w.tcp_nxtseq, w.tcp_ack }
                       join i in gz_gi.GnGiGw_Syn.FromCache()
                       on new { ip = n.ip2_dst_host, id = n.ip2_id, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack }
                       equals new { ip = i.ip2_dst_host, id = i.ip2_id, i.tcp_seq, i.tcp_nxtseq, i.tcp_ack }
                       where n.BeginFileNum != null && w.BeginFileNum != null && i.BeginFileNum != null
                       where n.Ack_Syn != null  //gn不丢包
                       where decimal.Parse(n.acksyn_ack == null ? "0" : n.acksyn_ack) - decimal.Parse(n.tcp_seq) == 1
                       where i.Ack_Syn != null  //gi不丢包
                       where decimal.Parse(i.acksyn_ack == null ? "0" : i.acksyn_ack) - decimal.Parse(i.tcp_seq) == 1
                       where w.Ack_Syn != null  //gw不丢包
                       where decimal.Parse(w.acksyn_ack == null ? "0" : w.acksyn_ack) - decimal.Parse(w.tcp_seq) == 1
                       select new
                       {
                           n_BeginFileNum = n.BeginFileNum ,
                           n_BeginFrameNum = n.BeginFrameNum,
                           n_PacketNum = n.PacketNum,
                           n_FileNum =  n.FileNum,
                           i_BeginFileNum = i.BeginFileNum,
                           i_BeginFrameNum = i.BeginFrameNum,
                           i_PacketNum = i.PacketNum,
                           i_FileNum = i.FileNum,
                           w_BeginFileNum =  w.BeginFileNum,
                           w_BeginFrameNum = w.BeginFrameNum,
                           w_PacketNum = w.PacketNum,
                           w_FileNum =  w.FileNum,

                       };

            var gngigw = from p in gngi
                         group p by p.n_PacketNum into ttt
                         select new
                         {
                             ttt.Key,
                             cnt = ttt.Count(),
                             i_packetnum = ttt.Select(e => e.i_PacketNum.ToString()).Aggregate((a, b) => a + "," + b),
                             w_packetnum = ttt.Select(e => e.w_PacketNum.ToString()).Aggregate((a, b) => a + "," + b),

                         };

            var dborder = gngigw.OrderByDescending(e => e.cnt).Take(100).ToList();
            return dborder.ToDataTable();
        }


        //上行丢包的计算
        /*
         * 
         * 
SELECT a.*,b.*
  FROM [GuangZhou_Giw].[dbo].[GnGiGw_Syn] as a
  left join (select * from [GuangZhou_Gn].[dbo].[GnGiGw_Syn]) as b
   on   a.tcp_seq=b.tcp_seq  and a.tcp_ack=b.tcp_ack
   and a.ip2_dst_host=b.ip_dst_host and a.ip2_id=b.ip2_id
   and a.tcp_nxtseq=b.tcp_nxtseq
  where b.PacketNum is not null
 */
        private DataTable viewKPI_upLost_FromCache(string tcp)
        {
            DateTime dtmin = DateTime.Parse("2012-6-20 15:00:01").AddSeconds(15);
            DateTime dtmax = DateTime.Parse("2012-6-20 15:02:05").AddSeconds(-15);

            if (tcp == "TCP")
            {
                //提取gi内口和外口都覆盖的gn时间段
                var gngi = from n in gz_gn.GnGiGw_Syn.FromCache().Where(e => e.PacketTime < dtmax && e.PacketTime > dtmin).Where(e => e.Syn_RepeatCounter == 0)
                           join w in gz_giw.GnGiGw_Syn.FromCache().Where(e=>e.Syn_RepeatCounter==0)
                           on new { ip = n.ip2_dst_host, id = n.ip2_id, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack }
                           equals new { ip = w.ip_dst_host, id = w.ip_id, w.tcp_seq, w.tcp_nxtseq, w.tcp_ack }
                           join i in gz_gi.GnGiGw_Syn.FromCache().Where(e => e.Syn_RepeatCounter == 0)
                           on new { ip = n.ip2_dst_host, id = n.ip2_id, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack }
                           equals new { ip = i.ip_dst_host, id = i.ip_id, i.tcp_seq, i.tcp_nxtseq, i.tcp_ack }
                           where n.BeginFileNum != null && w.BeginFileNum != null && i.BeginFileNum != null
                           where n.acksyn_ack == null && w.acksyn_ack == null && i.acksyn_ack == null
                           select new
                           {
                               n=1,
                               n_PacketNum = n.PacketNum,
                               n_PacketTime= n.PacketTime,
             
                               i_PacketNum = i.PacketNum,
                               i_PacketTime= i.PacketTime,
           
                               w_PacketNum = w.PacketNum,
                               w_PacketTime= w.PacketTime,

                           };

                var gn = from p in gngi
                         
                         group p by p.n into ttt
                         select new
                         {
                             ttt.Key,
                             gn_mintime=ttt.Min(e=>e.n_PacketTime),
                             gn_maxtime=ttt.Max(e=>e.n_PacketTime),
                             gn_synnum=ttt.Select(e=>e.n_PacketNum).Distinct().Count(),
                             gi_synnum=ttt.Select(e=>e.i_PacketNum).Distinct().Count(),
                             gw_synnum=ttt.Select(e=>e.w_PacketNum).Distinct().Count(),

                         };

                var dborder = gn;
                return dborder.ToDataTable();
            }
            else
            {
                var gngi = from n in gz_gn.GnGiGw_Syn.FromCache().Where(e => e.PacketTime < dtmax && e.PacketTime > dtmin).Where(e => e.Syn_RepeatCounter == 0)
                           join w in gz_giw.GnGiGw_Syn.FromCache().Where(e => e.Syn_RepeatCounter == 0)
                           on new { ip = n.ip2_dst_host, id = n.ip2_id, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack }
                           equals new { ip = w.ip2_dst_host, id = w.ip2_id, w.tcp_seq, w.tcp_nxtseq, w.tcp_ack }
                           join i in gz_gi.GnGiGw_Syn.FromCache().Where(e => e.Syn_RepeatCounter == 0)
                           on new { ip = n.ip2_dst_host, id = n.ip2_id, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack }
                           equals new { ip = i.ip2_dst_host, id = i.ip2_id, i.tcp_seq, i.tcp_nxtseq, i.tcp_ack }
                           where n.BeginFileNum != null && w.BeginFileNum != null && i.BeginFileNum != null
                           where n.acksyn_ack ==null && w.acksyn_ack ==null && i.acksyn_ack ==null
                           select new
                           {
                               n = 1,
                               n_PacketNum = n.PacketNum,
                               n_PacketTime = n.PacketTime,

                               i_PacketNum = i.PacketNum,
                               i_PacketTime = i.PacketTime,

                               w_PacketNum = w.PacketNum,
                               w_PacketTime = w.PacketTime,

                           };

                var gn = from p in gngi
               
                         group p by p.n into ttt
                         select new
                         {
                             ttt.Key,
                             gn_mintime = ttt.Min(e => e.n_PacketTime),
                             gn_maxtime = ttt.Max(e => e.n_PacketTime),
                             gn_synnum = ttt.Select(e => e.n_PacketNum).Distinct().Count(),
                             gi_synnum = ttt.Select(e => e.i_PacketNum).Distinct().Count(),
                             gw_synnum = ttt.Select(e => e.w_PacketNum).Distinct().Count(),

                         };

                var dborder = gn;
                return dborder.ToDataTable();
            }
        }


    }
}
