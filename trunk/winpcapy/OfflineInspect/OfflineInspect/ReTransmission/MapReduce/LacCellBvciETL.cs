using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OfflineInspect.CommonTools;
using OfflineInspect.Mongo;
using System.ComponentModel.DataAnnotations;
using OfflineInspect.ReTransmission.MapReduce;

namespace OfflineInspect.ReTransmission.MapReduce
{
    public class LacCellBvciETLDocument
    {
        [Key]
        public long trsdID { get; set; }
        public long _id;
        public string lac_ci { get; set; }
        public string bvci_aggre { get; set; }
        public int bvci_cnt { get; set; }
    }

    public class LacCellBvciETL : CommonToolx, IDisposable
    {
        private string mongo_collection = CommonAttribute.LacCellBvciETL[0];
        private string mongo_db = CommonAttribute.LacCellBvciETL[1];
        private string mongo_conn = CommonAttribute.LacCellBvciETL[2];

        public MongoCrud<LacCellBvciETLDocument> mongo_LacCellBvciETL;

        public LacCellBvciETL()
        {
            mongo_LacCellBvciETL = new MongoCrud<LacCellBvciETLDocument>(mongo_conn, mongo_db, mongo_collection);
        }
        #region Implementing IDisposable and the Dispose Pattern Properly
        private bool disposed = false; // to detect redundant calls
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~LacCellBvciETL()
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
        public void CreatCollection()
        {
            LacCellBvci lcb = new LacCellBvci();
            var query = from p in lcb.mongo_LacCellBvci.QueryMongo().ToList()
                        group p by p.lac_cell into ttt
                        select new LacCellBvciETLDocument
                        {
                            _id = GenerateId(),
                            lac_ci = ttt.Key,
                            bvci_aggre = ttt.Select(e => e.bvci.ToString()).Distinct().Aggregate((a, b) => a + "," + b),
                            bvci_cnt = ttt.Select(e => e.bvci).Distinct().Count()
                        };
            mongo_LacCellBvciETL.BulkMongo(query.ToList(), true);

            Console.WriteLine("LacCellBvciETLDocument->mongo->ok");
        }
    }
}
