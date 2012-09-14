using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OfflineInspect.CommonTools;
using OfflineInspect.Mongo;
using System.ComponentModel.DataAnnotations;

namespace OfflineInspect.ReTransmission.Table
{
    public class LacCellBvciStaticsDocument
    {
        [Key]
        public long trsdID { get; set; }
        public long _id;
        public string lac_ci { get; set; }
        public string bvci_aggre { get; set; }
        public int bvci_cnt { get; set; }
    }

    public class LacCellBvciStatics : CommonToolx, IDisposable
    {
        private string mongo_collection = CommonAttribute.LacCellBvciStatics[0];
        private string mongo_db = CommonAttribute.LacCellBvciStatics[1];
        private string mongo_conn = CommonAttribute.LacCellBvciStatics[2];

        public MongoCrud<LacCellBvciStaticsDocument> mongo_LacCellBvciStatics;

        public LacCellBvciStatics()
        {
            mongo_LacCellBvciStatics = new MongoCrud<LacCellBvciStaticsDocument>(mongo_conn, mongo_db, mongo_collection);
        }
        #region Implementing IDisposable and the Dispose Pattern Properly
        private bool disposed = false; // to detect redundant calls
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~LacCellBvciStatics()
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
            var query = from p in lcb.mongo_lac_cell_bvci.QueryMongo().ToList()
                        group p by p.lac_cell into ttt
                        select new LacCellBvciStaticsDocument
                        {
                            _id = GenerateId(),
                            lac_ci = ttt.Key,
                            bvci_aggre = ttt.Select(e => e.bvci.ToString()).Distinct().Aggregate((a, b) => a + "," + b),
                            bvci_cnt = ttt.Select(e => e.bvci).Distinct().Count()
                        };
            mongo_LacCellBvciStatics.BulkMongo(query.ToList(), true);

            Console.WriteLine("mongo_LacCellBvciStatics->mongo->ok");
        }
    }
}
