/*
use  [GuangZhou_Gb]
go
alter TABLE [Gb_TCP_ReTransmission]   alter  COLUMN [PacketNum] int  not null
go
alter TABLE [Gb_TCP_ReTransmission]  alter  COLUMN [FileNum] int  not null
go
alter table [Gb_TCP_ReTransmission] add  PRIMARY KEY (PacketNum,FileNum);
 * 
 * 
 * 
CREATE UNIQUE CLUSTERED INDEX [tcpport] ON [dbo].[Gb_TCP_ReTransmission] 
(
	[FileNum] ASC,
	[PacketNum] ASC,
	[BeginFileNum] ASC,
	[BeginFrameNum] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
 * */

/*
 * 输出目标是中间数据，也可以作为专业人士手动分析使用。
 * 
 * 
 * 
 * 2012.8.23
 * 
 * 
 * 
 * 
 * 
 * */

/*
 * 
 * 
 * 本模块不完成llc重传？
 * 
 * 
 * 本模块可以完成tcp重传和丢包计算。只计算业务包，不包含syn。
 * 
 * 
 * 2012.8.9
 * 
 * 
 * **/


/*
 * staging，阶段性的处理
 * 
 * 
 * 
 * 充分利用数据挖掘的原因，以事实表和维度表为中心做处理
 * 
 * 本模块实现事实表的阶段性处理，对事实表进行浓缩处理，
 * 
 * 1.把最原始的消息流按照callid汇总，即tlli,tcp会话汇总。以避免事实表的奇大无比。
 * 
 * 2.tcp会话，即callid是否进行小区级别的分割？
 * 
 * 3.是否需要分割tcp会话发生cell的维度渐变的处理
 * 
 * 2012.9.17
 * 
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using EntitySqlTable.SqlServer.Local.Gb_TCP_ReTransmission;
//using EntitySqlTable.SqlServer.ip33.tcp_data;
using EntitySqlTable.SqlServer.ip249.tcp_data;
using System.Data.Objects;
using OfflineInspect.Mongo;
using OfflineInspect.CommonTools;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace OfflineInspect.ReTransmission.MapReduce
{
    public class TcpPortSessionStagingDocument
    {
        #region 给sqlserver虚构一个主键，good,ef5 code first,/2012.8.29
        [Key]
        [DatabaseGenerated(DatabaseGenerationOption.None)]
        public long tpsdID { get; set; }
        #endregion

        public long _id;
        public string bsc_bvci { get; set; }
        public string lac_cell { get; set; }
        public string mscbsc_ip_aggre { get; set; }
        public int mscbsc_ip_count { get; set; }
        public string direction { get; set; }
        public double duration { get; set; }
        public string imsi { get; set; }
        [Column(TypeName = "ntext")]
        [MaxLength]
        public string tcp_seq_aggre { get; set; }//tcp聚合
        public string msg_distinct_aggre { get; set; }//消息聚合
        public decimal? seq_nxt_max { get; set; }
        //包总长度
        public long? ip2ip1_header { get; set; }
        public int sndcp_m_count { get; set; }
        public int? sndcp_m_total { get; set; }
        public decimal? seq_total_aggre { get; set; } //每次的nxt-seq之和，计算的值是ip2包之和，未计算sndcp包。
        public decimal? seqtotal_sndcp_aggre { get; set; }

        public decimal? seq_total_reduce { get; set; }//真实的总长度。最大nxt减去seq。未包含重传。
        //public long? ip2_total_aggre { get; set; }
        //public decimal? seqreduce_ip2total_aggre { get; set; }
        /*
         * seq_total_lost>0，则出现丢包，sndcp、ip分片等。
         * seq_total_lost<0，则出现重传等。
         * 
         * */
        public decimal? seq_total_lost { get; set; }//大于0，丢包流量占比，小于0，重传流量占比
        public string session_id { get; set; }
        public decimal seq_tcp_min { get; set; }
        //public double seq_total_aggre_rate;//速率计算
        public int seq_total_count { get; set; }
        public long? ip_total_aggre { get; set; }

        public int seq_repeat_cnt { get; set; }//重传数量占比
        public int seq_distinct_count { get; set; }
        public string ip_src_aggre { get; set; }
        public string ip2_src_aggre { get; set; }
        public string ip_dst_aggre { get; set; }
        public string ip2_dst_aggre { get; set; }
        public string ip_ttl_aggre { get; set; }
        public string ip2_ttl_aggre { get; set; }
        public string ip_flags_mf { get; set; }
        public string ip2_flags_mf { get; set; }
        public string tcp_flags_cwr { get; set; }
        public string tcp_win_size { get; set; }
        [Column(TypeName = "ntext")]
        [MaxLength]
        public string tcp_nxt_aggre { get; set; }
        public string dst_port_aggre { get; set; }
        public string src_port_aggre { get; set; }
        public string sndcp_m { get; set; }

        public string tcp_need_segment { get; set; }
        /*
         * 还需要增加一些维度，
         * 
         * 比如，uri,agent,response
         * 
         *   public string apn;//apn维度可以在之后生成
        public string qos;//qos维度可以在数据挖掘之后生生
         * 
         * */
        public string http_method { get; set; }
        public string absolute_uri { get; set; }//生成uri的绝对路径
        public string user_agent { get; set; }//提取用户的代理

        public string sndcp_nsapi { get; set; }
        public string llcgprs_sapi { get; set; }
        public string lac { get; set; }
        public string lac_cell_from_bvci { get; set; }

        public bool multi_cell_per_bvci { get; set; }
        public bool sgsn_lost_bsc_ip { get; set; }

        //切换序列
        public string cell_seq_aggre { get; set; }
        public string bvci_seq_aggre { get; set; }

        public string nsip_control_bits_c { get; set; }
        public string nsip_control_bits_r { get; set; }

    }

    public class TcpPortSessionStaging : CommonToolx, IDisposable
    {
        private string mongo_collection = CommonAttribute.TcpPortSessionStaging[0];
        private string mongo_db = CommonAttribute.TcpPortSessionStaging[1];
        private string mongo_conn = CommonAttribute.TcpPortSessionStaging[2];

        private int min_file_num = Int32.Parse(CommonAttribute.TcpPortSessionStaging[3]);
        private int max_file_num = Int32.Parse(CommonAttribute.TcpPortSessionStaging[4]);

        private int size = Int32.Parse(CommonAttribute.TcpPortSessionStaging[5]);

        public MongoCrud<TcpPortSessionStagingDocument> mongo_TcpPortSessionStaging;
        //private GuangZhou_Gb_TCP_ReTransmission gb;
        public TcpPortSessionStaging()
        {
            mongo_TcpPortSessionStaging = new MongoCrud<TcpPortSessionStagingDocument>(mongo_conn, mongo_db, mongo_collection);
            //gb = new GuangZhou_Gb_TCP_ReTransmission();

        }

        #region Implementing IDisposable and the Dispose Pattern Properly
        private bool disposed = false; // to detect redundant calls
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~TcpPortSessionStaging()
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

        //按照文件号进行分页
        private ILookup<int?, LacCellBvciStagingDocument> LookBvci;
        public void CreateCollection()
        {
            LacCellBvciStaging lcb = new LacCellBvciStaging();
            LookBvci = lcb.mongo_LacCellBvciStaging.QueryMongo()
                .Where(e => e.lac_cell != null)
                .ToLookup(e => e.bvci);
            for (int j = min_file_num; j < max_file_num; j++)
            {
                Console.WriteLine("file_num:{0}", j);
                CreateCollection(j);
            }
            Console.WriteLine("TcpPortSessionStagingDocument->mongo->ok");
        }
        /*
         * 这里max,count等会出现全部加载的情况，即table全部加入到内存
         * 
         * 1.entity和table的主键不匹配
         * 
         * 2.改用sql
         *
         * 3.改用上述方法后，max仍然需要把整个table加载到内存？
         * 
         * 4.如果兼任域服务器，一般为60-70%,实际上是sqlserver自身的问题，非ef5的问题
         * 
         * */

        private int? packet_cnt = null;
        //private string esql = @"SELECT max(it.BeginFrameNum) FROM Gb_TCP_ReTransmission as it where it.BeginFileNum=@v1";

        public void CreateCollection(int filenum)
        {
            using (foshan_tcp_dataEntities gb = new foshan_tcp_dataEntities())
            {
                //gb.CommandTimeout = 0;
                //SqlParameter v1 = new SqlParameter("v1", filenum);
                //packet_cnt = gb.ExecuteStoreQuery<int?>(esql, v1).FirstOrDefault();
                //数据库分页错误的问题？？？？？
                gb.CommandTimeout = 0;
                gb.Gb_TCP_ReTransmission.MergeOption = MergeOption.NoTracking;
                packet_cnt = gb.Gb_TCP_ReTransmission.Where(e => e.BeginFileNum == filenum).Max(e => e.BeginFrameNum);
            }

            if (packet_cnt == null) return;

            using (foshan_tcp_dataEntities gb = new foshan_tcp_dataEntities())
            {
                gb.CommandTimeout = 0;
                gb.Gb_TCP_ReTransmission.MergeOption = MergeOption.NoTracking;
                gb.ContextOptions.LazyLoadingEnabled = true;

                int step = (int)packet_cnt / size + 1;
                Console.WriteLine("packet_cnt...{0}", packet_cnt);
                string[] directions = new string[] { "Up", "Down" };
                IQueryable<Gb_TCP_ReTransmission> tcpsession = gb.Gb_TCP_ReTransmission.Where(e => e.BeginFileNum == filenum);
                //int packet_cnt = gb_tcp_retrans.Select(e => e.BeginFrameNum).Distinct().Count();
                CreateTable(directions, tcpsession, filenum, step);
            }
        }

        //提取下行方向BVCI对应的小区
        private string GetLacCellFromBvci(IEnumerable<Gb_TCP_ReTransmission> tcps, out bool multiCellPerBvci, out bool sgsnLostBscIp)
        {
            multiCellPerBvci = false;
            sgsnLostBscIp = false;
            List<string> cells = new List<string>();
            //string laccell = string.Empty;
            foreach (var tcp in tcps)
            {
                var laccell = LookBvci[tcp.nsip_bvci]
                .Where(e => (e.src == tcp.ip_src_host && e.dst == tcp.ip_dst_host) ||
                    (e.dst == tcp.ip_src_host && e.src == tcp.ip_dst_host))
                .Select(e => e.lac_cell).Distinct();

                if (laccell.Count() > 1) multiCellPerBvci = true;

                if (laccell.Count() == 0) sgsnLostBscIp = true;

                foreach (var ce in laccell)
                    cells.Add(ce);

            }
            return cells.IEnumDistinctStrComma();
        }


        /*
         * 
         * 这里做了了优化，ienumber是立即执行，iquery则是延时执行,2012.8.30
         * 
         * */
        //对每个文件号中的开始帧号进行分页

        /*
         * 存在单核工作负荷100%的问题
         * 
         * */

        public void CreateTable(string[] directions, IQueryable<Gb_TCP_ReTransmission> gb_tcp_retrans, int filenum, int step)
        {
            //执行帧号分页
            for (int i = 0; i < step; i++)
            //Parallel.For(0, step, (i) =>
            {
                IQueryable<Gb_TCP_ReTransmission> iq_tcp_session = gb_tcp_retrans
                    .Where(e => e.BeginFrameNum >= i * size && e.BeginFrameNum < (i + 1) * size);

                var tcp_sessions = iq_tcp_session.ToLookup(e => e.BeginFrameNum);

                Console.Write("step...{0},", i);

                foreach (var direction in directions)
                    //帧号分页中的每个tcp的会话过程
                    //foreach (var m in tcp_sessions)
                    Parallel.ForEach(tcp_sessions, (m) =>
                    {
                        #region 会话过滤，filter
                        string host = null;
                        string uri = null;
                        bool multicellperbvci;
                        bool sgsnlostbscip;
                        var gb_packet = m.Where(e => e.bssgp_direction == direction); //本次只计算下行速率
                        var pd_no_3tcp = gb_packet.Where(e => e.tcp_nxtseq != null);//不是3次握手的包
                        //http://stackoverflow.com/questions/3765038/is-there-an-equivalent-to-continue-in-a-parallel-foreach
                        if (pd_no_3tcp.Count() == 0) return ;//continue;
                        #endregion

                        TcpPortSessionStagingDocument tcps = new TcpPortSessionStagingDocument();

                        #region tcp会话的基础信息，callid/imsi/lac/cell/bvci/duration/

                        tcps._id = GenerateId();
                        tcps.tpsdID = tcps._id;

                        tcps.session_id = filenum.ToString() + "-" + m.Key.Value.ToString();
                        tcps.direction = direction;
                        tcps.imsi = m.Where(e => e.bssgp_imsi != null).Select(e => e.bssgp_imsi).FirstOrDefault();
                        host = m.Where(e => e.http_host != null).Select(e => e.http_host).FirstOrDefault();
                        uri = m.Where(e => e.http_request_uri != null).Select(e => e.http_request_uri).FirstOrDefault();
                        //合并uri算法
                        tcps.absolute_uri = uri;
                        if (host != null)
                        {
                            if (uri != null)
                            {
                                if (!uri.Contains(host))
                                    tcps.absolute_uri = host + uri;
                            }
                            else
                            {
                                tcps.absolute_uri = host;
                            }
                        }
                        tcps.user_agent = m.Where(e => e.http_user_agent != null).Select(e => e.http_user_agent).FirstOrDefault();
                        tcps.http_method = m.Where(e => e.http_request_method != null).Select(e => e.http_request_method).FirstOrDefault();
                        var src = m.Select(e => e.ip_src_host);
                        var dst = m.Select(e => e.ip_dst_host);
                        var bscip = src.Union(dst);
                        tcps.mscbsc_ip_aggre = string.Join(",", bscip.Distinct().OrderBy(e => e));
                        tcps.mscbsc_ip_count = bscip.Distinct().Count();
                        //还需要1个？留意ip的问题?
                        //????

                        tcps.lac_cell = m.Where(e => e.bssgp_lac != null).Count() == 0 ? "" : m.Where(e => e.bssgp_lac != null).Select(e => Convert.ToString(e.bssgp_lac) + "-" + Convert.ToString(e.bssgp_ci)).IEnumDistinctStrComma();
                        tcps.lac = m.Where(e => e.bssgp_lac != null).Select(e => e.bssgp_lac).IEnumDistinctStrComma();
                        //下行时延，包含3次握手，取这次会话的总长度吧。
                        TimeSpan? ts = m.Max(e => DateTime.Parse(e.GbOverLLC_time)) - pd_no_3tcp.Min(e => DateTime.Parse(e.GbOverLLC_time));
                        tcps.duration = ts.Value.TotalMilliseconds;
                        #endregion

                        tcps.bsc_bvci = m.Where(e => e.nsip_bvci != null).Select(e => Convert.ToString(e.nsip_bvci)).IEnumDistinctStrComma();
                        tcps.lac_cell_from_bvci = GetLacCellFromBvci(m, out multicellperbvci, out sgsnlostbscip);//??????pdp????

                        //小区切换序列？
                        tcps.cell_seq_aggre = m.Where(e => e.bssgp_ci != null).OrderBy(e => e.FileNum).ThenBy(e => e.PacketNum).Select(e => e.bssgp_ci).IEnumSequenceStrComma();
                        tcps.bvci_seq_aggre = m.Where(e => e.nsip_bvci != null).OrderBy(e => e.FileNum).ThenBy(e => e.PacketNum).Select(e => e.nsip_bvci).IEnumSequenceStrComma();

                        //计算2中问题，per bvci multi cell，
                        tcps.multi_cell_per_bvci = multicellperbvci;
                        tcps.sgsn_lost_bsc_ip = sgsnlostbscip;

                        tcps.nsip_control_bits_c = m.Select(e => e.nsip_control_bits_c).IEnumDistinctStrComma();
                        tcps.nsip_control_bits_r = m.Select(e => e.nsip_control_bits_r).IEnumDistinctStrComma();

                        #region seq算法，计算tcp重传和丢包
                        //序列号的计算
                        //正确计算是，每个包进行计算。
                        //tcps.ip_total_aggre = pd_no_3tcp.Sum(e => e.ip_len);
                        //sndcp分片的问题会影响丢包率的计算   
                        //tcps.ip2_total_aggre = pd_no_3tcp.Sum(e => e.ip2_len - 20 - e.tcp_hdr_len);
                        //tcps.seqreduce_ip2total_aggre = tcps.seq_total_reduce > tcps.ip2_total_aggre ? tcps.seq_total_reduce : tcps.ip2_total_aggre;

                        tcps.ip_total_aggre = gb_packet.Sum(e => e.ip_len);
                        tcps.sndcp_m_count = gb_packet.Where(e => e.ip2_len == null).Count();
                        tcps.seq_tcp_min = pd_no_3tcp.Min(e => Convert.ToInt64(e.tcp_seq));
                        tcps.seq_nxt_max = pd_no_3tcp.Max(e => Convert.ToInt64(e.tcp_nxtseq));
                        tcps.ip2ip1_header = pd_no_3tcp.Max(e => e.ip_len - (20 + e.tcp_hdr_len + (Convert.ToInt64(e.tcp_nxtseq) - Convert.ToInt64(e.tcp_seq))));
                        tcps.sndcp_m_total = gb_packet.Where(e => e.ip2_len == null).Sum(e => e.sndcp_len);
                        tcps.seq_total_aggre = pd_no_3tcp.Sum(e => Convert.ToInt64(e.tcp_nxtseq) - Convert.ToInt64(e.tcp_seq));
                        tcps.seqtotal_sndcp_aggre = tcps.seq_total_aggre + tcps.sndcp_m_total;

                        tcps.seq_total_reduce = tcps.seq_nxt_max - tcps.seq_tcp_min;

                        tcps.seq_total_lost = tcps.seq_total_reduce - tcps.seqtotal_sndcp_aggre;

                        //计算速率，取reduce吧
                        //tcps.seq_total_aggre_rate = (double)tcps.seq_total_reduce / tcps.duration
                        tcps.seq_total_count = pd_no_3tcp.Count();
                        tcps.seq_distinct_count = pd_no_3tcp.Select(e => e.tcp_seq).Distinct().Count();
                        tcps.seq_repeat_cnt = tcps.seq_total_count - tcps.seq_distinct_count;
                        tcps.tcp_seq_aggre = pd_no_3tcp.Select(e => e.tcp_seq).IEnumDistinctStrComma();
                        //第1层ip地址，第2层ip地址
                        tcps.ip_src_aggre = pd_no_3tcp.Select(e => e.ip_src_host).IEnumDistinctStrComma();
                        tcps.ip2_src_aggre = pd_no_3tcp.Select(e => e.ip2_src_host).IEnumDistinctStrComma();
                        tcps.ip_dst_aggre = pd_no_3tcp.Select(e => e.ip_dst_host).IEnumDistinctStrComma();
                        tcps.ip2_dst_aggre = pd_no_3tcp.Select(e => e.ip2_dst_host).IEnumDistinctStrComma();
                        //ttl
                        tcps.ip_ttl_aggre = pd_no_3tcp.Select(e => e.ip_ttl).IEnumDistinctStrComma();
                        tcps.ip2_ttl_aggre = pd_no_3tcp.Select(e => e.ip2_ttl).IEnumDistinctStrComma();
                        //分片
                        tcps.ip_flags_mf = pd_no_3tcp.Select(e => e.ip_flags_mf).IEnumDistinctStrComma();
                        tcps.ip2_flags_mf = pd_no_3tcp.Select(e => e.ip2_flags_mf).IEnumDistinctStrComma();
                        tcps.sndcp_m = pd_no_3tcp.Select(e => e.sndcp_m).IEnumDistinctStrComma();
                        tcps.tcp_need_segment = pd_no_3tcp.Select(e => e.tcp_need_segment).IEnumDistinctStrComma();
                        //windows_size,CWR
                        tcps.tcp_flags_cwr = pd_no_3tcp.Select(e => e.tcp_flags_cwr).IEnumDistinctStrComma();
                        tcps.tcp_win_size = pd_no_3tcp.Select(e => e.tcp_window_size).IEnumDistinctStrComma();
                        //端口
                        tcps.tcp_nxt_aggre = pd_no_3tcp.Select(e => e.tcp_nxtseq).OrderBy(e => e).Aggregate((a, b) => a + "," + b);
                        // + "-" + Convert.ToString(e.tcp_dstport),只取源端口吧。
                        tcps.src_port_aggre = pd_no_3tcp.Select(e => Convert.ToString(e.tcp_srcport)).IEnumDistinctStrComma();
                        tcps.dst_port_aggre = pd_no_3tcp.Select(e => Convert.ToString(e.tcp_dstport)).IEnumDistinctStrComma();
                        //消息类型
                        tcps.msg_distinct_aggre = pd_no_3tcp.Select(e => e.GbOverLLC_MsgType).IEnumDistinctStrComma();
                        #endregion

                        tcps.sndcp_nsapi = pd_no_3tcp.Select(e => e.sndcp_nsapi).IEnumDistinctStrComma();
                        tcps.llcgprs_sapi = pd_no_3tcp.Select(e => e.llcgprs_sapi).IEnumDistinctStrComma();

                        mongo_TcpPortSessionStaging.MongoCol.Insert(tcps);
                    });

            }

            string mess = string.Format("OK......size:{0}...step:{1}", size, step);
            Console.WriteLine(mess);
        }
    }
}