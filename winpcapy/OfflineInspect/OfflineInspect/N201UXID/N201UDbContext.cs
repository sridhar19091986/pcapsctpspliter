using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace OfflineInspect.N201UXID
{
    public class N201UDbContext : DbContext
    {
        public N201UDbContext(string connection) : base(connection) { }
        public DbSet<N201UXIDStaticsDocument> N201UXIDStaticsDocumentSet { get; set; }
        public IEnumerable<N201UXIDStaticsDocument> getN201UXIDStaticsDocumentSet()
        {
            N201UXIDStatics nxs = new N201UXIDStatics();
            foreach (var tcp in nxs.mongo_fcmr.QueryMongo())
                yield return tcp;
        }
        public DbSet<N201UXIDBeforeMessageDocument> N201UXIDBeforeMessageDocumentSet { get; set; }
        public IEnumerable<N201UXIDBeforeMessageDocument> getN201UXIDBeforeMessageDocumentSet()
        {
            N201UXIDBeforeMessage nxbf = new N201UXIDBeforeMessage();
            foreach (var tcp in nxbf.mongo_fcbm.QueryMongo())
                yield return tcp;
        }
    }
}
