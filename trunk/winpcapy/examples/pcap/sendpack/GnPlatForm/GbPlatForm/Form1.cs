/*
            
drop table if exists gb_gz_tousu;
create table gb_gz_tousu engine=myisam  select * from gb_common_201205151200 where 1<>1;
ALTER TABLE `gb_gz_tousu`  ADD PRIMARY KEY (`Session_ID`);

INSERT into gb_gz_tousu select * from  gb_common_201205152330  limit 1,1000

 * 

SELECT * FROM OPENQUERY (guangzhou_mpos_wangwei_aMYSQL,'select * from gb_gz_tousu');

 * 
 * 
 * 
 * 
insert into  dbo.mpos_gb_gz select *  FROM OPENQUERY (guangzhou_mpos_wangwei_aMYSQL,'select * from gb_2012_05_18_tousu1');

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
using DevExpress.Data;
using System.Threading;
using System.Diagnostics;

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
        private ObjectQuery<mpos_gb_gz> sqlserver_mpos_gb_gz;

        private ObjectQuery<Gb_DNS_Syn> gb_dns_syn;
        private ObjectQuery<Gb_PDP_Fin> gb_pdp_fin;
        private ObjectQuery<Gb_Cell_Syn> gb_cell_syn;
        private ObjectQuery<Gb_Cell_Repeat> gb_cell_repeat;
        private ObjectQuery<myTcpSession> mytcpsession;

        private void Form1_Load(object sender, EventArgs e)
        {
            //mpos_gb_gz0511Entities mpos_gb = new mpos_gb_gz0511Entities();
            GuangZhou_GnEntities gn = new GuangZhou_GnEntities();
            Guangzhou_GbEntities gb = new Guangzhou_GbEntities();
            gb.CommandTimeout = 0;

            sqlserver_mpos_gb_gz = gn.mpos_gb_gz;
            gb_dns_syn = gb.Gb_DNS_Syn;
            gb_pdp_fin = gn.Gb_PDP_Fin;
            gb_cell_syn = gn.Gb_Cell_Syn;
            gb_cell_repeat = gn.Gb_Cell_Repeat;
            mytcpsession = gb.myTcpSession;

            //gb_gz = mpos_gb.gb_gz_tousu;
        }

        private void navBarItem1_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            var maxt = sqlserver_mpos_gb_gz.Max(e => e.End_Date_Time);
            var mint = sqlserver_mpos_gb_gz.Min(e => e.End_Date_Time);
            TimeSpan ts = maxt - mint;
            var ttim = mint.ToString() + "-" + maxt.ToString() + "," + ts.TotalSeconds.ToString();
            textBox1.Text = ttim.ToString();
        }

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

            var gb = from p in sqlserver_mpos_gb_gz
                     where p.Event_Type == "DNS"
                     select new
                     {
                         p.Session_ID,
                         Start_ime = p.Start_Date_Time,//p.Start_Date_Time.ToString("yyyy-MM-dd HH:mm:ss"),
                         End_Time = p.End_Date_Time,//.ToString("yyyy-MM-dd HH:mm:ss"),
                         p.IMSI,
                         p.IMEI,
                         p.Event_Type,
                         p.APN,
                         p.Query_Type,
                         p.Query_Name,
                         Result = p.Reply + p.Response + p.Accept
                     };
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();
            gridControl1.DataSource = gb;


            gridView1.Columns[1].DisplayFormat.FormatString = "yyyy-MM-dd HH:mm:ss";
            gridView1.Columns[1].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;

            gridView1.Columns[2].DisplayFormat.FormatString = "yyyy-MM-dd HH:mm:ss";
            gridView1.Columns[2].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;

            gridControl1.Refresh();
        }

        //where p.Reply == 0
        //where p.Response == 0
        //where p.Accept == 0

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

        private void navBarItem6_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            var gb = from p in sqlserver_mpos_gb_gz
                     where p.Event_Type == "Syn"
                     select new
                     {
                         p.Session_ID,
                         Start_ime = p.Start_Date_Time,//p.Start_Date_Time.ToString("yyyy-MM-dd HH:mm:ss"),
                         End_Time = p.End_Date_Time,// p.End_Date_Time.ToString("yyyy-MM-dd HH:mm:ss"),
                         p.IMSI,
                         p.IMEI,
                         p.Event_Type,
                         p.APN,
                         p.SOURCE_IP,
                         p.DEST_IP,
                         Result = p.Reply + p.Response + p.Accept
                     };

            gridControl1.DataSource = null;
            gridView1.PopulateColumns();
            gridControl1.DataSource = gb;

            gridView1.Columns[1].DisplayFormat.FormatString = "yyyy-MM-dd HH:mm:ss";
            gridView1.Columns[1].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;

            gridView1.Columns[2].DisplayFormat.FormatString = "yyyy-MM-dd HH:mm:ss";
            gridView1.Columns[2].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;

            gridControl1.Refresh();
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

        public string getIpAddress(int longip)
        {
            string ipstr = new IPAddress(BitConverter.GetBytes((longip))).ToString();
            return ipstr;
        }

        private void navBarItem8_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            var gb = from p in sqlserver_mpos_gb_gz
                     where p.Event_Type == "Get" || p.Event_Type == "Post"
                     group p by new { p.LAC, p.CI } into ttt
                     select new
                     {

                         LAC = ttt.Key.LAC,
                         CI = ttt.Key.CI,
                         //Event_Type = ttt.Key.Event_Type,
                         suc = ttt.Where(e => e.Response + e.Accept + e.Reply > 0).Count(),
                         cnt = ttt.Count(),
                         percent = 1.0 * ttt.Where(e => e.Response + e.Accept + e.Reply > 0).Count() / ttt.Count(),

                         size = ttt.Average(e => e.IP_LEN_DL),

                         duration = ttt.Average(e => e.Duration),
                         down_rate = ttt.Average(e => e.Duration) != 0 ?
                         1.0 * ttt.Sum(e => e.IP_LEN_DL) / ttt.Sum(e => e.Duration) : 0,


                         cell_ab = sqlserver_mpos_gb_gz.Where(e => e.Event_Type == "LLC_Discard" || e.Event_Type == "Radio_Stat")
                         .Where(e => e.LAC == ttt.Key.LAC && e.CI == ttt.Key.CI).Count()

                     };

            gridControl1.DataSource = null;
            gridView1.PopulateColumns();
            gridControl1.DataSource = gb.OrderByDescending(e => e.cell_ab);
            gridView1.Columns[5].Caption = "size(Byte)";
            gridView1.Columns[6].Caption = "duration(ms)";
            gridView1.Columns[7].Caption = "down_rate(KByte/s)";
        }

        private void navBarItem9_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            var gb = from p in sqlserver_mpos_gb_gz
                     where p.Event_Type == "DNS"
                     where p.Query_Name.Contains("service1")
                     select new
                     {
                         p.Session_ID,
                         Start_ime = p.Start_Date_Time,//p.Start_Date_Time.ToString("yyyy-MM-dd HH:mm:ss"),
                         End_Time = p.End_Date_Time,//.ToString("yyyy-MM-dd HH:mm:ss"),
                         p.IMSI,
                         p.IMEI,
                         p.Event_Type,
                         p.APN,
                         p.Query_Type,
                         p.Query_Name,
                         Result = p.Reply + p.Response + p.Accept
                     };
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();
            gridControl1.DataSource = gb;


            gridView1.Columns[1].DisplayFormat.FormatString = "yyyy-MM-dd HH:mm:ss";
            gridView1.Columns[1].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;

            gridView1.Columns[2].DisplayFormat.FormatString = "yyyy-MM-dd HH:mm:ss";
            gridView1.Columns[2].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;

            gridControl1.Refresh();
        }

        private void navBarItem10_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();

            var imsi = sqlserver_mpos_gb_gz.Select(e => e.IMSI).Distinct().ToList();

            var gb = gb_cell_syn
            .Where(e => e.bssgp_pdu_type == direction)
            .Where(e => imsi.Contains(e.bssgp_imsi))
            .ToList()
            .Where(e => Convert.ToDecimal(e.Ack_tcp_seq) - Convert.ToDecimal(e.tcp_seq) == 1
            || Convert.ToDecimal(e.Ack_Push_tcp_seq) - Convert.ToDecimal(e.tcp_seq) == 1)
            .Where(e => e.Ack == 1)
            .Where(e => e.Ack_Syn != null || e.Ack_Push != null)
            .Where(e => e.Ack_Syn != null && e.Ack_Push != null)
            .ToList();

            var syn = from p in gb
                      group p by p.bssgp_imsi into ttt
                      select new
                      {
                          ttt.Key,
                          //abc="1",
                          cnt = ttt.Count(),
                          //syn_repeat_cnt = ttt.Where(e => e.Syn_RepeatCounter > 0).Count(),
                          syn_repeat_perc = 1.0 * ttt.Where(e => e.Syn_RepeatCounter > 0).Count() / ttt.Count(),
                          //ack_syn_repeat_cnt = ttt.Where(e => e.Ack_Syn_RepeatCounter > 0).Count(),
                          ack_syn_repeat_perc = 1.0 * ttt.Where(e => e.Ack_Syn_RepeatCounter > 0).Count() / ttt.Count(),
                          //seq_repeat = 1.0 * ttt.Where(e => Convert.ToDecimal(e.Ack_tcp_seq) - Convert.ToDecimal(e.tcp_seq) != 1).Count() / ttt.Count(),
                          //nu_repeat = 1.0 * ttt.Where(e => e.Ack_llcgprs_nu - e.llcgprs_nu > 1).Count() / ttt.Count(),
                          up_rtt = ttt.Average(e => e.Ack_Syn_delayFirst),
                          down_rtt = ttt.Average(e => (e.Ack_Push == null ? e.Ack_delayFirst : e.Ack_Push_delayFirst) - e.Ack_Syn_delayFirst),

                          AckSyn_rate = 1.0 * ttt.Sum(e => e.Ack_Syn_ip2_len) / ttt.Sum(e => e.Ack_delayFirst - e.Ack_Syn_delayFirst),
                          syn_len = ttt.Average(e => e.syn_ip2_len),
                          acksyn_len = ttt.Average(e => e.Ack_Syn_ip2_len),
                          ack_len = ttt.Average(e => e.Ack_Syn_ip2_len),

                      };

            gridControl1.DataSource = syn.OrderByDescending(e => e.down_rtt).ToList();
            //gridView1.Columns[0].Caption = "lac";
            //gridView1.Columns[1].Caption = "ci";
            //gridView1.Columns[7].Caption = "ack_delay(ms)";
            //gridView1.Columns[8].Caption = "ack_len(Byte)";
            //gridView1.Columns[9].Caption = "down_rate(KByte/s)";

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

        private void navBarItem14_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();

            var total = gb_pdp_fin
                .Where(e => e.TCP_Fin == 1)
                //.Where(e => e.DumpFor == "EndFlowByFlowDesigner")
                .Where(e => e.APN.ToLower() == "cmwap").Count();

            var fin = from p in gb_pdp_fin
                      //where p.DumpFor == "EndFlowByFlowDesigner"
                      where p.TCP_Fin == 1
                      where p.APN.ToLower() == "cmwap"
                      group p by new { p.bssgp_pdu_type, p.TCP_Fin_MsgType } into ttt
                      select new
                      {
                          APN = "cmwap",
                          ttt.Key.bssgp_pdu_type,
                          ttt.Key.TCP_Fin_MsgType,
                          cnt = ttt.Count(),
                          percent = 1.0 * ttt.Count() / total,
                          delay = ttt.Average(e => e.TCP_Fin_delayFirst)

                      };

            gridControl1.DataSource = fin;
        }

        private void navBarItem15_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {

            var maxt = gb_dns_syn.Max(e => e.PacketTime);
            var mint = gb_dns_syn.Min(e => e.PacketTime);
            TimeSpan ts = maxt.Value - mint.Value;
            var ttim = mint.ToString() + "-" + maxt.ToString() + "," + ts.TotalSeconds.ToString();
            textBox1.Text = ttim.ToString();


        }

        private void navBarItem16_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();

            var gb = gb_cell_syn
                .Where(e => e.bssgp_pdu_type == direction)
                .ToList()
            .Where(e => Convert.ToDecimal(e.Ack_tcp_seq) - Convert.ToDecimal(e.tcp_seq) == 1
            || Convert.ToDecimal(e.Ack_Push_tcp_seq) - Convert.ToDecimal(e.tcp_seq) == 1)
            .Where(e => e.Ack == 1)
            .Where(e => e.Ack_Syn != null || e.Ack_Push != null)
                //.Where(e => e.Ack_Syn != null && e.Ack_Push != null)
            .ToList();

            var syn = from p in gb
                      //where p.Ack_Syn_RepeatCounter+p.Syn_RepeatCounter==0
                      group p by p.bssgp_lac into ttt
                      select new
                      {
                          ttt.Key,
                          //abc="1",
                          cnt = ttt.Count(),
                          //syn_repeat_cnt = ttt.Where(e => e.Syn_RepeatCounter > 0).Count(),
                          syn_repeat_perc = 1.0 * ttt.Where(e => e.Syn_RepeatCounter > 0).Count() / ttt.Count(),
                          //ack_syn_repeat_cnt = ttt.Where(e => e.Ack_Syn_RepeatCounter > 0).Count(),
                          ack_syn_repeat_perc = 1.0 * ttt.Where(e => e.Ack_Syn_RepeatCounter > 0).Count() / ttt.Count(),
                          //seq_repeat = 1.0 * ttt.Where(e => Convert.ToDecimal(e.Ack_tcp_seq) - Convert.ToDecimal(e.tcp_seq) != 1).Count() / ttt.Count(),
                          //nu_repeat = 1.0 * ttt.Where(e => e.Ack_llcgprs_nu - e.llcgprs_nu > 1).Count() / ttt.Count(),
                          up_rtt = ttt.Average(e => e.Ack_Syn_delayFirst),
                          down_rtt = ttt.Average(e => (e.Ack_Push == null ? e.Ack_delayFirst : e.Ack_Push_delayFirst) - e.Ack_Syn_delayFirst),

                          AckSyn_rate2 = 2 * 1.0 * ttt.Sum(e => e.Ack_Syn_ip2_len) / ttt.Sum(e => e.Ack_delayFirst - e.Ack_Syn_delayFirst),
                          syn_len = ttt.Average(e => e.syn_ip2_len),
                          acksyn_len = ttt.Average(e => e.Ack_Syn_ip2_len),
                          ack_len = ttt.Average(e => e.Ack_Syn_ip2_len),

                      };

            gridControl1.DataSource = syn.OrderByDescending(e => e.down_rtt).ToList();
        }

        private void navBarItem18_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();

            var sql = from p in sqlserver_mpos_gb_gz
                      group p by new { p.BSC, p.BSC_IP } into ttt
                      select new
                      {
                          ttt.Key.BSC,
                          ttt.Key.BSC_IP,
                          cnt = ttt.Count(),
                      };

            var gbl = new List<Tuple<string, string, int>>();

            var gbo = sql.OrderByDescending(e => e.cnt);

            foreach (var m in gbo)
            {
                var mm = new Tuple<string, string, int>(m.BSC, getIpAddress((int)m.BSC_IP), m.cnt);
                gbl.Add(mm);
            }

            gridControl1.DataSource = gbl;

        }

        private void navBarItem17_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            var maxt = gb_pdp_fin.Max(e => e.PacketTime);
            var mint = gb_pdp_fin.Min(e => e.PacketTime);
            TimeSpan ts = maxt.Value - mint.Value;
            var ttim = mint.ToString() + "-" + maxt.ToString() + "," + ts.TotalSeconds.ToString();
            textBox1.Text = ttim.ToString();
        }

        private void navBarItem19_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();
            gridControl1.DataSource = gb_cell_syn;

        }

        private void navBarItem11_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();

            var gb = gb_cell_syn
          .Where(e => e.bssgp_pdu_type == direction)
          .ToList()
      .Where(e => Convert.ToDecimal(e.Ack_tcp_seq) - Convert.ToDecimal(e.tcp_seq) == 1
      || Convert.ToDecimal(e.Ack_Push_tcp_seq) - Convert.ToDecimal(e.tcp_seq) == 1)
      .Where(e => e.Ack == 1)
      .Where(e => e.Ack_Syn != null || e.Ack_Push != null)
      .ToList();

            var syn = from p in gb
                      //where p.Ack_Syn_RepeatCounter + p.Syn_RepeatCounter == 0
                      group p by new { p.bssgp_lac, p.bssgp_ci } into ttt
                      select new
                      {
                          ttt.Key.bssgp_lac,
                          ttt.Key.bssgp_ci,
                          cnt = ttt.Count(),
                          //syn_repeat_cnt = ttt.Where(e => e.Syn_RepeatCounter > 0).Count(),
                          syn_repeat_perc = 1.0 * ttt.Where(e => e.Syn_RepeatCounter > 0).Count() / ttt.Count(),
                          //ack_syn_repeat_cnt = ttt.Where(e => e.Ack_Syn_RepeatCounter > 0).Count(),
                          ack_syn_repeat_perc = 1.0 * ttt.Where(e => e.Ack_Syn_RepeatCounter > 0).Count() / ttt.Count(),
                          //seq_repeat = 1.0 * ttt.Where(e => Convert.ToDecimal(e.Ack_tcp_seq) - Convert.ToDecimal(e.tcp_seq) != 1).Count() / ttt.Count(),
                          //nu_repeat = 1.0 * ttt.Where(e => e.Ack_llcgprs_nu - e.llcgprs_nu > 1).Count() / ttt.Count(),
                          up_rtt = ttt.Average(e => e.Ack_Syn_delayFirst),
                          down_rtt = ttt.Average(e => (e.Ack_Push == null ? e.Ack_delayFirst : e.Ack_Push_delayFirst) - e.Ack_Syn_delayFirst),

                          AckSyn_rate = 1.0 * ttt.Sum(e => e.Ack_Syn_ip2_len) / ttt.Sum(e => e.Ack_delayFirst - e.Ack_Syn_delayFirst),
                          syn_len = ttt.Average(e => e.syn_ip2_len),
                          acksyn_len = ttt.Average(e => e.Ack_Syn_ip2_len),
                          ack_len = ttt.Average(e => e.Ack_Syn_ip2_len),

                          //cnt = ttt.Count(),
                          //syn_repeat_cnt = ttt.Where(e => e.Syn_RepeatCounter > 0).Count(),
                          //syn_repeat_perc = 1.0 * ttt.Where(e => e.Syn_RepeatCounter > 0).Count() / ttt.Count(),
                          //ack_syn_repeat_cnt = ttt.Where(e => e.Ack_Syn_RepeatCounter > 0).Count(),
                          //ack_syn_repeat_perc = 1.0 * ttt.Where(e => e.Ack_Syn_RepeatCounter > 0).Count() / ttt.Count(),
                          ////seq_repeat = 1.0 * ttt.Where(e => Convert.ToDecimal(e.Ack_tcp_seq) - Convert.ToDecimal(e.tcp_seq) != 1).Count() / ttt.Count(),
                          ////nu_repeat = 1.0 * ttt.Where(e => e.Ack_llcgprs_nu - e.llcgprs_nu > 1).Count() / ttt.Count(),
                          //ack_delay = ttt.Average(e => (e.Ack_Push == null?e.Ack_delayFirst:e.Ack_Push_delayFirst) - e.Ack_Syn_delayFirst),
                          //ack_len = ttt.Average(e => e.Ack_Syn_ip2_len),
                          //down_rate = 1.0 * ttt.Sum(e => e.Ack_Syn_ip2_len) / ttt.Sum(e => e.Ack_delayFirst - e.Ack_Syn_delayFirst),
                      };

            gridControl1.DataSource = syn.OrderByDescending(e => e.down_rtt).ToList();
            //gridView1.Columns[0].Caption = "lac";
            //gridView1.Columns[1].Caption = "ci";
            //gridView1.Columns[7].Caption = "ack_delay(ms)";
            //gridView1.Columns[8].Caption = "ack_len(Byte)";
            //gridView1.Columns[9].Caption = "down_rate(KByte/s)";
        }

        string direction = "UL-UNITDATA";

        private void navBarItem12_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();

            var gb = gb_cell_syn
                  .Where(e => e.bssgp_pdu_type == direction)
                  .ToList()
              .Where(e => Convert.ToDecimal(e.Ack_tcp_seq) - Convert.ToDecimal(e.tcp_seq) == 1
              || Convert.ToDecimal(e.Ack_Push_tcp_seq) - Convert.ToDecimal(e.tcp_seq) == 1)
              .Where(e => e.Ack == 1)
              .Where(e => e.Ack_Syn != null || e.Ack_Push != null)
                //.Where(e => e.Ack_Syn != null && e.Ack_Push != null)
              .ToList();

            var syn = from p in gb
                      select new
                      {
                          p.PacketNum,
                          p.bssgp_lac,
                          p.bssgp_ci,
                          p.Syn_RepeatCounter,
                          p.Ack_Syn_RepeatCounter,
                          up_rtt = p.Ack_Syn_delayFirst,
                          down_rtt = (p.Ack_Push == null ? p.Ack_delayFirst : p.Ack_Push_delayFirst) - p.Ack_Syn_delayFirst,
                          AckSyn_rate = 1.0 * p.Ack_Syn_ip2_len / (p.Ack_delayFirst - p.Ack_Syn_delayFirst),
                          syn_len = p.syn_ip2_len,
                          acksyn_len = p.Ack_Syn_ip2_len,
                          ack_len = p.Ack_Syn_ip2_len,
                          p.Ack_Push_MsgType
                      };

            gridControl1.DataSource = syn.OrderBy(e => e.PacketNum).ToList();
        }

        private void navBarItem19_LinkClicked_1(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            var total = sqlserver_mpos_gb_gz.Sum(e => e.Duration);

            var ts = (from p in sqlserver_mpos_gb_gz
                      group p by new { p.LAC, p.CI } into ttt
                      select new
                      {

                          LAC = ttt.Key.LAC,
                          CI = ttt.Key.CI,
                          //cnt = ttt.Count(),
                          duration = ttt.Sum(e => e.Duration),
                          percent = 1.0 * ttt.Sum(e => e.Duration) / total,

                      }).ToList();

            var gb = gb_cell_syn
            .Where(e => e.bssgp_pdu_type == direction)
                //.Where(e => imsi.Contains(e.bssgp_imsi))
            .ToList()
            .Where(e => Convert.ToDecimal(e.Ack_tcp_seq) - Convert.ToDecimal(e.tcp_seq) == 1
            || Convert.ToDecimal(e.Ack_Push_tcp_seq) - Convert.ToDecimal(e.tcp_seq) == 1)
            .Where(e => e.Ack == 1)
            .Where(e => e.Ack_Syn != null || e.Ack_Push != null)
            .ToList();

            var syn = (from p in gb
                       //where p.Ack_Syn_RepeatCounter + p.Syn_RepeatCounter == 0
                       group p by new { p.bssgp_lac, p.bssgp_ci } into ttt
                       select new
                       {
                           ttt.Key.bssgp_lac,
                           ttt.Key.bssgp_ci,
                           cnt = ttt.Count(),
                           repeat = 1.0 * ttt.Where(e => e.Syn_RepeatCounter > 0 || e.Ack_Syn_RepeatCounter > 0).Count() / ttt.Count(),
                           syn_repeat_perc = 1.0 * ttt.Where(e => e.Syn_RepeatCounter > 0).Count() / ttt.Count(),
                           ack_syn_repeat_perc = 1.0 * ttt.Where(e => e.Ack_Syn_RepeatCounter > 0).Count() / ttt.Count(),
                           up_rtt = ttt.Average(e => e.Ack_Syn_delayFirst),
                           down_rtt = ttt.Average(e => (e.Ack_Push == null ? e.Ack_delayFirst : e.Ack_Push_delayFirst) - e.Ack_Syn_delayFirst),

                           AckSyn_rate = 1.0 * ttt.Sum(e => e.Ack_Syn_ip2_len) / ttt.Sum(e => e.Ack_delayFirst - e.Ack_Syn_delayFirst),
                           syn_len = ttt.Average(e => e.syn_ip2_len),
                           acksyn_len = ttt.Average(e => e.Ack_Syn_ip2_len),
                           ack_len = ttt.Average(e => e.Ack_Syn_ip2_len),
                       }).ToList();


            var syn1 = from p in syn
                       join q in ts on new { LAC = (long)p.bssgp_lac, CI = (long)p.bssgp_ci } equals new { q.LAC, q.CI }
                       select new
                       {
                           p.bssgp_lac,
                           p.bssgp_ci,
                           p.cnt,
                           p.syn_repeat_perc,
                           p.ack_syn_repeat_perc,
                           p.up_rtt,
                           p.down_rtt,
                           p.AckSyn_rate,
                           q.duration,
                           q.percent
                       };

            gridControl1.DataSource = syn1.OrderByDescending(e => e.down_rtt).ToList();

        }

        private void navBarItem20_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            var total = sqlserver_mpos_gb_gz.Sum(e => e.Duration);
            var gb = from p in sqlserver_mpos_gb_gz
                     group p by new { p.LAC, p.CI } into ttt
                     select new
                     {

                         LAC = ttt.Key.LAC,
                         CI = ttt.Key.CI,
                         //cnt = ttt.Count(),
                         duration = ttt.Sum(e => e.Duration),
                         percent = 1.0 * ttt.Sum(e => e.Duration) / total,

                     };

            gridControl1.DataSource = null;
            gridView1.PopulateColumns();
            gridControl1.DataSource = gb.OrderByDescending(e => e.duration);
        }

        private void navBarItem21_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();

            var lac = sqlserver_mpos_gb_gz.ToLookup(e => (int?)e.LAC, e => (int?)e.CI);
            var gb = gb_cell_syn
                .Where(e => e.bssgp_pdu_type == direction)
                .ToList()
            .Where(e => Convert.ToDecimal(e.Ack_tcp_seq) - Convert.ToDecimal(e.tcp_seq) == 1
            || Convert.ToDecimal(e.Ack_Push_tcp_seq) - Convert.ToDecimal(e.tcp_seq) == 1)
            .Where(e => e.Ack == 1)
            .Where(e => e.Ack_Syn != null || e.Ack_Push != null)
                //.Where(e => e.Ack_Syn != null && e.Ack_Push != null)
            .ToList();

            var syn = from p in gb
                      //where p.Ack_Syn_RepeatCounter + p.Syn_RepeatCounter == 0
                      group p by new { tr = lac[p.bssgp_lac].Contains(p.bssgp_ci) } into ttt
                      select new
                      {
                          ttt.Key,
                          //abc="1",
                          cnt = ttt.Count(),
                          repeat = 1.0 * ttt.Where(e => e.Syn_RepeatCounter > 0 || e.Ack_Syn_RepeatCounter > 0).Count() / ttt.Count(),
                          //syn_repeat_cnt = ttt.Where(e => e.Syn_RepeatCounter > 0).Count(),
                          syn_repeat_perc = 1.0 * ttt.Where(e => e.Syn_RepeatCounter > 0).Count() / ttt.Count(),
                          //ack_syn_repeat_cnt = ttt.Where(e => e.Ack_Syn_RepeatCounter > 0).Count(),
                          ack_syn_repeat_perc = 1.0 * ttt.Where(e => e.Ack_Syn_RepeatCounter > 0).Count() / ttt.Count(),
                          //seq_repeat = 1.0 * ttt.Where(e => Convert.ToDecimal(e.Ack_tcp_seq) - Convert.ToDecimal(e.tcp_seq) != 1).Count() / ttt.Count(),
                          //nu_repeat = 1.0 * ttt.Where(e => e.Ack_llcgprs_nu - e.llcgprs_nu > 1).Count() / ttt.Count(),
                          up_rtt = ttt.Average(e => e.Ack_Syn_delayFirst),
                          down_rtt = ttt.Average(e => (e.Ack_Push == null ? e.Ack_delayFirst : e.Ack_Push_delayFirst) - e.Ack_Syn_delayFirst),

                          Syn_rate2 = 2 * 1.0 * ttt.Sum(e => e.syn_ip2_len) / ttt.Sum(e => e.Ack_Syn_delayFirst),
                          AckSyn_rate2 = 2 * 1.0 * ttt.Sum(e => e.Ack_Syn_ip2_len) / ttt.Sum(e => e.Ack_delayFirst - e.Ack_Syn_delayFirst),

                          syn_len = ttt.Average(e => e.syn_ip2_len),
                          acksyn_len = ttt.Average(e => e.Ack_Syn_ip2_len),
                          ack_len = ttt.Average(e => e.Ack_Syn_ip2_len),

                      };

            gridControl1.DataSource = syn.OrderByDescending(e => e.down_rtt).ToList();
        }

        //M-Trix生成的XCDR
        private void navBarItem22_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();
            gridControl1.DataSource = gb_cell_repeat.OrderBy(e => e.BeginFrameNum).ThenBy(e => e.PacketNum);
            gridView1.OptionsView.ColumnAutoWidth = false;
        }

        //会话详细记录
        private void navBarItem23_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs eee)
        {
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();

            var imsi = sqlserver_mpos_gb_gz.Select(e => e.IMSI).Distinct().ToList();

            var sess = from p in mytcpsession
                       select new
                       {
                           ts_imsi = p.imsi == null ? false : imsi.Contains(p.imsi),
                           p.session_id,
                           p.direction,
                           p.bsc_ip,
                           p.bsc_bvci,
                           p.lac,
                           p.ci,
                           p.imsi,
                           p.duration,

                           p.llc_cnt,
                           p.llc_max,
                           p.llc_min,

                           //如果llc_discard>0为重传，如果llc_discard<0则为丢包
                           llc_discard = p.llc_cnt - (p.llc_max - p.llc_min) - 1,

                           p.ip2_total,
                           p.seq_total,
                           p.seq_ip2,
                           p.headersize,
                           flags = p.seq_ip2 + p.headersize,
                           p.packet_discard_total,
                           p.seq_rate,
                           p.ip2_rate,
                           p.packet_count,
                           p.packet_count_repeat,
                           p.packet_sack_total,
                           repeat_percent = p.packet_count == 0 ? 0 : (1.0 * p.packet_count_repeat / p.packet_count),
                           p.seq_nxt,
                           p.seq_max,
                           p.seq_min,

                       };

            gridControl1.DataSource = sess.ToList();

            gridView1.OptionsView.ColumnAutoWidth = false;
            // mytcp = sessions;
        }

        //重传率

        //List<myTcpSession> mytcp = new List<myTcpSession>();

        private void navBarItem24_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs eee)
        {
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();

            var sess = from p in mytcpsession
                       group p by p.lac into ttt
                       //group p by new { p.lac, p.ci } into ttt
                       select new
                       {
                           ttt.Key,
                           //ttt.Key.lac,
                           //ttt.Key.ci,
                           packet_cnt = ttt.Sum(e => e.packet_count),
                           packet_repeat = ttt.Sum(e => e.packet_count_repeat),
                           repeat_percent = ttt.Sum(e => e.packet_count) == 0 ? 0 : 1.0 * ttt.Sum(e => e.packet_count_repeat) / ttt.Sum(e => e.packet_count),
                           down_rate = ttt.Sum(e => (double)e.duration) == 0 ? 0 : ttt.Sum(e => (double)e.ip2_total) / ttt.Sum(e => (double)e.duration)
                       };


            gridControl1.DataSource = sess.ToList();
        }

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

        private void navBarItem26_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs eee)
        {
            var gb = gb_cell_repeat.ToLookup(e => e.PacketTime, e => e.FileNum);
            var maxt = gb.Max(e => e.Key);
            var mint = gb.Min(e => e.Key);
            TimeSpan ts = maxt.Value - mint.Value;
            var ttim = mint.ToString() + "-" + maxt.ToString() + "," + ts.TotalSeconds.ToString();
            textBox1.Text = ttim.ToString();

            gridView1.OptionsView.ColumnAutoWidth = true;

        }

        //执行数据库重新生成？

        private void navBarItem27_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {

        }

        private void navBarItem27_LinkClicked_1(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();

            var imsi = sqlserver_mpos_gb_gz.Select(e => e.IMSI).Distinct().ToList();


            // var lac = sqlserver_mpos_gb_gz.ToLookup(e => (int?)e.LAC, e => (int?)e.CI);
            var mytcp = mytcpsession.ToList();

            var syn = from p in mytcp
                      group p by new { tr = imsi.Contains(p.imsi) } into ttt
                      select new
                      {
                          ttt.Key.tr,
                          packet_cnt = ttt.Sum(e => e.packet_count),
                          packet_repeat = ttt.Sum(e => e.packet_count_repeat),
                          repeat_percent = ttt.Sum(e => e.packet_count) == 0 ? 0 : 1.0 * ttt.Sum(e => e.packet_count_repeat) / ttt.Sum(e => e.packet_count),
                          ip_rate = ttt.Sum(e => (double)e.duration) == 0 ? 0 : ttt.Sum(e => (double)e.ip2_total) / ttt.Sum(e => (double)e.duration),
                          seq_rate = ttt.Sum(e => (double)e.duration) == 0 ? 0 : ttt.Sum(e => (double)e.seq_total) / ttt.Sum(e => (double)e.duration)
                      };

            gridControl1.DataSource = syn.ToList();
        }

        private void navBarItem28_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();

            var lac = sqlserver_mpos_gb_gz.ToLookup(e => (int?)e.LAC, e => (int?)e.CI);
            var mytcp = mytcpsession.ToList();

            var syn = from p in mytcp
                      //where p.seq_min != 0 && p.seq_max != 0
                      //where p.seq_max != null && p.seq_min != null
                      group p by new { ts_cell = lac[p.lac].Contains(p.ci), p.lac, p.ci, p.direction } into ttt
                      select new
                      {
                          ttt.Key.ts_cell,
                          ttt.Key.lac,
                          ttt.Key.ci,
                          ttt.Key.direction,
                          packet_cnt = ttt.Sum(e => e.packet_count),
                          packet_repeat = ttt.Sum(e => e.packet_count_repeat),
                          packet_sack = ttt.Sum(e => e.packet_sack_total),
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

        private void navBarItem27_ItemChanged(object sender, EventArgs e)
        {

        }

        private void navBarItem27_LinkPressed(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {

        }

        private void navBarItem29_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();

            // var lac = sqlserver_mpos_gb_gz.ToLookup(e => (int?)e.LAC, e => (int?)e.CI);

            var mytcp = mytcpsession.ToList();

            var syn = from p in mytcp
                      group p by p.bsc_ip into ttt
                      select new
                      {
                          ttt.Key,
                          packet_cnt = ttt.Sum(e => e.packet_count),
                          packet_repeat = ttt.Sum(e => e.packet_count_repeat),
                          repeat_percent = ttt.Sum(e => e.packet_count) == 0 ? 0 : 1.0 * ttt.Sum(e => e.packet_count_repeat) / ttt.Sum(e => e.packet_count),
                          ip_rate = ttt.Sum(e => (double)e.duration) == 0 ? 0 : ttt.Sum(e => (double)e.ip2_total) / ttt.Sum(e => (double)e.duration),
                          seq_rate = ttt.Sum(e => (double)e.duration) == 0 ? 0 : ttt.Sum(e => (double)e.seq_total) / ttt.Sum(e => (double)e.duration)
                      };

            gridControl1.DataSource = syn.OrderByDescending(e => e.repeat_percent).ToList();
        }

        private void navBarItem30_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();

            //var lac = sqlserver_mpos_gb_gz.ToLookup(e => (int?)e.LAC, e => (int?)e.CI);

            var mytcp = mytcpsession.ToList();

            var syn = from p in mytcp
                      where p.bsc_ip == "10.128.10.6"
                      group p by p.ci into ttt
                      select new
                      {
                          ttt.Key,
                          packet_cnt = ttt.Sum(e => e.packet_count),
                          packet_repeat = ttt.Sum(e => e.packet_count_repeat),
                          repeat_percent = ttt.Sum(e => e.packet_count) == 0 ? 0 : 1.0 * ttt.Sum(e => e.packet_count_repeat) / ttt.Sum(e => e.packet_count),
                          ip_rate = ttt.Sum(e => (double)e.duration) == 0 ? 0 : ttt.Sum(e => (double)e.ip2_total) / ttt.Sum(e => (double)e.duration),
                          seq_rate = ttt.Sum(e => (double)e.duration) == 0 ? 0 : ttt.Sum(e => (double)e.seq_total) / ttt.Sum(e => (double)e.duration)
                      };

            gridControl1.DataSource = syn.OrderByDescending(e => e.repeat_percent).ToList();
        }


        //int64 ,decimal 的问题等等？？？？
        //????
        //？？？？？

    

        private void button1_Click(object sender, EventArgs ee)
        {

            ExecuteCmd("net stop mssqlserver");
            ExecuteCmd("net start mssqlserver");

            Guangzhou_GbEntities gb = new Guangzhou_GbEntities();
            gb.DeleteDatabase();
            gb.CreateDatabase();
            CreateTable("Down");
            CreateTable("Up");
        }

        public void ExecuteCmd(string cmd)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe");
            //调用程序名
            {
                startInfo.Arguments = "/C " + cmd;
                //调用命令 CMD
                startInfo.RedirectStandardError = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.UseShellExecute = true;
                startInfo.CreateNoWindow = false;
            }
        }
        private void navBarItem23_ItemChanged(object sender, EventArgs e)
        {

        }


    }

}
