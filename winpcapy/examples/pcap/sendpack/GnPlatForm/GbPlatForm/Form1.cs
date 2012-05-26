/*
            
drop table if exists gb_gz_tousu;
create table gb_gz_tousu engine=myisam  select * from gb_common_201205151200 where 1<>1;
ALTER TABLE `gb_gz_tousu`  ADD PRIMARY KEY (`Session_ID`);

INSERT into gb_gz_tousu select * from  gb_common_201205152330  limit 1,1000
 * 
 SELECT * FROM OPENQUERY (guangzhou_mpos_wangwei_aMYSQL,'select * from gb_gz_tousu');

 * 
 * */


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Objects;
using System.Net;

namespace GbPlatForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void refreshGrid(IQueryable linqQuery)
        {
            gridView1.PopulateColumns();
            gridControl1.Refresh();
            gridView1.BestFitColumns();
            gridView1.OptionsView.AllowCellMerge = true;
            for (int i = 5; i < gridView1.Columns.Count; i++)
                gridView1.Columns[i].OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;

            QueryLogFile qlf = new QueryLogFile();
            qlf.QueryLog(linqQuery);
        }

        private void refreshGrid(IQueryable linqQuery, int begin)
        {
            gridView1.PopulateColumns();
            gridControl1.Refresh();
            gridView1.BestFitColumns();
            gridView1.OptionsView.AllowCellMerge = true;
            for (int i = begin; i < gridView1.Columns.Count; i++)
                gridView1.Columns[i].OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;

            QueryLogFile qlf = new QueryLogFile();
            qlf.QueryLog(linqQuery);
        }
        //private ObjectQuery<gb_gz_tousu> gb_gz;
        private ObjectQuery<mpos_gb_gz> sqlserver_gb;
        private void Form1_Load(object sender, EventArgs e)
        {
            //mpos_gb_gz0511Entities mpos_gb = new mpos_gb_gz0511Entities();
            GuangZhou_GnEntities gn = new GuangZhou_GnEntities();
            sqlserver_gb = gn.mpos_gb_gz;
            //gb_gz = mpos_gb.gb_gz_tousu;
        }

        private void navBarItem1_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            var maxt = sqlserver_gb.Max(e => e.End_Date_Time);
            var mint = sqlserver_gb.Min(e => e.End_Date_Time);
            TimeSpan ts = maxt - mint;
            var ttim = mint.ToString() + "-" + maxt.ToString() + "," + ts.TotalSeconds.ToString();
            textBox1.Text = ttim.ToString();
        }

        private void navBarItem2_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            //var total = sqlserver_gb.Count();
            var gb = from p in sqlserver_gb
                     group p by p.Event_Type into ttt
                     select new
                     {
                         Event_Type = ttt.Key,
                         suc = ttt.Where(e => e.Response == 1).Count() + ttt.Where(e => e.Accept == 1).Count() + ttt.Where(e => e.Reply == 1).Count(),
                         cnt = ttt.Count(),
                         percent = 1.0 * (ttt.Where(e => e.Response == 1).Count() + ttt.Where(e => e.Accept == 1).Count() + ttt.Where(e => e.Reply == 1).Count()) / ttt.Count(),
                     };
            //refreshGrid(gb);
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();
            gridControl1.DataSource = gb.OrderByDescending(e => e.cnt);

        }

        private void navBarItem5_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Excel2007文件(*.xlsx) |*.xlsx";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {

                gridView1.ExportToXlsx(saveFileDialog1.FileName);
            }
        }

        private void navBarItem3_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            var gb = from p in sqlserver_gb
                     where p.Reply == 0
                     where p.Response == 0
                     where p.Accept == 0
                     where p.Event_Type == "DNS"
                     select new
                     {
                         p.Session_ID,
                         Start_ime = p.Start_Date_Time.Hour,
                         End_Time = p.End_Date_Time.Hour,
                         p.IMSI,
                         p.IMEI,
                         p.Event_Type,
                         p.APN,
                         p.Query_Type,
                         p.Query_Name
                     };
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();
            gridControl1.DataSource = gb;
        }

        private void navBarItem4_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            var gb = from p in sqlserver_gb
                     where p.Reply == 0
                     where p.Response == 0
                     where p.Accept == 0
                     where p.Event_Type == "DNS"
                     group p by new { p.APN, p.Query_Type, p.Query_Name } into ttt
                     select new
                     {
                         ttt.Key.APN,
                         ttt.Key.Query_Type,
                         ttt.Key.Query_Name,
                         cnt = ttt.Count(),

                     };

            gridControl1.DataSource = null;
            gridView1.PopulateColumns();
            gridControl1.DataSource = gb.OrderByDescending(e => e.cnt);
        }

        private void navBarItem6_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            var gb = from p in sqlserver_gb
                     where p.Reply == 0
                     where p.Response == 0
                     where p.Accept == 0
                     where p.Event_Type == "Syn"
                     select new
                     {
                         p.Session_ID,
                         Start_ime = p.Start_Date_Time.Hour,
                         End_Time = p.End_Date_Time.Hour,
                         p.IMSI,
                         p.IMEI,
                         p.Event_Type,
                         p.APN,
                         p.SOURCE_IP,
                         p.DEST_IP
                     };
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();
            gridControl1.DataSource = gb;
        }

        private void navBarItem7_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            var gb = from p in sqlserver_gb
                     where p.Reply == 0
                     where p.Response == 0
                     where p.Accept == 0
                     where p.Event_Type == "Syn"
                     group p by new { p.APN, p.SOURCE_IP, p.DEST_IP } into ttt
                     select new
                     {
                         ttt.Key.APN,
                         ttt.Key.SOURCE_IP,
                         ttt.Key.DEST_IP,
                         cnt = ttt.Count(),

                     };

            gridControl1.DataSource = null;
            gridView1.PopulateColumns();

            var gbl = new List<Tuple<string, string, string, long, int>>();

            var gbo = gb.OrderByDescending(e => e.cnt);

            foreach (var m in gbo)
            {
                var mm = new Tuple<string, string, string, long, int>(m.APN, getIpAddress((int)m.SOURCE_IP), getIpAddress((int)m.DEST_IP), m.DEST_IP, m.cnt);
                gbl.Add(mm);
            }


            gridControl1.DataSource = gbl;


        }
        public string getIpAddress(int longip)
        {
            string ipstr = new IPAddress(BitConverter.GetBytes((longip))).ToString();
            return ipstr;
        }
    }
}
