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
        private DataTable viewTableDetail_All_FromCache_simple()
        {

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
                           //其他的问题？
                           packetfile = ttt.Select(e => e.FileNum).Select(e => e.ToString()).Distinct().Aggregate((a, b) => a + "," + b),
                           packetnum = ttt.Select(e => e.PacketNum).Select(e => e.ToString()).Distinct().Aggregate((a, b) => a + "," + b),
                           beginnum = ttt.Select(e => e.BeginFrameNum).Distinct().Select(e => e.ToString()).Distinct().Aggregate((a, b) => a + "," + b),
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
