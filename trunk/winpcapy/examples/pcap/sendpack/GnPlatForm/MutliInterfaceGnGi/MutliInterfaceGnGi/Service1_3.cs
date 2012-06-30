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


        //GnGi_Get2x_Multi
        //提取GnGi_Get2x_Multi表中的TOP
        private DataTable getTableRandownRows()
        //private void navBarItem4_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            // clearColumns();
            var dborder = gz_gngi.GnGi_Get2x_Multi.Take(100);
            //gridControl1.DataSource = dborder.ToList();
            //gridView1.OptionsView.ColumnAutoWidth = false;
            return dborder.ToDataTable();
        }


        private DataTable getTableAllRows()
        //private void navBarItem2_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            //gridControl1.DataSource = null;
            //gridView1.PopulateColumns();
            var dborder = gz_gngi.GnGi_Get2x_Multi.OrderBy(e => e.PacketNum);
            //gridControl1.DataSource = dborder.ToList();
            //gridView1.OptionsView.ColumnAutoWidth = false;
            return dborder.ToDataTable();
        }

        //private DataTable getAnalysis()
        //private void navBarItem3_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        //{
        //    gridControl1.DataSource = null;
        //    gridView1.PopulateColumns();
        //    var dborder = gz_gngi.GnGi_Get2x_Multi.OrderBy(e => e.PacketNum);
        //    gridControl1.DataSource = dborder.ToList();
        //    gridView1.OptionsView.ColumnAutoWidth = false;
        //}

        //get在Gn->Gi丢包统计-----2
        private DataTable statTableGnGiLost()
        //private void navBarItem9_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
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

                       };

            var total = gngi.Where(e => e.gtp_cnt > 0).Count();

            var gngifilter = from p in gngi
                             where p.gtp_cnt > 0
                             group p by new
                             {
                                 gtp_percent_equals_50 = p.gtp_percent == 0.5,
                                 gtp_percent_over_50 = p.gtp_percent > 0.5,
                                 gtp_percent_less_50 = p.gtp_percent < 0.5
                             } into ttt
                             select new
                             {
                                 ttt.Key.gtp_percent_equals_50,
                                 ttt.Key.gtp_percent_over_50,
                                 ttt.Key.gtp_percent_less_50,
                                 cnt = ttt.Count(),
                                 percent = 1.0 * ttt.Count() / total,
                             };
            //clearColumns();
            var dborder = gngifilter.OrderByDescending(e => e.cnt);
            //gridControl1.DataSource = dborder.ToList();
            //gridView1.OptionsView.ColumnAutoWidth = false;
            return dborder.ToDataTable();
        }


    }
}
