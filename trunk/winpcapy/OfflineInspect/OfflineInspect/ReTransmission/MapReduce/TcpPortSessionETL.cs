﻿/*
 * 
 * 输出目标是给SSAS进行数据挖掘使用。
 * 
 * 
 * 合理增加维度进行计算
 * 
 * 
 * 1.mongo->gridview->exportexcel->etl(SSIS)->ssas
 * 
 * 
 * 2012.8.23
 * 
 * 
 * 2.可以考虑直接使用ssis script compoment,制作data flow的自定义组件，mongo->sqlserver.
 * 
 * 
 * 2012.8.27
 * 
 * 
 * 3.EF code first也比较容易操作，mongo-sqlserver
 * 
 * 
 * 2012.8.28
 * 
 * 
 *纯ip的业务分析
 *FROM [TcpDbContext].[dbo].[TcpRetransStaticsDocuments] 
 *where [absolute_uri] like '%218.27.135.175%' or [ip2_sp_aggre] like '%218.27.135.175%' 
 * 
SELECT * FROM gn_common_201204181300 WHERE
gn_common_201204181300.SP_Address  like '%218.27.135.175%' OR
gn_common_201204181300.URI =  '%218.27.135.175%' OR
gn_common_201204181300.URI_Main =  '%218.27.135.175%'
limit 100
 * 
 * 需要针对用户和业务进行分析？
 * */

/*
 * 
 * ETL，正对staging做进一步的处理
 * 
 * 本模块需要按照维度表dimension和事实表fact进行分割，同时需要考虑mongodb的非常方便与数据挖掘的存储特性。
 * #region 给sqlserver虚构一个主键，good,ef5 code first,/2012.8.29 ,mongo主键，sql主键，外键，primarykey ,foreignkey,_id
 * 
 * 2012.9.17
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OfflineInspect.CommonTools;
using OfflineInspect.Mongo;
using System.ComponentModel.DataAnnotations;


/*
 * 
 * key
 * dimension
 * measurement
 * calculation
 * cube
 * partion
 * aggregation
 * 
 * 
 * */
namespace OfflineInspect.ReTransmission.MapReduce
{
    //这部分给sql保存
    public class DimensionIpUdpNs
    {
        [Key]
        [DatabaseGenerated(DatabaseGenerationOption.None)]
        public long IpUdpNsID { get; set; }

        public string ip_flags_mf { get; set; }
        public string ip_bsc_aggre { get; set; }

        public string bvci_bsc { get; set; }
        public int bvci_bsc_cnt { get; set; }
        public string lac_cell_from_bvci { get; set; }
        public int lac_cell_from_bvci_cnt { get; set; }

        public int bvci_cell_error_cnt { get; set; }
        public int multibvci_cnt { get; set; }

        public int multi_cell_per_bvci { get; set; }
        public int sgsn_lost_bsc_ip { get; set; }

        public string cell_seq_aggre { get; set; }
        public string bvci_seq_aggre { get; set; }
    }

    public class DimensionBssgp
    {
        [Key]
        [DatabaseGenerated(DatabaseGenerationOption.None)]
        public long BssgpID { get; set; }

        public string imsi { get; set; }

        public string direction { get; set; }

        public string lac { get; set; }
        public string lac_cell { get; set; }
        public int lac_cell_cnt { get; set; }

        public string bvci_from_lac_cell { get; set; }
        public int bvci_from_lac_cell_cnt { get; set; }
    }

    public class DimensionLlcSndcp
    {
        [Key]
        [DatabaseGenerated(DatabaseGenerationOption.None)]
        public long LlcSndcpID { get; set; }

        public string llcgprs_sapi { get; set; }

        public string sndcp_m { get; set; }
        public string sndcp_nsapi { get; set; }

    }

    public class DimensionIp2
    {
        [Key]
        [DatabaseGenerated(DatabaseGenerationOption.None)]
        public long Ip2ID { get; set; }
        public string ip2_sp_aggre { get; set; }
        public string ip2ttl_sp_aggre { get; set; }
        public string ip2_flags_mf { get; set; }
    }
    public class DimensionTcp
    {
        [Key]
        [DatabaseGenerated(DatabaseGenerationOption.None)]
        public long TcpID { get; set; }
        public string tcp_need_segment { get; set; }
        public string tcp_port_aggre { get; set; }
    }

    public class DimensionHttp
    {
        [Key]
        [DatabaseGenerated(DatabaseGenerationOption.None)]
        public long HttpID { get; set; }
        public string http_method { get; set; }
        public string absolute_uri { get; set; }//生成uri的绝对路径
        public string user_agent { get; set; }//提取用户的代理
    }

