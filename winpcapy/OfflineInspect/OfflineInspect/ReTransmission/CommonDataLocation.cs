﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfflineInspect.ReTransmission
{
    public class CommonDataLocation
    {
        protected static string db = "ReTransmission_TEST";
        //protected static string remote = "mongodb://localhost/?safe=true";
        //protected static string sqlconn = "Data Source=localhost;Initial Catalog=TcpDbContext;Integrated Security=True;";

        protected static string remote = "mongodb://192.168.4.249/?safe=true";


        protected static string sqlconn = "Data Source=192.168.4.249;Initial Catalog=TcpDbContextab_TEST;User ID=sa;Password=hantele123;MultipleActiveResultSets=True";

    }
}
