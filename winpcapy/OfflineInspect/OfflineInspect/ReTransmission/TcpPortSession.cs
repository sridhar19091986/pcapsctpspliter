﻿/*
use  [GuangZhou_Gb]
go
alter TABLE [Gb_TCP_ReTransmission]   alter  COLUMN [PacketNum] int  not null
go
alter TABLE [Gb_TCP_ReTransmission]  alter  COLUMN [FileNum] int  not null
go
alter table [Gb_TCP_ReTransmission] add  PRIMARY KEY (PacketNum,FileNum);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using EntitySqlTable.SqlServer.Local.Gb_TCP_ReTransmission;
using EntitySqlTable.SqlServer.ip33.tcp_data;
using System.Data.Objects;
using OfflineInspect.Mongo;
using OfflineInspect.CommonTools;
using System.ComponentModel.DataAnnotations;

namespace OfflineInspect.ReTransmission
{
    public class TcpPortSessionDocument
    {
        #region 给sqlserver虚构一个主键，good,ef5 code first,/2012.8.29
        [Key]
        public long tpsdID { get; set; }
        #endregion

        public long _id;
        public string bsc_bvci { get; set; }
        public string lac_ci { get; set; }
        public string mscbsc_ip_aggre { get; set; }
        public int mscbsc_ip_count { get; set; }
        public string direction { get; set; }
        public double duration { get; set; }
        public string imsi { get; set; }
        public string tcp_seq_aggre { get; set; }//tcp聚合
        public string msg_distinct_aggre { get; set; }//消息聚合
        public decimal? seq_nxt_max { get; set; }
        //包总长度
        public decimal? seq_total_aggre { get; set; } //每次的nxt-seq之和，计算的值是ip2包之和，未计算sndcp包。
        public decimal? seq_total_reduce { get; set; }//真实的总长度。最大nxt减去seq。未包含重传。
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
        public string tcp_nxt_aggre { get; set; }
        public string tcp_port_aggre { get; set; }
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


    }

    public class TcpPortSession : CommonToolx, IDisposable
    {
        private string mongo_collection = CommonAttribute.TlliTcpSession[0];
        private string mongo_db = CommonAttribute.TlliTcpSession[1];
        private string mongo_conn = CommonAttribute.TlliTcpSession[2];
        private int maxfilenum = Int32.Parse(CommonAttribute.TlliTcpSession[3]);
        private int size = Int32.Parse(CommonAttribute.TlliTcpSession[4]);

        public MongoCrud<TcpPortSessionDocument> mongo_tts;
        //private GuangZhou_Gb_TCP_ReTransmission gb;
        private tcp_dataEntities gb;

        public TcpPortSession()
        {
            mongo_tts = new MongoCrud<TcpPortSessionDocument>(mongo_conn, mongo_db, mongo_collection);
            //gb = new GuangZhou_Gb_TCP_ReTransmission();
            gb = new tcp_dataEntities();
            gb.CommandTimeout = 0;
            gb.ContextOptions.LazyLoadingEnabled = true;
            gb.Gb_TCP_ReTransmission.MergeOption = MergeOption.NoTracking;
        }

        #region Implementing IDisposable and the Dispose Pattern Properly
        private bool disposed = false; // to detect redundant calls
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~TcpPortSession()
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
        public void CreateCollection()
        {
            for (int j = 0; j < maxfilenum; j++)
            {
                Console.WriteLine(j);
                CreateCollection(j);
            }
        }

        public void CreateCollection(int filenum)
        {
            IQueryable<Gb_TCP_ReTransmission> tcpsession = gb.Gb_TCP_ReTransmission.Where(e => e.BeginFileNum == filenum);
            CreateTable("Up", tcpsession, filenum);
            CreateTable("Down", tcpsession, filenum);
        }
        /*
         * 
         * 这里做了了优化，ienumber是立即执行，iquery则是延时执行,2012.8.30
         * 
         * */
        //对每个文件号中的开始帧号进行分页
        public void CreateTable(string direction, IQueryable<Gb_TCP_ReTransmission> gb_tcp_retrans, int filenum)
        {
            string host = null;
            string uri = null;

            int packet_cnt = gb_tcp_retrans.Select(e => e.BeginFrameNum).Distinct().Count();

            int step = packet_cnt / size + 1;

            //执行帧号分页
            for (int i = 0; i < step; i++)
            {
                IQueryable<Gb_TCP_ReTransmission> tcp_session = gb_tcp_retrans
                    .Where(e => e.BeginFrameNum >= i * size && e.BeginFrameNum < (i + 1) * size);

                var tcp_sessions = tcp_session.ToLookup(e => e.BeginFrameNum);

                //帧号分页中的每个tcp的会话过程
                foreach (var m in tcp_sessions)
                {
                    #region 会话过滤，filter
                    var gb_packet = m.Where(e => e.bssgp_direction == direction); //本次只计算下行速率
                    var pd_no_3tcp = gb_packet.Where(e => e.tcp_nxtseq != null);//不是3次握手的包
                    if (pd_no_3tcp.Count() == 0) continue;
                    #endregion

                    TcpPortSessionDocument tcps = new TcpPortSessionDocument();

                    #region tcp会话的基础信息，callid/imsi/lac/cell/bvci/duration/
                    tcps._id = GenerateId();
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
                    tcps.bsc_bvci = m.Where(e => e.nsip_bvci != null).Select(e => Convert.ToString(e.nsip_bvci)).IEnumDistinctStrComma();
                    tcps.lac_ci = m.Where(e => e.bssgp_lac != null).Count() == 0 ? "" : m.Where(e => e.bssgp_lac != null).Select(e => Convert.ToString(e.bssgp_lac) + "-" + Convert.ToString(e.bssgp_ci)).IEnumDistinctStrComma();
                    //下行时延，包含3次握手，取这次会话的总长度吧。
                    TimeSpan? ts = m.Max(e => DateTime.Parse(e.GbOverLLC_time)) - pd_no_3tcp.Min(e => DateTime.Parse(e.GbOverLLC_time));
                    tcps.duration = ts.Value.TotalMilliseconds;
                    #endregion

                    #region seq算法，计算tcp重传和丢包
                    //序列号的计算
                    tcps.seq_tcp_min = pd_no_3tcp.Min(e => Convert.ToInt64(e.tcp_seq));
                    tcps.seq_nxt_max = pd_no_3tcp.Max(e => Convert.ToInt64(e.tcp_nxtseq));
                    //正确计算是，每个包进行计算。
                    tcps.seq_total_aggre = pd_no_3tcp.Sum(e => Convert.ToInt64(e.tcp_nxtseq) - Convert.ToInt64(e.tcp_seq));
                    tcps.ip_total_aggre = pd_no_3tcp.Sum(e => e.ip_len);
                    tcps.seq_total_reduce = tcps.seq_nxt_max - tcps.seq_tcp_min;
                    tcps.seq_total_lost = tcps.seq_total_reduce - tcps.seq_total_aggre;
                    //计算速率，取reduce吧
                    //tcps.seq_total_aggre_rate = (double)tcps.seq_total_reduce / tcps.duration;
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
                    tcps.tcp_port_aggre = pd_no_3tcp.Select(e => Convert.ToString(e.tcp_srcport) + "-" + Convert.ToString(e.tcp_dstport)).IEnumDistinctStrComma();
                    //消息类型
                    tcps.msg_distinct_aggre = pd_no_3tcp.Select(e => e.GbOverLLC_MsgType).IEnumDistinctStrComma();
                    #endregion

                    mongo_tts.MongoCol.Insert(tcps);
                }
            }

            string mess = string.Format("OK...direction...{0}...size:{1}...step:{2}...cnt:{3}",
                direction, size, step, packet_cnt);
            Console.WriteLine(mess);
        }
    }
}