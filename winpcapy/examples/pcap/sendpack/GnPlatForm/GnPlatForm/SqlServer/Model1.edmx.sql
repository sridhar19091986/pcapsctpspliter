
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 04/23/2012 10:03:04
-- Generated from EDMX file: G:\wysf\GnPlatForm\GnPlatForm\SqlServer\Model1.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [GuangZhou_Gn];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[mpos_gn_common]', 'U') IS NOT NULL
    DROP TABLE [dbo].[mpos_gn_common];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'mpos_gn_common'
CREATE TABLE [dbo].[mpos_gn_common] (
    [gn_id] int IDENTITY(1,1) NOT NULL,
    [Online_ID] bigint  NULL,
    [Session_ID] bigint  NULL,
    [Start_Date_Time] datetime  NULL,
    [End_Date_Time] datetime  NULL,
    [SGSN_IP] bigint  NULL,
    [SGSN] nvarchar(30)  NULL,
    [GGSN_IP] bigint  NULL,
    [GGSN] nvarchar(30)  NULL,
    [LAC] int  NULL,
    [CI] int  NULL,
    [SAC] int  NULL,
    [IMSI] nvarchar(17)  NULL,
    [IMEI] nvarchar(17)  NULL,
    [Prefix_IMEI] nvarchar(8)  NULL,
    [MSISDN] bigint  NULL,
    [APN] nvarchar(30)  NULL,
    [RAT_TYPE] int  NULL,
    [my_RAT_TYPE] nvarchar(128)  NULL,
    [PROTOCOL] int  NULL,
    [my_Protocol] nvarchar(128)  NULL,
    [Event_Type] int  NULL,
    [my_Event_Type] nvarchar(128)  NULL,
    [IP_LEN_UL] int  NULL,
    [IP_LEN_DL] int  NULL,
    [Count_Packet_UL] int  NULL,
    [Count_Packet_DL] int  NULL,
    [Duration] decimal(20,0)  NULL,
    [SOURCE_IP] bigint  NULL,
    [my_SOURCE_IP] nvarchar(128)  NULL,
    [DEST_IP] bigint  NULL,
    [my_DEST_IP] nvarchar(128)  NULL,
    [SOURCE_PORT] int  NULL,
    [DEST_PORT] int  NULL,
    [URI] nvarchar(1024)  NULL,
    [my_URI_Len] int  NULL,
    [my_URI_IMEI] nvarchar(128)  NULL,
    [my_URI_UA] nvarchar(128)  NULL,
    [my_URI_UA_MS_Type] nvarchar(128)  NULL,
    [my_URI_UA_Weibo_Ver] nvarchar(128)  NULL,
    [my_URI_UA_OS] nvarchar(128)  NULL,
    [URI_Main] nvarchar(128)  NULL,
    [my_URI_Main] nvarchar(128)  NULL,
    [my_URI_Main_header] nvarchar(128)  NULL,
    [Content_Type] nvarchar(30)  NULL,
    [my_Content_Type] nvarchar(30)  NULL,
    [User_Agent_Main] nvarchar(30)  NULL,
    [Service_TYPE] int  NULL,
    [my_Service_TYPE] nvarchar(128)  NULL,
    [Sub_Service_Type] int  NULL,
    [my_Sub_Service_Type] nvarchar(128)  NULL,
    [IsReassemble] bit  NULL,
    [Delete_PDP] bit  NULL,
    [Delete_PDP_Direction] bit  NULL,
    [Abnormal_reason] int  NULL,
    [my_Abnormal_reason] nvarchar(128)  NULL,
    [Repeat_Count] int  NULL,
    [Resp] bit  NULL,
    [Resp_DelayFirst] int  NULL,
    [Resp_cause] int  NULL,
    [abort] int  NULL,
    [Disconnect] bit  NULL,
    [Result] bit  NULL,
    [Result_DelayFirst] int  NULL,
    [MMS_request] bit  NULL,
    [MMS_resp_status] int  NULL,
    [From] nvarchar(30)  NULL,
    [To] nvarchar(30)  NULL,
    [Subject] nvarchar(40)  NULL,
    [Delivery_Report] bit  NULL,
    [Query_Name] nvarchar(128)  NULL,
    [Query_Type] nvarchar(128)  NULL,
    [DNS_TTL] int  NULL,
    [SP_Address] bigint  NULL,
    [Identifier] int  NULL,
    [Is_UserAbnormal] int  NULL,
    [ErrorPacketLost] int  NULL,
    [ErrorPacketWired] int  NULL,
    [ErrorPacketWireless] int  NULL,
    [Ack] int  NULL,
    [IpLenDlAck] int  NULL,
    [IpLenUpAck] int  NULL,
    [SynDirection] int  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [gn_id] in table 'mpos_gn_common'
ALTER TABLE [dbo].[mpos_gn_common]
ADD CONSTRAINT [PK_mpos_gn_common]
    PRIMARY KEY CLUSTERED ([gn_id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------