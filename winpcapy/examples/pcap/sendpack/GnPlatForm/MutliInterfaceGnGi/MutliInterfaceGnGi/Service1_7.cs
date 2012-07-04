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
using MutliInterface_Gi;
using MutliInterface_Giw;
using MutliInterface_Gn;

//using System.Data.DataSetExtensions;


namespace MutliInterfaceGnGi
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public partial class Service1 : IService1
    {
        GuangZhou_GiEntities1 gz_gi = new GuangZhou_GiEntities1();
        GuangZhou_GiwEntities gz_giw = new GuangZhou_GiwEntities();
        GuangZhou_GnEntities gz_gn = new GuangZhou_GnEntities();

        private DataTable viewKeyKPI_gn_FromCache()
        {
            gz_gn.CommandTimeout = 0;
            gz_gn.ContextOptions.LazyLoadingEnabled = true;
            gz_gn.GnGiGw_Get2x.MergeOption = MergeOption.NoTracking;
            gz_gn.GnGiGw_Get2x.Load();

            var gngi = from p in gz_gn.GnGiGw_Get2x.FromCache()
                       group p by p.Get2x into ttt
                       select new
                       {
                           ttt.Key,
                           itface = "Gn",
                           get_cnt = ttt.Count(),
                           reponse_cnt = ttt.Where(e => e.Response != null).Count(),
                           reponse_rate = 1.0 * ttt.Where(e => e.Response != null).Count() / ttt.Count(),
                           reponse_delay = ttt.Where(e => e.Response != null).Average(e => e.Response_delayFirst),

                       };

            var dborder = gngi.OrderByDescending(e => e.get_cnt);
            return dborder.ToDataTable();
        }

        private DataTable viewKeyKPI_gi_FromCache()
        {
            gz_gi.CommandTimeout = 0;
            gz_gi.ContextOptions.LazyLoadingEnabled = true;
            gz_gi.GnGiGw_Get2x.MergeOption = MergeOption.NoTracking;
            gz_gi.GnGiGw_Get2x.Load();

            var gngi = from p in gz_gi.GnGiGw_Get2x.FromCache()
                       group p by p.Get2x into ttt
                       select new
                       {
                           ttt.Key,
                           itface = "Gin",
                           get_cnt = ttt.Count(),
                           reponse_cnt = ttt.Where(e => e.Response != null).Count(),
                           reponse_rate = 1.0 * ttt.Where(e => e.Response != null).Count() / ttt.Count(),
                           reponse_delay = ttt.Where(e => e.Response != null).Average(e => e.Response_delayFirst),

                       };

            var dborder = gngi.OrderByDescending(e => e.get_cnt);
            return dborder.ToDataTable();
        }
        private DataTable viewKeyKPI_giw_FromCache()
        {
            gz_giw.CommandTimeout = 0;
            gz_giw.ContextOptions.LazyLoadingEnabled = true;
            gz_giw.GnGiGw_Get2x.MergeOption = MergeOption.NoTracking;
            gz_giw.GnGiGw_Get2x.Load();

            var gi = from p in gz_giw.GnGiGw_Get2x
                     group p by p.Get2x into ttt
                     select new
                     {
                         ttt.Key,
                         itface = "Giw",
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
        private DataTable viewKeyKPI_gi_from_giw_FromCache()
        {
            gz_gi.CommandTimeout = 0;
            gz_gi.ContextOptions.LazyLoadingEnabled = true;
            gz_gi.GnGiGw_Get2x.MergeOption = MergeOption.NoTracking;
            gz_gi.GnGiGw_Get2x.Load();

            var giwk = gz_giw.GnGiGw_Get2x.Where(e => e.Response != null)
        .ToLookup(e => new { e.http_request_uri, e.tcp_seq, e.tcp_nxtseq, e.tcp_ack }, e => e.FileNum);

            var gngi = from p in gz_gi.GnGiGw_Get2x.FromCache()
                       where giwk.Contains(new { p.http_request_uri, p.tcp_seq, p.tcp_nxtseq, p.tcp_ack })
                       group p by p.Get2x into ttt
                       select new
                       {
                           ttt.Key,
                           itface = "Gi_neikou",
                           get_cnt = ttt.Count(),
                           reponse_cnt = ttt.Where(e => e.Response != null).Count(),
                           reponse_rate = 1.0 * ttt.Where(e => e.Response != null).Count() / ttt.Count(),
                           reponse_delay = ttt.Where(e => e.Response != null).Average(e => e.Response_delayFirst),

                       };

            var dborder = gngi.OrderByDescending(e => e.get_cnt);
            return dborder.ToDataTable();
        }

        //gn成功率的计算
        //在Gi外口中过滤不成功的？
        private DataTable viewKeyKPI_gn_from_giw_FromCache()
        {
            gz_gn.CommandTimeout = 0;
            gz_gn.ContextOptions.LazyLoadingEnabled = true;
            gz_gn.GnGiGw_Get2x.MergeOption = MergeOption.NoTracking;
            gz_gn.GnGiGw_Get2x.Load();

            var giwk = gz_giw.GnGiGw_Get2x.Where(e => e.Response != null)
                .ToLookup(e => new { e.http_request_uri, e.tcp_seq, e.tcp_nxtseq, e.tcp_ack }, e => e.FileNum);

            var gngi = from p in gz_gn.GnGiGw_Get2x.FromCache()
                       where giwk.Contains(new { p.http_request_uri, p.tcp_seq, p.tcp_nxtseq, p.tcp_ack })
                       group p by p.Get2x into ttt
                       select new
                       {
                           ttt.Key,
                           itface = "Gn",
                           get_cnt = ttt.Count(),
                           reponse_cnt = ttt.Where(e => e.Response != null).Count(),
                           reponse_rate = 1.0 * ttt.Where(e => e.Response != null).Count() / ttt.Count(),
                           reponse_delay = ttt.Where(e => e.Response != null).Average(e => e.Response_delayFirst),

                       };

            var dborder = gngi.OrderByDescending(e => e.get_cnt);
            return dborder.ToDataTable();
        }

        //在Gn中出现丢包的例子
        private DataTable viewLost_gn_from_giw_FromCache()
        {
            var gngi = from n in gz_gn.GnGiGw_Get2x
                       join w in gz_giw.GnGiGw_Get2x on new { n.http_request_uri, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack }
                       equals new { w.http_request_uri, w.tcp_seq, w.tcp_nxtseq, w.tcp_ack }
                       join i in gz_gi.GnGiGw_Get2x on new { n.http_request_uri, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack }
                       equals new { i.http_request_uri, i.tcp_seq, i.tcp_nxtseq, i.tcp_ack }
                       where n.Response == null  //gn丢包
                       where i.Response != null  //gi不丢包
                       where w.Response != null  //gw不丢包
                       select new
                       {
                           n_BeginFileNum = gz_gn.RawFileList.Where(e=>e.FileNum==n.BeginFileNum).First(),
                           n_BeginFrameNum = n.BeginFrameNum,
                           n_PacketNum = n.PacketNum,
                           n_FileNum = gz_gn.RawFileList.Where(e=>e.FileNum==n.FileNum).First(),
                           i_BeginFileNum =  gz_gi.RawFileList.Where(e=>e.FileNum==i.BeginFileNum).First(),
                           i_BeginFrameNum = i.BeginFrameNum,
                           i_PacketNum = i.PacketNum,
                           i_FileNum = gz_gi.RawFileList.Where(e=>e.FileNum==i.FileNum).First(),
                           w_BeginFileNum =  gz_giw.RawFileList.Where(e=>e.FileNum==w.BeginFileNum).First(),
                           w_BeginFrameNum = w.BeginFrameNum,
                           w_PacketNum = w.PacketNum,
                           w_FileNum = gz_giw.RawFileList.Where(e=>e.FileNum==w.FileNum).First(),

                       };

            var dborder = gngi.OrderBy(e => e.n_BeginFileNum).ThenBy(e => e.n_BeginFrameNum);
            return dborder.ToDataTable();

        }

        //在Gi内口中出现丢包的例子
        private DataTable viewLost_gi_from_giw_FromCache()
        {
            var gngi = from n in gz_gn.GnGiGw_Get2x
                       join w in gz_giw.GnGiGw_Get2x on new { n.http_request_uri, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack }
                      equals new { w.http_request_uri, w.tcp_seq, w.tcp_nxtseq, w.tcp_ack }
                       join i in gz_gi.GnGiGw_Get2x on new { n.http_request_uri, n.tcp_seq, n.tcp_nxtseq, n.tcp_ack }
                       equals new { i.http_request_uri, i.tcp_seq, i.tcp_nxtseq, i.tcp_ack }
                       where n.Response == null  //gn丢包
                       where i.Response == null  //gi丢包
                       where w.Response != null  //gw不丢包
                       select new
                       {
                           n_BeginFileNum = gz_gn.RawFileList.Where(e => e.FileNum == n.BeginFileNum).First(),
                           n_BeginFrameNum = n.BeginFrameNum,
                           n_PacketNum = n.PacketNum,
                           n_FileNum = gz_gn.RawFileList.Where(e => e.FileNum == n.FileNum).First(),
                           i_BeginFileNum = gz_gi.RawFileList.Where(e => e.FileNum == i.BeginFileNum).First(),
                           i_BeginFrameNum = i.BeginFrameNum,
                           i_PacketNum = i.PacketNum,
                           i_FileNum = gz_gi.RawFileList.Where(e => e.FileNum == i.FileNum).First(),
                           w_BeginFileNum = gz_giw.RawFileList.Where(e => e.FileNum == w.BeginFileNum).First(),
                           w_BeginFrameNum = w.BeginFrameNum,
                           w_PacketNum = w.PacketNum,
                           w_FileNum = gz_giw.RawFileList.Where(e => e.FileNum == w.FileNum).First(),

                       };

            var dborder = gngi.OrderBy(e => e.n_BeginFileNum).ThenBy(e => e.n_BeginFrameNum);
            return dborder.ToDataTable();
        }
    }
}
