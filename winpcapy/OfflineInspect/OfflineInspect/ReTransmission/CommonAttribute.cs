using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Infrastructure;
using System.Data.EntityClient;

namespace OfflineInspect.ReTransmission
{
    public class CommonAttribute
    {
        private static string remote = "mongodb://localhost/?safe=true";
        private static string db = "ReTransmission";
        public static string[] LacCellBvci = new String[] { "LacCellBvci", db, remote };
        public static string[] TlliTcpSession = new String[] { "TlliTcpSession", db, remote, "3", "5000" };
        public static string[] TlliLLCSession = new String[] { "TlliLLCSession", db, remote, "3", "5000" };
        public static string[] TcpRetransStatics = new String[] { "TcpRetransStatics", db, remote };

        public static void ExecReTransmission()
        {
            using (LacCellBvci lcb = new LacCellBvci())
            {
                lcb.CreatCollection();
                Console.WriteLine(" LacCellBvci lcb = new LacCellBvci();ok");
            }
            GC.Collect();
            //using (TlliTcpSession tts = new TlliTcpSession())
            //{
            //    tts.CreateCollection();
            //    Console.WriteLine("TlliTcpSession tts = new TlliTcpSession();ok");
            //}
            //GC.Collect();
            using (TlliLLCSession tls = new TlliLLCSession())
            {
                tls.CreateCollection();
                Console.WriteLine("TlliLLCSession tls = new TlliLLCSession();ok");
            }
            GC.Collect();
            //using (TcpRetransStatics trs = new TcpRetransStatics())
            //{
            //    trs.CreatCollection();
            //    Console.WriteLine("TcpRetransStatics trs = new TcpRetransStatics();ok");
            //}
            //GC.Collect();
        }
        private static string sqlconn = "Data Source=localhost;Initial Catalog=MyDbContext;Integrated Security=True;";
        public static void ExecMongoExportSql()
        {
            SqlConnection con = new SqlConnection(sqlconn);
            using (MyDbContext db = new MyDbContext(con))
            {
                db.Database.DeleteIfExists();
                db.Database.Create();
                Console.WriteLine(db.Database.Connection.ConnectionString);

                foreach (var tcp in db.getTcpRetransStaticsTableRow())
                {
                    db.Set<TcpRetransStaticsTable>().Add(tcp);
                    db.SaveChanges();
                }

                foreach (var llc in db.getTlliLLCSessionTableRow())
                {
                    db.Set<TlliLLCSessionTable>().Add(llc);
                    db.SaveChanges();
                }
            }
            Console.WriteLine("Finish");
            Console.ReadKey();
        }
    }
}
