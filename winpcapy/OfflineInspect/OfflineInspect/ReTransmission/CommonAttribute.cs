using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfflineInspect.ReTransmission
{
    public class CommonAttribute
    {
        private static string remote = "mongodb://localhost/?safe=true";
        private static string db = "ReTransmission";
        public static string[] LacCellBvci = new String[] { "LacCellBvci", db, remote };
        public static string[] TlliTcpSession = new String[] { "TlliTcpSession", db, remote, "3", "5000" };
        public static string[] TlliLLCSession = new String[] { "TlliLLCSession", db, remote, "3", "5000" };
        public static string[] TcpRetransStatics = new String[] { "TcpRetransStatics", db, remote};

        public static void ExecReTransmission()
        {
            //using (LacCellBvci lcb = new LacCellBvci())
            //{
            //    lcb.CreatCollection();
            //    Console.WriteLine(" LacCellBvci lcb = new LacCellBvci();ok");
            //}
            //GC.Collect();
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
            GC.Collect();
        }
    }
}
