using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
//using OfflineInspect.ReTransmission.Table;
using DreamSongs.MongoRepository;
using OfflineInspect.ReTransmission.MapReduce;

namespace OfflineInspect.ReTransmission.Context
{
    public class TcpDataContextSave
    {
        //执行mongo导入sqlserver
        private int bulksize = 1000;
        public void saveTcpPortSessionDocumentSet(string sqlconn)
        {
            TcpDbContext context = null;
            try
            {
                context = new TcpDbContext(sqlconn);
                context.Configuration.AutoDetectChangesEnabled = false;

                int count = 0;
                TcpPortSession tps = new TcpPortSession();
                foreach (var tcp in tps.mongo_TcpPortSession.QueryMongo())
                {
                    ++count;
                    context = context.AddToContext(context, tcp, count, bulksize, true, sqlconn);
                }

                context.SaveChanges();
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
            Console.WriteLine("TcpPortSessionDocument->TcpDbContext->ok");
        }
        public void saveTcpPortSessionETLDocumentSet(string sqlconn)
        {
            TcpDbContext context = null;
            try
            {
                context = new TcpDbContext(sqlconn);
                context.Configuration.AutoDetectChangesEnabled = false;

                int count = 0;
                TcpPortSessionETL trs = new TcpPortSessionETL();
                foreach (var tcp in trs.mongo_TcpPortSessionETL.QueryMongo())
                {
                    ++count;
                    context = context.AddToContext(context, tcp, count, bulksize, true, sqlconn);
                }

                context.SaveChanges();
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
            Console.WriteLine("TcpPortSessionETLDocument->TcpDbContext->ok");
        }

        public void saveLacCellBvciDocumentSet(string sqlconn)
        {
            TcpDbContext context = null;
            try
            {
                context = new TcpDbContext(sqlconn);
                context.Configuration.AutoDetectChangesEnabled = false;

                int count = 0;
                LacCellBvci lcb = new LacCellBvci();
                foreach (var cell in lcb.mongo_LacCellBvci.QueryMongo())
                {
                    ++count;
                    context = context.AddToContext(context, cell, count, bulksize, true, sqlconn);
                }

                context.SaveChanges();
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
            Console.WriteLine("LacCellBvciDocument->TcpDbContext->ok");
        }

        public void saveLacCellBvciETLDocumentSet(string sqlconn)
        {

            TcpDbContext context = null;
            try
            {
                context = new TcpDbContext(sqlconn);
                context.Configuration.AutoDetectChangesEnabled = false;

                int count = 0;
                LacCellBvciETL lcbs = new LacCellBvciETL();
                foreach (var cell in lcbs.mongo_LacCellBvciETL.QueryMongo())
                {
                    ++count;
                    context = context.AddToContext(context, cell, count, bulksize, true, sqlconn);
                }

                context.SaveChanges();
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
            Console.WriteLine("LacCellBvciETLDocument->TcpDbContext->ok");
        }

        public void saveLlcTlliSessionDocumentSet(string sqlconn)
        {
            TcpDbContext context = null;
            try
            {
                context = new TcpDbContext(sqlconn);
                context.Configuration.AutoDetectChangesEnabled = false;

                int count = 0;
                LlcTlliSession tls = new LlcTlliSession();
                foreach (var llc in tls.mongo_LlcTlliSession.QueryMongo())
                {
                    ++count;
                    context = context.AddToContext(context, llc, count, bulksize, true, sqlconn);
                }

                context.SaveChanges();
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
            Console.WriteLine("LlcTlliSessionDocument->TcpDbContext->ok");
        }


    }
}
