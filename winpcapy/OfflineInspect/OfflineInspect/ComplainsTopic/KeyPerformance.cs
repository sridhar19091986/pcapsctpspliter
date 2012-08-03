using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfflineInspect.ComplainsTopic
{
    class KeyPerformance
    {
        private void navBarItem2_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {

            var gb = from p in sqlserver_mpos_gb_gz
                     group p by p.Event_Type into ttt
                     select new
                     {
                         Event_Type = ttt.Key,
                         suc = ttt.Where(e => e.Response + e.Accept + e.Reply > 0).Count(),
                         cnt = ttt.Count(),

                         percent = 1.0 * ttt.Where(e => e.Response + e.Accept + e.Reply > 0).Count() / ttt.Count(),

                         size = ttt.Average(e => e.IP_LEN_DL),

                         duration = ttt.Average(e => e.Duration),

                         down_rate = ttt.Sum(e => e.Duration) != 0 ?
                         1.0 * ttt.Sum(e => e.IP_LEN_DL) / ttt.Sum(e => e.Duration) : 0,

                         cell_ab = ttt.Key == "LLC_Discard" || ttt.Key == "Radio_Stat" ?
                         1.0 * ttt.Count() / sqlserver_mpos_gb_gz.Sum(e => e.Count_Packet_DL) : 0

                     };

            gridControl1.DataSource = null;
            gridView1.PopulateColumns();
            gridControl1.DataSource = gb.OrderByDescending(e => e.cnt);

            gridView1.Columns[4].Caption = "size(Byte)";
            gridView1.Columns[5].Caption = "duration(ms)";
            gridView1.Columns[6].Caption = "down_rate(KByte/s)";

        }
    }
}
