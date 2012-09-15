/*
 * 生成相应的数据结构
 * 
 * 直接插入到mongo或者sqlserver
 * 
 * 2012.7.31
 * 
 * 
net start mssqlserverolapservice

net start msdtsserver100

net start mssqlserver

net start reportserver

net start mssqllfdlauncher
 * 
 * */



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OfflineInspect.FlowControl;
using OfflineInspect.MultiInterface;
using OfflineInspect.ReTransmission;
using OfflineInspect.ReTransmission.Context;

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

            //N201UXID.N201UDataFactory.BathMakeN201UData();

            TcpDataFactory.BathMakeTcpData();

            Console.WriteLine("complete.");

            Console.ReadKey();
        }
    }
}
