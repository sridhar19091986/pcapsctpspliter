using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;
using System.Data.Objects;
//using System.Data.DataSetExtensions;


namespace MutliInterfaceGnGi
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public partial class Service1 : IService1
    {
        //IP_GnGi_TCPy-GnGi_Get2x_Multi---1
        //串联条件已修改
        private DataTable viewTableDetail()
        {
            var gngi = from p in gz_gngi.GnGi_Get2x_Multi
                       group p by new { p.tcp_seq, p.tcp_nxtseq, p.tcp_ack, p.Request_URI, p.User_Agent } into ttt
                       select new
                       {
                           ttt.Key.tcp_seq,
                           ttt.Key.tcp_nxtseq,
                           ttt.Key.tcp_ack,
                           ttt.Key.Request_URI,
                           ttt.Key.User_Agent,
                           total_cnt = ttt.Count(),
                           gtp_cnt = ttt.Where(e => e.gtp_flags_payload == "GTP").Count(),
                           gtp_percent = 1.0 * ttt.Where(e => e.gtp_flags_payload == "GTP").Count() / ttt.Count(),
                           packetfile = ttt.Select(e => e.FileNum),
                           packetnum = ttt.Select(e => e.PacketNum),
                           beginnum = ttt.Select(e => e.BeginFrameNum),
                           ip_proto = ttt.Select(e => e.ip_proto),
                           ip2_proto = ttt.Select(e => e.ip2_proto),
                           ip_id = ttt.Select(e => e.ip_id),
                           ip2_id = ttt.Select(e => e.ip2_id),
                           udp_port = ttt.Select(e => e.Source_Port == null ? "0" : e.Source_Port),

                       };


            var gngi_list = from p in gngi.AsParallel().ToList()
                            where p.gtp_cnt > 0
                            select new
                            {
                                p.tcp_seq,
                                p.tcp_nxtseq,
                                p.tcp_ack,
                                p.Request_URI,
                                p.User_Agent,
                                p.total_cnt,
                                p.gtp_cnt,
                                p.gtp_percent,
                                packetfile = p.packetfile.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                                packetnum = p.packetnum.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                                beginnum = p.beginnum.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                                ip_proto = p.ip_proto.Aggregate((a, b) => a + "," + b),
                                ip2_proto = p.ip2_proto.Aggregate((a, b) => a + "," + b),
                                ip_id = p.ip_id.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                                ip2_id = p.ip2_id.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                                udp_port = p.udp_port.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),

                            };

            var dborder = gngi_list.OrderByDescending(e => e.total_cnt);

            return dborder.ToDataTable();

        }

        //IP_GnGi_TCPy-GnGi_Get2x_Multi-ip2.id
        private DataTable viewTableDetail_ip2_id()
        // private void navBarItem6_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            var gngi = from p in gz_gngi.GnGi_Get2x_Multi
                       group p by new { p.tcp_seq, p.tcp_nxtseq, p.tcp_ack, p.Request_URI, p.User_Agent, p.ip2_id } into ttt
                       select new
                       {
                           ttt.Key.tcp_seq,
                           ttt.Key.tcp_nxtseq,
                           ttt.Key.tcp_ack,
                           ttt.Key.Request_URI,
                           ttt.Key.User_Agent,
                           ttt.Key.ip2_id,
                           total_cnt = ttt.Count(),
                           gtp_cnt = ttt.Where(e => e.gtp_flags_payload == "GTP").Count(),
                           gtp_percent = 1.0 * ttt.Where(e => e.gtp_flags_payload == "GTP").Count() / ttt.Count(),
                           packetfile = ttt.Select(e => e.FileNum.ToString()).Aggregate((a, b) => a + "," + b),
                           packetnum = ttt.Select(e => e.PacketNum.ToString()).Aggregate((a, b) => a + "," + b),
                           beginnum = ttt.Select(e => e.BeginFrameNum.ToString()).Aggregate((a, b) => a + "," + b),
                           ip_proto = ttt.Select(e => e.ip_proto).Aggregate((a, b) => a + "," + b),
                           ip2_proto = ttt.Select(e => e.ip2_proto).Aggregate((a, b) => a + "," + b),
                           ip_id = ttt.Select(e => e.ip_id.ToString()).Aggregate((a, b) => a + "," + b),
                           //ip2_id = ttt.Select(e => e.ip2_id.ToString()).Aggregate((a, b) => a + "," + b),

                       };
            //clearColumns();
            var dborder = gngi.OrderByDescending(e => e.total_cnt);
            //gridControl1.DataSource = dborder.ToList();
            //gridView1.OptionsView.ColumnAutoWidth = false;
            return dborder.ToDataTable();
        }


        private DataTable viewTableDetail_All()
        //private void navBarItem10_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {


            var gngi = from p in gz_gngi.GnGi_Get2x_Multi
                       group p by new { p.tcp_seq, p.tcp_nxtseq, p.tcp_ack, p.Request_URI, p.User_Agent } into ttt
                       select new
                       {
                           ttt.Key.tcp_seq,
                           ttt.Key.tcp_nxtseq,
                           ttt.Key.tcp_ack,
                           ttt.Key.Request_URI,
                           ttt.Key.User_Agent,
                           total_cnt = ttt.Count(),
                           gtp_cnt = ttt.Where(e => e.gtp_flags_payload == "GTP").Count(),
                           gtp_percent = 1.0 * ttt.Where(e => e.gtp_flags_payload == "GTP").Count() / ttt.Count(),

                           //判断是否nat转换的问题
                           gre_cnt = ttt.Where(e => e.gtp_flags_payload == null && e.ip2_proto != null).Count(),
                           nat_cnt = ttt.Where(e => e.gtp_flags_payload == null && e.ip2_proto == null).Count(),
                           //判断地址的问题？
                           ip_dst = ttt.Select(e => e.Dest_IP==null?"0":e.Dest_IP),
                           ip2_dst = ttt.Select(e => e.Dest_IP2==null?"0":e.Dest_IP2),
                           //端口的问题？
                           tcp_dst_port = ttt.Select(e => e.tcp_dstport==null?0:e.tcp_dstport),
                           udp_dst_port = ttt.Select(e => e.Dest_Port==null?"0":e.Dest_Port),
                           //判断分片的问题？
                           ip_seg = ttt.Select(e => e.ip_flags_mf==null?"0":e.ip_flags_mf),
                           ip2_seg = ttt.Select(e => e.ip2_flags_mf==null?"0":e.ip2_flags_mf),
                           tcp_seg = ttt.Select(e => e.tcp_need_segment==null?"0":e.tcp_need_segment),
                           //ttl的问题？
                           ip_ttl = ttt.Select(e => e.ip_ttl==null?0:e.ip_ttl),
                           ip2_ttl = ttt.Select(e => e.ip2_ttl==null?0:e.ip2_ttl),

                           packetfile = ttt.Select(e => e.FileNum==null?0:e.FileNum),
                           packetnum = ttt.Select(e => e.PacketNum==null?0:e.PacketNum),
                           beginnum = ttt.Select(e => e.BeginFrameNum==null?0:e.BeginFrameNum),
                           ip_proto = ttt.Select(e => e.ip_proto==null?"0":e.ip_proto),
                           ip2_proto = ttt.Select(e => e.ip2_proto==null?"0":e.ip2_proto),
                           ip_id = ttt.Select(e => e.ip_id==null?0:e.ip_id),
                           ip2_id = ttt.Select(e => e.ip2_id==null?0:e.ip2_id),


                       };
            var gngi_list = from p in gngi.AsParallel().ToList()
                            where p.gtp_cnt > 0
                            //where p.gtp_percent==0.5
                            select new
                            {
                                p.tcp_seq,
                                p.tcp_nxtseq,
                                p.tcp_ack,
                                p.Request_URI,
                                p.User_Agent,
                                p.total_cnt,
                                p.gtp_cnt,
                                p.gtp_percent,
                                p.gre_cnt,
                                p.nat_cnt,
                                //判断地址的问题？
                                ip_dst = p.ip_dst.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                                ip2_dst = p.ip2_dst.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                                //端口的问题？
                                tcp_dst_port = p.tcp_dst_port.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                                udp_dst_port = p.udp_dst_port.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                                //判断分片的问题？
                                ip_seg = p.ip_seg.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                                ip2_seg = p.ip2_seg.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                                tcp_seg = p.tcp_seg.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                                //ttl的问题？
                                ip_ttl = p.ip_ttl.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                                ip2_ttl = p.ip2_ttl.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),



                                packetfile = p.packetfile.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                                packetnum = p.packetnum.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                                beginnum = p.beginnum.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                                ip_proto = p.ip_proto.Aggregate((a, b) => a + "," + b),
                                ip2_proto = p.ip2_proto.Aggregate((a, b) => a + "," + b),
                                ip_id = p.ip_id.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
                                ip2_id = p.ip2_id.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),


                            };
            //clearColumns();
            var dborder = gngi_list.OrderByDescending(e => e.total_cnt);
            //gridControl1.DataSource = dborder.AsParallel().ToList();
            //gridView1.OptionsView.ColumnAutoWidth = false;
            //MessageBox.Show(gngi_list.Count().ToString());
            return dborder.ToDataTable();
        }

    }
}
