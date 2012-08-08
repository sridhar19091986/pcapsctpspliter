﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfflineInspect.ReTransmission
{
    public class CommonAttribute
    {
        private static string remote = "mongodb://localhost/?safe=true";
        private static string db = "ReTransmission";
        public static string[] TlliTcpSession = new String[] { "GiGETRateMap", db, remote, "3", "5000" };
        public static string[] GnGETRateMap = new String[] { "GnGETRateMap", db, remote, "Gn", "TCP", "GTP" };
        public static string[] GwGETRateMap = new String[] { "GwGETRateMap", db, remote, "Gw", "TCP", "GRE" };

        public static void ExecReTransmission()
        {
            using (TlliTcpSession tts = new TlliTcpSession())
            {
                tts.CreateCollection();
                Console.WriteLine("GiGETRateMap giget = new GiGETRateMap();;ok");
            }
            GC.Collect();//手工销毁    
        }
    }
}