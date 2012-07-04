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

        ObjectQuery<Gn_Get2x_Multi> gnget;
        ObjectQuery<Gi_Get2x_Multi> giget;
        ObjectQuery<GnGi_Get2x_Multi> gngiget;
        private void Form1_Load(object sender, EventArgs e)
        {
            numericUpDown1.Value = 5;

            Gn gndb = new Gn();
            Gi gidb = new Gi();
            Guangzhou_GnGiEntities gngidb = new Guangzhou_GnGiEntities();

            //启动延迟加载
            gngidb.ContextOptions.LazyLoadingEnabled = true;

            gngidb.CommandTimeout = 0;
            gnget = gndb.Gn_Get2x_Multi;
            giget = gidb.Gi_Get2x_Multi;
            gngiget = gngidb.GnGi_Get2x_Multi;

            //http://www.cnblogs.com/LingzhiSun/archive/2011/04/27/EF_Trick4.html

            //gngiget.MergeOption = MergeOption.NoTracking;

            //我们可以这样来进行NoTracking查询：

            gngiget.MergeOption = System.Data.Objects.MergeOption.NoTracking;

        }

        private void clearColumns()
        {
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();
        }
        private void formatColumns(int i)
        {
            gridView1.Columns[i].DisplayFormat.FormatString = "yyyy-MM-dd HH:mm:ss";
            gridView1.Columns[i].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
        }

        private void navBarItem1_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            clearColumns();

            var get = from p in gnget.ToList()
                      join q in giget.ToList() on new { p.Request_URI, p.User_Agent, p.tcp_seq, p.tcp_nxtseq }
                      equals new { q.Request_URI, q.User_Agent, q.tcp_seq, q.tcp_nxtseq }

                      select new
                      {
                          gn_filenum = p.FileNum,
                          gn_packetnum = p.PacketNum,
                          gn_packettime = p.Get2x_time,
                          gi_filenum = q.FileNum,
                          gi_packetnum = q.PacketNum,
                          gi_packettime = q.Get2x_time,
                          dt = DateTime.Parse(p.Get2x_time) - DateTime.Parse(q.Get2x_time),
                          gn_ip2 = q.Dest_IP2,
                          gi_ip2 = p.Dest_IP2,
                          p.User_Agent,
                          p.Request_URI,
                          p.tcp_seq,
                          p.tcp_nxtseq,
                      };

            var dborder = get.OrderBy(e => e.gn_filenum).ThenBy(e => e.gn_packetnum);

            gridControl1.DataSource = dborder.ToList();

            //formatColumns(1);
        }

        private void navBarItem2_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();
            var dborder = gnget.OrderBy(e => e.PacketNum);
            gridControl1.DataSource = dborder.ToList();
            gridView1.OptionsView.ColumnAutoWidth = false;
        }

        private void navBarItem3_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();
            var dborder = giget.OrderBy(e => e.PacketNum);
            gridControl1.DataSource = dborder.ToList();
            gridView1.OptionsView.ColumnAutoWidth = false;
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
            clearColumns();
            var dborder = gngiget.Take(100);
            gridControl1.DataSource = dborder.ToList();
            gridView1.OptionsView.ColumnAutoWidth = false;
        }

        private void navBarItem5_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {


            var gngi = from p in gngiget
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
            clearColumns();
            var dborder = gngi_list.OrderByDescending(e => e.total_cnt);
            gridControl1.DataSource = dborder.AsParallel().ToList();
            gridView1.OptionsView.ColumnAutoWidth = false;
            MessageBox.Show(gngi_list.Count().ToString());
        }

        private void navBarItem6_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            var gngi = from p in gngiget.ToList()
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
            clearColumns();
            var dborder = gngi.OrderByDescending(e => e.total_cnt);
            gridControl1.DataSource = dborder.ToList();
            gridView1.OptionsView.ColumnAutoWidth = false;
        }

        private void navBarItem7_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            var maxt = gngiget.Max(e => e.PacketTime);
            var mint = gngiget.Min(e => e.PacketTime);
            TimeSpan ts = maxt.Value - mint.Value;
            var ttim = mint.Value.ToString() + "-" + maxt.Value.ToString() + "," + ts.TotalSeconds.ToString();
            richTextBox1.Text = ttim.ToString();
        }

        private void navBarControl1_Click(object sender, EventArgs ee)
        {

        }

        private void navBarItem8_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Excel2007文件(*.xlsx) |*.xlsx";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                gridView1.ExportToXlsx(saveFileDialog1.FileName);
            }
        }

        private void navBarItem9_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {

            var gngi = from p in gngiget
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
            clearColumns();
            var dborder = gngifilter.OrderByDescending(e => e.cnt);
            gridControl1.DataSource = dborder.ToList();
            gridView1.OptionsView.ColumnAutoWidth = false;
        }

        private void navBarItem10_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {


            var gngi = from p in gngiget
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
                           ip_dst = ttt.Select(e => e.Dest_IP),
                           ip2_dst = ttt.Select(e => e.Dest_IP2),
                           //端口的问题？
                           tcp_dst_port = ttt.Select(e => e.tcp_dstport),
                           udp_dst_port = ttt.Select(e => e.Dest_Port),
                           //判断分片的问题？
                           ip_seg = ttt.Select(e => e.ip_flags_mf),
                           ip2_seg = ttt.Select(e => e.ip2_flags_mf),
                           tcp_seg = ttt.Select(e => e.tcp_need_segment),
                           //ttl的问题？
                           ip_ttl = ttt.Select(e => e.ip_ttl),
                           ip2_ttl = ttt.Select(e => e.ip2_ttl),

                           packetfile = ttt.Select(e => e.FileNum),
                           packetnum = ttt.Select(e => e.PacketNum),
                           beginnum = ttt.Select(e => e.BeginFrameNum),
                           ip_proto = ttt.Select(e => e.ip_proto),
                           ip2_proto = ttt.Select(e => e.ip2_proto),
                           ip_id = ttt.Select(e => e.ip_id),
                           ip2_id = ttt.Select(e => e.ip2_id),


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
            clearColumns();
            var dborder = gngi_list.OrderByDescending(e => e.total_cnt);
            gridControl1.DataSource = dborder.AsParallel().ToList();
            gridView1.OptionsView.ColumnAutoWidth = false;
            MessageBox.Show(gngi_list.Count().ToString());
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
            WSHttpBinding theC = new WSHttpBinding();
            EndpointAddress address = new EndpointAddress("http://192.168.4.209:8732/Design_Time_Addresses/MutliInterfaceGnGi/Service1/");
            Service1Client sv = new Service1Client(theC, address);

            richTextBox1.Text = sv.GetData(10);

        }

        private void navBarItem17_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            try
            {
                execWcfTcpService((int)numericUpDown1.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}