    public class DimensionMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGenerationOption.None)]
        public long MeasureID { get; set; }
        public string session_id { get; set; }
        public long? ip2ip1_header { get; set; }
        public string msg_distinct_aggre { get; set; }
    }

    //其他的维度和计算
    public class FactTcpPortSession
    {

        [Key]
        public long FactID { get; set; }

        public long IpUdpNsID { get; set; }
        public long BssgpID { get; set; }
        public long LlcSndcpID { get; set; }
        public long Ip2ID { get; set; }
        public long TcpID { get; set; }
        public long HttpID { get; set; }
        public long MeasureID { get; set; }

        public int sndcp_m_count { get; set; }
        public int? sndcp_m_total { get; set; }
        public decimal? seq_total_aggre { get; set; } //每次的nxt-seq之和，计算的值是ip2包之和，未计算sndcp包。
        public decimal? seqtotal_sndcp_aggre { get; set; }
        public decimal? seq_total_reduce { get; set; }//真实的总长度。最大nxt减去seq。未包含重传。
        public decimal? ip_total_aggre { get; set; }//流量计算
        public int seq_total_count { get; set; }   //数量计算
        public int seq_distinct_count { get; set; }
        public int seq_repeat_cnt { get; set; }
        public decimal? seq_total_lost { get; set; }   //丢包和重传计算
        public decimal? seq_total_repeat { get; set; }
        public double duration { get; set; }  //时间计算

    }

    //这部分给mongo保存
    public class TcpPortSessionETLDocument
    {
        public long _id;
        public DimensionIpUdpNs DimIpUdpNs = new DimensionIpUdpNs();
        public DimensionBssgp DimBssgp = new DimensionBssgp();
        public DimensionLlcSndcp DimLlcSndcp = new DimensionLlcSndcp();
        public DimensionIp2 DimIp2 = new DimensionIp2();
        public DimensionTcp DimTcp = new DimensionTcp();
        public DimensionHttp DimHttp = new DimensionHttp();
        public DimensionMessage DimMessage = new DimensionMessage();
        public FactTcpPortSession FactTcp = new FactTcpPortSession();
    }

    public class TcpPortSessionETL : CommonToolx, IDisposable
    {
        private string mongo_collection = CommonAttribute.TcpPortSessionETL[0];
        private string mongo_db = CommonAttribute.TcpPortSessionETL[1];
        private string mongo_conn = CommonAttribute.TcpPortSessionETL[2];

        private string fillnull = "Inull";
        private string directiondown = "Down";
        private string tcp_data = "TCP_DATA";

        public MongoCrud<TcpPortSessionETLDocument> mongo_TcpPortSessionETL;

        public TcpPortSessionETL()
        {
            mongo_TcpPortSessionETL = new MongoCrud<TcpPortSessionETLDocument>(mongo_conn, mongo_db, mongo_collection);
        }
        #region Implementing IDisposable and the Dispose Pattern Properly
        private bool disposed = false; // to detect redundant calls
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~TcpPortSessionETL()
        {
            Dispose(false);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing) { }
                disposed = true;
            }
        }
        #endregion


        private string SplitLacCellStr(string laccell, ILookup<string, LacCellBvciETLDocument> lookcell)
        {
            string bvci = string.Empty;
            var cells = laccell.Split(',');
            foreach (var ce in cells.Distinct())
                bvci = bvci + lookcell[ce].Select(e => e.bvci_aggre).FirstOrDefault() + ",";
            return bvci;
        }

        private int SplitLacCellCnt(string laccell, ILookup<string, LacCellBvciETLDocument> lookcell)
        {
            int bvci = 0;
            var cells = laccell.Split(',');
            foreach (var ce in cells.Distinct())
                bvci = bvci + lookcell[ce].Sum(e => e.bvci_cnt);
            return bvci;
        }


        private int GetCellCount(string laccell)
        {
            if (laccell == null) return 0;
            var cells = laccell.Split(',');
            return cells.Distinct().Count();
        }

        public void CreatCollection()
        {
            LacCellBvciETL lcbs = new LacCellBvciETL();
            var cellslook = lcbs.mongo_LacCellBvciETL.QueryMongo().ToLookup(e => e.lac_ci);

            TcpPortSessionStaging tts = new TcpPortSessionStaging();
            foreach (var p in tts.mongo_TcpPortSessionStaging.QueryMongo())
            {
                TcpPortSessionETLDocument trsd = new TcpPortSessionETLDocument();

                trsd._id = p._id;

                trsd.DimIpUdpNs.IpUdpNsID = p._id;
                trsd.DimBssgp.BssgpID = p._id;
                trsd.DimLlcSndcp.LlcSndcpID = p._id;
                trsd.DimIp2.Ip2ID = p._id;
                trsd.DimTcp.TcpID = p._id;
                trsd.DimHttp.HttpID = p._id;
                trsd.DimMessage.MeasureID = p._id;

                trsd.FactTcp.FactID = p._id;

                trsd.FactTcp.IpUdpNsID = p._id;
                trsd.FactTcp.BssgpID = p._id;
                trsd.FactTcp.LlcSndcpID = p._id;
                trsd.FactTcp.Ip2ID = p._id;
                trsd.FactTcp.TcpID = p._id;
                trsd.FactTcp.HttpID = p._id;
                trsd.FactTcp.MeasureID = p._id;

                //trsd.session_id = p.session_id;
                trsd.DimIpUdpNs.ip_bsc_aggre = p.direction == directiondown ? p.ip_dst_aggre : p.ip_src_aggre;
                trsd.DimIpUdpNs.cell_seq_aggre = p.cell_seq_aggre;
                trsd.DimIpUdpNs.bvci_seq_aggre = p.bvci_seq_aggre;
                trsd.DimIpUdpNs.multi_cell_per_bvci = p.multi_cell_per_bvci;
                trsd.DimIpUdpNs.sgsn_lost_bsc_ip = p.sgsn_lost_bsc_ip;
                trsd.DimIpUdpNs.ip_flags_mf = p.ip_flags_mf;
                trsd.DimIpUdpNs.bvci_bsc = p.bsc_bvci;
                trsd.DimIpUdpNs.bvci_bsc_cnt = GetCellCount(p.bsc_bvci);//
                trsd.DimIpUdpNs.lac_cell_from_bvci = p.lac_cell_from_bvci;
                trsd.DimIpUdpNs.lac_cell_from_bvci_cnt = GetCellCount(p.lac_cell_from_bvci);//
                trsd.DimIpUdpNs.multibvci_cnt = trsd.DimIpUdpNs.lac_cell_from_bvci_cnt - trsd.DimIpUdpNs.bvci_bsc_cnt;

                //mulit-bvci-percell的问题
                trsd.DimBssgp.lac = p.lac;      
                trsd.DimBssgp.imsi = p.imsi;
                trsd.DimBssgp.direction = p.direction;
                trsd.DimBssgp.lac_cell = p.lac_cell;
                trsd.DimBssgp.lac_cell_cnt = GetCellCount(p.lac_cell);
                trsd.DimBssgp.bvci_from_lac_cell = SplitLacCellStr(p.lac_cell, cellslook);
                trsd.DimBssgp.bvci_from_lac_cell_cnt = SplitLacCellCnt(p.lac_cell, cellslook);
                trsd.DimIpUdpNs.bvci_cell_error_cnt = trsd.DimIpUdpNs.bvci_bsc_cnt - trsd.DimBssgp.lac_cell_cnt;

                trsd.DimLlcSndcp.sndcp_m = p.sndcp_m;
                trsd.DimLlcSndcp.sndcp_nsapi = p.sndcp_nsapi;
                trsd.DimLlcSndcp.llcgprs_sapi = p.llcgprs_sapi;

                trsd.DimIp2.ip2_flags_mf = p.ip2_flags_mf;
                trsd.DimIp2.ip2_sp_aggre = p.direction == directiondown ? p.ip2_src_aggre : p.ip2_dst_aggre;
                trsd.DimIp2.ip2ttl_sp_aggre = p.direction == directiondown ? p.ip2_ttl_aggre : fillnull;

                trsd.DimTcp.tcp_port_aggre = p.direction == directiondown ? p.src_port_aggre : p.dst_port_aggre;
                trsd.DimTcp.tcp_need_segment = p.tcp_need_segment;

                trsd.DimHttp.http_method = p.http_method == null ? tcp_data : p.http_method;
                trsd.DimHttp.user_agent = p.user_agent == null ? fillnull : p.user_agent;
                trsd.DimHttp.absolute_uri = p.absolute_uri == null ? fillnull : p.absolute_uri;

                trsd.DimMessage.session_id = p.session_id;
                trsd.DimMessage.msg_distinct_aggre = p.msg_distinct_aggre;
                trsd.DimMessage.ip2ip1_header = p.ip2ip1_header;

                trsd.FactTcp.ip_total_aggre = p.ip_total_aggre;
                trsd.FactTcp.sndcp_m_count = p.sndcp_m_count;
                trsd.FactTcp.sndcp_m_total = p.sndcp_m_total;
                trsd.FactTcp.seq_total_aggre = p.seq_total_aggre;
                trsd.FactTcp.seqtotal_sndcp_aggre = p.seqtotal_sndcp_aggre;
                trsd.FactTcp.seq_total_reduce = p.seq_total_reduce;
                trsd.FactTcp.seq_total_count = p.seq_total_count;
                trsd.FactTcp.seq_distinct_count = p.seq_distinct_count;
                trsd.FactTcp.seq_repeat_cnt = p.seq_repeat_cnt;
                trsd.FactTcp.seq_total_lost = p.seq_total_lost > 0 ? p.seq_total_lost : 0;
                trsd.FactTcp.seq_total_repeat = p.seq_total_lost < 0 ? -1 * p.seq_total_lost : 0;
                trsd.FactTcp.duration = p.duration;

                mongo_TcpPortSessionETL.MongoCol.Insert(trsd);
            }

            Console.WriteLine("TcpPortSessionETLDocument->mongo->ok");
        }
    }
}
