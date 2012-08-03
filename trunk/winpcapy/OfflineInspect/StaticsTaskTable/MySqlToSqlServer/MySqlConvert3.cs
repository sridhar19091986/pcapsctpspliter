using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GnPlatForm.SqlServer
{
    public partial class MySqlConvert0
    {


        public static string getProtocol(object value)
        {
            string proto = null;
            int len = Protocol.GetUpperBound(0) + 1;
            for (int i = 0; i < len; i++)
            {
                if (Protocol[i, 1].ToString() == value.ToString())
                    proto = Protocol[i, 0].ToString();
            }
            return proto;
        }

        public static string getRateType(object value)
        {
            string rattype = null;
            int len = Rat_Type.GetUpperBound(0) + 1;
            for (int i = 0; i < len; i++)
            {
                if (Rat_Type[i, 1].ToString() == value.ToString())
                    rattype = Rat_Type[i, 0].ToString();
            }
            return rattype;
        }
        private static object[,] Protocol = new object[,]{
        {"WAP1.0",1},
        {"HTTP",2}, 
        };

        private static object[,] Rat_Type = new object[,]{                 
        {"3G",1},
        {"2G",2},     
        };

    }
}
