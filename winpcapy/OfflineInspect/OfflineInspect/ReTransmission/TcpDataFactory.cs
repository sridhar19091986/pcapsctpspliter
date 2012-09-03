using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OfflineInspect.ReTransmission.Table;

namespace OfflineInspect.ReTransmission
{
    public class TcpDataFactory : CommonDataLocation
    {
        public static void BathMakeTcpData()
        {
            BatchMakeTcpDataForMongo();
            CreateTcpDb();
            BatchMakeTcpDataForSqlServer();
        }

        private static void BatchMakeTcpDataForMongo()
        {
            using (LacCellBvci lcb = new LacCellBvci())
                lcb.CreatCollection();
            GC.Collect();
            using (TcpPortSession tps = new TcpPortSession())
                tps.CreateCollection();
            GC.Collect();
            using (TcpRetransStatics trs = new TcpRetransStatics())
                trs.CreatCollection();
            GC.Collect();
            using (TlliLLCSession tls = new TlliLLCSession())
                tls.CreateCollection();
            GC.Collect();
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
        }

        private static void BatchMakeTcpDataForSqlServer()
        {
            using (TcpDbContext db = new TcpDbContext(sqlconn))
            {
                //加快入库，2012.8.30，30->0.5s级别。
                db.Configuration.AutoDetectChangesEnabled = false;

                db.saveLacCellBvciDocumentSet(db);
                db.saveTcpPortSessionDocumentSet(db);
                db.saveTcpRetransStaticsDocumentSet(db);
                db.saveTlliLLCSessionDocumentSet(db);
            }
            Console.WriteLine("Finish");
            Console.ReadKey();
        }
    }
}
