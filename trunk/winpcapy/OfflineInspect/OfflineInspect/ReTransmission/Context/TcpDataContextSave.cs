using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
//using OfflineInspect.ReTransmission.Table;
using DreamSongs.MongoRepository;
using OfflineInspect.ReTransmission.MapReduce;
using System.Threading.Tasks;

namespace OfflineInspect.ReTransmission.Context
{
    public class TcpDataContextSave
    {
        //执行mongo导入sqlserver
        private int bulksize = 5000;
        private int contextlen = 11;
        public void saveTcpPortSessionDocumentSet(string sqlconn)
        {
            TcpDbContext context = null;
            try
            {
                context = new TcpDbContext(sqlconn);
                context.Configuration.AutoDetectChangesEnabled = false;

                int count = 0;
                TcpPortSessionStaging tps = new TcpPortSessionStaging();
                foreach (var tcp in tps.mongo_TcpPortSessionStaging.QueryMongo())
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

        //生成维度表和事实表，同时开启多个连接进行处理？
        public void saveTcpPortSessionETLDocumentSet(string sqlconn)
        {
            TcpDbContext[] context = new TcpDbContext[contextlen];
            try
            {
                for (int i = 0; i < context.Length; i++)
                {
                    context[i] = new TcpDbContext(sqlconn);
                    context[i].Configuration.AutoDetectChangesEnabled = false;
                }

                int count = 0;
                TcpPortSessionETL trs = new TcpPortSessionETL();
                foreach (var tcp in trs.mongo_TcpPortSessionETL.QueryMongo())
                {
                    ++count;
                    Parallel.Invoke(() =>
                        {
                            context[0] = context[0].AddToContext(context[0], tcp.DimIp, count, bulksize, true, sqlconn);
                            context[1] = context[1].AddToContext(context[1], tcp.DimUdp, count, bulksize, true, sqlconn);
                            context[2] = context[2].AddToContext(context[2], tcp.DimNs, count, bulksize, true, sqlconn);
                            context[3] = context[3].AddToContext(context[3], tcp.DimBssgp, count, bulksize, true, sqlconn);
                            context[4] = context[4].AddToContext(context[4], tcp.DimLlcSndcp, count, bulksize, true, sqlconn);
                            context[5] = context[5].AddToContext(context[5], tcp.DimIp2, count, bulksize, true, sqlconn);
                            context[6] = context[6].AddToContext(context[6], tcp.DimTcp, count, bulksize, true, sqlconn);
                            context[7] = context[7].AddToContext(context[7], tcp.DimHttp, count, bulksize, true, sqlconn);
                            context[8] = context[8].AddToContext(context[8], tcp.DimMessage, count, bulksize, true, sqlconn);
                            context[9] = context[9].AddToContext(context[9], tcp.CalItem, count, bulksize, true, sqlconn);
                            context[10] = context[10].AddToContext(context[10], tcp.FactTcp, count, bulksize, true, sqlconn);
                        });
                }

                for (int i = 0; i < context.Length; i++)
                    context[i].SaveChanges();

            }
            finally
            {
                for (int i = 0; i < context.Length; i++)
                    if (context[i] != null)
                        context[i].Dispose();
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
                LacCellBvciStaging lcb = new LacCellBvciStaging();
                foreach (var cell in lcb.mongo_LacCellBvciStaging.QueryMongo())
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
                LlcTlliSessionStaging tls = new LlcTlliSessionStaging();
                foreach (var llc in tls.mongo_LlcTlliSessionStaging.QueryMongo())
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
