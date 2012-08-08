using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OfflineInspect.Mongo;
using OfflineInspect.CommonTools;
using EntitySqlTable.SqlServer.ip209.GuangZhou.GbFlowControl;
using EntitySqlTable.SqlServer.ip209.GuangZhou.GbFlowControlms;
using System.Data.Objects;

namespace OfflineInspect.FollowControl
{
    public class FlowControlMessageRate : CommonToolx, IDisposable
    {
        public object _id;
        public string message_type;
        public int cnt;
        public int cnt_total;
        public double cnt_percent;
        public int? size;
        public int? size_total;
        public double? size_percent;

        private string mongo_collection = CommonAttribute.FlowControlMessageRate[0];
        private string mongo_db = CommonAttribute.FlowControlMessageRate[1];
        private string mongo_conn = CommonAttribute.FlowControlMessageRate[2];

        public MongoCrud<FlowControlMessageRate> mongo_fcmr;

        public FlowControlMessageRate()
        {
            mongo_fcmr = new MongoCrud<FlowControlMessageRate>(mongo_conn, mongo_db, mongo_collection);
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
        /*
        public void BulkMongo(List<FlowControlMessageRate> fcmr)
        {
            mongo_fcmr.BulkMongo(fcmr, true);
        }

        public IQueryable<FlowControlMessageRate> QueryMongo()
        {
            return mongo_fcmr.QueryMongo();
        }
        **/
        public void CreateCollection()
        {
            FlowControlOneMs fcom = new FlowControlOneMs();
            var fcommongo = fcom.mongo_fcom.ListT;
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
            //BulkMongo(query.ToList());
            mongo_fcmr.BulkMongo(query.ToList(), true);
        }
    }
}
