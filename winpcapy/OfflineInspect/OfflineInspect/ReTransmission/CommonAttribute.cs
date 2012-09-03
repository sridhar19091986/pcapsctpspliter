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
using OfflineInspect.ReTransmission.Table;

namespace OfflineInspect.ReTransmission
{
    public class CommonAttribute : CommonDataLocation
    {
        public static string[] LacCellBvci = new String[] { "LacCellBvci", db, remote };
        public static string[] TlliTcpSession = new String[] { "TlliTcpSession", db, remote, "0", "5", "5000" };
        public static string[] TlliLLCSession = new String[] { "TlliLLCSession", db, remote, "0", "1", "5000" };
        public static string[] TcpRetransStatics = new String[] { "TcpRetransStatics", db, remote };
    }
}
