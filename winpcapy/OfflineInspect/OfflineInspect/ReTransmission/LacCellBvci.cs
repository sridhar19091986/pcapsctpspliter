/*算法优化，2012.8.8
 * 
 * 尽量增加代码的复用度
 * 
 * 
 * 1.每个子类，只写实现方法，即完成统计的算法。
 * 
 * 2.每个子类，只写销毁方法，即完成算法后立即销毁。
 * 
 * 3.变化部分放入公共部分，以方便修改。
 * 
 * */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using OfflineInspect.Mongo;
using EntitySqlTable.SqlServer.Local.Gb_TCP_ReTransmission;
using OfflineInspect.CommonTools;



namespace OfflineInspect.ReTransmission
{
    public class LacCellBvciDocument
    {
        public object _id;
        public string src;
        public string dst;
        public string bvci;
        public string lac_cell;
        public int cnt;
    }

    public class LacCellBvci : CommonToolx, IDisposable
    {
        private string mongo_collection = CommonAttribute.LacCellBvci[0];
        private string mongo_db = CommonAttribute.LacCellBvci[1];
        private string mongo_conn = CommonAttribute.LacCellBvci[2];

        public MongoCrud<LacCellBvciDocument> mongo_lac_cell_bvci;

        public LacCellBvci()
        {
            mongo_lac_cell_bvci = new MongoCrud<LacCellBvciDocument>(mongo_conn, mongo_db, mongo_collection);
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
            GuangZhou_Gb_TCP_ReTransmission gb = new GuangZhou_Gb_TCP_ReTransmission();
            gb.CommandTimeout = 0;
            gb.ContextOptions.LazyLoadingEnabled = true;
            gb.Gb_TCP_ReTransmission.MergeOption = MergeOption.NoTracking;
            var fc = from p in gb.Gb_TCP_ReTransmission
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
                         cnt = ttt.Count(),
                     };

            var bv = from p in fc.ToList()
                     where p.nsip_bvci != null
                     where p.bssgp_ci != null
                     where p.bssgp_lac != null
                     where p.ip_src_host != null
                     where p.ip_dst_host != null
                     select new LacCellBvciDocument
                     {
                         _id = GenerateId(),
                         lac_cell = p.bssgp_lac.ToString() + "-" + p.bssgp_ci.ToString(),
                         src = p.ip_src_host,
                         dst = p.ip_dst_host,
                         bvci = p.nsip_bvci.ToString(),
                         cnt = p.cnt,
                     };
            mongo_lac_cell_bvci.BulkMongo(bv.ToList(), true);
            //BulkMongo(bv.ToList());
        }

        /*
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
        **/
        public string GetLacCell(string src, string dst, string bvci)
        {
            //var query = from p in ListLacCellBvci
            var query = from p in mongo_lac_cell_bvci.ListT
                        where (p.src == src && p.dst == dst && p.bvci == bvci)
                        || (p.dst == src && p.src == dst && p.bvci == bvci)
                        select p;
            return query.Select(e => e.lac_cell).FirstOrDefault();
        }
    }
}
