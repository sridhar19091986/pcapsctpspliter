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

            ExecMultiInterface();

            //ExecFlowControl();

            Console.WriteLine("complete.");
            Console.ReadKey();
        }

        public static void ExecMultiInterface()
        {
            GiGETRateMap giget = new GiGETRateMap();
            giget.CreateCollection();
            Console.WriteLine("GiGETRateMap giget = new GiGETRateMap();;ok");
        }

        public static void ExecFlowControl()
        {
            Console.WriteLine("Flow Control Statics Write");

            LacCellBvci lcb = new LacCellBvci();
            lcb.CreatCollection();
            Console.WriteLine(" LacCellBvci lcb = new LacCellBvci();ok");

            FlowControlOneBvc fcob = new FlowControlOneBvc();
            fcob.CreateCollection();
            Console.WriteLine(" FlowControlOneBvc fcob = new FlowControlOneBvc();ok");

            FlowControlMapBvc fcmb = new FlowControlMapBvc();
            fcmb.CreateCollection();
            Console.WriteLine(" FlowControlMapBvc fcmb = new FlowControlMapBvc();ok");

            FlowControlOneMs fcom = new FlowControlOneMs();
            fcom.CreateCollection();
            Console.WriteLine("   FlowControlOneMs fcom = new FlowControlOneMs();;ok");

            FlowControlMapMs fcmm = new FlowControlMapMs();
            fcmm.CreateCollection();
            Console.WriteLine(" FlowControlMapMs fcmm = new FlowControlMapMs();;ok");

            FlowControlMessageRate fcmr = new FlowControlMessageRate();
            fcmr.CreateCollection();
            Console.WriteLine("  FlowControlMessageRate fcmr = new FlowControlMessageRate();;ok");

            FlowControlBeforeMessage fcbm = new FlowControlBeforeMessage();
            fcbm.CreateCollection();
            Console.WriteLine("FlowControlBeforeMessage fcbm = new FlowControlBeforeMessage();;ok");

            FlowControlBeforeMessageMap fcbmm = new FlowControlBeforeMessageMap();
            fcbmm.CreateCollection();
            Console.WriteLine("  FlowControlBeforeMessageMap fcbmm = new FlowControlBeforeMessageMap();ok");


            Console.ReadKey();
        }
    }
}
