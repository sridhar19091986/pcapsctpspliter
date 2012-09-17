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
//using EntitySqlTable.SqlServer.Local.Gb_TCP_ReTransmission;
using OfflineInspect.CommonTools;
using System.Data.Objects.SqlClient;
//using EntitySqlTable.SqlServer.Local.Gb_LLC_ReTransmission;
using EntitySqlTable.SqlServer.ip249.llc_data;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace OfflineInspect.ReTransmission.MapReduce
{
    public class LacCellBvciStagingDocument
    {
        #region 给sqlserver虚构一个主键，good,ef5 code first,/2012.8.29
        [Key]
        public long trsdID { get; set; }
        #endregion

        public long _id;
        public string src { get; set; }
        public string dst { get; set; }
        public int? bvci { get; set; }
        public string lac { get; set; }
        public string lac_cell { get; set; }
        public int cnt { get; set; }
        public string msg { get; set; }

        ////http://stackoverflow.com/questions/8111125/how-do-i-map-text-field-type-of-sql-server-in-ef-code-first
        //[Column(TypeName = "ntext")]
        //[MaxLength]
        public string callid { get; set; }
    }

    public class LacCellBvciStaging : CommonToolx, IDisposable
    {
        private string mongo_collection = CommonAttribute.LacCellBvciStaging[0];
        private string mongo_db = CommonAttribute.LacCellBvciStaging[1];
        private string mongo_conn = CommonAttribute.LacCellBvciStaging[2];

        public MongoCrud<LacCellBvciStagingDocument> mongo_LacCellBvciStaging;

        public LacCellBvciStaging()
        {
            mongo_LacCellBvciStaging = new MongoCrud<LacCellBvciStagingDocument>(mongo_conn, mongo_db, mongo_collection);
        }
        #region Implementing IDisposable and the Dispose Pattern Properly
        private bool disposed = false; // to detect redundant calls
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~LacCellBvciStaging()
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
            foshan_llc_dataEntities gb = new foshan_llc_dataEntities();
            //GuangZhou_Gb_LLC_ReTransmission gb = new GuangZhou_Gb_LLC_ReTransmission();
            //GuangZhou_Gb_TCP_ReTransmission gb = new GuangZhou_Gb_TCP_ReTransmission();
            gb.CommandTimeout = 0;
            gb.ContextOptions.LazyLoadingEnabled = true;
            gb.Gb_LLC_ReTransmission.MergeOption = MergeOption.NoTracking;

            var fc = from p in gb.Gb_LLC_ReTransmission
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
                         msg = ttt.Select(e => e.LLC_MsgType).FirstOrDefault(),//.First(),
                         callid = ttt.Select(e => e.BeginFrameNum).FirstOrDefault(),//.Distinct().First(),
                         /*
                         //http://stackoverflow.com/questions/1066760/problem-with-converting-int-to-string-in-linq-to-entities
                         callid = ttt.Select(e => SqlFunctions.StringConvert((double)e.BeginFileNum.Value).Trim()
                             + "-" + SqlFunctions.StringConvert((double)e.BeginFrameNum.Value).Trim()).Distinct()
                          * */

                     };

            foreach (var p in fc)
            {
                LacCellBvciStagingDocument lcbd = new LacCellBvciStagingDocument();
                lcbd._id = GenerateId();
                if (p.bssgp_lac != null)
                    lcbd.lac = p.bssgp_lac.ToString();
                if (p.bssgp_lac != null && p.bssgp_ci != null)
                    lcbd.lac_cell = p.bssgp_lac.ToString() + "-" + p.bssgp_ci.ToString();
                lcbd.src = p.ip_src_host;
                lcbd.dst = p.ip_dst_host;
                lcbd.bvci = p.nsip_bvci;
                lcbd.cnt = p.cnt;
                lcbd.msg = p.msg;
                if (p.callid != null)
                    lcbd.callid = p.callid.Value.ToString();

                mongo_LacCellBvciStaging.MongoCol.Insert(lcbd);
            }

            //mongo_lac_cell_bvci.BulkMongo(bv.ToList(), true);

            Console.WriteLine("LacCellBvciStagingDocument->mongo->ok");

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
        public string GetLacCell(string src, string dst, int? bvci)
        {
            //var query = from p in ListLacCellBvci
            var query = from p in mongo_LacCellBvciStaging.ListT
                        where (p.src == src && p.dst == dst && p.bvci == bvci)
                        || (p.dst == src && p.src == dst && p.bvci == bvci)
                        select p;
            return query.Select(e => e.lac_cell).IEnumDistinctStrComma();
        }
    }
}
