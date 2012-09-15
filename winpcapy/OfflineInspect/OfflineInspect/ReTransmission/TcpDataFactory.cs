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

            BatchMakeTcpDataForMongo();
            //DeleteTcpTable();

            BatchMakeTcpDataForSqlServer();
            Console.WriteLine("BathMakeTcpData,Finish");
            Console.ReadKey();
        }

        private static void BatchMakeTcpDataForMongo()
        {
            using (LacCellBvci lcb = new LacCellBvci())
                lcb.CreatCollection();
            GC.Collect();
            using (LacCellBvciETL lcbs = new LacCellBvciETL())
                lcbs.CreatCollection();
            GC.Collect();
            using (TcpPortSession tps = new TcpPortSession())
                tps.CreateCollection();
            GC.Collect();
            using (TcpPortSessionETL trs = new TcpPortSessionETL())
                trs.CreatCollection();
            GC.Collect();
            using (LlcTlliSession tls = new LlcTlliSession())
                tls.CreateCollection();
            GC.Collect();
            Console.WriteLine("BatchMakeTcpDataForMongo,Finish");
        }

        private static void CreateTcpDb()
        {
            using (TcpDbContext db = new TcpDbContext(sqlconn))
            {
                if (db.Database.Exists())
                    db.Database.Delete();
                db.Database.Create();
                Console.WriteLine(db.Database.Connection.ConnectionString);
                Console.WriteLine("CreateTcpDb,Finish");
            }
            GC.Collect();
        }


        private static void BatchMakeTcpDataForSqlServer()
        {
            TcpDataContextSave tdcs = new TcpDataContextSave();
            tdcs.saveTcpPortSessionETLDocumentSet(sqlconn);

            GC.Collect();
            Console.WriteLine("......");
            Console.WriteLine("BatchMakeTcpDataForSqlServer,Finish");
        }

        private static void DeleteTcpTable()
        {
            using (TcpDbContext db = new TcpDbContext(sqlconn))
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                //DontDropDbJustCreateTablesIfModelChanged 

                //db.ChangeTracker.

                db.Database.SqlCommand("delete from TcpPortSessionDocuments");
                db.Database.SqlCommand("delete from TcpRetransStaticsDocuments");

                Console.WriteLine(db.Database.Connection.ConnectionString);
                Console.WriteLine("DeleteTcpTable,Finish");
            }
            GC.Collect();
        }

    }
}
