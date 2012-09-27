/*
 * 
 * 
 * 
 * 
1.SGSN发给BSC，每个LLC的生存时间是多少？
PDU Lifetime=5762/100s=5.762秒. 
 
2. 当编码是MSC-9的时候，1个PDCH，1小时的承载数据量是多少？
59.2kbps*3600s=213120kbit=213.120Mbit。
 * 
 * 
 * 
 * 
 * 
 * */


/*
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
 * 
 * ETL，维度处理和事实表处理，在维度不大的情况下，可以考虑重复使用。
 * 
 * ETL的目标是给olap和数据挖掘使用。
 * 
 * 
 * staging的目标是，主要是使用聚合的方法，尽量的压缩数据，但是不能metadata元数据的特性？
 * 
 * 
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
 * cube structure
 * dimension usage
 * calculation
 * kpi
 * action
 * partition
 * perspecitve
 * translation
 * browser
 * 
 * aggregation
 * 
 * fact
 * 
 * 
AssociationModel
ClusteringModel
DecisionTreesModel
LinearRegressionModel
LogisticRegressionModel
NaiveBayesModel
NeuralNetworkModel
SequenceClusteringModel
TimeSeriesModel
 * 
 * */


/*
 * 
 * 关于维度的命名规则，能够一眼识别其计算方法，2012.9.19
 * 
 * 维度的组合和处理需要很高的专业的基础，选择最合适的维度去发现问题？
 * 
 * ETL是一个反复抽取数据的过程，反复清洗数据的过程，因此，针对不同的项目需要不同的清洗方法，即ETL也会不同。
 * 
 * 相对于元数据metadata，ETL是一个数据挖掘的重要过程。数据挖掘关键技术在于维度的处理。
 * 
 * etl:1.mtrix->sqlserver,2.sqlserver->mongo,3.mongo->mongo,4.mongo->sqlserver,5.olap
 * 
 * 另外的一种ETL方法：
 * 如何建立小区维度级别的重传率，丢包率，当前是可以实现小区累加级别的，aggregate级别？，重现钻去回到原始表的方法？
 * 多列合并的ETL转换，主键用packetnum，外键用？以TCP会话过程中小区切换作为最小事实粒度，不是以每个消息做最小的事实粒度？
 * 
 * 
 * 小区切换作为一个事实表，发起呼叫也是一个事实表，2者存在关联，小区切换，1个呼叫存在（0～n）小区切换(FK,callid)。
 * 如何进行数据库建模成了数据挖掘的关键。
 * 
 * 以小区切换作为ETL的最小粒度？
 * 
 * 
 * 维表存在比较多的重复值，需要进一步的ETL。
 * 
 * 小区或者BVCI粒度的ETL需要重新开启设计。
 * 
 * 
 * 表结构还需要进行调整，不然，不能放入数据挖掘模型
 * 
 * */


namespace OfflineInspect.ReTransmission.MapReduce
{
    //这部分给sql保存
    public class DimensionIp
    {
        [Key]
        [DatabaseGenerated(DatabaseGenerationOption.None)]
        public long IpID { get; set; }
        public string ip_flags_mf { get; set; }
        public string bsc_ip_distinct { get; set; }
        public bool sgsn_lost_bsc_ip { get; set; }
    }
    public class DimensionUdp
    {
        [Key]
        [DatabaseGenerated(DatabaseGenerationOption.None)]
        public long UdpID { get; set; }
    }
    public class DimensionNs
    {
        [Key]
        [DatabaseGenerated(DatabaseGenerationOption.None)]
        public long NsID { get; set; }

        public string bvci_distinct { get; set; }//记录不同的小区
        public int bvci_distinct_cnt { get; set; }
        public string bvci_seq { get; set; }//记录切换序列
        public int bvci_seq_cnt { get; set; }
        public int bvci_pp_cnt { get; set; }//记录乒乓切换

        public bool multi_cell_per_bvci { get; set; }
        public int multibvci_cnt { get; set; }

