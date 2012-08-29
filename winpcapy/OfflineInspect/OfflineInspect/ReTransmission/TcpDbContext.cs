/*
 * 
 * 主要用户从mongo导出到sqlserver
 * 
 * */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Infrastructure;
using System.Data.Services;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Data.Entity.ModelConfiguration.Conventions.Edm.Db;
using System.Data.Entity.Database;
//using System.Linq;

namespace OfflineInspect.ReTransmission
{
    public class TcpDbContext : DbContext
    {
        public TcpDbContext(string connection) : base(connection) { }
        public DbSet<TcpPortSessionDocument> TcpPortSessionDocumentSet { get; set; }
        public IEnumerable<TcpPortSessionDocument> getTcpPortSessionDocumentSet()
        {
            TcpPortSession tps = new TcpPortSession();
            foreach (var tcp in tps.mongo_tts.QueryMongo())
                yield return tcp;
        }
        public DbSet<TcpRetransStaticsDocument> TcpRetransStaticsDocumentSet { get; set; }
        public IEnumerable<TcpRetransStaticsDocument> getTcpRetransStaticsDocumentSet()
        {
            TcpRetransStatics trs = new TcpRetransStatics();
            foreach (var tcp in trs.mongo_TcpRetransStatics.QueryMongo())
                yield return tcp;
        }
    }
}