using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using OfflineInspect.Mongo;
using EntitySqlTable.SqlServer.ip209.GuangZhou.GbFlowControl;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace OfflineInspect.FlowControl
{
    public class FlowControlOneBvcDocument
    {
        public object _id;
        public DateTime Flow_Control_time;
        public string lac_cell;
        public string Flow_Control_MsgType;
        public int ip_len;
        public string tlli;
        public string bssgp_direction;
        public double bssgp_bmax_default_ms;
        public int? bssgp_bucket_full_ratio;
        public double bssgp_bucket_leak_rate;
        public double bssgp_bvc_bucket_size;
        public double bssgp_ms_bucket_size;
        public double bssgp_R_default_ms;
        public string bvci;
    }
    public class FlowControlOneBvc : IDisposable
    {
        private string mongo_collection = CommonAttribute.FlowControlOneBvc[0];
        private string mongo_db = CommonAttribute.FlowControlOneBvc[1];
        private string mongo_conn = CommonAttribute.FlowControlOneBvc[2];
        private int maxfilenum = Int32.Parse(CommonAttribute.FlowControlOneBvc[3]);

        public MongoCrud<FlowControlOneBvcDocument> mongo_fcob;

        public FlowControlOneBvc()
        {
            mongo_fcob = new MongoCrud<FlowControlOneBvcDocument>(mongo_conn, mongo_db, mongo_collection);
        }
        #region Implementing IDisposable and the Dispose Pattern Properly
        private bool disposed = false; // to detect redundant calls
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~FlowControlOneBvc()
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
        public void CreateCollection()
        {
            for (int j = 0; j < maxfilenum; j++)
                CreateCollection(j);
        }

        public void CreateCollection(int filenum)
        {
            GuangZhou_GbEntities_fc gb = new GuangZhou_GbEntities_fc();
            gb.CommandTimeout = 0;
            gb.ContextOptions.LazyLoadingEnabled = true;
            gb.Gb_FlowControly.MergeOption = MergeOption.NoTracking;
            LacCellBvci lcb = new LacCellBvci();

            var fc = from p in gb.Gb_FlowControly.Where(e => e.FileNum == filenum)
                     select new
                     {
                         p.FileNum,
                         p.PacketNum,
                         p.bssgp_tlli,
                         p.Flow_Control_time,
                         p.ip_src_host,
                         p.ip_dst_host,
                         p.nsip_bvci,
                         p.Flow_Control_MsgType,
                         p.ip_len,
                         p.bssgp_direction,
                         p.bssgp_bmax_default_ms,
                         p.bssgp_bucket_full_ratio,
                         p.bssgp_bucket_leak_rate,
                         p.bssgp_bvc_bucket_size,
                         p.bssgp_ms_bucket_size,
                         p.bssgp_R_default_ms,
                     };

            Parallel.ForEach(fc, p =>
            {
                FlowControlOneBvcDocument fcob = new FlowControlOneBvcDocument();
                fcob._id = p.FileNum * 100000000 + p.PacketNum;
                fcob.tlli = p.bssgp_tlli;
                fcob.Flow_Control_time = DateTime.Parse(p.Flow_Control_time);
                fcob.lac_cell = lcb.GetLacCell(p.ip_src_host, p.ip_dst_host, p.nsip_bvci.ToString());
                fcob.bvci = p.nsip_bvci.ToString();
                fcob.Flow_Control_MsgType = p.Flow_Control_MsgType;
                fcob.ip_len = (int)p.ip_len;
                fcob.bssgp_direction = p.bssgp_direction;
                fcob.bssgp_bmax_default_ms = Convert.ToDouble(p.bssgp_bmax_default_ms) / 1000.0;
                fcob.bssgp_bucket_full_ratio = p.bssgp_bucket_full_ratio;
                fcob.bssgp_bucket_leak_rate = Convert.ToDouble(p.bssgp_bucket_leak_rate) / 1000.0;
                fcob.bssgp_bvc_bucket_size = Convert.ToDouble(p.bssgp_bvc_bucket_size) / 1000.0;
                fcob.bssgp_ms_bucket_size = Convert.ToDouble(p.bssgp_ms_bucket_size) / 1000.0;
                fcob.bssgp_R_default_ms = Convert.ToDouble(p.bssgp_R_default_ms) / 1000.0;

                //FCOB_col.Insert(fcob);
                mongo_fcob.MongoCol.Insert(fcob);
            });

        }

        /*
        private MongoCollection fcob_col = null;
        private MongoCollection FCOB_col
        {
            get
            {
                if (fcob_col == null)
                {
                    fcob_col = mongo_fcob.GetMongoCollection(true);
                }
                return fcob_col;
            }
            set
            {
                value = fcob_col;
            }
        }

        public IQueryable<FlowControlOneBvc> QueryMongo()
        {
            return mongo_fcob.QueryMongo();
        }
         * */
    }
}
