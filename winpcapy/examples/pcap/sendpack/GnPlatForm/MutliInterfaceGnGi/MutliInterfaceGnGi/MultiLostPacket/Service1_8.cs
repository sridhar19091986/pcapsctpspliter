﻿using System;
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


namespace MutliInterfaceGnGi
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    //public partial class Service1 : IService1
         public  class Service2
    {
        GuangZhou_GiEntities1 gz_gi = new GuangZhou_GiEntities1();
        GuangZhou_GiwEntities gz_giw = new GuangZhou_GiwEntities();
        GuangZhou_GnEntities gz_gn = new GuangZhou_GnEntities();

        //单接口指标统计
        private DataTable viewKeyKPI_gn_FromCache()
        {
      


            var gngi = from p in gz_gn.GnGiGw_Get2x.FromCache()
                       group p by p.Get2x into ttt
                       select new
                       {
                           ttt.Key,
                           min_time = ttt.Min(e => e.PacketTime),
                           max_time = ttt.Max(e => e.PacketTime),
                           itface = "Gn",
                           get_cnt = ttt.Count(),
                           reponse_cnt = ttt.Where(e => e.Response != null).Count(),
                           reponse_rate = 1.0 * ttt.Where(e => e.Response != null).Count() / ttt.Count(),
                           reponse_delay = ttt.Where(e => e.Response != null).Average(e => e.Response_delayFirst),

                       };

            var dborder = gngi.OrderByDescending(e => e.get_cnt);
            return dborder.ToDataTable();
        }

        //单接口指标统计
        private DataTable viewKeyKPI_gi_FromCache()
        {
  

            var gngi = from p in gz_gi.GnGiGw_Get2x.FromCache()
                       group p by p.Get2x into ttt
                       select new
                       {
                           ttt.Key,
                           min_time = ttt.Min(e => e.PacketTime),
                           max_time = ttt.Max(e => e.PacketTime),
                           itface = "Gi_neikou",

                           get_cnt = ttt.Count(),
                           reponse_cnt = ttt.Where(e => e.Response != null).Count(),
                           reponse_rate = 1.0 * ttt.Where(e => e.Response != null).Count() / ttt.Count(),
                           reponse_delay = ttt.Where(e => e.Response != null).Average(e => e.Response_delayFirst),

                       };

            var dborder = gngi.OrderByDescending(e => e.get_cnt);
            return dborder.ToDataTable();
        }

        //单接口指标统计
        private DataTable viewKeyKPI_giw_FromCache()
        {
            gz_giw.CommandTimeout = 0;
            gz_giw.ContextOptions.LazyLoadingEnabled = true;
            gz_giw.GnGiGw_Get2x.MergeOption = MergeOption.NoTracking;
            gz_giw.GnGiGw_Get2x.Load();
            //gz_giw.RawFileList.Load();

            var gi = from p in gz_giw.GnGiGw_Get2x
                     group p by p.Get2x into ttt
                     select new
                     {
                         ttt.Key,
                         min_time = ttt.Min(e => e.PacketTime),
                         max_time = ttt.Max(e => e.PacketTime),
                         itface = "Gi_waikou",
                         get_cnt = ttt.Count(),
                         reponse_cnt = ttt.Where(e => e.Response != null).Count(),
                         reponse_rate = 1.0 * ttt.Where(e => e.Response != null).Count() / ttt.Count(),
                         reponse_delay = ttt.Where(e => e.Response != null).Average(e => e.Response_delayFirst),

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
                var gngi = from n in gz_gn.GnGiGw_Get2x.FromCache()
                           join w in gz_giw.GnGiGw_Get2x.FromCache() on new { ip = n.ip2_dst_host, id = n.ip2_id, n.http_request_uri, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack, n.http_user_agent }
                           equals new { ip = w.ip_dst_host, id = w.ip_id, w.http_request_uri, w.tcp_seq, w.tcp_nxtseq, w.tcp_ack, w.http_user_agent }
                           join i in gz_gi.GnGiGw_Get2x.FromCache() on new { ip = n.ip2_dst_host, id = n.ip2_id, n.http_request_uri, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack, n.http_user_agent }
                           equals new { ip = i.ip_dst_host, id = i.ip_id, i.http_request_uri, i.tcp_seq, i.tcp_nxtseq, i.tcp_ack, i.http_user_agent }
                           where n.BeginFileNum != null && w.BeginFileNum != null && i.BeginFileNum != null
                           where w.Response != null  //gw不丢包
                           group i by i.Get2x into ttt
                           select new
                           {
                               ttt.Key,
                               min_time = ttt.Min(e => e.PacketTime),
                               max_time = ttt.Max(e => e.PacketTime),
                               itface = "Gi_neikou",
                               get_cnt = ttt.Count(),
                               reponse_cnt = ttt.Where(e => e.Response != null).Count(),
                               reponse_rate = 1.0 * ttt.Where(e => e.Response != null).Count() / ttt.Count(),
                               reponse_delay = ttt.Where(e => e.Response != null).Average(e => e.Response_delayFirst),

                           };

                var dborder = gngi.OrderByDescending(e => e.get_cnt);
                return dborder.ToDataTable();
            }
            else
            {
                var gngi = from n in gz_gn.GnGiGw_Get2x.FromCache()
                           join w in gz_giw.GnGiGw_Get2x.FromCache() on new { ip = n.ip2_dst_host, id = n.ip2_id, n.http_request_uri, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack, n.http_user_agent }
                           equals new { ip = w.ip2_dst_host, id = w.ip2_id, w.http_request_uri, w.tcp_seq, w.tcp_nxtseq, w.tcp_ack, w.http_user_agent }
                           join i in gz_gi.GnGiGw_Get2x.FromCache() on new { ip = n.ip2_dst_host, id = n.ip2_id, n.http_request_uri, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack, n.http_user_agent }
                           equals new { ip = i.ip2_dst_host, id = i.ip2_id, i.http_request_uri, i.tcp_seq, i.tcp_nxtseq, i.tcp_ack, i.http_user_agent }
                           where n.BeginFileNum != null && w.BeginFileNum != null && i.BeginFileNum != null
                           where w.Response != null  //gw不丢包
                           group i by i.Get2x into ttt
                           select new
                           {
                               ttt.Key,
                               min_time = ttt.Min(e => e.PacketTime),
                               max_time = ttt.Max(e => e.PacketTime),
                               itface = "Gi_neikou",
                               get_cnt = ttt.Count(),
                               reponse_cnt = ttt.Where(e => e.Response != null).Count(),
                               reponse_rate = 1.0 * ttt.Where(e => e.Response != null).Count() / ttt.Count(),
                               reponse_delay = ttt.Where(e => e.Response != null).Average(e => e.Response_delayFirst),

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
                var gngi = from n in gz_gn.GnGiGw_Get2x.FromCache()
                           join w in gz_giw.GnGiGw_Get2x.FromCache() on new { ip = n.ip2_dst_host, id = n.ip2_id, n.http_request_uri, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack, n.http_user_agent }
                           equals new { ip = w.ip_dst_host, id = w.ip_id, w.http_request_uri, w.tcp_seq, w.tcp_nxtseq, w.tcp_ack, w.http_user_agent }
                           join i in gz_gi.GnGiGw_Get2x.FromCache() on new { ip = n.ip2_dst_host, id = n.ip2_id, n.http_request_uri, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack, n.http_user_agent }
                           equals new { ip = i.ip_dst_host, id = i.ip_id, i.http_request_uri, i.tcp_seq, i.tcp_nxtseq, i.tcp_ack, i.http_user_agent }
                           where n.BeginFileNum != null && w.BeginFileNum != null && i.BeginFileNum != null
                           where w.Response != null  //gw不丢包
                           group w by w.Get2x into ttt
                           select new
                           {
                               ttt.Key,
                               min_time = ttt.Min(e => e.PacketTime),
                               max_time = ttt.Max(e => e.PacketTime),
                               itface = "Gi_waikou",
                               get_cnt = ttt.Count(),
                               reponse_cnt = ttt.Where(e => e.Response != null).Count(),
                               reponse_rate = 1.0 * ttt.Where(e => e.Response != null).Count() / ttt.Count(),
                               reponse_delay = ttt.Where(e => e.Response != null).Average(e => e.Response_delayFirst),

                           };

                var dborder = gngi.OrderByDescending(e => e.get_cnt);
                return dborder.ToDataTable();
            }
            else
            {
                var gngi = from n in gz_gn.GnGiGw_Get2x.FromCache()
                           join w in gz_giw.GnGiGw_Get2x.FromCache() on new { ip = n.ip2_dst_host, id = n.ip2_id, n.http_request_uri, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack, n.http_user_agent }
                           equals new { ip = w.ip2_dst_host, id = w.ip2_id, w.http_request_uri, w.tcp_seq, w.tcp_nxtseq, w.tcp_ack, w.http_user_agent }
                           join i in gz_gi.GnGiGw_Get2x.FromCache() on new { ip = n.ip2_dst_host, id = n.ip2_id, n.http_request_uri, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack, n.http_user_agent }
                           equals new { ip = i.ip2_dst_host, id = i.ip2_id, i.http_request_uri, i.tcp_seq, i.tcp_nxtseq, i.tcp_ack, i.http_user_agent }
                           where n.BeginFileNum != null && w.BeginFileNum != null && i.BeginFileNum != null
                           where w.Response != null  //gw不丢包
                           group w by w.Get2x into ttt
                           select new
                           {
                               ttt.Key,
                               min_time = ttt.Min(e => e.PacketTime),
                               max_time = ttt.Max(e => e.PacketTime),
                               itface = "Gi_waikou",
                               get_cnt = ttt.Count(),
                               reponse_cnt = ttt.Where(e => e.Response != null).Count(),
                               reponse_rate = 1.0 * ttt.Where(e => e.Response != null).Count() / ttt.Count(),
                               reponse_delay = ttt.Where(e => e.Response != null).Average(e => e.Response_delayFirst),

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
                var gngi = from n in gz_gn.GnGiGw_Get2x.FromCache()
                           join w in gz_giw.GnGiGw_Get2x.FromCache() on new { ip = n.ip2_dst_host, id = n.ip2_id, n.http_request_uri, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack, n.http_user_agent }
                           equals new { ip = w.ip_dst_host, id = w.ip_id, w.http_request_uri, w.tcp_seq, w.tcp_nxtseq, w.tcp_ack, w.http_user_agent }
                           join i in gz_gi.GnGiGw_Get2x.FromCache() on new { ip = n.ip2_dst_host, id = n.ip2_id, n.http_request_uri, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack, n.http_user_agent }
                           equals new { ip = i.ip_dst_host, id = i.ip_id, i.http_request_uri, i.tcp_seq, i.tcp_nxtseq, i.tcp_ack, i.http_user_agent }
                           where n.BeginFileNum != null && w.BeginFileNum != null && i.BeginFileNum != null
                           where w.Response != null  //gw不丢包
                           group n by n.Get2x into ttt
                           select new
                           {
                               ttt.Key,
                               min_time = ttt.Min(e => e.PacketTime),
                               max_time = ttt.Max(e => e.PacketTime),
                               itface = "Gn",
                               get_cnt = ttt.Count(),
                               reponse_cnt = ttt.Where(e => e.Response != null).Count(),
                               reponse_rate = 1.0 * ttt.Where(e => e.Response != null).Count() / ttt.Count(),
                               reponse_delay = ttt.Where(e => e.Response != null).Average(e => e.Response_delayFirst),

                           };

                var dborder = gngi.OrderByDescending(e => e.get_cnt);
                return dborder.ToDataTable();
            }
            else
            {
                var gngi = from n in gz_gn.GnGiGw_Get2x.FromCache()
                           join w in gz_giw.GnGiGw_Get2x.FromCache() on new { ip = n.ip2_dst_host, id = n.ip2_id, n.http_request_uri, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack, n.http_user_agent }
                           equals new { ip = w.ip2_dst_host, id = w.ip2_id, w.http_request_uri, w.tcp_seq, w.tcp_nxtseq, w.tcp_ack, w.http_user_agent }
                           join i in gz_gi.GnGiGw_Get2x.FromCache() on new { ip = n.ip2_dst_host, id = n.ip2_id, n.http_request_uri, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack, n.http_user_agent }
                           equals new { ip = i.ip2_dst_host, id = i.ip2_id, i.http_request_uri, i.tcp_seq, i.tcp_nxtseq, i.tcp_ack, i.http_user_agent }
                           where n.BeginFileNum != null && w.BeginFileNum != null && i.BeginFileNum != null
                           where w.Response != null  //gw不丢包
                           group n by n.Get2x into ttt
                           select new
                           {
                               ttt.Key,
                               min_time = ttt.Min(e => e.PacketTime),
                               max_time = ttt.Max(e => e.PacketTime),
                               itface = "Gn",
                               get_cnt = ttt.Count(),
                               reponse_cnt = ttt.Where(e => e.Response != null).Count(),
                               reponse_rate = 1.0 * ttt.Where(e => e.Response != null).Count() / ttt.Count(),
                               reponse_delay = ttt.Where(e => e.Response != null).Average(e => e.Response_delayFirst),

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
                var gngi = from n in gz_gn.GnGiGw_Get2x.FromCache()
                           join w in gz_giw.GnGiGw_Get2x.FromCache() on new { ip = n.ip2_dst_host, id = n.ip2_id, n.http_request_uri, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack, n.http_user_agent }
                           equals new { ip = w.ip_dst_host, id = w.ip_id, w.http_request_uri, w.tcp_seq, w.tcp_nxtseq, w.tcp_ack, w.http_user_agent }
                           join i in gz_gi.GnGiGw_Get2x.FromCache() on new { ip = n.ip2_dst_host, id = n.ip2_id, n.http_request_uri, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack, n.http_user_agent }
                           equals new { ip = i.ip_dst_host, id = i.ip_id, i.http_request_uri, i.tcp_seq, i.tcp_nxtseq, i.tcp_ack, i.http_user_agent }
                           where n.BeginFileNum != null && w.BeginFileNum != null && i.BeginFileNum != null
                           where n.Response == null  //gn丢包
                           where i.Response != null  //gi不丢包
                           where w.Response != null  //gw不丢包
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

                var dborder = gngi.Take(100).ToList();
                return dborder.ToDataTable();
            }
            else
            {
                var gngi = from n in gz_gn.GnGiGw_Get2x.FromCache()
                           join w in gz_giw.GnGiGw_Get2x.FromCache() on new { ip = n.ip2_dst_host, id = n.ip2_id, n.http_request_uri, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack, n.http_user_agent }
                           equals new { ip = w.ip2_dst_host, id = w.ip2_id, w.http_request_uri, w.tcp_seq, w.tcp_nxtseq, w.tcp_ack, w.http_user_agent }
                           join i in gz_gi.GnGiGw_Get2x.FromCache() on new { ip = n.ip2_dst_host, id = n.ip2_id, n.http_request_uri, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack, n.http_user_agent }
                           equals new { ip = i.ip2_dst_host, id = i.ip2_id, i.http_request_uri, i.tcp_seq, i.tcp_nxtseq, i.tcp_ack, i.http_user_agent }
                           where n.BeginFileNum != null && w.BeginFileNum != null && i.BeginFileNum != null
                           where n.Response == null  //gn丢包
                           where i.Response != null  //gi不丢包
                           where w.Response != null  //gw不丢包
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

                var dborder = gngi.Take(100).ToList();
                return dborder.ToDataTable();
            }

        }

        //在Gi内口中出现丢包的例子
        private DataTable viewLost_gi_from_giw_FromCache(string tcp)
        {
      


            if (tcp == "TCP")
            {
                var gngi = from n in gz_gn.GnGiGw_Get2x.FromCache()
                           join w in gz_giw.GnGiGw_Get2x.FromCache() on new { ip = n.ip2_dst_host, id = n.ip2_id, n.http_request_uri, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack, n.http_user_agent }
                           equals new { ip = w.ip_dst_host, id = w.ip_id, w.http_request_uri, w.tcp_seq, w.tcp_nxtseq, w.tcp_ack, w.http_user_agent }
                           join i in gz_gi.GnGiGw_Get2x.FromCache() on new { ip = n.ip2_dst_host, id = n.ip2_id, n.http_request_uri, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack, n.http_user_agent }
                           equals new { ip = i.ip_dst_host, id = i.ip_id, i.http_request_uri, i.tcp_seq, i.tcp_nxtseq, i.tcp_ack, i.http_user_agent }
                           where n.BeginFileNum != null && w.BeginFileNum != null && i.BeginFileNum != null
                           where n.Response == null  //gn丢包
                           where i.Response == null  //gi丢包
                           where w.Response != null  //gw不丢包
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

                var dborder = gngi.Take(100).ToList();
                return dborder.ToDataTable();
            }
            else
            {
                var gngi = from n in gz_gn.GnGiGw_Get2x.FromCache()
                           join w in gz_giw.GnGiGw_Get2x.FromCache() on new { ip = n.ip2_dst_host, id = n.ip2_id, n.http_request_uri, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack, n.http_user_agent }
                           equals new { ip = w.ip2_dst_host, id = w.ip2_id, w.http_request_uri, w.tcp_seq, w.tcp_nxtseq, w.tcp_ack, w.http_user_agent }
                           join i in gz_gi.GnGiGw_Get2x.FromCache() on new { ip = n.ip2_dst_host, id = n.ip2_id, n.http_request_uri, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack, n.http_user_agent }
                           equals new { ip = i.ip2_dst_host, id = i.ip2_id, i.http_request_uri, i.tcp_seq, i.tcp_nxtseq, i.tcp_ack, i.http_user_agent }
                           where n.BeginFileNum != null && w.BeginFileNum != null && i.BeginFileNum != null
                           where n.Response == null  //gn丢包
                           where i.Response == null  //gi丢包
                           where w.Response != null  //gw不丢包
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

                var dborder = gngi.Take(100).ToList();
                return dborder.ToDataTable();
            }

        }

        //在Gi内口出现重复发包的例子

        //计算方法有待进一步研究

        private DataTable viewAll_gigngw_from_giw_FromCache()
        {
      


            var gngi = from n in gz_gn.GnGiGw_Get2x.FromCache()
                       join w in gz_giw.GnGiGw_Get2x.FromCache() on new { ip = n.ip2_dst_host, id = n.ip2_id, n.http_request_uri, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack, n.http_user_agent }
                       equals new { ip = w.ip2_dst_host, id = w.ip2_id, w.http_request_uri, w.tcp_seq, w.tcp_nxtseq, w.tcp_ack, w.http_user_agent }
                       join i in gz_gi.GnGiGw_Get2x.FromCache() on new { ip = n.ip2_dst_host, id = n.ip2_id, n.http_request_uri, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack, n.http_user_agent }
                       equals new { ip = i.ip2_dst_host, id = i.ip2_id, i.http_request_uri, i.tcp_seq, i.tcp_nxtseq, i.tcp_ack, i.http_user_agent }
                       where n.BeginFileNum != null && w.BeginFileNum != null && i.BeginFileNum != null
                       where n.Response != null  //gn不丢包
                       where i.Response != null  //gi不丢包
                       where w.Response != null  //gw不丢包
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

    }
}
