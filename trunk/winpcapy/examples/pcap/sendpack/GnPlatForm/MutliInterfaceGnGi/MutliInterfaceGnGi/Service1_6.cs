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

//using System.Data.DataSetExtensions;


namespace MutliInterfaceGnGi
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public partial class Service1 : IService1
    {
        private DataTable viewTableDetail_All_FromCache()
        //private void navBarItem10_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {


            //gz_gngi.
            //gz_gngi.GnGiGw_Http_Any_Multi.GetCacheKey()

            //var gngi = from p in gz_gngi.GnGiGw_Http_Any_Multi.FromCache()
            //gz_gngi.GnGiGw_Http_Any_Multi.Load();

            //gz_gngi.
            //gz_gngi.GnGiGw_Http_Any_Multi.GetCacheKey()

            //foreach(var key in gngilook)
            //{
            //    dynamic dy=new dynamic();
            //    dy.tcp_seq=key.Key.tcp_seq,
            //    dy.gtp_cnt=key.Where(e=>e.
            //}

            var gngi = from p in gz_gngi.GnGiGw_Http_Any_Multi.FromCache()
                       group p by new { p.tcp_seq, p.tcp_nxtseq, p.tcp_ack, p.http_request_uri, p.http_user_agent } into ttt
                       select new
                       {
                           ttt.Key.tcp_seq,
                           ttt.Key.tcp_nxtseq,
                           ttt.Key.tcp_ack,
                           ttt.Key.http_request_uri,
                           ttt.Key.http_user_agent,
                           total_cnt = ttt.Count(),
                           //gn接口消息的数量
                           gtp_cnt = ttt.Where(e => e.gtp_flags_payload == "GTP").Count(),
                           gtp_percent = 1.0 * ttt.Where(e => e.gtp_flags_payload == "GTP").Count() / ttt.Count(),
                           //gi接口gre消息的数量
                           gre_cnt = ttt.Where(e => e.ip_proto == "GRE").Count(),
                           gre_percent = 1.0 * ttt.Where(e => e.ip_proto == "GRE").Count() / ttt.Count(),
                           //gi接口非gre消息，即TCP消息的数量
                           tcp_cnt = ttt.Where(e => e.ip_proto == "TCP").Count(),
                           tcp_percent = 1.0 * ttt.Where(e => e.ip_proto == "TCP").Count() / ttt.Count(),
                           //计算会话的数量
                           session_cnt = ttt.Select(e => e.BeginFrameNum == null ? -1 : e.BeginFrameNum).Distinct().Count(),
                           session_id = ttt.Select(e => e.BeginFrameNum).First(),
                           //判断是否nat转换的问题?
                           ip2_proto_cnt = ttt.Select(e => e.ip2_proto).Distinct().Count(),
                           ip2_proto_aggr = ttt.Select(e => e.ip2_proto == null ? "" : e.ip2_proto).Distinct().Aggregate((a, b) => a + "," + b),
                           //判断地址的问题？
                           ip_dst_cnt = ttt.Select(e => e.ip_dst_host== null ? "" : e.ip_dst_host).Distinct().Count(),
                           ip_dst_aggr= ttt.Select(e => e.ip_dst_host == null ? "" : e.ip_dst_host).Distinct().Aggregate((a, b) => a + "," + b),
                           ip2_dst_cnt = ttt.Select(e => e.ip2_dst_host== null ? "" : e.ip2_dst_host).Distinct().Count(),
                           ip2_dst_aggr= ttt.Select(e => e.ip2_dst_host == null ? "" : e.ip2_dst_host).Distinct().Aggregate((a, b) => a + "," + b),
                           //tcp端口的问题？
                           tcp_src_port_cnt = ttt.Select(e => e.tcp_srcport == null ? -1 : e.tcp_srcport).Distinct().Count(),
                           tcp_src_port_aggr = ttt.Select(e => e.tcp_srcport == null ? -1 : e.tcp_srcport).Distinct().Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                           tcp_dst_port_cnt = ttt.Select(e => e.tcp_dstport == null ? -1 : e.tcp_dstport).Distinct().Count(),
                           tcp_dst_port_aggr = ttt.Select(e => e.tcp_dstport == null ? -1 : e.tcp_dstport).Distinct().Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                           //udp端口的问题？
                           udp_dst_port_cnt = ttt.Select(e => e.udp_dstport == null ? 0 : e.udp_dstport).Distinct().Count(),
                           udp_dst_port_aggr = ttt.Select(e => e.udp_dstport== null ? 0 : e.udp_dstport).Select(e=>e.Value.ToString()).Distinct().Aggregate((a, b) => a + "," + b),
                           //判断分片的问题？
                           ip_seg = ttt.Select(e => e.ip_flags_mf == null ? "" : e.ip_flags_mf).Distinct().Aggregate((a, b) => a + "," + b),
                           ip2_seg = ttt.Select(e => e.ip2_flags_mf == null ? "" : e.ip2_flags_mf).Distinct().Aggregate((a, b) => a + "," + b),
                           tcp_seg = ttt.Select(e => e.tcp_need_segment == null ? "" : e.tcp_need_segment).Distinct().Aggregate((a, b) => a + "," + b),
                           //ttl的问题？
                           ip_ttl = ttt.Select(e => e.ip_ttl == null ? -1 : e.ip_ttl).Select(e => e.ToString()).Distinct().Aggregate((a, b) => a + "," + b),
                           ip2_ttl = ttt.Select(e => e.ip2_ttl == null ? -1 : e.ip2_ttl).Select(e => e.ToString()).Distinct().Aggregate((a, b) => a + "," + b),
                           //其他的问题？
                           packetfile = ttt.Select(e => e.FileNum).Select(e => e.ToString()).Distinct().Aggregate((a, b) => a + "," + b),
                           packetnum = ttt.Select(e => e.PacketNum).Select(e => e.ToString()).Distinct().Aggregate((a, b) => a + "," + b),
                           beginnum = ttt.Select(e => e.BeginFrameNum).Distinct().Select(e => e.ToString()).Distinct().Aggregate((a, b) => a + "," + b),
                           ip_proto = ttt.Select(e => e.ip_proto == null ? "" : e.ip_proto).Distinct().Aggregate((a, b) => a + "," + b),
                           ip2_proto = ttt.Select(e => e.ip2_proto == null ? "" : e.ip2_proto).Distinct().Aggregate((a, b) => a + "," + b),
                           ip_id = ttt.Select(e => e.ip_id == null ? -1 : e.ip_id).Distinct().Select(e => e.ToString()).Distinct().Aggregate((a, b) => a + "," + b),
                           ip2_id = ttt.Select(e => e.ip2_id == null ? -1 : e.ip2_id).Distinct().Select(e => e.ToString()).Distinct().Aggregate((a, b) => a + "," + b),


                       };

            //clearColumns();
            var dborder = gngi.Where(e => e.gtp_cnt > 0).OrderByDescending(e => e.total_cnt);
            //gridControl1.DataSource = dborder.AsParallel().ToList();
            //gridView1.OptionsView.ColumnAutoWidth = false;
            //MessageBox.Show(gngi_list.Count().ToString());
            return dborder.ToDataTable();
        }

    }
}
