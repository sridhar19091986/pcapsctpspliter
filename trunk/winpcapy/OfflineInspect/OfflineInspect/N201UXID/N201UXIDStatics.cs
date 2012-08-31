
#define abc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OfflineInspect.Mongo;
using OfflineInspect.CommonTools;
using EntitySqlTable.SqlServer.ip209.GuangZhou.GbFlowControl;
using EntitySqlTable.SqlServer.ip209.GuangZhou.GbFlowControlms;
using System.Data.Objects;
using OfflineInspect.FlowControl;

namespace OfflineInspect.N201UXID
{
#if abc

    public class N201UXIDStaticsDocument
    {
        public object _id;
        public string message_type;
        public int cnt;
        public int cnt_total;
        public double cnt_percent;
        public int? size;
        public int? size_total;
        public double? size_percent;
    }
    public class N201UXIDStatics : CommonToolx, IDisposable
    {

        private string mongo_collection = CommonAttribute.N201UXIDRate[0];
        private string mongo_db = CommonAttribute.N201UXIDRate[1];
        private string mongo_conn = CommonAttribute.N201UXIDRate[2];

        public MongoCrud<N201UXIDStaticsDocument> mongo_fcmr;

        public N201UXIDStatics()
        {
            mongo_fcmr = new MongoCrud<N201UXIDStaticsDocument>(mongo_conn, mongo_db, mongo_collection);
        }
        #region Implementing IDisposable and the Dispose Pattern Properly
        private bool disposed = false; // to detect redundant calls
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~N201UXIDStatics()
        {
            Dispose(false);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing) { }
                disposed = true;
            }
        }
        #endregion

        public void CreateCollection()
        {
            FlowControlOneMs fcom = new FlowControlOneMs();
            var fcommongo = fcom.mongo_fcom.QueryMongo().AsParallel().ToList();
            var cnt_t = fcommongo.Count();
            var size_t = fcommongo.Sum(e => e.ip_len);
            var query = from p in fcommongo
                        group p by p.Flow_Control_MsgType into ttt
                        select new N201UXIDStaticsDocument
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
            mongo_fcmr.BulkMongo(query.ToList(), true);
        }
    }
#endif
}
