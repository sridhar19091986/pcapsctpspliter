using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfflineInspect.MultiInterface
{
    public class CommonAttribute
    {
        private static string remote = "mongodb://192.168.4.209/?safe=true";
        private static string db = "MultiInterface";
        public static string[] GiGETRateMap = new String[] { "GiGETRateMap", db, remote, "Gi", "TCP", "GRE" };
        public static string[] GnGETRateMap = new String[] { "GnGETRateMap", db, remote, "Gn", "TCP", "GTP" };
        public static string[] GwGETRateMap = new String[] { "GwGETRateMap", db, remote,"Gw", "TCP", "GRE" };

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
    }
}