        public string lac_cell_from_bvci { get; set; }
        public int lac_cell_from_bvci_cnt { get; set; }
        public int bvci_cell_error_cnt { get; set; }

        public string bvci_from_lac_cell { get; set; }
        public int bvci_from_lac_cell_cnt { get; set; }

        public string nsip_control_bits_c { get; set; }
        public string nsip_control_bits_r { get; set; }
    }

    public class DimensionBssgp
    {
        [Key]
        [DatabaseGenerated(DatabaseGenerationOption.None)]
        public long BssgpID { get; set; }
        public string imsi { get; set; }
        public string direction { get; set; }
        public string lac_distinct { get; set; }
        public string lac_cell_distinct { get; set; }
        public int lac_cell_distinct_cnt { get; set; }
        public string cell_seq { get; set; }
        //BSSGP.LLC-DISCARDED
        //radio-status
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
        public string ip2host_distinct { get; set; }
        public string ip2ttl_distinct { get; set; }
        public string ip2_flags_mf { get; set; }
        public int ip2_id_lost_seq { get; set; }
        public int lost_ipid_seq { get; set; }
    }

    public class DimensionTcp
    {
        [Key]
        [DatabaseGenerated(DatabaseGenerationOption.None)]
        public long TcpID { get; set; }
        public string tcp_need_segment { get; set; }
        public string tcp_port_distinct { get; set; }
        public int tcp_nxtseq_lost_seq { get; set; }
        public int lost_tcp_seq { get; set; }
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

