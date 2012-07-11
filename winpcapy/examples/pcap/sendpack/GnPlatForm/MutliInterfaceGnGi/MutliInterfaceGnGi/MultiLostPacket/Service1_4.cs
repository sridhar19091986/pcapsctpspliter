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
        private DataTable viewTableDetail_All_ToLookup()
        //private void navBarItem10_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            var gngilook = gz_gngi.GnGiGw_Http_Any_Multi.ToLookup(p =>
                new { p.tcp_seq, p.tcp_nxtseq, p.tcp_ack, p.http_request_uri, p.http_user_agent });
            //gz_gngi.GnGiGw_Http_Any_Multi.Load();

            //gz_gngi.
            //gz_gngi.GnGiGw_Http_Any_Multi.GetCacheKey()

            //foreach(var key in gngilook)
            //{
            //    dynamic dy=new dynamic();
            //    dy.tcp_seq=key.Key.tcp_seq,
            //    dy.gtp_cnt=key.Where(e=>e.
            //}

            var gngi = from ttt in gngilook
                       //group p by p.Key into ttt
                       select new
                       {
                           ttt.Key.tcp_seq,
                           ttt.Key.tcp_nxtseq,
                           ttt.Key.tcp_ack,
                           ttt.Key.http_request_uri,
                           ttt.Key.http_user_agent,
                           total_cnt = ttt.Count(),
                           gtp_cnt = ttt.Where(e => e.gtp_flags_payload == "GTP").Count(),
                           gtp_percent = 1.0 * ttt.Where(e => e.gtp_flags_payload == "GTP").Count() / ttt.Count(),

                           //判断是否nat转换的问题
                           gre_cnt = ttt.Where(e => e.gtp_flags_payload == null && e.ip2_proto != null).Count(),
                           nat_cnt = ttt.Where(e => e.gtp_flags_payload == null && e.ip2_proto == null).Count(),
                           //判断地址的问题？
                           ip_dst = ttt.Select(e => e.ip_dst_host == null ? "0" : e.ip_dst_host).Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                           ip2_dst = ttt.Select(e => e.ip2_dst_host == null ? "0" : e.ip2_dst_host).Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                           //端口的问题？
                           tcp_dst_port = ttt.Select(e => e.tcp_dstport == null ? 0 : e.tcp_dstport).Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                           udp_dst_port = ttt.Select(e => e.udp_dstport == null ? 0 : e.udp_dstport).Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                           //判断分片的问题？
                           ip_seg = ttt.Select(e => e.ip_flags_mf == null ? "0" : e.ip_flags_mf).Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                           ip2_seg = ttt.Select(e => e.ip2_flags_mf == null ? "0" : e.ip2_flags_mf).Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                           tcp_seg = ttt.Select(e => e.tcp_need_segment == null ? "0" : e.tcp_need_segment).Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                           //ttl的问题？
                           ip_ttl = ttt.Select(e => e.ip_ttl == null ? 0 : e.ip_ttl).Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                           ip2_ttl = ttt.Select(e => e.ip2_ttl == null ? 0 : e.ip2_ttl).Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),

                           packetfile = ttt.Select(e => e.FileNum).Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                           packetnum = ttt.Select(e => e.PacketNum).Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                           beginnum = ttt.Select(e => e.BeginFrameNum == null ? 0 : e.BeginFrameNum).Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                           ip_proto = ttt.Select(e => e.ip_proto == null ? "0" : e.ip_proto).Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                           ip2_proto = ttt.Select(e => e.ip2_proto == null ? "0" : e.ip2_proto).Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                           ip_id = ttt.Select(e => e.ip_id == null ? 0 : e.ip_id).Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                           ip2_id = ttt.Select(e => e.ip2_id == null ? 0 : e.ip2_id).Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),


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
