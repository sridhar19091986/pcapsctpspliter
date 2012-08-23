//#define abc

/*
 * 
 * 重传率和丢包率同时进行统计
 * 
 * */



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfflineInspect.ReTransmission
{

#if abc
    

    class RetransStatics
    {

        private void navBarItem25_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();

            var lac = sqlserver_mpos_gb_gz.ToLookup(e => (int?)e.LAC, e => (int?)e.CI);
            var mytcp = mytcpsession.ToList();

            var syn = from p in mytcp
                      //where p.seq_min != 0 && p.seq_max != 0
                      //where p.seq_max != null && p.seq_min != null
                      group p by new { ts_cell = lac[p.lac].Contains(p.ci), p.direction } into ttt
                      select new
                      {
                          ttt.Key.ts_cell,
                          ttt.Key.direction,
                          cell_cnt = ttt.Select(e => e.lac.ToString() + e.ci.ToString()).Distinct().Count(),
                          packet_cnt = ttt.Sum(e => e.packet_count),
                          packet_repeat = ttt.Sum(e => e.packet_count_repeat),
                          packet_sack = ttt.Sum(e => e.packet_sack_total),
                          //llc_repeat=;

                          repeat_percent = ttt.Sum(e => e.packet_count) == 0 ? 0 : (1.0 * ttt.Sum(e => e.packet_count_repeat) / ttt.Sum(e => e.packet_count)),
                          sack_percent = ttt.Sum(e => e.packet_count) == 0 ? 0 : (1.0 * ttt.Sum(e => e.packet_sack_total) / ttt.Sum(e => e.packet_count)),
                          gb_ip2_total = ttt.Sum(e => e.ip2_total),
                          gb_seq_total = ttt.Sum(e => e.seq_total),
                          duration = ttt.Sum(e => e.duration),
                          gb_ip2_rate = ttt.Sum(e => (double)e.duration) == 0 ? 0 : (ttt.Sum(e => (double)e.ip2_total) / ttt.Sum(e => (double)e.duration)),
                          gb_seq_rate = ttt.Sum(e => (double)e.duration) == 0 ? 0 : (ttt.Sum(e => (double)e.seq_total) / ttt.Sum(e => (double)e.duration)),
                          tcp_session_cnt = ttt.Select(e => e.session_id).Distinct().Count(),
                          packet_discard = 1.0 * ttt.Where(e => e.packet_discard_total == 1).Count() / ttt.Select(e => e.session_id).Distinct().Count()
                      };

            gridControl1.DataSource = syn.OrderByDescending(e => e.repeat_percent).ToList();

        }
    }

#endif
}
