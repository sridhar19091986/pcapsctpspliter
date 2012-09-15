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
    public class TcpPortSessionETLDocument
    {
        #region 给sqlserver虚构一个主键，good,ef5 code first,/2012.8.29 ,mongo主键，sql主键，外键，primarykey ,foreignkey,_id
        [Key]
        //[DatabaseGenerated(DatabaseGenerationOption.None)]
        public long trsdID { get; set; }
        public long _id;
        public long tpsdID { get; set; }
        #endregion

        #region 维度
        public string bvci_from_lac_cell { get; set; }
        public int bvci_from_lac_cell_cnt { get; set; }

        public string session_id { get; set; }
        public string imsi { get; set; }
        //[Key,ForeignKey("TcpRetransStaticsDocuments"), Column(Order = 0)]
        //[ForeignKey("TcpRetransStaticsDocuments")]
        public string lac_cell { get; set; }
        public string direction { get; set; }
        public string msg_distinct_aggre { get; set; }

        public string http_method { get; set; }
        public string absolute_uri { get; set; }//生成uri的绝对路径
        public string user_agent { get; set; }//提取用户的代理

        public string ip_bsc_aggre { get; set; }
        public string ip2_sp_aggre { get; set; }
        public string ip2ttl_sp_aggre { get; set; }

        public string tcp_port_aggre { get; set; }

        public string ip_flags_mf { get; set; }
        public string sndcp_m { get; set; }
        public string ip2_flags_mf { get; set; }
        public string tcp_need_segment { get; set; }

        //注入apn、imei维度等
        #endregion

        #region 度量

        public long? ip2ip1_header { get; set; }
        public int sndcp_m_count { get; set; }
        public int? sndcp_m_total { get; set; }
        public decimal? seq_total_aggre { get; set; } //每次的nxt-seq之和，计算的值是ip2包之和，未计算sndcp包。
        public decimal? seqtotal_sndcp_aggre { get; set; }


        public decimal? seq_total_reduce { get; set; }//真实的总长度。最大nxt减去seq。未包含重传。
        //public long? ip2_total_aggre { get; set; }
        //public decimal? seqreduce_ip2total_aggre { get; set; }

        //流量计算
        //public decimal? seqtotal_sndcp_aggre { get; set; }
        public decimal? ip_total_aggre { get; set; }
        //public decimal? seq_total_aggre { get; set; }
        //public decimal? seq_total_reduce { get; set; }
        //public decimal? seqreduce_ip2total_aggre { get; set; }
        //数量计算
        public int seq_total_count { get; set; }
        public int seq_distinct_count { get; set; }
        public int seq_repeat_cnt { get; set; }
        //丢包和重传计算
        public decimal? seq_total_lost { get; set; }
        public decimal? seq_total_repeat { get; set; }
        //时间计算
        public double duration { get; set; }
        #endregion

        //public long? ip2ip1_header { get; set; }
        //public int sndcp_m_count { get; set; }
        //public int? sndcp_m_total { get; set; }
        public string bvci_bsc { get; set; }
        public string sndcp_nsapi { get; set; }
        public string llcgprs_sapi { get; set; }
        public string lac { get; set; }
        public int lac_cell_cnt { get; set; }
        public int bvci_cell_error_cnt { get; set; }
        public string lac_cell_from_bvci { get; set; }
        public int lac_cell_from_bvci_cnt { get; set; }
        public int bvci_bsc_cnt { get; set; }
        public int multibvci_cnt { get; set; }
 

        public int MultiCellPerBvci { get; set; }
        public int SgsnLostBscIp { get; set; }

        public string cell_seq_aggre { get; set; }
        public string bvci_seq_aggre { get; set; }
        //public TcpPortSessionDocument vTcpPortSessionDocument;
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

            TcpPortSession tts = new TcpPortSession();
            foreach (var p in tts.mongo_TcpPortSession.QueryMongo())
            {
                TcpPortSessionETLDocument trsd = new TcpPortSessionETLDocument();

                trsd.trsdID = p._id;
                trsd.tpsdID = p._id;
                trsd._id = p._id;

                trsd.session_id = p.session_id;

                trsd.cell_seq_aggre = p.cell_seq_aggre;
                trsd.bvci_seq_aggre = p.bvci_seq_aggre;

                trsd.MultiCellPerBvci = p.MultiCellPerBvci;
                trsd.SgsnLostBscIp = p.SgsnLostBscIp;

                //mulit-bvci-percell的问题
                trsd.lac_cell = p.lac_ci;
                trsd.lac_cell_cnt = GetCellCount(p.lac_ci);
                trsd.bvci_bsc = p.bsc_bvci;
                trsd.bvci_bsc_cnt = GetCellCount(p.bsc_bvci);//
                trsd.bvci_cell_error_cnt = trsd.bvci_bsc_cnt - trsd.lac_cell_cnt;

                trsd.bvci_from_lac_cell = SplitLacCellStr(p.lac_ci, cellslook);
                trsd.bvci_from_lac_cell_cnt = SplitLacCellCnt(p.lac_ci, cellslook);
                trsd.lac_cell_from_bvci = p.lac_cell_from_bvci;
                trsd.lac_cell_from_bvci_cnt = GetCellCount(trsd.lac_cell_from_bvci);//
                trsd.multibvci_cnt = trsd.lac_cell_from_bvci_cnt - trsd.bvci_bsc_cnt;

                trsd.lac = p.lac;
                trsd.sndcp_nsapi = p.sndcp_nsapi;
                trsd.llcgprs_sapi = p.llcgprs_sapi;


                trsd.imsi = p.imsi;
                trsd.msg_distinct_aggre = p.msg_distinct_aggre;
                trsd.direction = p.direction;
                trsd.http_method = p.http_method == null ? tcp_data : p.http_method;
                trsd.user_agent = p.user_agent == null ? fillnull : p.user_agent;
                trsd.absolute_uri = p.absolute_uri == null ? fillnull : p.absolute_uri;

                trsd.ip_bsc_aggre = p.direction == directiondown ? p.ip_dst_aggre : p.ip_src_aggre;
                trsd.ip2_sp_aggre = p.direction == directiondown ? p.ip2_src_aggre : p.ip2_dst_aggre;
                trsd.ip2ttl_sp_aggre = p.direction == directiondown ? p.ip2_ttl_aggre : fillnull;
                trsd.tcp_port_aggre = p.direction == directiondown ? p.src_port_aggre : p.dst_port_aggre;

                trsd.ip_flags_mf = p.ip_flags_mf;
                trsd.sndcp_m = p.sndcp_m;
                trsd.ip2_flags_mf = p.ip2_flags_mf;
                trsd.tcp_need_segment = p.tcp_need_segment;

                trsd.ip_total_aggre = p.ip_total_aggre;

                trsd.ip2ip1_header = p.ip2ip1_header;
                trsd.sndcp_m_count = p.sndcp_m_count;
                trsd.sndcp_m_total = p.sndcp_m_total;
                trsd.seq_total_aggre = p.seq_total_aggre;
                trsd.seqtotal_sndcp_aggre = p.seqtotal_sndcp_aggre;

                trsd.seq_total_reduce = p.seq_total_reduce;
                //trsd.ip2_total_aggre = p.ip2_total_aggre;
                //trsd.seqreduce_ip2total_aggre = p.seqreduce_ip2total_aggre;

                trsd.seq_total_count = p.seq_total_count;
                trsd.seq_distinct_count = p.seq_distinct_count;
                trsd.seq_repeat_cnt = p.seq_repeat_cnt;

                trsd.seq_total_lost = p.seq_total_lost > 0 ? p.seq_total_lost : 0;
                trsd.seq_total_repeat = p.seq_total_lost < 0 ? -1 * p.seq_total_lost : 0;

                trsd.duration = p.duration;

                mongo_TcpPortSessionETL.MongoCol.Insert(trsd);
            }

            Console.WriteLine("TcpPortSessionETLDocument->mongo->ok");
        }
    }
}

