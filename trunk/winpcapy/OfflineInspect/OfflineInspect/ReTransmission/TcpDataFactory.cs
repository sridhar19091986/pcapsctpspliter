using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfflineInspect.ReTransmission
{
    public class TcpDataFactory : CommonDataLocation
    {
        public static void BathMakeTcpData()
        {
            //BatchMakeTcpDataForMongo();
            BatchMakeTcpDataForSqlServer();
        }

        private static void BatchMakeTcpDataForMongo()
        {
            using (TcpPortSession tps = new TcpPortSession())
            {
                tps.CreateCollection();
                Console.WriteLine("TcpPortSession->mongo->ok");
            }
            GC.Collect();
            using (TcpRetransStatics trs = new TcpRetransStatics())
            {
                trs.CreatCollection();
                Console.WriteLine("TcpRetransStatics->mongo->ok");
            }
            GC.Collect();
        }

        private static void BatchMakeTcpDataForSqlServer()
        {
            using (TcpDbContext db = new TcpDbContext(sqlconn))
            {
                if (db.Database.Exists()) db.Database.Delete();

                db.Database.Create();

                Console.WriteLine(db.Database.Connection.ConnectionString);

                //加快入库，2012.8.30，30->0.5s级别。
                db.Configuration.AutoDetectChangesEnabled = false;

                /*
                foreach (var tcp in db.getTcpPortSessionDocumentSet())
                {
                    db.Set<TcpPortSessionDocument>().Add(tcp);
                    db.SaveChanges();
                }
                Console.WriteLine("TcpPortSession->TcpDbContext->ok");
                **/

                foreach (var tcp in db.getTcpRetransStaticsDocumentSet())
                {
                    db.Set<TcpRetransStaticsDocument>().Add(tcp);
                    db.SaveChanges();
                }
                Console.WriteLine("TcpRetransStatics->TcpDbContext->ok");

            }
            Console.WriteLine("Finish");
            Console.ReadKey();
        }
    }
}
