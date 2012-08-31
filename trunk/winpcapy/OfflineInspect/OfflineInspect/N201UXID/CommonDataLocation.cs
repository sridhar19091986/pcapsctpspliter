using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfflineInspect.N201UXID
{
    public class CommonDataLocation
    {
        protected static string sqlconn = "Data Source=localhost;Initial Catalog=N201UDbContext;Integrated Security=True;";

        protected static string remote = "mongodb://localhost/?safe=true";
    }
}
