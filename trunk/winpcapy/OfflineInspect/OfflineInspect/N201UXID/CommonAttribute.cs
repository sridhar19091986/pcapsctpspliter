using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfflineInspect.N201UXID
{
    public class CommonAttribute:CommonDataLocation
    {
        //protected static string remote = "mongodb://192.168.4.33/?safe=true";
        private static string db = "N201UXID";
        public static string[] N201UXIDRate = new String[] { "N201UXIDRate", db, remote };
        public static string[] N201UXIDBeforeMessage = new String[] { "N201UXIDBeforeMessage", db, remote };
        public static string[] TlliLLCSession = new String[] { "TlliLLCSession", db, remote, "3", "5000" };
        public static string[] TcpRetransStatics = new String[] { "TcpRetransStatics", db, remote };

    }
}
