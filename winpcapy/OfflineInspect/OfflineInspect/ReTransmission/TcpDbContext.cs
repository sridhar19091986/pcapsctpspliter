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
using OfflineInspect.ReTransmission.Table;
//using System.Linq;

namespace OfflineInspect.ReTransmission
{
    public class TcpDbContext : DbContext
    {
        public TcpDbContext(string connection) : base(connection) { }
        public DbSet<TcpPortSessionDocument> TcpPortSessionDocumentSet { get; set; }
        private IEnumerable<TcpPortSessionDocument> getTcpPortSessionDocumentSet()
        {
            TcpPortSession tps = new TcpPortSession();
            foreach (var tcp in tps.mongo_tts.QueryMongo())
                yield return tcp;
        }
        public void saveTcpPortSessionDocumentSet(TcpDbContext db)
        {
            foreach (var tcp in db.getTcpPortSessionDocumentSet())
            {
                db.Set<TcpPortSessionDocument>().Add(tcp);
            }
            db.SaveChanges();
            Console.WriteLine("TcpPortSession->TcpDbContext->ok");
        }
        public DbSet<TcpRetransStaticsDocument> TcpRetransStaticsDocumentSet { get; set; }
        private IEnumerable<TcpRetransStaticsDocument> getTcpRetransStaticsDocumentSet()
        {
            TcpRetransStatics trs = new TcpRetransStatics();
            foreach (var tcp in trs.mongo_TcpRetransStatics.QueryMongo())
                yield return tcp;
        }
        public void saveTcpRetransStaticsDocumentSet(TcpDbContext db)
        {
            foreach (var tcp in db.getTcpRetransStaticsDocumentSet())
            {
                db.Set<TcpRetransStaticsDocument>().Add(tcp);
            }
            db.SaveChanges();
            Console.WriteLine("TcpRetransStatics->TcpDbContext->ok");
        }
        public DbSet<LacCellBvciDocument> LacCellBvciDocumentSet { get; set; }
        private IEnumerable<LacCellBvciDocument> getLacCellBvciDocumentSet()
        {
            LacCellBvci lcb = new LacCellBvci();
            foreach (var cell in lcb.mongo_lac_cell_bvci.QueryMongo())
                yield return cell;
        }
        public void saveLacCellBvciDocumentSet(TcpDbContext db)
        {
            foreach (var cell in db.getLacCellBvciDocumentSet())
            {
                db.Set<LacCellBvciDocument>().Add(cell);
            }
            db.SaveChanges();
            Console.WriteLine("LacCellBvci->TcpDbContext->ok");
        }
        public DbSet<TlliLLCSessionDocument> TlliLLCSessionDocumentSet { get; set; }
        private IEnumerable<TlliLLCSessionDocument> getTlliLLCSessionDocumentSet()
        {
            TlliLLCSession tls = new TlliLLCSession();
            foreach (var llc in tls.mongo_tls.QueryMongo())
                yield return llc;
        }
        public void saveTlliLLCSessionDocumentSet(TcpDbContext db)
        {
            foreach (var llc in db.getTlliLLCSessionDocumentSet())
            {
                db.Set<TlliLLCSessionDocument>().Add(llc);
            }
            db.SaveChanges();
            Console.WriteLine("LacCellBvci->TcpDbContext->ok");
        }
    }
}