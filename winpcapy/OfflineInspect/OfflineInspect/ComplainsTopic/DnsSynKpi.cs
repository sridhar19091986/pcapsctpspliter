using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfflineInspect.ComplainsTopic
{
    class DnsSynKpi
    {
        private void navBarItem4_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            var gb = from p in sqlserver_mpos_gb_gz
                     where p.Event_Type == "DNS"
                     group p by new { p.APN, p.Query_Type, p.Query_Name } into ttt
                     select new
                     {
                         ttt.Key.APN,
                         ttt.Key.Query_Type,
                         ttt.Key.Query_Name,
                         suc = ttt.Where(e => e.Reply + e.Response + e.Accept > 0).Count(),
                         failure = ttt.Where(e => e.Reply + e.Response + e.Accept == 0).Count(),
                         percent = 1.0 * ttt.Where(e => e.Reply + e.Response + e.Accept > 0).Count() / ttt.Count(),
                     };
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();
            gridControl1.DataSource = gb.OrderByDescending(e => e.failure);
        }
        private void navBarItem7_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            var gb = from p in sqlserver_mpos_gb_gz
                     where p.Event_Type == "Syn"
                     group p by new { p.APN, p.SOURCE_IP, p.DEST_IP } into ttt
                     select new
                     {
                         ttt.Key.APN,
                         ttt.Key.SOURCE_IP,
                         ttt.Key.DEST_IP,
                         suc = ttt.Where(e => e.Reply + e.Response + e.Accept > 0).Count(),
                         failure = ttt.Where(e => e.Reply + e.Response + e.Accept == 0).Count(),
                         percent = 1.0 * ttt.Where(e => e.Reply + e.Response + e.Accept > 0).Count() / ttt.Count(),

                     };

            gridControl1.DataSource = null;
            gridView1.PopulateColumns();

            var gbl = new List<Tuple<string, string, string, long, int, int, double>>();

            var gbo = gb.OrderByDescending(e => e.failure);

            foreach (var m in gbo)
            {
                var mm = new Tuple<string, string, string, long, int, int, double>
                    (m.APN, getIpAddress((int)m.SOURCE_IP), getIpAddress((int)m.DEST_IP), m.DEST_IP,
                    m.suc, m.failure, m.percent);
                gbl.Add(mm);
            }


            gridControl1.DataSource = gbl;

            gridView1.Columns[0].Caption = "APN";
            gridView1.Columns[1].Caption = "SOURCE_IP";
            gridView1.Columns[2].Caption = "DEST_IP";
            gridView1.Columns[3].Caption = "DEST_IP_int";
            gridView1.Columns[4].Caption = "suc";
            gridView1.Columns[5].Caption = "failure";
            gridView1.Columns[6].Caption = "percent";


        }

        private void navBarItem13_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();

            var dns = from p in gb_dns_syn
                      //where p.DumpFor != "Flush"
                      where p.SP_Name.Contains("apple")
                      where p.SP_Name.Contains("service1")
                      group p by new { p.SP_Name, p.Query_Type } into ttt
                      select new
                      {
                          ttt.Key.SP_Name,
                          ttt.Key.Query_Type,
                          dns_resp_type = ttt.Where(e => e.dns_resp_type != null).Select(e => e.dns_resp_type).FirstOrDefault(),
                          cnt = ttt.Where(e => e.Query == 1).Count(),
                          resp = ttt.Where(e => e.Response == 1).Count(),
                          percent = 1.0 * ttt.Where(e => e.Response == 1).Count() / ttt.Where(e => e.Query == 1).Count(),
                          ans = ttt.Where(e => e.Response == 1).Average(e => e.dns_count_answers),
                          resp_len = ttt.Where(e => e.Response == 1).Average(e => e.Data_Length),
                          delay = ttt.Where(e => e.Response == 1).Average(e => e.Response_delayFirst)
                      };


            gridControl1.DataSource = dns.OrderByDescending(e => e.cnt);

        }
    }
}
