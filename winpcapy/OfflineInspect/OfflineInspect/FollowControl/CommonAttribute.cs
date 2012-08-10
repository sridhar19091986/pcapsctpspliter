/*
 * 
 * 通过数组统一传递参数
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfflineInspect.FlowControl
{
    //mongo_collection , mongo_db, mongo_conn, maxfilenum
    public class CommonAttribute
    {
        private static string remote = "mongodb://192.168.4.209/?safe=true";
        private static string db = "Guangzhou_FlowControl";
        public static string[] LacCellBvci = new String[] { "LacCellBvci", db, remote };
        public static string[] FlowControlOneMs = new String[] { "FlowControlOneMs", db, remote, "1" };
        public static string[] FlowControlOneBvc = new String[] { "FlowControlOneBvc", db, remote, "3" };
        public static string[] FlowControlMessageRate = new String[] { "FlowControlMessageRate", db, remote };
        public static string[] FlowControlMapMs = new String[] { "FlowControlMapMs", db, remote, "BSSGP.FLOW-CONTROL-MS" };
        public static string[] FlowControlMapBvc = new String[] { "FlowControlMapBvc", db, remote, "BSSGP.FLOW-CONTROL-MS", "BSSGP.FLOW-CONTROL-BVC" };
        public static string[] FlowControlBeforeMessageMap = new String[] { "FlowControlBeforeMessageMap", db, remote };
        public static string[] FlowControlBeforeMessage = new String[] { "FlowControlBeforeMessage", db, remote, "BSSGP.FLOW-CONTROL-MS" };

        public static void ExecFlowControl()
        {
            Console.WriteLine("Flow Control Statics Write");

            using (LacCellBvci lcb = new LacCellBvci())
            {
                lcb.CreatCollection();
                Console.WriteLine(" LacCellBvci lcb = new LacCellBvci();ok");
            }
            GC.Collect();
            using (FlowControlOneBvc fcob = new FlowControlOneBvc())
            {
                fcob.CreateCollection();
                Console.WriteLine(" FlowControlOneBvc fcob = new FlowControlOneBvc();ok");
            }
            GC.Collect();
            using (FlowControlMapBvc fcmb = new FlowControlMapBvc())
            {
                fcmb.CreateCollection();
                Console.WriteLine(" FlowControlMapBvc fcmb = new FlowControlMapBvc();ok");
            }
            GC.Collect();
            using (FlowControlOneMs fcom = new FlowControlOneMs())
            {
                fcom.CreateCollection();
                Console.WriteLine("   FlowControlOneMs fcom = new FlowControlOneMs();;ok");
            }
            GC.Collect();
            using (FlowControlMapMs fcmm = new FlowControlMapMs())
            {
                fcmm.CreateCollection();
                Console.WriteLine(" FlowControlMapMs fcmm = new FlowControlMapMs();;ok");
            }
            GC.Collect();
            using (FlowControlMessageRate fcmr = new FlowControlMessageRate())
            {
                fcmr.CreateCollection();
                Console.WriteLine("  FlowControlMessageRate fcmr = new FlowControlMessageRate();;ok");
            }
            GC.Collect();
            using (FlowControlBeforeMessage fcbm = new FlowControlBeforeMessage())
            {
                fcbm.CreateCollection();
                Console.WriteLine("FlowControlBeforeMessage fcbm = new FlowControlBeforeMessage();;ok");
            }
            GC.Collect();
            using (FlowControlBeforeMessageMap fcbmm = new FlowControlBeforeMessageMap())
            {
                fcbmm.CreateCollection();
                Console.WriteLine("  FlowControlBeforeMessageMap fcbmm = new FlowControlBeforeMessageMap();ok");
            }
            GC.Collect();

            Console.ReadKey();
        }
    }
}
