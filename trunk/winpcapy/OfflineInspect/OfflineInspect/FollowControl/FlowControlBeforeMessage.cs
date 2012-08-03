using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OfflineInspect.Mongo;
using OfflineInspect.CommonTools;
using EntitySqlTable.SqlServer.ip209.GuangZhou.GbFlowControl;
using System.Data.Objects;
using MongoDB.Driver;
using OfflineInspect.FollowControl;

namespace OfflineInspect.FollowControl
{
    public class FlowControlBeforeMessage : CommonToolx, IDisposable
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
        private MongoCrud<FlowControlBeforeMessage> mongo_fcbm;

        public FlowControlBeforeMessage()
        {
            mongo_fcbm = new MongoCrud<FlowControlBeforeMessage>(mongo_conn, mongo_db, mongo_collection);
        }
        #region Implementing IDisposable and the Dispose Pattern Properly
        private bool disposed = false; // to detect redundant calls
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~FlowControlBeforeMessage()
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
        public IQueryable<FlowControlBeforeMessage> QueryMongo()
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
                        FlowControlBeforeMessage fcbm = new FlowControlBeforeMessage();

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
    }
}
