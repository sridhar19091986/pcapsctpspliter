using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OfflineInspect.Mongo;
using OfflineInspect.CommonTools;

namespace OfflineInspect.FollowControl
{
    public class FlowControlMapBvc : CommonToolx, IDisposable
    {
        public object _id;
        public string lac_cell;
        public string bvci;
        public int fcb_cnt;
        public int packet_cnt;
        public int tlli_cnt;
        public double bssgp_bmax_default_ms;
        public double? bssgp_bucket_full_ratio;
        public string bssgp_bucket_leak_rate;
        public string bssgp_bvc_bucket_size;
        public double bssgp_ms_bucket_size;
        public double bssgp_R_default_ms;
        public double ms_bucket_size;
        public double ms_leak_rate;
        public string tlli_distinct_aggre;
        public string msg_distinct_aggre;
        public string fcb_delay_aggre;
        public string down_total_len;
        public string down_packet_rate;
        public string fcb_time_aggre;

        private string mongo_db = "Guangzhou_FlowControl";
        private string mongo_collection = "FlowControlMapBvc";
        private string mongo_conn = "mongodb://192.168.4.209/?safe=true";
        private MongoCrud<FlowControlMapBvc> mongo_fcmb;

        public FlowControlMapBvc()
        {
            mongo_fcmb = new MongoCrud<FlowControlMapBvc>(mongo_conn, mongo_db, mongo_collection);
        }
        #region Implementing IDisposable and the Dispose Pattern Properly
        private bool disposed = false; // to detect redundant calls
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~FlowControlMapBvc()
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
        public void BulkMongo(List<FlowControlMapBvc> fcmb)
        {
            mongo_fcmb.BulkMongo(fcmb, true);
        }

        public IQueryable<FlowControlMapBvc> QueryMongo()
        {
            return mongo_fcmb.QueryMongo();
        }

        private string msfc_msg = "BSSGP.FLOW-CONTROL-MS";
        private string fc_msg = "BSSGP.FLOW-CONTROL-BVC";

        public void CreateCollection()
        {
            FlowControlOneBvc fcob = new FlowControlOneBvc();
            var fcobmongo = fcob.QueryMongo().Where(e => e.lac_cell != null).AsParallel().ToList();
            var query = from p in fcobmongo
                        group p by p.lac_cell into ttt
                        select new FlowControlMapBvc
                        {
                            _id = GenerateId(),
                            lac_cell = ttt.Key,
                            fcb_cnt = ttt.Where(e => e.Flow_Control_MsgType == fc_msg).Count(),
                            packet_cnt = ttt.Count(),
                            tlli_cnt = ttt.Select(e => e.tlli).Distinct().Count(),

                            bssgp_R_default_ms = ttt.Where(e => e.Flow_Control_MsgType == fc_msg).Average(e => e.bssgp_R_default_ms),
                            bssgp_ms_bucket_size = ttt.Where(e => e.Flow_Control_MsgType == fc_msg).Average(e => e.bssgp_ms_bucket_size),
                            bssgp_bucket_full_ratio = ttt.Where(e => e.Flow_Control_MsgType == fc_msg).Average(e => e.bssgp_bucket_full_ratio),
                            bssgp_bmax_default_ms = ttt.Where(e => e.Flow_Control_MsgType == fc_msg).Average(e => e.bssgp_bmax_default_ms),

                            //用户级别
                            ms_bucket_size = ttt.Where(e => e.Flow_Control_MsgType == msfc_msg).Where(e => e.bssgp_ms_bucket_size > 0).Average(e => e.bssgp_ms_bucket_size),
                            ms_leak_rate = ttt.Where(e => e.Flow_Control_MsgType == msfc_msg).Where(e => e.bssgp_bucket_leak_rate > 0).Average(e => e.bssgp_bucket_leak_rate),

                            fcb_delay_aggre = ttt.Where(e => e.Flow_Control_MsgType == fc_msg).OrderBy(e => e._id)
                                                .Select(e => new { fd = (e.Flow_Control_time - ttt.Min(f => f.Flow_Control_time)).TotalMilliseconds })
                                                .Select(e => (e.fd / 1000).ToString("f1"))
                                                .Aggregate((a, b) => a + "," + b),

                            bssgp_bucket_leak_rate = ttt.Where(e => e.Flow_Control_MsgType == fc_msg)
                            .OrderBy(e => e._id)
                                            .Select(e => (e.bssgp_bucket_leak_rate).ToString("f1"))
                                            .Aggregate((a, b) => a + "," + b),

                            bssgp_bvc_bucket_size = ttt.Where(e => e.Flow_Control_MsgType == fc_msg)
                            .OrderBy(e => e._id)
                                             .Select(e => (e.bssgp_bvc_bucket_size).ToString("f1"))
                                             .Aggregate((a, b) => a + "," + b),

                            ////用扩展方法实现， 在fc消息之间进行ip.len的累加
                            tlli_distinct_aggre = ttt.Select(e => e.tlli).Distinct().Aggregate((a, b) => a + "," + b),

                            msg_distinct_aggre = ttt.Select(e => e.Flow_Control_MsgType).Distinct().Aggregate((a, b) => a + "," + b),

                            down_total_len = ttt.OrderBy(e => e._id)
                            .AggregateSum(e => (int)e.ip_len, e => e.Flow_Control_MsgType, fc_msg),

                            down_packet_rate = ttt.OrderBy(e => e._id)
                            .AggregatePacketRate(e => (int)e.ip_len, e => e.Flow_Control_time, e => e.Flow_Control_MsgType, fc_msg),


                            fcb_time_aggre = ttt.OrderBy(e => e._id)
                            .AggregatePacketTime(e => e.Flow_Control_time, e => e.Flow_Control_MsgType, fc_msg),
                        };

            BulkMongo(query.ToList());
        }
    }
}
