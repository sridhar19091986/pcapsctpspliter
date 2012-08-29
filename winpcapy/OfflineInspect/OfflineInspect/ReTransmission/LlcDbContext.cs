using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace OfflineInspect.ReTransmission
{
    public class LlcDbContext : DbContext
    {
        public DbSet<TlliLLCSessionDocument> TlliLLCSessionDocumentSet { get; set; }
        public IEnumerable<TlliLLCSessionDocument> getTlliLLCSessionDocumentSet()
        {
            TlliLLCSession tls = new TlliLLCSession();
            foreach (var llc in tls.mongo_tls.QueryMongo())
                yield return llc;
        }
    }
}
