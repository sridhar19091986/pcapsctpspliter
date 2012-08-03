using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using OfflineInspect.Mongo;
using EntitySqlTable.SqlServer.ip209.GuangZhou.GbFlowControl;
using OfflineInspect.CommonTools;


namespace OfflineInspect.FollowControl
{
    public class LacCellBvci : CommonToolx, IDisposable
    {
        public object _id;
        public string src;
        public string dst;
        public string bvci;
        public string lac_cell;


        private string mongo_db = "Guangzhou_FlowControl";
        private string mongo_collection = "LacCellBvci";
        private string mongo_conn = "mongodb://192.168.4.209/?safe=true";
        private MongoCrud<LacCellBvci> mongo_lac_cell_bvci;

        public LacCellBvci()
        {
            mongo_lac_cell_bvci = new MongoCrud<LacCellBvci>(mongo_conn, mongo_db, mongo_collection);
        }
        #region Implementing IDisposable and the Dispose Pattern Properly
        private bool disposed = false; // to detect redundant calls
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~LacCellBvci()
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
        public void CreatCollection()
        {
            GuangZhou_GbEntities_fc gb = new GuangZhou_GbEntities_fc();
            gb.CommandTimeout = 0;
            gb.ContextOptions.LazyLoadingEnabled = true;
            gb.Gb_FlowControly.MergeOption = MergeOption.NoTracking;
            var fc = from p in gb.Gb_FlowControly
                     group p by new
                     {
                         p.bssgp_lac,
                         p.bssgp_ci,
                         p.nsip_bvci,
                         p.ip_src_host,
                         p.ip_dst_host,
                     } into ttt
                     select new
                     {
                         ttt.Key.bssgp_lac,
                         ttt.Key.bssgp_ci,
                         ttt.Key.nsip_bvci,
                         ttt.Key.ip_src_host,
                         ttt.Key.ip_dst_host,
                         cnt = ttt.Count()
                     };

            var bv = from p in fc.ToList()
                     where p.nsip_bvci != null
                     where p.bssgp_ci != null
                     where p.bssgp_lac != null
                     where p.ip_src_host != null
                     where p.ip_dst_host != null
                     select new LacCellBvci
                     {
                         _id = GenerateId(),
                         lac_cell = p.bssgp_lac.ToString() + "-" + p.bssgp_ci.ToString(),
                         src = p.ip_src_host,
                         dst = p.ip_dst_host,
                         bvci = p.nsip_bvci.ToString(),
                     };

            BulkMongo(bv.ToList());
        }

        public void BulkMongo(List<LacCellBvci> lcb)
        {
            mongo_lac_cell_bvci.BulkMongo(lcb, true);
        }

        public IQueryable<LacCellBvci> QueryMongo()
        {
            return mongo_lac_cell_bvci.QueryMongo();
        }

        private List<LacCellBvci> listlaccellbvci = null;
        public List<LacCellBvci> ListLacCellBvci
        {
            get
            {
                if (listlaccellbvci == null)
                {
                    listlaccellbvci = mongo_lac_cell_bvci.QueryMongo().AsQueryable<LacCellBvci>().ToList();
                }
                return listlaccellbvci;
            }
            set
            {
                value = listlaccellbvci;
            }
        }

        public string GetLacCell(string src, string dst, string bvci)
        {
            var query = from p in ListLacCellBvci
                        where (p.src == src && p.dst == dst && p.bvci == bvci) || (p.dst == src && p.src == dst && p.bvci == bvci)
                        select p;
            return query.Select(e => e.lac_cell).FirstOrDefault();
        }
    }
}
