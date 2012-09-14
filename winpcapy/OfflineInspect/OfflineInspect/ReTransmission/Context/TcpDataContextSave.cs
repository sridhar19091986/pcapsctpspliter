using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using OfflineInspect.ReTransmission.Table;
using DreamSongs.MongoRepository;

namespace OfflineInspect.ReTransmission
{
    class TcpDataContextSave
    {
        //执行mongo导入sqlserver
        private int bulksize = 1000;
        public void saveTcpRetransStaticsDocumentSet(string sqlconn)
        {
            TcpDbContext context = null;
            try
            {
                context = new TcpDbContext(sqlconn);
                context.Configuration.AutoDetectChangesEnabled = false;

                int count = 0;
                TcpRetransStatics trs = new TcpRetransStatics();
                foreach (var tcp in trs.mongo_TcpRetransStatics.QueryMongo())
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
            Console.WriteLine("TcpRetransStatics->TcpDbContext->ok");
        }

        public void saveLacCellBvciStaticsDocumentSet(string sqlconn)
        {

            TcpDbContext context = null;
            try
            {
                context = new TcpDbContext(sqlconn);
                context.Configuration.AutoDetectChangesEnabled = false;

                int count = 0;
                LacCellBvciStatics lcbs = new LacCellBvciStatics();
                foreach (var cell in lcbs.mongo_LacCellBvciStatics.QueryMongo())
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
            Console.WriteLine("LacCellBvciStatics->TcpDbContext->ok");
        }

        public void saveTlliLLCSessionDocumentSet(string sqlconn)
        {
            TcpDbContext context = null;
            try
            {
                context = new TcpDbContext(sqlconn);
                context.Configuration.AutoDetectChangesEnabled = false;

                int count = 0;
                TlliLLCSession tls = new TlliLLCSession();
                foreach (var llc in tls.mongo_tls.QueryMongo())
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
            Console.WriteLine("TlliLLCSession->TcpDbContext->ok");
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
                foreach (var cell in lcb.mongo_lac_cell_bvci.QueryMongo())
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
            Console.WriteLine("LacCellBvci->TcpDbContext->ok");
        }

        public void saveTcpPortSessionDocumentSet(string sqlconn)
        {
            TcpDbContext context = null;
            try
            {
                context = new TcpDbContext(sqlconn);
                context.Configuration.AutoDetectChangesEnabled = false;

                int count = 0;
                TcpPortSession tps = new TcpPortSession();
                foreach (var tcp in tps.mongo_tts.QueryMongo())
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
            Console.WriteLine("TcpPortSession->TcpDbContext->ok");
        }
    }
}