    //其他的维度和计算
    public class DimensionMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGenerationOption.None)]
        public long MessageID { get; set; }
        public string session_id { get; set; }
        public long? ip2ip1_header { get; set; }
        public string msg_distinct_aggre { get; set; }

    }

    //计算模型,datamining定制
    public class CalculationItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGenerationOption.None)]
        public long CalculationItemID { get; set; }

        public decimal? IP实传字节数 { get; set; }
        public decimal? TCP实传字节数 { get; set; }
        public decimal? TCP完整字节数 { get; set; }

        public double? TCP传输速率 { get; set; }
        public double? TCP会话时长 { get; set; }

        public int? TCP传输次数 { get; set; }
        public int? TCP重发次数 { get; set; }
        public double? TCP重传次数占比 { get; set; }

        public decimal? TCP重传字节数 { get; set; }
        public double? TCP重传字节占比 { get; set; }

        public decimal? TCP丢包字节数 { get; set; }
        public double? TCP丢包字节占比 { get; set; }
    }

    //事实表
    public class FactTcpPortSession
    {
        [Key]
        public long FactID { get; set; }

        public long IpID { get; set; }
        public long UdpID { get; set; }
        public long NsID { get; set; }
        public long BssgpID { get; set; }
        public long LlcSndcpID { get; set; }
        public long Ip2ID { get; set; }
        public long TcpID { get; set; }
        public long HttpID { get; set; }
        public long MessageID { get; set; }
        public long CalculationItemID { get; set; }

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
        public DimensionIp DimIp = new DimensionIp();
        public DimensionUdp DimUdp = new DimensionUdp();
        public DimensionNs DimNs = new DimensionNs();
        public DimensionBssgp DimBssgp = new DimensionBssgp();
        public DimensionLlcSndcp DimLlcSndcp = new DimensionLlcSndcp();
        public DimensionIp2 DimIp2 = new DimensionIp2();
        public DimensionTcp DimTcp = new DimensionTcp();
        public DimensionHttp DimHttp = new DimensionHttp();
        public DimensionMessage DimMessage = new DimensionMessage();
        public CalculationItem CalItem = new CalculationItem();
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

        //返回2个值的方法
        private string SplitLacCellStr(string laccell, ILookup<string, LacCellBvciETLDocument> lookcell, out int bvci_count)
        {
            bvci_count = 0;
            string bvci = string.Empty;
            var cells = laccell.Split(',');
            foreach (var ce in cells.Distinct())
            {
                bvci = bvci + lookcell[ce].Select(e => e.bvci_aggre).FirstOrDefault() + ",";
                bvci_count = bvci_count + lookcell[ce].Sum(e => e.bvci_cnt);
            }
            return bvci;
        }

        private double EdgeMax = 4 * 59.2;

        public void CreatCollection()
        {
            LacCellBvciETL lcbs = new LacCellBvciETL();
            var cellslook = lcbs.mongo_LacCellBvciETL.QueryMongo().ToLookup(e => e.lac_ci);
            int bvci_count = 0;

            TcpPortSessionStaging tts = new TcpPortSessionStaging();
            foreach (var p in tts.mongo_TcpPortSessionStaging.QueryMongo())
            {
                TcpPortSessionETLDocument trsd = new TcpPortSessionETLDocument();

                trsd._id = p._id;

                trsd.DimIp.IpID = p._id;
                trsd.DimUdp.UdpID = p._id;
                trsd.DimNs.NsID = p._id;
                trsd.DimBssgp.BssgpID = p._id;
                trsd.DimLlcSndcp.LlcSndcpID = p._id;
                trsd.DimIp2.Ip2ID = p._id;
                trsd.DimTcp.TcpID = p._id;
                trsd.DimHttp.HttpID = p._id;
                trsd.DimMessage.MessageID = p._id;
                trsd.CalItem.CalculationItemID = p._id;

                trsd.FactTcp.FactID = p._id;
                trsd.FactTcp.IpID = p._id;
                trsd.FactTcp.UdpID = p._id;
                trsd.FactTcp.NsID = p._id;
                trsd.FactTcp.BssgpID = p._id;
                trsd.FactTcp.LlcSndcpID = p._id;
                trsd.FactTcp.Ip2ID = p._id;
                trsd.FactTcp.TcpID = p._id;
                trsd.FactTcp.HttpID = p._id;
                trsd.FactTcp.MessageID = p._id;
                trsd.FactTcp.CalculationItemID = p._id;

                trsd.DimIp.bsc_ip_distinct = p.direction == directiondown ? p.ip_dst_aggre : p.ip_src_aggre;
                trsd.DimIp.ip_flags_mf = p.ip_flags_mf;
                trsd.DimIp.sgsn_lost_bsc_ip = p.sgsn_lost_bsc_ip;
        
                trsd.DimNs.bvci_seq = p.bvci_seq_aggre;
                trsd.DimNs.bvci_distinct = p.bsc_bvci;
                trsd.DimNs.bvci_distinct_cnt = p.bsc_bvci.GetDistinctCount();//进行不重复计数
                trsd.DimNs.lac_cell_from_bvci = p.lac_cell_from_bvci;
                trsd.DimNs.lac_cell_from_bvci_cnt = p.lac_cell_from_bvci.GetDistinctCount();//进行不重复计数
                trsd.DimNs.multi_cell_per_bvci = p.multi_cell_per_bvci;
                trsd.DimNs.multibvci_cnt = trsd.DimNs.lac_cell_from_bvci_cnt - trsd.DimNs.bvci_distinct_cnt;
                trsd.DimNs.bvci_from_lac_cell = SplitLacCellStr(p.lac_cell, cellslook, out bvci_count);
                trsd.DimNs.bvci_from_lac_cell_cnt = bvci_count;
                trsd.DimNs.bvci_seq_cnt = p.bvci_seq_aggre.GetSequenceCount();//进行序列计数
                trsd.DimNs.bvci_pp_cnt = trsd.DimNs.bvci_seq_cnt - trsd.DimNs.bvci_distinct_cnt;
                trsd.DimNs.nsip_control_bits_c = p.nsip_control_bits_c;//R-bit和C-bit不响应的问题，bsc_ip更新问题
                trsd.DimNs.nsip_control_bits_r = p.nsip_control_bits_r;
 
                trsd.DimBssgp.lac_distinct = p.lac;
                trsd.DimBssgp.imsi = p.imsi;
                trsd.DimBssgp.direction = p.direction;
                trsd.DimBssgp.lac_cell_distinct = p.lac_cell; //mulit-bvci-percell的问题
                trsd.DimBssgp.lac_cell_distinct_cnt = p.lac_cell.GetDistinctCount();//进行不重复计数
                trsd.DimBssgp.cell_seq = p.cell_seq_aggre;

                trsd.DimNs.bvci_cell_error_cnt = trsd.DimNs.bvci_distinct_cnt - trsd.DimBssgp.lac_cell_distinct_cnt;

                trsd.DimLlcSndcp.sndcp_m = p.sndcp_m;
                trsd.DimLlcSndcp.sndcp_nsapi = p.sndcp_nsapi;
                trsd.DimLlcSndcp.llcgprs_sapi = p.llcgprs_sapi;

                trsd.DimIp2.ip2_flags_mf = p.ip2_flags_mf;
                trsd.DimIp2.ip2host_distinct = p.direction == directiondown ? p.ip2_src_aggre : p.ip2_dst_aggre;
                trsd.DimIp2.ip2ttl_distinct = p.direction == directiondown ? p.ip2_ttl_aggre : fillnull;

                trsd.DimTcp.tcp_port_distinct = p.direction == directiondown ? p.src_port_aggre : p.dst_port_aggre;
                trsd.DimTcp.tcp_need_segment = p.tcp_need_segment;

                #region TCP传输乱序的计算,ip2_id和tcp_nxtseq结合运算       
                trsd.DimIp2.ip2_id_lost_seq = p.ip2_id_aggre.LostSequenctCount();
                trsd.DimTcp.tcp_nxtseq_lost_seq = p.tcp_nxt_aggre.LostSequenctCount();
                trsd.DimIp2.lost_ipid_seq = trsd.DimIp2.ip2_id_lost_seq > 0 ? 1 : 0;
                trsd.DimTcp.lost_tcp_seq = trsd.DimTcp.tcp_nxtseq_lost_seq > 0 ? 1 : 0; 
                #endregion

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

                trsd.CalItem.IP实传字节数 = trsd.FactTcp.ip_total_aggre;
                trsd.CalItem.TCP实传字节数 = trsd.FactTcp.seqtotal_sndcp_aggre;
                trsd.CalItem.TCP完整字节数 = trsd.FactTcp.seq_total_reduce;
                trsd.CalItem.TCP传输速率 = trsd.FactTcp.duration != 0 ? (double)trsd.FactTcp.seqtotal_sndcp_aggre / trsd.FactTcp.duration : EdgeMax;
                trsd.CalItem.TCP会话时长 = trsd.FactTcp.duration;
                trsd.CalItem.TCP传输次数 = trsd.FactTcp.seq_total_count;
                trsd.CalItem.TCP重发次数 = trsd.FactTcp.seq_repeat_cnt;
                trsd.CalItem.TCP重传次数占比 = 1.0 * p.seq_repeat_cnt / p.seq_total_count;
                trsd.CalItem.TCP重传字节数 = trsd.FactTcp.seq_total_repeat;
                trsd.CalItem.TCP重传字节占比 = trsd.FactTcp.seqtotal_sndcp_aggre != 0 ? (double)trsd.FactTcp.seq_total_repeat / (double)trsd.FactTcp.seqtotal_sndcp_aggre : 1.0;
                trsd.CalItem.TCP丢包字节数 = trsd.FactTcp.seq_total_lost;
                trsd.CalItem.TCP丢包字节占比 = trsd.FactTcp.seq_total_reduce != 0 ? (double)trsd.FactTcp.seq_total_lost / (double)trsd.FactTcp.seq_total_reduce : 1.0;

                mongo_TcpPortSessionETL.MongoCol.Insert(trsd);
            }

            Console.WriteLine("TcpPortSessionETLDocument->mongo->ok");
        }
    }
}

