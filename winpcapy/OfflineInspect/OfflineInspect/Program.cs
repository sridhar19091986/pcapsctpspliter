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
using OfflineInspect.FlowControl;
using OfflineInspect.MultiInterface;
using OfflineInspect.ReTransmission;

namespace OfflineInspect
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("begin batch offline inspect items!");

            //OfflineInspect.MultiInterface.CommonAttribute.ExecMultiInterface();

            //OfflineInspect.FlowControl.CommonAttribute.ExecFlowControl();

            //OfflineInspect.ReTransmission.CommonAttribute.ExecReTransmission();

            //OfflineInspect.ReTransmission.CommonAttribute.ExecMongoExportSql();

            TcpDataFactory.BathMakeTcpData();

            Console.WriteLine("complete.");

            Console.ReadKey();
        }
    }
}
