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
//using OfflineInspect.ReTransmission.Table;

namespace OfflineInspect.ReTransmission
{
    public class CommonAttribute : CommonDataLocation
    {
        public static string[] LacCellBvci = new String[] { "LacCellBvci", db, remote };
        public static string[] TcpPortSession = new String[] { "TcpPortSession", db, remote, "0", "5", "50000" };
        public static string[] LlcTlliSession = new String[] { "LlcTlliSession", db, remote, "0", "2", "5000" };
        public static string[] TcpPortSessionETL = new String[] { "TcpPortSessionETL", db, remote };
        public static string[] LacCellBvciETL = new string[] { "LacCellBvciETL", db, remote };
    }
}
