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
//using OfflineInspect.ReTransmission.Table;
using DreamSongs.MongoRepository;
using OfflineInspect.ReTransmission.MapReduce;
//using System.Linq;

namespace OfflineInspect.ReTransmission.Context
{
    public class TcpDbContext : DbContext, IDisposable
    {
        public TcpDbContext(string connection) : base(connection) { }

        //定义数据库
        public DbSet<TcpPortSessionDocument> TcpPortSessionDocumentSet { get; set; }
        public DbSet<TcpPortSessionETLDocument> TcpRetransStaticsDocumentSet { get; set; }
        public DbSet<LacCellBvciDocument> LacCellBvciDocumentSet { get; set; }
        public DbSet<LacCellBvciETLDocument> LacCellBvciStaticsDocumentSet { get; set; }
        public DbSet<LlcTlliSessionDocument> TlliLLCSessionDocumentSet { get; set; }

        public TcpDbContext AddToContext(TcpDbContext context, TcpPortSessionETLDocument entity, int count, int commitCount, bool recreateContext, string sqlconn)
        {
            context.Set<TcpPortSessionETLDocument>().Add(entity);
            return AddToContextCon(context, count, commitCount, recreateContext, sqlconn);
        }

        public TcpDbContext AddToContext(TcpDbContext context, LacCellBvciETLDocument entity, int count, int commitCount, bool recreateContext, string sqlconn)
        {
            context.Set<LacCellBvciETLDocument>().Add(entity);
            return AddToContextCon(context, count, commitCount, recreateContext, sqlconn);
        }

        public TcpDbContext AddToContext(TcpDbContext context, LlcTlliSessionDocument entity, int count, int commitCount, bool recreateContext, string sqlconn)
        {
            context.Set<LlcTlliSessionDocument>().Add(entity);
            return AddToContextCon(context, count, commitCount, recreateContext, sqlconn);
        }

        public TcpDbContext AddToContext(TcpDbContext context, TcpPortSessionDocument entity, int count, int commitCount, bool recreateContext, string sqlconn)
        {
            context.Set<TcpPortSessionDocument>().Add(entity);
            return AddToContextCon(context, count, commitCount, recreateContext, sqlconn);
        }

        public TcpDbContext AddToContext(TcpDbContext context, LacCellBvciDocument entity, int count, int commitCount, bool recreateContext, string sqlconn)
        {
            context.Set<LacCellBvciDocument>().Add(entity);
            return AddToContextCon(context, count, commitCount, recreateContext, sqlconn);
        }

        public TcpDbContext AddToContextCon(TcpDbContext context, int count, int commitCount, bool recreateContext, string sqlconn)
        {
            if (count % commitCount == 0)
            {
                Console.WriteLine(count);
                context.SaveChanges();
                if (recreateContext)
                {
                    context.Dispose();
                    context = new TcpDbContext(sqlconn);
                    context.Configuration.AutoDetectChangesEnabled = false;
                }
            }
            return context;
        }
    }
}