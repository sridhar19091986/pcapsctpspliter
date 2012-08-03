using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OfflineInspect.Mongo;
using OfflineInspect.CommonTools;
using EntitySqlTable.SqlServer.ip209.GuangZhou.GbFlowControl;
using System.Data.Objects;
using MongoDB.Driver;

namespace OfflineInspect.N201UXID
{
    public class N201UXIDBeforeMessage : CommonToolx, IDisposable
    {
        public object _id;
        public double bssgp_ms_bucket_size;
        public double bssgp_bucket_leak_rate;
        public double bssgp_bucket_full_ratio;
        public double duration;
        public int fcb_packetnum;
        public DateTime? fcb_packettime;
        public string fcb_msg;
        public int fc_packetnum;
        public DateTime? fc_packettime;
        public string fc_msg;

        private string mongo_db = "Guangzhou_FlowControl";
        private string mongo_collection = "FlowControlBeforeMessage";
        private string mongo_conn = "mongodb://192.168.4.209/?safe=true";
        private MongoCrud<N201UXIDBeforeMessage> mongo_fcbm;

        public N201UXIDBeforeMessage()
        {
            mongo_fcbm = new MongoCrud<N201UXIDBeforeMessage>(mongo_conn, mongo_db, mongo_collection);
        }
        #region Implementing IDisposable and the Dispose Pattern Properly
        private bool disposed = false; // to detect redundant calls
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~N201UXIDBeforeMessage()
        {
            Dispose(false);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Free other state (managed objects).
                }
                // Free your own state (unmanaged objects).
                // Set large fields to null.
                disposed = true;
            }
        }
        #endregion
        private MongoCollection fcbm_col = null;

        private MongoCollection FCBM_col
        {
            get
            {
                if (fcbm_col == null)
                {
                    fcbm_col = mongo_fcbm.GetMongoCollection(true);
                }
                return fcbm_col;
            }
            set
            {
                value = fcbm_col;
            }
        }
        public IQueryable<N201UXIDBeforeMessage> QueryMongo()
        {
            return mongo_fcbm.QueryMongo();
        }

        private string msfc_msg = "BSSGP.FLOW-CONTROL-MS";

        public void CreateCollection()
        {
            FlowControlOneMs fcom = new FlowControlOneMs();
            var beginframes = fcom.QueryMongo().AsParallel().ToLookup(e => e.BeginFrameNum);
            foreach (var m in beginframes)
            {
                var fc = m.Where(e => e.Flow_Control_MsgType.StartsWith(msfc_msg)).OrderBy(e => e.PacketNum).FirstOrDefault();
                if (fc != null)
                {
                    var fcb = m
                        .Where(e => e.Flow_Control_MsgType.StartsWith("GMMSM."))
                        .Where(e => e.Flow_Control_MsgType.StartsWith("GMMSM.GMM Information") == false)
                        .Where(e => e.PacketNum < fc.PacketNum)
                        .OrderByDescending(e => e.PacketNum).FirstOrDefault();
                    if (fcb != null)
                    {
                        N201UXIDBeforeMessage fcbm = new N201UXIDBeforeMessage();

                        fcbm._id = m.Key;

                        fcbm.fc_msg = fc.Flow_Control_MsgType;
                        fcbm.fc_packetnum = fc.PacketNum;
                        fcbm.fc_packettime = fc.Flow_Control_time;

                        fcbm.bssgp_ms_bucket_size = fc.bucket_size;
                        fcbm.bssgp_bucket_leak_rate = fc.leak_rate;
                        fcbm.bssgp_bucket_full_ratio = fc.bucket_ratio;

                        TimeSpan ts = fc.Flow_Control_time - fcb.Flow_Control_time;
                        fcbm.duration = ts.TotalSeconds;

                        fcbm.fcb_msg = fcb.Flow_Control_MsgType;
                        fcbm.fcb_packetnum = fcb.PacketNum;
                        fcbm.fcb_packettime = fcb.Flow_Control_time;

                        FCBM_col.Insert(fcbm);
                    }
                }
            }
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

    }
}
