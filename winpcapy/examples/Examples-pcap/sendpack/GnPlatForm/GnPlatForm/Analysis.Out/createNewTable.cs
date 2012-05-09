using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.Odbc;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace GnPlatForm.AnalysisOut
{
    public class CreateNewTable
    {

        public static void KillSleepingConnections(int iMinSecondsToExpire,ref int conn_num)
        {
            //conn_num=2;
            string strSQL = "show processlist";
            System.Collections.ArrayList m_ProcessesToKill = new ArrayList();

            MySqlConnection myConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["mysqlconn"].ToString());
            MySqlCommand myCmd = new MySqlCommand(strSQL, myConn);
            MySqlDataReader MyReader = null;

            try
            {
                myConn.Open();

                // Get a list of processes to kill. 
                MyReader = myCmd.ExecuteReader();
           
                while (MyReader.Read())
                {
                    conn_num +=1;
                    // Find all processes sleeping with a timeout value higher than our threshold. 
                    int iPID = Convert.ToInt32(MyReader["Id"].ToString());
                    string strState = MyReader["Command"].ToString();
                    int iTime = Convert.ToInt32(MyReader["Time"].ToString());

                    if (strState == "Sleep" && iTime >= iMinSecondsToExpire && iPID > 0)
                    {
                        m_ProcessesToKill.Add(iPID);
                    }
                }

                MyReader.Close();
                foreach (int aPID in m_ProcessesToKill)
                {
                    strSQL = "kill " + aPID;
                    myCmd.CommandText = strSQL;
                    myCmd.ExecuteNonQuery();
                }
            }
            catch (Exception excep)
            {
                Console.WriteLine(excep.ToString());
                //conn_num = 9;
            }
            finally
            {
                if (MyReader != null && !MyReader.IsClosed)
                {
                    MyReader.Close();
                }

                if (myConn != null && myConn.State == ConnectionState.Open)
                {
                    myConn.Close();
                }
            }
        }


        public static string CreateNewServerTable = @"
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[mpos_gn_common]') AND type in (N'U'))
DROP TABLE [dbo].[mpos_gn_common]
;
CREATE TABLE [dbo].[mpos_gn_common](
	[gn_id] [int] IDENTITY(1,1) NOT NULL,
	[Online_ID] [bigint] NULL,
	[Session_ID] [bigint] NULL,
	[Start_Date_Time] [datetime] NULL,
	[End_Date_Time] [datetime] NULL,
	[SGSN_IP] [bigint] NULL,
	[SGSN] [nvarchar](30) NULL,
	[GGSN_IP] [bigint] NULL,
	[GGSN] [nvarchar](30) NULL,
	[LAC] [int] NULL,
	[CI] [int] NULL,
	[SAC] [int] NULL,
	[IMSI] [nvarchar](17) NULL,
	[IMEI] [nvarchar](17) NULL,
	[Prefix_IMEI] [nvarchar](8) NULL,
	[MSISDN] [bigint] NULL,
	[APN] [nvarchar](30) NULL,
	[RAT_TYPE] [int] NULL,
	[my_RAT_TYPE] [nvarchar](128) NULL,
	[PROTOCOL] [int] NULL,
	[my_Protocol] [nvarchar](128) NULL,
	[Event_Type] [int] NULL,
	[my_Event_Type] [nvarchar](128) NULL,
	[IP_LEN_UL] [int] NULL,
	[IP_LEN_DL] [int] NULL,
	[Count_Packet_UL] [int] NULL,
	[Count_Packet_DL] [int] NULL,
	[Duration] [decimal](20, 0) NULL,
	[SOURCE_IP] [bigint] NULL,
	[my_SOURCE_IP] [nvarchar](128) NULL,
	[DEST_IP] [bigint] NULL,
	[my_DEST_IP] [nvarchar](128) NULL,
	[SOURCE_PORT] [int] NULL,
	[DEST_PORT] [int] NULL,
	[URI] [nvarchar](1024) NULL,
[my_URI_Len] [int] NULL,
	[my_URI_IMEI] [nvarchar](128) NULL,

	[my_URI_UA] [nvarchar](128) NULL,
[my_URI_UA_MS_Type] [nvarchar](128) NULL,
	[my_URI_UA_Weibo_Ver] [nvarchar](128) NULL,
	[my_URI_UA_OS] [nvarchar](128) NULL,

	[URI_Main] [nvarchar](128) NULL,
	[my_URI_Main] [nvarchar](128) NULL,
	[my_URI_Main_header] [nvarchar](128) NULL,
	[Content_Type] [nvarchar](30) NULL,
	[my_Content_Type] [nvarchar](30) NULL,
	[User_Agent_Main] [nvarchar](30) NULL,
	[Service_TYPE] [int] NULL,
	[my_Service_TYPE] [nvarchar](128) NULL,
	[Sub_Service_Type] [int] NULL,
	[my_Sub_Service_Type] [nvarchar](128) NULL,
	[IsReassemble] [bit] NULL,
	[Delete_PDP] [bit] NULL,
	[Delete_PDP_Direction] [bit] NULL,
	[Abnormal_reason] [int] NULL,
	[my_Abnormal_reason] [nvarchar](128) NULL,
	[Repeat_Count] [int] NULL,
	[Resp] [bit] NULL,
	[Resp_DelayFirst] [int] NULL,
	[Resp_cause] [int] NULL,
	[abort] [int] NULL,
	[Disconnect] [bit] NULL,
	[Result] [bit] NULL,
	[Result_DelayFirst] [int] NULL,
	[MMS_request] [bit] NULL,
	[MMS_resp_status] [int] NULL,
	[From] [nvarchar](30) NULL,
	[To] [nvarchar](30) NULL,
	[Subject] [nvarchar](40) NULL,
	[Delivery_Report] [bit] NULL,
	[Query_Name] [nvarchar](128) NULL,
	[Query_Type] [nvarchar](128) NULL,
	[DNS_TTL] [int] NULL,
	[SP_Address] [bigint] NULL,
	[Identifier] [int] NULL,
	[Is_UserAbnormal] [int] NULL,
	[ErrorPacketLost] [int] NULL,
	[ErrorPacketWired] [int] NULL,
	[ErrorPacketWireless] [int] NULL,
	[Ack] [int] NULL,
	[IpLenDlAck] [int] NULL,
	[IpLenUpAck] [int] NULL,
	[SynDirection] [int] NULL,
 CONSTRAINT [PK_mpos_gn_common] PRIMARY KEY CLUSTERED 
(
	[gn_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

";
    }
}
