using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Objects;

namespace XID
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Guangzhou_GbEntities gb = new Guangzhou_GbEntities();
        private ObjectQuery<Gb_GMMSM_XID> gb_gmm_xid;
        private ObjectQuery<Gb_XID> gb_xid;
        private void Form1_Load(object sender, EventArgs e)
        {
            gb_gmm_xid = gb.Gb_GMMSM_XID;
            gb_xid = gb.Gb_XID;


        }

        private void navBarItem1_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();

            var gmm = gb_gmm_xid.ToLookup(a => a.BeginFrameNum);
            //var xid = gb_xid.ToLookup(a => a.BeginFrameNum);
            var xid2 = gb_xid.Where(e => e.llcgprs_xid1type == 5).ToList();

            var xid1 = from p in xid2
                       let packetnum = gmm[p.BeginFrameNum]
                                .Where(e => e.mGMMSM_MsgType != "GMMSM.GMM Information")
                       .Where(e => e.mGMMSM_MsgType != "GMMSM.SM Status")
                       .Where(e => e.PacketNum < p.PacketNum)  //gmm的帧号要小于xid的帧号
                       .OrderByDescending(e => e.PacketNum)
                       .Select(e => e.PacketNum).FirstOrDefault()
                       let tpacketnum = gmm[p.BeginFrameNum].Where(e => e.PacketNum == packetnum)
                       select new
                       {
                           p.BeginFrameNum,
                           gmmPacketNum = packetnum,
                           p.PacketNum,
                           gmmPacketTime = tpacketnum.Count() == 0 ? p.PacketTime : tpacketnum.Select(e => e.PacketTime).FirstOrDefault().Value.AddMilliseconds((int)tpacketnum.Select(e => e.PacketTime_ms_).FirstOrDefault().Value),
                           PacketTime = p.PacketTime.Value.AddMilliseconds((int)p.PacketTime_ms_),
                           gmmMsgType = tpacketnum.Select(e => e.mGMMSM_MsgType).FirstOrDefault(),
                           p.bssgp_direction,
                           p.llcgprs_xid1type,
                       };

            var xid3 = from p in xid1
                       let dt = p.PacketTime - p.gmmPacketTime
                       select new
                       {
                           p.BeginFrameNum,
                           p.gmmPacketNum,
                           p.PacketNum,
                           p.gmmPacketTime,
                           p.PacketTime,
                           p.gmmMsgType,
                           duration = dt.Value.TotalMilliseconds,
                           p.bssgp_direction,
                           p.llcgprs_xid1type
                       };

            var dborder = xid3.OrderBy(e => e.BeginFrameNum);
            gridControl1.DataSource = dborder.ToList();

            gridView1.Columns[3].DisplayFormat.FormatString = "yyyy-MM-dd HH:mm:ss.fff";
            gridView1.Columns[3].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;

            gridView1.Columns[4].DisplayFormat.FormatString = "yyyy-MM-dd HH:mm:ss.fff";
            gridView1.Columns[4].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;

        }

        private void splitContainerControl1_SizeChanged(object sender, EventArgs e)
        {
            //splitContainerControl1.SizeChanged -= splitContainerControl1_SizeChanged;
            splitContainerControl1.SplitterPosition = 7 * splitContainerControl1.Width / 10;

        }

        private void navBarItem2_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();

            var gmm = gb_gmm_xid.ToLookup(a => a.BeginFrameNum);
            //var xid = gb_xid.ToLookup(a => a.BeginFrameNum);
            var xid2 = gb_xid.Where(e => e.llcgprs_xid1type == 5).ToList();

            var xid1 = from p in xid2
                       let packetnum = gmm[p.BeginFrameNum]
                       .Where(e => e.mGMMSM_MsgType != "GMMSM.GMM Information")
                       .Where(e => e.mGMMSM_MsgType != "GMMSM.SM Status")
                       .Where(e => e.PacketNum < p.PacketNum)  //gmm的帧号要小于xid的帧号
                       .OrderByDescending(e => e.PacketNum)
                       .Select(e => e.PacketNum).FirstOrDefault()
                       let tpacketnum = gmm[p.BeginFrameNum].Where(e => e.PacketNum == packetnum)
                       select new
                       {
                           p.BeginFrameNum,
                           gmmPacketNum = packetnum,
                           p.PacketNum,
                           gmmPacketTime = tpacketnum.Select(e => e.PacketTime).FirstOrDefault() == null ? DateTime.Now : tpacketnum.Select(e => e.PacketTime).FirstOrDefault().Value.AddMilliseconds((int)tpacketnum.Select(e => e.PacketTime_ms_).FirstOrDefault().Value),
                           PacketTime = p.PacketTime.Value.AddMilliseconds((int)p.PacketTime_ms_),
                           gmmMsgType = tpacketnum.Select(e => e.mGMMSM_MsgType).FirstOrDefault(),
                           p.bssgp_direction,
                           p.llcgprs_xid1type,

                       };

            var total = xid1.Where(e => e.gmmMsgType != null).Count();

            var xid3 = from p in xid1
                       where p.gmmMsgType != null
                       group p by p.gmmMsgType into ttt
                       select new
                       {
                           ttt.Key,
                           cnt = ttt.Count(),
                           percnet = 1.0 * ttt.Count() / total,
                       };

            var dborder = xid3.OrderByDescending(e => e.cnt);

            gridControl1.DataSource = dborder.ToList();

        }

        private void navBarItem3_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Excel2007文件(*.xlsx) |*.xlsx";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                gridView1.ExportToXlsx(saveFileDialog1.FileName);
            }
        }

        private void navBarItem4_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            var maxt = gb_gmm_xid.Max(e => e.PacketTime);
            var mint = gb_gmm_xid.Min(e => e.PacketTime);
            TimeSpan ts = maxt.Value - mint.Value;
            var ttim = mint.Value.ToString() + "-" + maxt.Value.ToString() + "," + ts.TotalSeconds.ToString();
            richTextBox1.Text = ttim.ToString();
        }
    }
}
