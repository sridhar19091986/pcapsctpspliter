using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OfflineInspect.Mongo;
using OfflineInspect.CommonTools;

namespace OfflineInspect.FlowControl
{
    public class FlowControlBeforeMessageMapDocument
    {
        public object _id;
        public string fcBeforMsgs;
        public int cnt;
        public double cnt_percent;
        public double duration;
        public double bucket_size_avg;
        public double leak_rate;
        public double full_rate_avg;
    }
    public class FlowControlBeforeMessageMap : CommonToolx, IDisposable
    {
        private string mongo_collection = CommonAttribute.FlowControlBeforeMessageMap[0];
        private string mongo_db = CommonAttribute.FlowControlBeforeMessageMap[1];
        private string mongo_conn = CommonAttribute.FlowControlBeforeMessageMap[2];

        public MongoCrud<FlowControlBeforeMessageMapDocument> mongo_fcbmm;

        public FlowControlBeforeMessageMap()
        {
            mongo_fcbmm = new MongoCrud<FlowControlBeforeMessageMapDocument>(mongo_conn, mongo_db, mongo_collection);
        }
        #region Implementing IDisposable and the Dispose Pattern Properly
        private bool disposed = false; // to detect redundant calls
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~FlowControlBeforeMessageMap()
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

        /*
        public void BulkMongo(List<FlowControlBeforeMessageMap> fcbmm)
        {
            mongo_fcbmm.BulkMongo(fcbmm, true);
        }

        public IQueryable<FlowControlBeforeMessageMap> QueryMongo()
        {
            return mongo_fcbmm.QueryMongo();
        }
        **/

        public void CreateCollection()
        {
            FlowControlBeforeMessage fcbm = new FlowControlBeforeMessage();
            var fcBeforMsgs = fcbm.mongo_fcbm.ListT;
            var dborders = from p in fcBeforMsgs
                           group p by p.fcb_msg into ttt
                           select new FlowControlBeforeMessageMapDocument
                           {
                               _id = GenerateId(),
                               fcBeforMsgs = ttt.Key,
                               cnt = ttt.Count(),
                               cnt_percent = 1.0 * ttt.Count() / fcBeforMsgs.Count(),
                               duration = ttt.Average(e => e.duration),
                               bucket_size_avg = ttt.Average(e => e.bssgp_ms_bucket_size),
                               leak_rate = ttt.Average(e => e.bssgp_bucket_leak_rate),
                               full_rate_avg = ttt.Average(e => e.bssgp_bucket_full_ratio),

                           };
            mongo_fcbmm.BulkMongo(dborders.OrderByDescending(e => e.cnt).ToList(), true);
            //BulkMongo(dborders.OrderByDescending(e => e.cnt).ToList());
        }
    }
}
