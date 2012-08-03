/*
 * 
 * 数据库数据测试
 * 
 * */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
//using GnPlatForm.MySql;
using GnPlatForm.SqlServer;

namespace GnPlatForm.BusinessLogic
{
    public static class ElementDumper
    {
        public static void dumpEvnentType()
        {

            GuangZhou_GnEntities mmt = new GuangZhou_GnEntities();
            var mmts = mmt.mpos_gn_common.Take(1000);


            var db = from p in mmts
                     where p.Event_Type == 4
                     select new
                     {
                         //EventTypes = p.Event_Type,
                         //p.Query_Name,
                         //p.URI,
                         p.URI_Main,
                         p.Content_Type,
                         p.URI,
                         //p.Duration
                     };

            var dblist = db.ToList();


            Console.WriteLine(dblist.Count());

            var dblists = from p in dblist
                          select new
                          {
                              //EventType = Event_Type_MySql.getEventType(p.EventTypes),
                              //p.Query_Name,

                              //header=MySqlConvert0.getUriMainHeader(p.URI_Main),

                              //host = MySqlConvert0.getUriMain(p.URI_Main),

                              //p.Content_Type,

                              //p.URI,

                              abc = MySqlConvert0.getURI_MS_Type(MySqlConvert0.getURI_UA(p.URI)),

                              abc1 = MySqlConvert0.getURI_Weibo_Ver(MySqlConvert0.getURI_UA(p.URI)),

                              abc2 = MySqlConvert0.getURI_OS(MySqlConvert0.getURI_UA(p.URI)),

                              control_n = MySqlConvert0.getURI_Control_N(p.URI),


                              //imei=MySqlConvert0.getURI_IMEI(p.URI),
                              //ua=MySqlConvert0.getURI_UA(p.URI),

                              //content=MySqlConvert0.getContentTypeHeader(p.Content_Type),

                              //p.URI_Main,
                              //p.URI,
                              //p.Duration

                          };

            foreach (var i in dblists)
                MicroObjectDumper.Write(i);
        }

    }
}
