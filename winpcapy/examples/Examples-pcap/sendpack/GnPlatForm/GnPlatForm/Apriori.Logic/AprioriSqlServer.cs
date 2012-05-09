/*
 * 
 *注意用法
 *
 *http://msdn.microsoft.com/en-us/library/bb534291.aspx
 * 
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using GnPlatForm.SqlServer;

namespace GnPlatForm.BusinessLogic
{
    public class AprioriSqlServer
    {
        public ILookup<string, string> m_dicTransactions;
        public ILookup<string, string> m_dicTransactions_ex;
        private GuangZhou_GnEntities servercontext;
        public HashSet<string> hs;

        public AprioriSqlServer()
        {
            string serverconnstring = ConfigurationManager.ConnectionStrings["GuangZhou_GnEntities"].ToString();
            servercontext = new GuangZhou_GnEntities(serverconnstring);
            //get_IMSI_UriMain_iphone();
            //get_ab_reason();

            get_UserAgent_IMEI();
        }

        //统计用户喜好登录的网页
        private void get_IMSI_UriMain()
        {
            m_dicTransactions = servercontext.mpos_gn_common
                .Where(e => e.IMSI != null)
                .Where(e => e.my_URI_Main != null)
                .ToLookup(e => e.IMSI, e => e.my_URI_Main);
        }

        //统计网络无响应的情况
        private void get_ab_reason()
        {
            m_dicTransactions = servercontext.mpos_gn_common
                .Where(e => e.my_Abnormal_reason != null)
                .Where(e => e.Event_Type == 4 || e.Event_Type == 5)
                .ToLookup(e => e.my_Event_Type,
                e => e.my_Abnormal_reason + e.CI.ToString() + "-" + e.APN + "-" + e.my_URI_Main + "-" + e.Prefix_IMEI + "-" + e.my_URI_Main_header);
        }

        //统计iphone终端的imei
        private void get_UserAgent_IMEI()
        {
            m_dicTransactions = servercontext.mpos_gn_common
                .Where(e => e.Prefix_IMEI != null)
                .Where(e => e.User_Agent_Main != null)
                .Where(e => e.User_Agent_Main.IndexOf("iphone") != -1 | e.User_Agent_Main.IndexOf("iPhone") != -1)
                .ToLookup(e => e.User_Agent_Main.Trim(' ').Trim(), e => e.Prefix_IMEI.Trim(' ').Trim());

            m_dicTransactions_ex = servercontext.mpos_gn_common
    .Where(e => e.Prefix_IMEI != null)
                //.Where(e => e.User_Agent_Main != null)
    .Where(e => e.my_URI_UA_MS_Type != null | e.User_Agent_Main != null)
                //.Where(e => e.my_URI_UA_MS_Type.IndexOf("iphone") != -1 | e.my_URI_UA_MS_Type.IndexOf("iPhone") != -1)
    .ToLookup(e => e.Prefix_IMEI.Trim(), e => (e.my_URI_UA_MS_Type + e.User_Agent_Main).Trim(',').Trim());

        }

        //提取iphone终端喜好的网页，指标，等等呢
        private void get_IMSI_UriMain_iphone()
        {
            get_UserAgent_IMEI();
            hs = new HashSet<string>();
            foreach (var h in m_dicTransactions)
            {
                foreach (var v in h)
                    if (v != null)
                        if (!hs.Contains(v))
                        {
                            hs.Add(v); Console.WriteLine(v);
                        }
            }
            m_dicTransactions = servercontext.mpos_gn_common
                .Where(e => hs.Contains(e.Prefix_IMEI))
                .Where(e => e.IMSI != null)
                .Where(e => e.my_URI_Main != null)
                .ToLookup(e => e.IMSI, e => e.my_URI_Main);
        }
    }
}
