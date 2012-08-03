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
    public class FlowControlOneMs : IDisposable
    {
        public object _id;
        public int? BeginFrameNum;
        public DateTime Flow_Control_time;
        public int PacketNum;
        public int? ip_len;
        public string Flow_Control_MsgType;
        public string bssgp_direction;
        public double bucket_size;
        public double leak_rate;
        public double bucket_ratio;//???

        private string mongo_db = "Guangzhou_FlowControl";
        private string mongo_collection = "FlowControlOneMs";
        private string mongo_conn = "mongodb://192.168.4.209/?safe=true";
        private MongoCrud<FlowControlOneMs> mongo_fcom;
        private int maxfilenum = 1;

        public FlowControlOneMs()
        {
            mongo_fcom = new MongoCrud<FlowControlOneMs>(mongo_conn, mongo_db, mongo_collection);
        }
        #region Implementing IDisposable and the Dispose Pattern Properly
        private bool disposed = false; // to detect redundant calls
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~FlowControlOneMs()
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
        public IQueryable<FlowControlOneMs> QueryMongo()
        {
            return mongo_fcom.QueryMongo();
        }

        private MongoCollection fcom_col = null;
        private MongoCollection FCOM_col
        {
            get
            {
                if (fcom_col == null)
                {
                    fcom_col = mongo_fcom.GetMongoCollection(true);
                }
                return fcom_col;
            }
            set
            {
                value = fcom_col;
            }
        }

        public void CreateCollection()
        {
            for (int j = 0; j < maxfilenum; j++)
                CreateCollection(j);
        }

        private void CreateCollection(int filenum)
        {
            GuangZhou_GbEntities_fc_ms gb = new GuangZhou_GbEntities_fc_ms();
            gb.ContextOptions.LazyLoadingEnabled = true;
            gb.Gb_FlowControlx.MergeOption = MergeOption.NoTracking;
            //查询
            var fcms = from e in gb.Gb_FlowControlx.Where(p => p.FileNum == filenum)
                       select new
                       {
                           e.BeginFrameNum,
                           e.PacketNum,
                           //e.PacketTime,
                           e.Flow_Control_time,
                           e.Flow_Control_MsgType,
                           e.bssgp_direction,
                           //e.bssgp_tlli,
                           e.bssgp_ms_bucket_size,
                           e.bssgp_bucket_leak_rate,
                           e.ip_len
                       };

            Parallel.ForEach(fcms, p =>
                {
                    FlowControlOneMs fcom = new FlowControlOneMs();
                    fcom._id = (int)p.PacketNum;
                    fcom.BeginFrameNum = p.BeginFrameNum;
                    fcom.Flow_Control_time = DateTime.Parse(p.Flow_Control_time);
                    fcom.PacketNum = p.PacketNum;
                    fcom.ip_len = p.ip_len;
                    fcom.Flow_Control_MsgType = p.Flow_Control_MsgType;
                    fcom.bssgp_direction = p.bssgp_direction;
                    fcom.bucket_size = Convert.ToDouble(p.bssgp_ms_bucket_size) / 1000.0;   //转换成KByte
                    fcom.leak_rate = Convert.ToDouble(p.bssgp_bucket_leak_rate) / 1000.0; //转化成kbps
                    FCOM_col.Insert(fcom);
                });
        }
    }
}
