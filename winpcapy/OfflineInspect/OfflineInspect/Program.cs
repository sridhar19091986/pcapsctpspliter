/*
 * 生成相应的数据结构
 * 
 * 直接插入到mongo或者sqlserver
 * 
 * 2012.7.31
 * 
 * 
 * 
 * */



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OfflineInspect.FollowControl;
using OfflineInspect.MultiInterface;

namespace OfflineInspect
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("begin batch offline inspect items!");

            ExecMultiInterface();

            //ExecFlowControl();

            Console.WriteLine("complete.");

            Console.ReadKey();
        }

        public static void ExecMultiInterface()
        {
            using (GnGETRateMap giget = new GnGETRateMap())
            {
                giget.CreateCollection();
                Console.WriteLine("GiGETRateMap giget = new GiGETRateMap();;ok");
            }
            GC.Collect();//手工销毁
            using (GiGETRateMap giget = new GiGETRateMap())
            {
                giget.CreateCollection();
                Console.WriteLine("GiGETRateMap giget = new GiGETRateMap();;ok");
            }
            GC.Collect();//手工销毁
            using (GwGETRateMap giget = new GwGETRateMap())
            {
                giget.CreateCollection();
                Console.WriteLine("GiGETRateMap giget = new GiGETRateMap();;ok");
            }
            GC.Collect();//手工销毁
        }

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
