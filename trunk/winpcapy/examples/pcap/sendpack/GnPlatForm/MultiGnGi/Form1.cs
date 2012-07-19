using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Objects;
//using MultiGnGi.MultiInterFace;
using MultiGnGi.ServiceReference1;
//using MultiGnGi.MutliInterfaceGnGi;
using System.ServiceModel;
using System.ServiceModel.Security;

using System.Data.Objects.SqlClient;

//using EntityClass.ServerEntity.Gi;
//using EntityClass.ServerEntity.Gw;
//using EntityClass.ServerEntity.Gn;
//using EntityClass.ServerEntity.Other;
using EntityClass.ServerEntity.Gb;
using EntityClass.ServerEntity.Gb_209;
using DevExpress.XtraCharts;

namespace MultiGnGi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void splitContainerControl1_SizeChanged(object sender, EventArgs e)
        {
            splitContainerControl1.SplitterPosition = 7 * splitContainerControl1.Width / 10;
        }

        //ObjectQuery<EntityClass.ServerEntity.Gi.GnGiGw_Get2x> gnget;
        //ObjectQuery<EntityClass.ServerEntity.Gw.GnGiGw_Get2x> giget;
        //ObjectQuery<EntityClass.ServerEntity.Other.GnGiGw_Http_Any_Multi> gngiget;
        private void Form1_Load(object sender, EventArgs e)
        {
            numericUpDown1.Value = 5;

            //GuangZhou_GiEntities1 gndb = new GuangZhou_GiEntities1();
            ////GuangZhou_GiEntities gndb = new GuangZhou_GiEntities();
            //GuangZhou_GiwEntities gidb = new GuangZhou_GiwEntities();
            ////Gn gndb = new Gn();
            ////Gi gidb = new Gi();
            //Guangzhou_GnGiEntities gngidb = new Guangzhou_GnGiEntities();

            //启动延迟加载
            //gngidb.ContextOptions.LazyLoadingEnabled = true;

            //gngidb.CommandTimeout = 0;
            //gnget = gndb.GnGiGw_Get2x;
            //giget = gidb.GnGiGw_Get2x;
            //gngiget = gngidb.GnGiGw_Http_Any_Multi;

            //http://www.cnblogs.com/LingzhiSun/archive/2011/04/27/EF_Trick4.html

            //gngiget.MergeOption = MergeOption.NoTracking;

            //我们可以这样来进行NoTracking查询：

            //gngiget.MergeOption = System.Data.Objects.MergeOption.NoTracking;

        }

        private void clearColumns()
        {
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();
        }
        private void formatColumns(int i)
        {
            gridView1.Columns[i].DisplayFormat.FormatString = "yyyy-MM-dd HH:mm:ss.fff";
            gridView1.Columns[i].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
        }

        private void navBarItem1_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            clearColumns();

            //var get = from p in gnget.ToList()
            //          join q in giget.ToList() on new { p.http_request_uri, p.http_user_agent, p.tcp_seq, p.tcp_nxtseq }
            //          equals new { q.http_request_uri, q.http_user_agent, q.tcp_seq, q.tcp_nxtseq }

            //          select new
            //          {
            //              gn_filenum = p.FileNum,
            //              gn_packetnum = p.PacketNum,
            //              gn_packettime = p.Get2x_time,
            //              gi_filenum = q.FileNum,
            //              gi_packetnum = q.PacketNum,
            //              gi_packettime = q.Get2x_time,
            //              dt = DateTime.Parse(p.Get2x_time) - DateTime.Parse(q.Get2x_time),
            //              gn_ip2 = q.ip2_dst_host,
            //              gi_ip2 = p.ip2_dst_host,
            //              p.http_user_agent,
            //              p.http_request_uri,
            //              p.tcp_seq,
            //              p.tcp_nxtseq,
            //          };

            //var dborder = get.OrderBy(e => e.gn_filenum).ThenBy(e => e.gn_packetnum);

            //gridControl1.DataSource = dborder.ToList();

            //formatColumns(1);
        }

        private void navBarItem2_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            //gridControl1.DataSource = null;
            //gridView1.PopulateColumns();
            //var dborder = gnget.OrderBy(e => e.PacketNum);
            //gridControl1.DataSource = dborder.ToList();
            //gridView1.OptionsView.ColumnAutoWidth = false;
        }

        private void navBarItem3_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            //gridControl1.DataSource = null;
            //gridView1.PopulateColumns();
            //var dborder = giget.OrderBy(e => e.PacketNum);
            //gridControl1.DataSource = dborder.ToList();
            //gridView1.OptionsView.ColumnAutoWidth = false;
        }

        //C# - Timestamp 與 DateTime 互轉

        private DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp).ToLocalTime();
        }

        private double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime dt = ConvertFromUnixTimestamp(double.Parse(textBox1.Text));
                listBox1.Items.Add(dt.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void navBarItem4_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            //clearColumns();
            //var dborder = gngiget.Take(100);
            //gridControl1.DataSource = dborder.ToList();
            //gridView1.OptionsView.ColumnAutoWidth = false;
        }

        private void navBarItem5_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {


            //var gngi = from p in gngiget
            //           group p by new { p.tcp_seq, p.tcp_nxtseq, p.tcp_ack, p.http_request_uri, p.http_user_agent } into ttt
            //           select new
            //           {
            //               ttt.Key.tcp_seq,
            //               ttt.Key.tcp_nxtseq,
            //               ttt.Key.tcp_ack,
            //               ttt.Key.http_request_uri,
            //               ttt.Key.http_user_agent,
            //               total_cnt = ttt.Count(),
            //               gtp_cnt = ttt.Where(e => e.gtp_flags_payload == "GTP").Count(),
            //               gtp_percent = 1.0 * ttt.Where(e => e.gtp_flags_payload == "GTP").Count() / ttt.Count(),
            //               packetfile = ttt.Select(e => e.FileNum),
            //               packetnum = ttt.Select(e => e.PacketNum),
            //               beginnum = ttt.Select(e => e.BeginFrameNum),
            //               ip_proto = ttt.Select(e => e.ip_proto),
            //               ip2_proto = ttt.Select(e => e.ip2_proto),
            //               ip_id = ttt.Select(e => e.ip_id),
            //               ip2_id = ttt.Select(e => e.ip2_id),
            //               udp_port = ttt.Select(e => e.udp_srcport == null ? 0 : e.udp_srcport),

            //           };
            //var gngi_list = from p in gngi.AsParallel().ToList()
            //                where p.gtp_cnt > 0
            //                select new
            //                    {
            //                        p.tcp_seq,
            //                        p.tcp_nxtseq,
            //                        p.tcp_ack,
            //                        p.http_request_uri,
            //                        p.http_user_agent,
            //                        p.total_cnt,
            //                        p.gtp_cnt,
            //                        p.gtp_percent,
            //                        packetfile = p.packetfile.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
            //                        packetnum = p.packetnum.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
            //                        beginnum = p.beginnum.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
            //                        ip_proto = p.ip_proto.Aggregate((a, b) => a + "," + b),
            //                        ip2_proto = p.ip2_proto.Aggregate((a, b) => a + "," + b),
            //                        ip_id = p.ip_id.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
            //                        ip2_id = p.ip2_id.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
            //                        udp_port = p.udp_port.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),

            //                    };
            //clearColumns();
            //var dborder = gngi_list.OrderByDescending(e => e.total_cnt);
            //gridControl1.DataSource = dborder.AsParallel().ToList();
            //gridView1.OptionsView.ColumnAutoWidth = false;
            //MessageBox.Show(gngi_list.Count().ToString());
        }

        private void navBarItem6_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            //var gngi = from p in gngiget.ToList()
            //           group p by new { p.tcp_seq, p.tcp_nxtseq, p.tcp_ack, p.http_request_uri, p.http_user_agent, p.ip2_id } into ttt
            //           select new
            //           {
            //               ttt.Key.tcp_seq,
            //               ttt.Key.tcp_nxtseq,
            //               ttt.Key.tcp_ack,
            //               ttt.Key.http_request_uri,
            //               ttt.Key.http_user_agent,
            //               ttt.Key.ip2_id,
            //               total_cnt = ttt.Count(),
            //               gtp_cnt = ttt.Where(e => e.gtp_flags_payload == "GTP").Count(),
            //               gtp_percent = 1.0 * ttt.Where(e => e.gtp_flags_payload == "GTP").Count() / ttt.Count(),
            //               packetfile = ttt.Select(e => e.FileNum.ToString()).Aggregate((a, b) => a + "," + b),
            //               packetnum = ttt.Select(e => e.PacketNum.ToString()).Aggregate((a, b) => a + "," + b),
            //               beginnum = ttt.Select(e => e.BeginFrameNum.ToString()).Aggregate((a, b) => a + "," + b),
            //               ip_proto = ttt.Select(e => e.ip_proto).Aggregate((a, b) => a + "," + b),
            //               ip2_proto = ttt.Select(e => e.ip2_proto).Aggregate((a, b) => a + "," + b),
            //               ip_id = ttt.Select(e => e.ip_id.ToString()).Aggregate((a, b) => a + "," + b),
            //               //ip2_id = ttt.Select(e => e.ip2_id.ToString()).Aggregate((a, b) => a + "," + b),

            //           };
            //clearColumns();
            //var dborder = gngi.OrderByDescending(e => e.total_cnt);
            //gridControl1.DataSource = dborder.ToList();
            //gridView1.OptionsView.ColumnAutoWidth = false;
        }

        private void navBarItem7_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            //var maxt = gngiget.Max(e => e.PacketTime);
            //var mint = gngiget.Min(e => e.PacketTime);
            //TimeSpan ts = maxt.Value - mint.Value;
            //var ttim = mint.Value.ToString() + "-" + maxt.Value.ToString() + "," + ts.TotalSeconds.ToString();
            //richTextBox1.Text = ttim.ToString();
        }

        private void navBarControl1_Click(object sender, EventArgs ee)
        {

        }

        private void navBarItem8_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {

            gridView1.ExportToXlsx(@"C:\" + numericUpDown1.Value.ToString() + ".xlsx");

        }

        private void navBarItem9_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {

            //var gngi = from p in gngiget
            //           group p by new { p.tcp_seq, p.tcp_nxtseq, p.tcp_ack, p.http_request_uri, p.http_user_agent } into ttt
            //           select new
            //           {
            //               ttt.Key.tcp_seq,
            //               ttt.Key.tcp_nxtseq,
            //               ttt.Key.tcp_ack,
            //               ttt.Key.http_request_uri,
            //               ttt.Key.http_user_agent,
            //               total_cnt = ttt.Count(),
            //               gtp_cnt = ttt.Where(e => e.gtp_flags_payload == "GTP").Count(),
            //               gtp_percent = 1.0 * ttt.Where(e => e.gtp_flags_payload == "GTP").Count() / ttt.Count(),

            //           };

            //var total = gngi.Where(e => e.gtp_cnt > 0).Count();

            //var gngifilter = from p in gngi
            //                 where p.gtp_cnt > 0
            //                 group p by new
            //                 {
            //                     gtp_percent_equals_50 = p.gtp_percent == 0.5,
            //                     gtp_percent_over_50 = p.gtp_percent > 0.5,
            //                     gtp_percent_less_50 = p.gtp_percent < 0.5
            //                 } into ttt
            //                 select new
            //                     {
            //                         ttt.Key.gtp_percent_equals_50,
            //                         ttt.Key.gtp_percent_over_50,
            //                         ttt.Key.gtp_percent_less_50,
            //                         cnt = ttt.Count(),
            //                         percent = 1.0 * ttt.Count() / total,
            //                     };
            //clearColumns();
            //var dborder = gngifilter.OrderByDescending(e => e.cnt);
            //gridControl1.DataSource = dborder.ToList();
            //gridView1.OptionsView.ColumnAutoWidth = false;
        }

        private void navBarItem10_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {


            //var gngi = from p in gngiget
            //           group p by new { p.tcp_seq, p.tcp_nxtseq, p.tcp_ack, p.http_request_uri, p.http_user_agent } into ttt
            //           select new
            //           {
            //               ttt.Key.tcp_seq,
            //               ttt.Key.tcp_nxtseq,
            //               ttt.Key.tcp_ack,
            //               ttt.Key.http_request_uri,
            //               ttt.Key.http_user_agent,
            //               total_cnt = ttt.Count(),
            //               gtp_cnt = ttt.Where(e => e.gtp_flags_payload == "GTP").Count(),
            //               gtp_percent = 1.0 * ttt.Where(e => e.gtp_flags_payload == "GTP").Count() / ttt.Count(),

            //               //判断是否nat转换的问题
            //               gre_cnt = ttt.Where(e => e.gtp_flags_payload == null && e.ip2_proto != null).Count(),
            //               nat_cnt = ttt.Where(e => e.gtp_flags_payload == null && e.ip2_proto == null).Count(),
            //               //判断地址的问题？
            //               ip_dst = ttt.Select(e => e.ip_dst_host),
            //               ip2_dst = ttt.Select(e => e.ip2_dst_host),
            //               //端口的问题？
            //               tcp_dst_port = ttt.Select(e => e.tcp_dstport),
            //               udp_dst_port = ttt.Select(e => e.tcp_dstport),
            //               //判断分片的问题？
            //               ip_seg = ttt.Select(e => e.ip_flags_mf),
            //               ip2_seg = ttt.Select(e => e.ip2_flags_mf),
            //               tcp_seg = ttt.Select(e => e.tcp_need_segment),
            //               //ttl的问题？
            //               ip_ttl = ttt.Select(e => e.ip_ttl),
            //               ip2_ttl = ttt.Select(e => e.ip2_ttl),

            //               packetfile = ttt.Select(e => e.FileNum),
            //               packetnum = ttt.Select(e => e.PacketNum),
            //               beginnum = ttt.Select(e => e.BeginFrameNum),
            //               ip_proto = ttt.Select(e => e.ip_proto),
            //               ip2_proto = ttt.Select(e => e.ip2_proto),
            //               ip_id = ttt.Select(e => e.ip_id),
            //               ip2_id = ttt.Select(e => e.ip2_id),


            //           };
            //var gngi_list = from p in gngi.AsParallel().ToList()
            //                where p.gtp_cnt > 0
            //                //where p.gtp_percent==0.5
            //                select new
            //                {
            //                    p.tcp_seq,
            //                    p.tcp_nxtseq,
            //                    p.tcp_ack,
            //                    p.http_request_uri,
            //                    p.http_user_agent,
            //                    p.total_cnt,
            //                    p.gtp_cnt,
            //                    p.gtp_percent,
            //                    p.gre_cnt,
            //                    p.nat_cnt,
            //                    //判断地址的问题？
            //                    ip_dst = p.ip_dst.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
            //                    ip2_dst = p.ip2_dst.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
            //                    //端口的问题？
            //                    tcp_dst_port = p.tcp_dst_port.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
            //                    udp_dst_port = p.udp_dst_port.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
            //                    //判断分片的问题？
            //                    ip_seg = p.ip_seg.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
            //                    ip2_seg = p.ip2_seg.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
            //                    tcp_seg = p.tcp_seg.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
            //                    //ttl的问题？
            //                    ip_ttl = p.ip_ttl.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
            //                    ip2_ttl = p.ip2_ttl.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),



            //                    packetfile = p.packetfile.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
            //                    packetnum = p.packetnum.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
            //                    beginnum = p.beginnum.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
            //                    ip_proto = p.ip_proto.Aggregate((a, b) => a + "," + b),
            //                    ip2_proto = p.ip2_proto.Aggregate((a, b) => a + "," + b),
            //                    ip_id = p.ip_id.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),
            //                    ip2_id = p.ip2_id.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b),


            //                };
            //clearColumns();
            //var dborder = gngi_list.OrderByDescending(e => e.total_cnt);
            //gridControl1.DataSource = dborder.AsParallel().ToList();
            //gridView1.OptionsView.ColumnAutoWidth = false;
            //MessageBox.Show(gngi_list.Count().ToString());
        }

        private void navBarItem11_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            //WCF编程,调用服务端的方法？
            WSHttpBinding theC = new WSHttpBinding();
            EndpointAddress address = new EndpointAddress("http://192.168.4.209:8732/Design_Time_Addresses/MultiInterFace/Service1/");
            Service1Client sv = new Service1Client(theC, address);
            MessageBox.Show(sv.GetData(1));
        }

        private void navBarItem12_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            WSHttpBinding theC = new WSHttpBinding();
            EndpointAddress address = new EndpointAddress("http://localhost:8732/Design_Time_Addresses/MutliInterfaceGnGi/Service1/");
            Service1Client sv = new Service1Client(theC, address);
            MessageBox.Show(sv.GetData(1));
        }

        private void navBarItem13_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            WSHttpBinding theC = new WSHttpBinding();
            EndpointAddress address = new EndpointAddress("http://localhost:8732/Design_Time_Addresses/MutliInterfaceGnGi/Service1/");
            Service1Client sv = new Service1Client(theC, address);
            clearColumns();
            DataSet ds = new DataSet();
            ds = sv.GetDataCollection(1);
            gridControl1.DataSource = ds.Tables[0];
            gridView1.OptionsView.ColumnAutoWidth = false;
        }

        private void initTcpBinding(ref NetTcpBinding nt)
        {
            nt.CloseTimeout = new TimeSpan(2, 1, 0);
            nt.OpenTimeout = new TimeSpan(2, 1, 0);
            nt.ReceiveTimeout = new TimeSpan(2, 20, 0);
            nt.SendTimeout = new TimeSpan(2, 1, 0);
            nt.MaxBufferSize = 2147483647;
            nt.MaxReceivedMessageSize = 2147483647;
        }

        private void initHttpBinding(ref WSHttpBinding ws)
        {
            ws.CloseTimeout = new TimeSpan(2, 1, 0);
            ws.OpenTimeout = new TimeSpan(2, 1, 0);
            ws.ReceiveTimeout = new TimeSpan(2, 20, 0);
            ws.SendTimeout = new TimeSpan(2, 1, 0);
            //ws.BypassProxyOnLocal = false;
            //ws.TransactionFlow = false;
            //ws.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
            ws.MaxBufferPoolSize = 2147483647;
            ws.MaxReceivedMessageSize = 2147483647;

            ws.MessageEncoding = WSMessageEncoding.Mtom;
            //ws.TextEncoding = Encoding.UTF8;
            //ws.UseDefaultWebProxy = true;
            //ws.AllowCookies = false;
            //ws.ReaderQuotas.MaxDepth = 6553600;
            //ws.ReaderQuotas.MaxStringContentLength = 2147483647;
            //ws.ReaderQuotas.MaxArrayLength = 6553600;
            //ws.ReaderQuotas.MaxBytesPerRead = 6553600;
            //ws.ReaderQuotas.MaxNameTableCharCount = 6553600;
            //ws.ReliableSession.Ordered = true;
            //ws.ReliableSession.InactivityTimeout = new TimeSpan(0, 20, 0);
            //ws.ReliableSession.Enabled = true;
            //ws.Security.Mode = SecurityMode.None;
            //ws.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            //ws.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
            //ws.Security.Transport.Realm = "";
            //ws.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
            //ws.Security.Message.NegotiateServiceCredential = true;
            //ws.Security.Message.AlgorithmSuite = SecurityAlgorithmSuite.Default;
            //ws.Security.Message.EstablishSecurityContext = false; 
        }

        private void execWcfTcpService(int value)
        {
            NetTcpBinding nt = new NetTcpBinding();
            initTcpBinding(ref nt);
            EndpointAddress address = new EndpointAddress("net.tcp://192.168.4.209:64567/Service1");
            Service1Client sv = new Service1Client(nt, address);
            clearColumns();
            DataSet ds = new DataSet();
            ds = sv.GetDataCollection(value);
            gridControl1.DataSource = ds.Tables[0];
            gridView1.OptionsView.ColumnAutoWidth = false;
        }

        private void execWcfService(int value)
        {
            WSHttpBinding ws = new WSHttpBinding();
            initHttpBinding(ref ws);
            EndpointAddress address = new EndpointAddress("http://192.168.4.209:8732/Design_Time_Addresses/MutliInterfaceGnGi/Service1/");
            Service1Client sv = new Service1Client(ws, address);
            clearColumns();
            DataSet ds = new DataSet();
            ds = sv.GetDataCollection(value);
            gridControl1.DataSource = ds.Tables[0];
            gridView1.OptionsView.ColumnAutoWidth = false;
        }

        private void navBarItem15_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            try
            {
                execWcfService((int)numericUpDown1.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void navBarItem16_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            NetTcpBinding nt = new NetTcpBinding();
            initTcpBinding(ref nt);
            EndpointAddress address = new EndpointAddress("net.tcp://192.168.4.209:64567/Service1");
            Service1Client sv = new Service1Client(nt, address);
            clearColumns();

            richTextBox1.Text = sv.GetData(10);

        }

        private void navBarItem17_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            try
            {
                execWcfTcpService((int)numericUpDown1.Value);
                //formatColumns(1);
                //formatColumns(2);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void navBarItem18_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            try
            {
                execWcfTcpService((int)numericUpDown1.Value);
                //formatColumns(1);
                //  formatColumns(2);
                //formatColumns(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /*
         * 
use GuangZhou_Gb
go
SELECT  * into  [Gb_Flow_Control_MS]
  FROM  gzserver.[GuangZhou_Gb].[dbo].[Gb_Flow_Control_MS]
*/

        private void navBarItem20_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            //var gb = new ExtendedGuangZhou_GbEntities();
            Guangzhou_GbEntities gb = new Guangzhou_GbEntities();
            //GuangZhou_GbEntities gb = new GuangZhou_GbEntities();
            //查询
            var fc = from p in gb.Gb_Flow_Control_MS
                     select new
                     {
                         p.BeginFrameNum,
                         p.PacketTime,
                         p.PacketNum,
                         p.bssgp_tlli,
                         p.bssgp_ms_bucket_size,
                         p.bssgp_bucket_leak_rate,

                     };

            //转换
            var fcl = from p in fc.ToList()
                      let bucket_size = Convert.ToInt64(p.bssgp_ms_bucket_size)
                      let leak_rate = Convert.ToInt64(p.bssgp_bucket_leak_rate)
                      select new
                      {
                          p.BeginFrameNum,
                          p.PacketTime,
                          p.PacketNum,
                          p.bssgp_tlli,
                          bucket_size,
                          leak_rate,
                      };

            //分组
            var query = from p in fcl
                        group p by new { p.BeginFrameNum, p.bssgp_tlli } into ttt
                        select new
                        {
                            ttt.Key.BeginFrameNum,
                            ttt.Key.bssgp_tlli,
                            cnt = ttt.Count(),
                            bucket_size_avg = ttt.Average(e => e.bucket_size) / 1000,
                            bucket_size_min = ttt.Min(e => e.bucket_size) / 1000,
                            bucket_size_max = ttt.Max(e => e.bucket_size) / 1000,
                            leak_rate_avg = ttt.Average(e => e.leak_rate) / 1000,
                            leak_rate_min = ttt.Min(e => e.leak_rate) / 1000,
                            leak_rate_max = ttt.Max(e => e.leak_rate) / 1000,

                            first_delay = ttt.OrderBy(e => e.PacketNum)
                                                .Select(e => new { fd = (e.PacketTime.Value - ttt.Min(f => f.PacketTime).Value).TotalSeconds })
                                                .Select(e => ((int)e.fd).ToString())
                                                .Aggregate((a, b) => a + "," + b),

                            leak_rate = ttt.OrderBy(e => e.PacketNum)
                                            .Select(e => ((int)e.leak_rate).ToString())
                                            .Aggregate((a, b) => a + "," + b),

                            bucket_size = ttt.OrderBy(e => e.PacketNum)
                                             .Select(e => ((int)e.bucket_size).ToString())
                                             .Aggregate((a, b) => a + "," + b),

                        };

            clearColumns();
            var dborder = query.OrderByDescending(e => e.cnt).Take(100);
            gridControl1.DataSource = dborder.AsParallel().ToList();
            gridView1.OptionsView.ColumnAutoWidth = false;
        }

        private void chartControl1_Click(object sender, EventArgs ee)
        {
            //chartControl1.ExportToXlsx(@"C:\" + numericUpDown1.Value.ToString() + ".xlsx");

        }


        //选中其他数据进行研究
        /*
         * 
          use GuangZhou_Gb
go
SELECT  * into  [Gb_FlowControlx]
  FROM  gzserver.[GuangZhou_Gb].[dbo].[Gb_FlowControlx]
         * */

        private void navBarItem21_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            //var gb = new ExtendedGuangZhou_GbEntities();
            Guangzhou_GbEntities gb = new Guangzhou_GbEntities();
            gb.ContextOptions.LazyLoadingEnabled = true;
            gb.Gb_FlowControlx.MergeOption = MergeOption.NoTracking;
            //GuangZhou_GbEntities gb = new GuangZhou_GbEntities();
            //查询
            var fc = gb.Gb_FlowControlx.ToLookup(e => new
            {
                e.BeginFrameNum,
                e.PacketNum,
                //e.PacketTime,
                e.Flow_Control_time,
                e.Flow_Control_MsgType,
                e.bssgp_direction,
                //e.bssgp_tlli,
                e.bssgp_ms_bucket_size,
                e.bssgp_bucket_leak_rate,
                e.ip_len
            }, null); //为空更加能加少内存

            //MessageBox.Show(fc.Count().ToString());
            //转换
            var fcl = from p in fc.Select(e => e.Key)
                      let bucket_size = Convert.ToDouble(p.bssgp_ms_bucket_size) / 1000.0   //转换成KByte
                      let leak_rate = Convert.ToDouble(p.bssgp_bucket_leak_rate) / 1000.0  //转化成kbps
                      let Flow_Control_time = DateTime.Parse(p.Flow_Control_time)
                      select new
                      {
                          p.BeginFrameNum,
                          Flow_Control_time,
                          p.PacketNum,
                          p.ip_len,
                          p.Flow_Control_MsgType,
                          p.bssgp_direction,
                          //p.Key.bssgp_tlli,
                          bucket_size,
                          leak_rate,
                      };

            //MessageBox.Show(fcl.Count().ToString());

            string fc_msg = "BSSGP.FLOW-CONTROL-MS";
            //分组
            var query = from p in fcl
                        group p by new { p.BeginFrameNum } into ttt
                        select new FlowControlOneMs
                        {
                            _id = ttt.Key.BeginFrameNum,
                            BeginFrameNum = ttt.Key.BeginFrameNum,
                            //ttt.Key.bssgp_tlli,
                            fcontrol_cnt = ttt.Where(e => e.Flow_Control_MsgType == fc_msg).Count(),
                            packet_cnt = ttt.Count(),
                            bucket_size_avg = ttt.Where(e => e.Flow_Control_MsgType == fc_msg).Average(e => e.bucket_size),
                            bucket_size_min = ttt.Where(e => e.Flow_Control_MsgType == fc_msg).Min(e => e.bucket_size),
                            bucket_size_max = ttt.Where(e => e.Flow_Control_MsgType == fc_msg).Max(e => e.bucket_size),
                            leak_rate_avg = ttt.Where(e => e.Flow_Control_MsgType == fc_msg).Average(e => e.leak_rate),
                            leak_rate_min = ttt.Where(e => e.Flow_Control_MsgType == fc_msg).Min(e => e.leak_rate),
                            leak_rate_max = ttt.Where(e => e.Flow_Control_MsgType == fc_msg).Max(e => e.leak_rate),

                            first_delay = ttt.Where(e => e.Flow_Control_MsgType == fc_msg)
                            .OrderBy(e => e.PacketNum)
                                                .Select(e => new { fd = (e.Flow_Control_time - ttt.Min(f => f.Flow_Control_time)).TotalMilliseconds })
                                                .Select(e => (e.fd / 1000).ToString("f1"))
                                                .Aggregate((a, b) => a + "," + b),

                            leak_rate = ttt.Where(e => e.Flow_Control_MsgType == fc_msg)
                            .OrderBy(e => e.PacketNum)
                                            .Select(e => (e.leak_rate).ToString("f1"))
                                            .Aggregate((a, b) => a + "," + b),

                            bucket_size = ttt.Where(e => e.Flow_Control_MsgType == fc_msg)
                            .OrderBy(e => e.PacketNum)
                                             .Select(e => (e.bucket_size).ToString("f1"))
                                             .Aggregate((a, b) => a + "," + b),

                            //用扩展方法实现， 在fc消息之间进行ip.len的累加

                            down_total_len = ttt
                                // .Where(e => e.bssgp_direction == "Down")
                            .OrderBy(e => e.PacketNum)
                            .AggregateSum(e => (int)e.ip_len, e => e.Flow_Control_MsgType, fc_msg),

                            down_packet_rate = ttt
                                // .Where(e => e.bssgp_direction == "Down")
               .OrderBy(e => e.PacketNum)
               .AggregatePacketRate(e => (int)e.ip_len, e => e.Flow_Control_time, e => e.Flow_Control_MsgType, fc_msg),

                            fcm_time = ttt
                                // .Where(e => e.bssgp_direction == "Down")
.OrderBy(e => e.PacketNum)
.AggregatePacketTime(e => e.Flow_Control_time, e => e.Flow_Control_MsgType, fc_msg),

                        };

            FlowControlOneMs fcos = new FlowControlOneMs();
            fcos.BulkMongo(query.ToList());

            MessageBox.Show("OK");



        }

        private void ClearChart()
        {
            var diagram = chartControl1.Diagram as XYDiagram;
            if (diagram != null)
            {
                diagram.SecondaryAxesY.Clear();
                diagram.SecondaryAxesX.Clear();
                diagram.Panes.Clear();
                diagram.AxisY.Range.Auto = true;
                diagram.AxisY.Range.Auto = true;

            }
            chartControl1.Series.Clear();
            chartControl1.Titles.Clear();

        }


        private void gridControl1_Click(object sender, EventArgs ee)
        {
            ClearChart();

            int[] a = this.gridView1.GetSelectedRows(); //传递实体类过去 获取选中的行
            string fd = this.gridView1.GetRowCellValue(a[0], "first_delay").ToString();//获取选中行的内容
            string lr = this.gridView1.GetRowCellValue(a[0], "leak_rate").ToString();//获取选中行的内容
            string bs = this.gridView1.GetRowCellValue(a[0], "bucket_size").ToString();//获取选中行的内容
            string len = this.gridView1.GetRowCellValue(a[0], "down_total_len").ToString();//获取选中行的内容
            string pr = this.gridView1.GetRowCellValue(a[0], "down_packet_rate").ToString();//获取选中行的内容
            string times = this.gridView1.GetRowCellValue(a[0], "fcm_time").ToString();//获取选中行的内容
            string callid = this.gridView1.GetRowCellValue(a[0], "BeginFrameNum").ToString();//获取选中行的内容

            var arrfd = fd.Split(',');
            var arrlr = lr.Split(',');
            var arrbs = bs.Split(',');
            var arrlen = len.Split(',');
            var arrpr = pr.Split(',');
            var arrts = times.Split(',');

            textBox7.Text = "Callid=" + callid;

            textBox1.Text = "first_delay(flow-control-ms发送时间序列second)：平均值=" + arrfd.Where(e => e != "").Average(e => double.Parse(e)).ToString("f1") + "," +
                                        "最大值=" + arrfd.Where(e => e != "").Max(e => double.Parse(e)).ToString("f1") + "," +
                                        "最小值" + arrfd.Where(e => e != "").Min(e => double.Parse(e)).ToString("f1") + "," +
                                        "时间序列=" + fd;
            textBox2.Text = "leak_rate(flow-control-ms参数值kbps)：平均值=" + arrlr.Where(e => e != "").Average(e => double.Parse(e)).ToString("f1") + "," +
                                        "最大值=" + arrlr.Where(e => e != "").Max(e => double.Parse(e)).ToString("f1") + "," +
                                        "最小值" + arrlr.Where(e => e != "").Min(e => double.Parse(e)).ToString("f1") + "," +
                                        "时间序列=" + lr;
            textBox3.Text = "bucket_size(flow-control-ms参数值Kbyte)：平均值=" + arrbs.Where(e => e != "").Average(e => double.Parse(e)).ToString("f1") + "," +
                                        "最大值=" + arrbs.Where(e => e != "").Max(e => double.Parse(e)).ToString("f1") + "," +
                                        "最小值" + arrbs.Where(e => e != "").Min(e => double.Parse(e)).ToString("f1") + "," +
                                        "时间序列=" + bs;
            textBox4.Text = "down_total_len(flow-control-ms之间MS下行包总长度Kbyte)：平均值=" + arrlen.Where(e => e != "").Average(e => double.Parse(e)).ToString("f1") + "," +
                                        "最大值=" + arrlen.Where(e => e != "").Max(e => double.Parse(e)).ToString("f1") + "," +
                                        "最小值" + arrlen.Where(e => e != "").Min(e => double.Parse(e)).ToString("f1") + "," +
                                        "时间序列=" + len;
            textBox5.Text = "fcm_time(flow-control-ms发送时间间隔second)：平均值=" + arrts.Where(e => e != "").Average(e => double.Parse(e)).ToString("f1") + "," +
                                        "最大值=" + arrts.Where(e => e != "").Max(e => double.Parse(e)).ToString("f1") + "," +
                                        "最小值" + arrts.Where(e => e != "").Min(e => double.Parse(e)).ToString("f1") + "," +
                                        "时间序列=" + times;
            textBox6.Text = "down_packet_rate(flow-control-ms之间MS下行速率kbps)：平均值=" + arrpr.Where(e => e != "").Average(e => double.Parse(e)).ToString("f1") + "," +
                                        "最大值=" + arrpr.Where(e => e != "").Max(e => double.Parse(e)).ToString("f1") + "," +
                                        "最小值" + arrpr.Where(e => e != "").Min(e => double.Parse(e)).ToString("f1") + "," +
                                        "时间序列=" + pr;



            Series series1 = new Series("leak_rate(kbps)", ViewType.Line);
            Series series4 = new Series("down_packet_rate(kbps)", ViewType.Line);

            Series series2 = new Series("bucket_size(KByte)", ViewType.Line);
            Series series3 = new Series("down_total_len(KByte)", ViewType.Line);

            Series series5 = new Series("fcm_time(seconds)", ViewType.Line);

            double x, y, z, m, n, k;
            for (int i = 0; i < arrfd.Count(); i++)
            {
                x = double.Parse(arrfd[i]);
                y = double.Parse(arrlr[i]);
                z = double.Parse(arrbs[i]);
                m = double.Parse(arrlen[i]);
                n = double.Parse(arrpr[i]);
                k = double.Parse(arrts[i]);
                series1.Points.Add(new SeriesPoint(x, y));
                series2.Points.Add(new SeriesPoint(x, z));
                series3.Points.Add(new SeriesPoint(x, m));
                series4.Points.Add(new SeriesPoint(x, n));
                series5.Points.Add(new SeriesPoint(x, k));
            }

            chartControl1.Series.Add(series1);
            chartControl1.Series.Add(series4);


            chartControl1.Series.Add(series3);
            chartControl1.Series.Add(series2);

            chartControl1.Series.Add(series5);


            ChartTitle chartTitle1 = new ChartTitle();
            chartTitle1.Antialiasing = true;
            chartTitle1.Font = new Font("Tahoma", 12, FontStyle.Bold);
            chartTitle1.Text = string.Format("Callid={0} down_packet_rate和FLOW-CONTROL-MS的Bucket_Size、Leak_Rate时间走势", callid);
            chartControl1.Titles.Add(chartTitle1);


            series1.Label.Visible = true;
            series2.Label.Visible = true;
            series3.Label.Visible = true;
            series4.Label.Visible = true;
            series5.Label.Visible = true;


            //chartControl1.Legend.TextColor = series1.Label.LineColor;
            chartControl1.Legend.Antialiasing = true;
            chartControl1.Legend.Font = new Font("Tahoma", 10, FontStyle.Bold);



            XYDiagram diagram = (XYDiagram)chartControl1.Diagram;



            // Customize the appearance of the Y-axis title.
            diagram.AxisY.Title.Visible = true;
            diagram.AxisY.Title.Alignment = StringAlignment.Center;
            diagram.AxisY.Title.Text = "kbps";
            diagram.AxisY.Title.TextColor = Color.Blue;
            diagram.AxisY.Title.Antialiasing = true;
            diagram.AxisY.Title.Font = new Font("Tahoma", 10, FontStyle.Bold);

            diagram.AxisX.Visible = false;


            // Add secondary axes to the diagram, and adjust their options.
            diagram.SecondaryAxesX.Add(new SecondaryAxisX("My Axis X"));
            diagram.SecondaryAxesY.Add(new SecondaryAxisY("My Axis Y"));
            diagram.SecondaryAxesX[0].Alignment = AxisAlignment.Near;
            diagram.SecondaryAxesY[0].Alignment = AxisAlignment.Near;

            // Add a new additional pane to the diagram.
            diagram.Panes.Add(new XYDiagramPane("My Pane"));

            // Assign both the additional pane and, if required,
            // the secondary axes to the second series. 
            LineSeriesView myView = (LineSeriesView)series2.View;
            myView.AxisX = diagram.SecondaryAxesX[0];
            myView.AxisY = diagram.SecondaryAxesY[0];
            // Note that the created pane has the zero index in the collection,
            // because the existing Default pane is a separate entity.
            myView.Pane = diagram.Panes[0];

            // Assign both the additional pane and, if required,
            // the secondary axes to the second series. 
            myView = (LineSeriesView)series3.View;
            myView.AxisX = diagram.SecondaryAxesX[0];
            myView.AxisY = diagram.SecondaryAxesY[0];
            // Note that the created pane has the zero index in the collection,
            // because the existing Default pane is a separate entity.
            myView.Pane = diagram.Panes[0];

            // Customize the appearance of the Y-axis title.
            myView.AxisY.Title.Visible = true;
            myView.AxisY.Title.Alignment = StringAlignment.Center;
            myView.AxisY.Title.Text = "KByte";
            myView.AxisY.Title.TextColor = Color.Blue;
            myView.AxisY.Title.Antialiasing = true;
            myView.AxisY.Title.Font = new Font("Tahoma", 10, FontStyle.Bold);

            myView.AxisX.Visible = false;

            // Add secondary axes to the diagram, and adjust their options.
            diagram.SecondaryAxesX.Add(new SecondaryAxisX("My Axis X1"));
            diagram.SecondaryAxesY.Add(new SecondaryAxisY("My Axis Y1"));
            diagram.SecondaryAxesX[1].Alignment = AxisAlignment.Near;
            diagram.SecondaryAxesY[1].Alignment = AxisAlignment.Near;

            // Add a new additional pane to the diagram.
            diagram.Panes.Add(new XYDiagramPane("My Pane1"));
            // Assign both the additional pane and, if required,
            // the secondary axes to the second series. 
            LineSeriesView myView1 = (LineSeriesView)series5.View;
            myView1.AxisX = diagram.SecondaryAxesX[1];
            myView1.AxisY = diagram.SecondaryAxesY[1];
            // Note that the created pane has the zero index in the collection,
            // because the existing Default pane is a separate entity.
            myView1.Pane = diagram.Panes[1];


            // Customize the appearance of the Y-axis title.
            myView1.AxisY.Title.Visible = true;
            myView1.AxisY.Title.Alignment = StringAlignment.Center;
            myView1.AxisY.Title.Text = "second";
            myView1.AxisY.Title.TextColor = Color.Blue;
            myView1.AxisY.Title.Antialiasing = true;
            myView1.AxisY.Title.Font = new Font("Tahoma", 10, FontStyle.Bold);

            // Customize the appearance of the X-axis title.
            myView1.AxisX.Title.Visible = true;
            myView1.AxisX.Title.Alignment = StringAlignment.Center;
            myView1.AxisX.Title.Text = "FLOW-CONTROL-MS delay(seconds)";
            myView1.AxisX.Title.TextColor = Color.Red;
            myView1.AxisX.Title.Antialiasing = true;
            myView1.AxisX.Title.Font = new Font("Tahoma", 10, FontStyle.Bold);
            myView1.AxisX.Label.Staggered = true;



            // Customize the layout of the diagram's panes.
            diagram.PaneDistance = 10;
            diagram.PaneLayoutDirection = PaneLayoutDirection.Vertical;
            diagram.DefaultPane.SizeMode = PaneSizeMode.UseWeight;
            diagram.DefaultPane.Weight = 1.2;



            diagram.SecondaryAxesY[0].LogarithmicBase = 2;
            diagram.SecondaryAxesY[0].Logarithmic = true;
            diagram.AxisY.LogarithmicBase = 2;
            diagram.AxisY.Logarithmic = true;

            //SecondaryAxisY myAxisY = new SecondaryAxisY("my Y-Axis");
            //myAxisY.Title.Text="KByte";

            //diagram.SecondaryAxesY.Add(myAxisY);
            //((LineSeriesView)series2.View).AxisY = myAxisY;
            //diagram.SecondaryAxesY.Add(myAxisY);
            //((PointSeriesView)series5.View).AxisY = myAxisY;

            //this.xtraTabPage3.Controls.Add(chartControl1);

            //this.Controls.Add(chartControl1);
            //series1.Points.Clear();
            //series2.Points.Clear();

        }

        private void navBarItem22_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            FlowControlOneMs fcos = new FlowControlOneMs();
            var query = from p in fcos.QueryMongo()
                        select new
                        {
                            p.BeginFrameNum,
                            p.fcontrol_cnt,
                            p.packet_cnt,
                            p.leak_rate_avg,
                            p.leak_rate_max,
                            p.leak_rate_min,
                            p.bucket_size_avg,
                            p.bucket_size_max,
                            p.bucket_size_min,

                            p.down_packet_rate,
                            p.down_total_len,
                            p.fcm_time,
                            p.bucket_size,
                            p.first_delay,
                            p.leak_rate,


                        };
            clearColumns();
            var dborder = query.OrderByDescending(e => e.fcontrol_cnt);
            gridControl1.DataSource = dborder.AsParallel().ToList();
            gridView1.OptionsView.ColumnAutoWidth = false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Series series in chartControl1.Series)
                if (series.Label != null)
                    series.Label.Visible = checkBox1.Checked;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox7.Text + "\r\n" +
                       textBox1.Text + "\r\n" +
                textBox2.Text + "\r\n" +
                textBox3.Text + "\r\n" +
                textBox4.Text + "\r\n" +
                textBox5.Text + "\r\n" +
                textBox6.Text);

        }

        private void button3_Click(object sender, EventArgs e)
        {

            Clipboard.SetText(textBox7.Text + "\r\n" +
          textBox1.Text.GetHeader(",时间序列") + "\r\n" +
             textBox2.Text.GetHeader(",时间序列") + "\r\n" +
          textBox3.Text.GetHeader(",时间序列") + "\r\n" +
          textBox4.Text.GetHeader(",时间序列") + "\r\n" +
          textBox5.Text.GetHeader(",时间序列") + "\r\n" +
          textBox6.Text.GetHeader(",时间序列"));
        }

        /*计算bvci级流量控制
         *    ,[bssgp_ms_bucket_size]
      ,[bssgp_bucket_leak_rate]
      ,[bssgp_bvc_bucket_size]
      ,[bssgp_bmax_default_ms]
      ,[bssgp_R_default_ms]
      ,[bssgp_bucket_full_ratio]
         * */

        //再弄一个库出来
        private void navBarItem23_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            FlowControlOneBvc fcob = new FlowControlOneBvc();
            fcob.CreateCollection();
            MessageBox.Show("ok");
        }


        //先弄一个库出来，
        private void navBarItem24_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {

            LacCellBvci lcb = new LacCellBvci();
            lcb.CreatCollection();
            MessageBox.Show("ok");
        }

        private void navBarItem25_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            FlowControlOneBvc fcob = new FlowControlOneBvc();
            var query = from p in fcob.QueryMongo()
                        select new
                            {
                                p.lac_cell,
                                p.PacketNum,
                                p.Flow_Control_MsgType,
                                p.Flow_Control_time,
                            };
            clearColumns();
            var dborder = query.Take(100);
            gridControl1.DataSource = dborder.AsParallel().ToList();
            gridView1.OptionsView.ColumnAutoWidth = false;
        }

        private void navBarItem26_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            LacCellBvci lcb = new LacCellBvci();
            var query = from p in lcb.QueryMongo()
                        select new
                            {
                                p.lac_cell,
                                p.src,
                                p.dst,
                                p.bvci,
                            };
            clearColumns();
            var dborder = query.OrderBy(e => e.lac_cell);
            gridControl1.DataSource = dborder.AsParallel().ToList();
            gridView1.OptionsView.ColumnAutoWidth = false;
        }
    }
}

