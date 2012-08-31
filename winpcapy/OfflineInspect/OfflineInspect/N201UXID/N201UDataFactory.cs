using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfflineInspect.N201UXID
{
    public class N201UDataFactory : CommonDataLocation
    {
        public static void BathMakeN201UData()
        {
            BatchMakeN201UDataForMongo();
            BatchMakeN201UDataForSqlServer();
        }

        private static void BatchMakeN201UDataForMongo()
        {
            using (N201UXIDBeforeMessage nxbm = new N201UXIDBeforeMessage())
            {
                nxbm.CreateCollection();
                Console.WriteLine("N201UXIDBeforeMessage->mongo->ok");
            }
            GC.Collect();
            using (N201UXIDStatics nxs = new N201UXIDStatics())
            {
                nxs.CreateCollection();
                Console.WriteLine("N201UXIDStatics->mongo->ok");
            }
            GC.Collect();
        }

        private static void BatchMakeN201UDataForSqlServer()
        {
            using (N201UDbContext db = new N201UDbContext(sqlconn))
            {
                if (db.Database.Exists()) db.Database.Delete();

                db.Database.Create();

                Console.WriteLine(db.Database.Connection.ConnectionString);

                //加快入库，2012.8.30，30->0.5s级别。
                db.Configuration.AutoDetectChangesEnabled = false;


                foreach (var tcp in db.getN201UXIDStaticsDocumentSet())
                {
                    db.Set<N201UXIDStaticsDocument>().Add(tcp);
                    db.SaveChanges();
                }
                Console.WriteLine("N201UXIDStaticsDocument->N201UDbContext->ok");


                foreach (var tcp in db.getN201UXIDBeforeMessageDocumentSet())
                {
                    db.Set<N201UXIDBeforeMessageDocument>().Add(tcp);
                    db.SaveChanges();
                }
                Console.WriteLine("N201UXIDBeforeMessageDocument->N201UDbContext->ok");

            }
            Console.WriteLine("Finish");
            Console.ReadKey();
        }
    }
}
