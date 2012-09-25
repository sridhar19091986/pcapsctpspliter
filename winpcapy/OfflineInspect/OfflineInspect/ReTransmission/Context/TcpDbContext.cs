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
        //public DbSet<TcpPortSessionStagingDocument> TcpPortSessionStagingDocumentSet { get; set; }
        //public DbSet<LacCellBvciStagingDocument> LacCellBvciStagingDocumentSet { get; set; }
        //public DbSet<LacCellBvciETLDocument> LacCellBvciETLDocumentSet { get; set; }
        //public DbSet<LlcTlliSessionStagingDocument> LlcTlliSessionDocumentSet { get; set; }

        #region  //public DbSet<TcpPortSessionETLDocument> TcpPortSessionETLDocumentSet { get; set; }
        public DbSet<DimensionIp> DimensionIpSet { get; set; }
        public DbSet<DimensionUdp> DimensionUdpSet { get; set; }
        public DbSet<DimensionNs> DimensionNsSet { get; set; }
        public DbSet<DimensionBssgp> DimensionBssgpSet { get; set; }
        public DbSet<DimensionLlcSndcp> DimensionLlcSndcpSet { get; set; }
        public DbSet<DimensionIp2> DimensionIp2Set { set; get; }
        public DbSet<DimensionTcp> DimensionTcpSet { get; set; }
        public DbSet<DimensionHttp> DimensionHttpSet { get; set; }
        public DbSet<DimensionMessage> DimensionMessageSet { get; set; }
        public DbSet<CalculationItem> CalculationItemSet { get; set; }
        public DbSet<FactTcpPortSession> FactTcpPortSessionSet { get; set; }
        #endregion


        public TcpDbContext AddToContext(TcpDbContext context, DimensionIp entity, int count, int commitCount, bool recreateContext, string sqlconn)
        {
            context.Set<DimensionIp>().Add(entity);
            return AddToContextCon(context, count, commitCount, recreateContext, sqlconn);
        }
        public TcpDbContext AddToContext(TcpDbContext context, DimensionUdp entity, int count, int commitCount, bool recreateContext, string sqlconn)
        {
            context.Set<DimensionUdp>().Add(entity);
            return AddToContextCon(context, count, commitCount, recreateContext, sqlconn);
        }
        public TcpDbContext AddToContext(TcpDbContext context, DimensionNs entity, int count, int commitCount, bool recreateContext, string sqlconn)
        {
            context.Set<DimensionNs>().Add(entity);
            return AddToContextCon(context, count, commitCount, recreateContext, sqlconn);
        }
        public TcpDbContext AddToContext(TcpDbContext context, DimensionBssgp entity, int count, int commitCount, bool recreateContext, string sqlconn)
        {
            context.Set<DimensionBssgp>().Add(entity);
            return AddToContextCon(context, count, commitCount, recreateContext, sqlconn);
        }
        public TcpDbContext AddToContext(TcpDbContext context, DimensionLlcSndcp entity, int count, int commitCount, bool recreateContext, string sqlconn)
        {
            context.Set<DimensionLlcSndcp>().Add(entity);
            return AddToContextCon(context, count, commitCount, recreateContext, sqlconn);
        }
        public TcpDbContext AddToContext(TcpDbContext context, DimensionIp2 entity, int count, int commitCount, bool recreateContext, string sqlconn)
        {
            context.Set<DimensionIp2>().Add(entity);
            return AddToContextCon(context, count, commitCount, recreateContext, sqlconn);
        }
        public TcpDbContext AddToContext(TcpDbContext context, DimensionTcp entity, int count, int commitCount, bool recreateContext, string sqlconn)
        {
            context.Set<DimensionTcp>().Add(entity);
            return AddToContextCon(context, count, commitCount, recreateContext, sqlconn);
        }
        public TcpDbContext AddToContext(TcpDbContext context, DimensionHttp entity, int count, int commitCount, bool recreateContext, string sqlconn)
        {
            context.Set<DimensionHttp>().Add(entity);
            return AddToContextCon(context, count, commitCount, recreateContext, sqlconn);
        }
        public TcpDbContext AddToContext(TcpDbContext context, DimensionMessage entity, int count, int commitCount, bool recreateContext, string sqlconn)
        {
            context.Set<DimensionMessage>().Add(entity);
            return AddToContextCon(context, count, commitCount, recreateContext, sqlconn);
        }
        public TcpDbContext AddToContext(TcpDbContext context, CalculationItem entity, int count, int commitCount, bool recreateContext, string sqlconn)
        {
            context.Set<CalculationItem>().Add(entity);
            return AddToContextCon(context, count, commitCount, recreateContext, sqlconn);
        }
        public TcpDbContext AddToContext(TcpDbContext context, FactTcpPortSession entity, int count, int commitCount, bool recreateContext, string sqlconn)
        {
            context.Set<FactTcpPortSession>().Add(entity);
            return AddToContextCon(context, count, commitCount, recreateContext, sqlconn);
        }

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

        public TcpDbContext AddToContext(TcpDbContext context, LlcTlliSessionStagingDocument entity, int count, int commitCount, bool recreateContext, string sqlconn)
        {
            context.Set<LlcTlliSessionStagingDocument>().Add(entity);
            return AddToContextCon(context, count, commitCount, recreateContext, sqlconn);
        }

        public TcpDbContext AddToContext(TcpDbContext context, TcpPortSessionStagingDocument entity, int count, int commitCount, bool recreateContext, string sqlconn)
        {
            context.Set<TcpPortSessionStagingDocument>().Add(entity);
            return AddToContextCon(context, count, commitCount, recreateContext, sqlconn);
        }

        public TcpDbContext AddToContext(TcpDbContext context, LacCellBvciStagingDocument entity, int count, int commitCount, bool recreateContext, string sqlconn)
        {
            context.Set<LacCellBvciStagingDocument>().Add(entity);
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