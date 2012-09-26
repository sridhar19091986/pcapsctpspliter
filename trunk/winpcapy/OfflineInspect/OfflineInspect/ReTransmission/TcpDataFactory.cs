using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OfflineInspect.ReTransmission.MapReduce;
using OfflineInspect.ReTransmission.Context;
//using OfflineInspect.ReTransmission.Table;

namespace OfflineInspect.ReTransmission
{
    public class TcpDataFactory : CommonDataLocation
    {
        public static void BathMakeTcpData()
        {
            CreateTcpDb();
            //BatchMakeLlcDataForMongo();
            BatchMakeTcpDataForMongo();
            BatchMakeTcpDataForSqlServer();

            Console.WriteLine("\r\n......");
            Console.WriteLine("BathMakeTcpData,Finish");
            Console.ReadKey();
        }

        private static void CreateTcpDb()
        {
            using (TcpDbContext db = new TcpDbContext(sqlconn))
            {
                if (db.Database.Exists())
                    db.Database.Delete();
                db.Database.Create();
                Console.WriteLine(db.Database.Connection.ConnectionString);
            }
            GC.Collect();
            Console.WriteLine("CreateTcpDb,Finish");
        }

        private static void BatchMakeLlcDataForMongo()
        {
            using (LacCellBvciStaging lcb = new LacCellBvciStaging())
                lcb.CreatCollection();
            GC.Collect();
            using (LacCellBvciETL lcbs = new LacCellBvciETL())
                lcbs.CreatCollection();
            GC.Collect();
            //using (LlcTlliSessionStaging tls = new LlcTlliSessionStaging())
            //    tls.CreateCollection();
            //GC.Collect();
            Console.WriteLine("BatchMakeLlcDataForMongo,Finish");
        }

        private static void BatchMakeTcpDataForMongo()
        {
            //using (TcpPortSessionStaging tps = new TcpPortSessionStaging())
            //    tps.CreateCollection();
            //GC.Collect();
            using (TcpPortSessionETL trs = new TcpPortSessionETL())
                trs.CreatCollection();
            GC.Collect();
            Console.WriteLine("BatchMakeTcpDataForMongo,Finish");
        }

        private static void BatchMakeTcpDataForSqlServer()
        {
            TcpDataContextSave tdcs = new TcpDataContextSave();
            tdcs.saveTcpPortSessionETLDocumentSet(sqlconn);
            GC.Collect();
            Console.WriteLine("BatchMakeTcpDataForSqlServer,Finish");
        }
    }
}
