/*
 * http://stackoverflow.com/questions/6376436/mongodb-drop-every-database
 * you can create a javascript loop that do the job and then execute it in the mongoconsole.
 * var dbs = db.getMongo().getDBNames()
for(var i in dbs){
db = db.getMongo().getDB( dbs[i] );
print( "dropping db " + db.getName() );
db.dropDatabase();
}
save it to dropall.js and then execute:

mongo dropall.js
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Linq;
using MongoDB.Driver.Linq;
using OfflineInspect.Mongo;
using EntitySqlTable.SqlServer.ip209.GuangZhou.GbFlowControlms;
using System.Data.Objects;
using OfflineInspect.CommonTools;
using System.Threading.Tasks;

namespace OfflineInspect.FollowControl
{
    public class FlowControlMapMs : IDisposable
    {
        public object _id;
        public int? BeginFrameNum;
        public int fcontrol_cnt;
        public int packet_cnt;
        public double bucket_size_avg;
        public double bucket_size_min;
        public double bucket_size_max;
        public double leak_rate_avg;
        public double leak_rate_min;
        public double leak_rate_max;
        public string first_delay;
        public string leak_rate;
        public string bucket_size;
        public string down_total_len;
        public string down_packet_rate;
        public string fcm_time;

        private string mongo_db = "Guangzhou_FlowControl";
        private string mongo_collection = "FlowControlMapMs";
        private string mongo_conn = "mongodb://192.168.4.209/?safe=true";
        private MongoCrud<FlowControlMapMs> mongo_fcmm;

        public FlowControlMapMs()
        {
            mongo_fcmm = new MongoCrud<FlowControlMapMs>(mongo_conn, mongo_db, mongo_collection);
        }
        #region Implementing IDisposable and the Dispose Pattern Properly
        private bool disposed = false; // to detect redundant calls
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~FlowControlMapMs()
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
        private MongoCollection fcmm_col = null;
        private MongoCollection FCMM_col
        {
            get
            {
                if (fcmm_col == null)
                {
                    fcmm_col = mongo_fcmm.GetMongoCollection(true);
                }
                return fcmm_col;
            }
            set
            {
                value = fcmm_col;
            }
        }

        public IQueryable<FlowControlMapMs> QueryMongo()
        {
            return mongo_fcmm.QueryMongo();
        }

        private string fc_msg = "BSSGP.FLOW-CONTROL-MS";

        public void CreateCollection()
        {
            FlowControlOneMs fcom = new FlowControlOneMs();

            var fcommongo = fcom.QueryMongo().AsParallel().ToLookup(e => e.BeginFrameNum);

            foreach (var ttt in fcommongo)
            {
                if (ttt.Where(e => e.Flow_Control_MsgType == fc_msg).Count() > 0)
                {
                    FlowControlMapMs fcmm = new FlowControlMapMs();
                    fcmm._id = ttt.Key;
                    fcmm.BeginFrameNum = ttt.Key;
                    //ttt.Key.bssgp_tlli,
                    fcmm.fcontrol_cnt = ttt.Where(e => e.Flow_Control_MsgType == fc_msg).Count();
                    fcmm.packet_cnt = ttt.Count();
                    fcmm.bucket_size_avg = ttt.Where(e => e.Flow_Control_MsgType == fc_msg).Average(e => e.bucket_size);
                    fcmm.bucket_size_min = ttt.Where(e => e.Flow_Control_MsgType == fc_msg).Min(e => e.bucket_size);
                    fcmm.bucket_size_max = ttt.Where(e => e.Flow_Control_MsgType == fc_msg).Max(e => e.bucket_size);
                    fcmm.leak_rate_avg = ttt.Where(e => e.Flow_Control_MsgType == fc_msg).Average(e => e.leak_rate);
                    fcmm.leak_rate_min = ttt.Where(e => e.Flow_Control_MsgType == fc_msg).Min(e => e.leak_rate);
                    fcmm.leak_rate_max = ttt.Where(e => e.Flow_Control_MsgType == fc_msg).Max(e => e.leak_rate);

                    fcmm.first_delay = ttt.Where(e => e.Flow_Control_MsgType == fc_msg).OrderBy(e => e.PacketNum)
                                        .Select(e => new { fd = (e.Flow_Control_time - ttt.Min(f => f.Flow_Control_time)).TotalMilliseconds })
                                        .Select(e => (e.fd / 1000).ToString("f1"))
                                        .Aggregate((a, b) => a + "," + b);

                    fcmm.leak_rate = ttt.Where(e => e.Flow_Control_MsgType == fc_msg).OrderBy(e => e.PacketNum)
                                    .Select(e => (e.leak_rate).ToString("f1"))
                                    .Aggregate((a, b) => a + "," + b);

                    fcmm.bucket_size = ttt.Where(e => e.Flow_Control_MsgType == fc_msg).OrderBy(e => e.PacketNum)
                                     .Select(e => (e.bucket_size).ToString("f1"))
                                     .Aggregate((a, b) => a + "," + b);

                    //用扩展方法实现， 在fc消息之间进行ip.len的累加
                    fcmm.down_total_len = ttt.OrderBy(e => e.PacketNum).AggregateSum(e => (int)e.ip_len, e => e.Flow_Control_MsgType, fc_msg);
                    fcmm.down_packet_rate = ttt.OrderBy(e => e.PacketNum).AggregatePacketRate(e => (int)e.ip_len, e => e.Flow_Control_time, e => e.Flow_Control_MsgType, fc_msg);
                    fcmm.fcm_time = ttt.OrderBy(e => e.PacketNum).AggregatePacketTime(e => e.Flow_Control_time, e => e.Flow_Control_MsgType, fc_msg);

                    FCMM_col.Insert(fcmm);
                };
            }
        }
        //BulkMongo(query.ToList());
    }
}

