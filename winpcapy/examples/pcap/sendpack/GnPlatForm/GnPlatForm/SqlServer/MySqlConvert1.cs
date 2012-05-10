/*
 * 
 * 
 * 
 * */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GnPlatForm.SqlServer
{
    public partial class MySqlConvert0
    {
        public static string getEventType(object value)
        {
            string eventtype = null;
            int len = Event_Type.GetUpperBound(0) + 1;
            for (int i = 0; i < len; i++)
            {
                if (Event_Type[i, 1].ToString() == value.ToString())
                    eventtype = Event_Type[i, 0].ToString();
            }
            return eventtype;
        }

        public static int getEventTypeValue(string eventname)
        {
            int eventvalue = 0;
            int len = Event_Type.GetUpperBound(0) + 1;
            for (int i = 0; i < len; i++)
            {
                if (Event_Type[i, 0].ToString() == eventname)
                    eventvalue = (int)Event_Type[i, 1];
            }
            return eventvalue;
        }

        private static object[,] Event_Type = new object[,]{
        {"CREATE_PDP",1},
        {"DELETE_PDP",2},
        {"UPDATE_PDP",3},
        {"CONNECT",8},
        {"SYN",9},
        {"GET",4},
        {"POST",5},
        {"MMS_MO",6},
        {"MMS_MT",7},
        {"TCP_DATA",10},
        {"UDP_DATA",11},
        {"DNS",12},
        {"Ping",13},
        {"MMS_IND",14},
       };
    }
}
