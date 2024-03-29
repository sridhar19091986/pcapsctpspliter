﻿using System;
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
        public static string[] LacCellBvciStaging = new String[] { "LacCellBvciStaging", db, remote };
        public static string[] TcpPortSessionStaging = new String[] { "TcpPortSessionStaging", db, remote, "0", "5", "5000" };
        public static string[] TcpPortSessionFlushLLStaging = new String[] { "TcpPortSessionFlushLLStaging", db, remote, "0", "5", "5000" };
        public static string[] LlcTlliSessionStaging = new String[] { "LlcTlliSessionStaging", db, remote, "0", "2", "5000" };
        public static string[] LacCellBvciETL = new string[] { "LacCellBvciETL", db, remote };
        public static string[] TcpPortSessionETL = new String[] { "TcpPortSessionETL", db, remote };

    }
}
