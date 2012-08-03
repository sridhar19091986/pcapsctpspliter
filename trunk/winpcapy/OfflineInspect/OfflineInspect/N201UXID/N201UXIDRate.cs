
//#define abc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OfflineInspect.Mongo;
using OfflineInspect.CommonTools;
using EntitySqlTable.SqlServer.ip209.GuangZhou.GbFlowControl;
using EntitySqlTable.SqlServer.ip209.GuangZhou.GbFlowControlms;
using System.Data.Objects;

namespace OfflineInspect.N201UXID
{
#if abc
		  
	
    public class N201UXIDRate : CommonToolx, IDisposable
    {
        public object _id;
        public string message_type;
        public int cnt;
        public int cnt_total;
        public double cnt_percent;
        public int? size;
        public int? size_total;
        public double? size_percent;

        private string mongo_db = "Guangzhou_FlowControl";
        private string mongo_collection = " FlowControlMessageRate";
        private string mongo_conn = "mongodb://192.168.4.209/?safe=true";
        private MongoCrud<N201UXIDRate> mongo_fcmr;

        public N201UXIDRate()
        {
            mongo_fcmr = new MongoCrud<N201UXIDRate>(mongo_conn, mongo_db, mongo_collection);
        }
        #region Implementing IDisposable and the Dispose Pattern Properly
        private bool disposed = false; // to detect redundant calls
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~FlowControlMessageRate()
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
        public void BulkMongo(List<N201UXIDRate> fcmr)
        {
            mongo_fcmr.BulkMongo(fcmr, true);
        }

        public IQueryable<N201UXIDRate> QueryMongo()
        {
            return mongo_fcmr.QueryMongo();
        }

        public void CreateCollection()
        {
            FlowControlOneMs fcom = new FlowControlOneMs();
            var fcommongo = fcom.QueryMongo().AsParallel().ToList();
            var cnt_t = fcommongo.Count();
            var size_t = fcommongo.Sum(e => e.ip_len);
            var query = from p in fcommongo
                        group p by p.Flow_Control_MsgType into ttt
                        select new FlowControlMessageRate
                        {
                            _id = GenerateId(),
                            message_type = ttt.Key,
                            cnt = ttt.Count(),
                            cnt_total = cnt_t,
                            cnt_percent = 1.0 * ttt.Count() / cnt_t,
                            size = ttt.Sum(e => e.ip_len),
                            size_total = size_t,
                            size_percent = 1.0 * ttt.Sum(e => e.ip_len) / size_t,
                        };
            BulkMongo(query.ToList());
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
    }

#endif
}
