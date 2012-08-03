using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OfflineInspect.Mongo;
using OfflineInspect.CommonTools;

namespace OfflineInspect.FollowControl
{
    public class FlowControlBeforeMessageMap : CommonToolx
    {
        public object _id;
        public string fcBeforMsgs;
        public int cnt;
        public double cnt_percent;
        public double duration;
        public double bucket_size_avg;
        public double leak_rate;
        public double full_rate_avg;

        private string mongo_db = "Guangzhou_FlowControl";
        private string mongo_collection = "FlowControlBeforeMessageMap";
        private string mongo_conn = "mongodb://192.168.4.209/?safe=true";
        private MongoCrud<FlowControlBeforeMessageMap> mongo_fcbmm;

        public FlowControlBeforeMessageMap()
        {
            mongo_fcbmm = new MongoCrud<FlowControlBeforeMessageMap>(mongo_conn, mongo_db, mongo_collection);
        }

        public void BulkMongo(List<FlowControlBeforeMessageMap> fcbmm)
        {
            mongo_fcbmm.BulkMongo(fcbmm, true);
        }

        public IQueryable<FlowControlBeforeMessageMap> QueryMongo()
        {
            return mongo_fcbmm.QueryMongo();
        }

        public void CreateCollection()
        {
            FlowControlBeforeMessage fcbm = new FlowControlBeforeMessage();
            var fcBeforMsgs = fcbm.QueryMongo().ToList();
            var dborders = from p in fcBeforMsgs
                           group p by p.fcb_msg into ttt
                           select new FlowControlBeforeMessageMap
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
            BulkMongo(dborders.OrderByDescending(e => e.cnt).ToList());
        }
    }
}
