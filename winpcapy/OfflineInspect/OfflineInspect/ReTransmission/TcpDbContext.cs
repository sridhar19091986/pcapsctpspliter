/*
 * 
 * 主要用户从mongo导出到sqlserver
 * 
 * */


using System;
using System.Transactions;
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
using DreamSongs.MongoRepository;
//using System.Linq;

namespace OfflineInspect.ReTransmission
{
    public class TcpDbContext : DbContext, IDisposable
    {
        public TcpDbContext(string connection) : base(connection) { }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<TcpRetransStaticsDocument>()
        //                .HasRequired(a => a.vTcpPortSessionDocument)
        //                .WithMany()
        //                .HasForeignKey(u => u.tpsdID);
        //}

        //定义数据库
        public DbSet<TcpPortSessionDocument> TcpPortSessionDocumentSet { get; set; }
        //定义mongo导入sqlserver
        private IEnumerable<TcpPortSessionDocument> getTcpPortSessionDocumentSet()
        {
            TcpPortSession tps = new TcpPortSession();
            foreach (var tcp in tps.mongo_tts.QueryMongo())
                yield return tcp;
        }
        //执行mongo导入sqlserver
        public void saveTcpPortSessionDocumentSet(TcpDbContext db)
        {
            foreach (var tcp in db.getTcpPortSessionDocumentSet())
            {
                db.Set<TcpPortSessionDocument>().Add(tcp);
                //db.SaveChanges();
            }
            db.SaveChanges();
            Console.WriteLine("TcpPortSession->TcpDbContext->ok");
        }
        public DbSet<TcpRetransStaticsDocument> TcpRetransStaticsDocumentSet { get; set; }

        private int step = 0;
    
      

        public DbSet<LacCellBvciDocument> LacCellBvciDocumentSet { get; set; }
        private IEnumerable<LacCellBvciDocument> getLacCellBvciDocumentSet()
        {
            LacCellBvci lcb = new LacCellBvci();
            foreach (var cell in lcb.mongo_lac_cell_bvci.QueryMongo())
                yield return cell;
        }
        public void saveLacCellBvciDocumentSet(TcpDbContext db)
        {
            step = 0;
            foreach (var cell in db.getLacCellBvciDocumentSet())
            {
                step++;
                db.Set<LacCellBvciDocument>().Add(cell);
                if (step == 5000)
                {
                    db.SaveChanges();
                    step = 0;
                }
            }
            db.SaveChanges();
            Console.WriteLine("LacCellBvci->TcpDbContext->ok");
        }
        public DbSet<LacCellBvciStaticsDocument> LacCellBvciStaticsDocumentSet { get; set; }
        private IEnumerable<LacCellBvciStaticsDocument> getLacCellBvciStaticsDocumentSet()
        {
            LacCellBvciStatics lcbs = new LacCellBvciStatics();
            foreach (var cell in lcbs.mongo_LacCellBvciStatics.QueryMongo())
                yield return cell;
        }
        public void saveLacCellBvciStaticsDocumentSet(TcpDbContext db)
        {
            foreach (var cell in db.getLacCellBvciStaticsDocumentSet())
                db.Set<LacCellBvciStaticsDocument>().Add(cell);
            db.SaveChanges();
            Console.WriteLine("LacCellBvciStatics->TcpDbContext->ok");
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