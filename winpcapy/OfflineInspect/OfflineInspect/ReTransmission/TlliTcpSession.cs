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
using EntitySqlTable.SqlServer.Local.Gb_TCP_ReTransmission;
using System.Data.Objects;
using OfflineInspect.Mongo;
using OfflineInspect.CommonTools;

namespace OfflineInspect.ReTransmission
{
    public class TlliTcpSessionDocument
    {
        public object _id;
        public string bsc_bvci;
        public string lac_ci;
        public string bsc_ip;
        public int? ci;
        public string direction;
        public double? down_rate;
        public double duration;
        public decimal? headersize;
        public string imsi;
        public string ip_aggre;//ip聚合
        public string ip2_aggre;//ip2聚合
        public string llc_aggre;//llc聚合
        public string tcp_seq_aggre;//tcp聚合
        public string msg_aggre;//消息聚合
        public double ip_rate;
        public int? ip_total;
        public int? ip2_min_len;	//ack的包头
        public double ip2_rate;
        public int? ip2_total;
        public int? lac;
        public int llc_cnt;
        public int? llc_max;
        public int? llc_min;
        public int packet_count;
        public int packet_count_repeat;
        public int? packet_discard_total;
        public int packet_sack_total;
        public int? seq_ip2;
        public decimal? seq_max;
        public decimal? seq_min;
        public decimal? seq_nxt_max;
        public double seq_rate;
        public decimal? seq_total_aggre;
        public decimal? seq_total_reduce;
        public decimal? seq_total_lost;
        public double? seq_aggre_rate;
        public decimal? seq_totals;
        public int? session_id;
        public decimal seq_tcp_min;
        public double seq_total_aggre_rate;
        public double seq_total_count;
        public int? ip_total_aggre;


        public string ip_addr_aggre;
        public string ip2_addr_aggre;
        public string ip_ttl_aggre;
        public string ip2_ttl_aggre;
        public string ip_flags_mf;
        public string ip2_flags_mf;
        public string tcp_flags_cwr;
        public string tcp_win_size;
        public string tcp_nxt_aggre;
        public string tcp_port_aggre;
        public string sndcp_m;
        public string tcp_need_segment;
    }

    public class TlliTcpSession : CommonToolx, IDisposable
    {
        private string mongo_collection = CommonAttribute.TlliTcpSession[0];
        private string mongo_db = CommonAttribute.TlliTcpSession[1];
        private string mongo_conn = CommonAttribute.TlliTcpSession[2];
        private int maxfilenum = Int32.Parse(CommonAttribute.TlliTcpSession[3]);
        private int size = Int32.Parse(CommonAttribute.TlliTcpSession[4]);

        public MongoCrud<TlliTcpSessionDocument> mongo_tts;
        private GuangZhou_Gb_TCP_ReTransmission gb;
        private LacCellBvci lcb;

        public TlliTcpSession()
        {
            mongo_tts = new MongoCrud<TlliTcpSessionDocument>(mongo_conn, mongo_db, mongo_collection);
            gb = new GuangZhou_Gb_TCP_ReTransmission();
            lcb = new LacCellBvci();
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
        ~TlliTcpSession()
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

        public void CreateCollection()
        {
            for (int j = 0; j < maxfilenum; j++)
                CreateCollection(j);
        }

        public void CreateCollection(int filenum)
        {
            var tcpsession = gb.Gb_TCP_ReTransmission.Where(e => e.FileNum == filenum);
            CreateTable("Up", tcpsession);
            CreateTable("Down", tcpsession);
        }

        public void CreateTable(string direction, IEnumerable<Gb_TCP_ReTransmission> gb_tcp_retrans)
        {
            int packet_cnt = gb_tcp_retrans.Select(e => e.BeginFrameNum).Distinct().Count();

            int step = packet_cnt / size + 1;

            //progressBar1.Maximum = step;

            for (int i = 0; i < step; i++)
            {
                //progressBar1.Value = i;
                //textBox1.Text = i.ToString();
                //Application.DoEvents();

                var tcp_session = gb_tcp_retrans.Where(e => e.BeginFrameNum >= i * size && e.BeginFrameNum < (i + 1) * size);

                var tcp_sessions = tcp_session.ToLookup(e => e.BeginFrameNum);

                //List<TlliTcpSession> sessions = new List<TlliTcpSession>();

                //tcp的会话过程
                foreach (var m in tcp_sessions)
                {
                    #region 会话过滤，filter

                    var packet_down = m.Where(e => e.bssgp_direction == direction); //本次只计算下行速率

                    var pd_no_3tcp = packet_down.Where(e => e.tcp_nxtseq != null);//不是3次握手的包

                    if (pd_no_3tcp.Count() == 0) continue;
                    //if (pd_no_3tcp.Count() < 2) continue;

                    #endregion

                    TlliTcpSessionDocument tcps = new TlliTcpSessionDocument();

                    #region tcp会话的基础信息，callid/imsi/lac/cell/bvci/duration/

                    //TlliTcpSession tcps = new TlliTcpSession();

                    tcps._id = GenerateId();
                    tcps.direction = direction;
                    tcps.session_id = (int)m.Key;
                    tcps.bsc_ip = packet_down.Select(e => e.ip_dst_host).FirstOrDefault();


                    //还需要1个？留意ip的问题?
                    tcps.bsc_bvci = m.Where(e => e.nsip_bvci != null).Select(e => Convert.ToString(e.nsip_bvci)).Distinct().Aggregate((a, b) => a + "," + b);
                    tcps.lac_ci = m.Where(e => e.bssgp_lac != null).Count() == 0 ? "" : m.Where(e => e.bssgp_lac != null).Select(e => Convert.ToString(e.bssgp_lac) + "-" + Convert.ToString(e.bssgp_ci)).Distinct().Aggregate((a, b) => a + "," + b);


                    tcps.lac = m.Where(e => e.bssgp_lac != null).Select(e => e.bssgp_lac).FirstOrDefault();
                    tcps.ci = m.Where(e => e.bssgp_ci != null).Select(e => e.bssgp_ci).FirstOrDefault();
                    tcps.imsi = packet_down.Select(e => e.bssgp_imsi).FirstOrDefault();

                    //下行时延，包含3次握手
                    TimeSpan? ts = pd_no_3tcp.Max(e => DateTime.Parse(e.TCP_time)) - pd_no_3tcp.Min(e => DateTime.Parse(e.TCP_time));
                    tcps.duration = ts.Value.TotalMilliseconds;

                    //下行包，非3次握手
                    tcps.ip2_total = pd_no_3tcp.Sum(e => e.ip2_len);

                    tcps.ip_total = pd_no_3tcp.Sum(e => e.ip_len);

                    //提取ack的包头
                    tcps.ip2_min_len = packet_down.Min(e => e.ip2_len);

                    //包头的计算
                    //取最小值
                    //.FirstOrDefault();
                    decimal? header = pd_no_3tcp.Min(e => e.ip2_len -
                        (Convert.ToInt64(e.tcp_nxtseq) - Convert.ToInt64(e.tcp_seq)));

                    tcps.headersize = (header == null ?
                        (tcps.ip2_min_len == 0 ? header : tcps.ip2_min_len)
                        : header);

                    //ip2的问题可能导致计算错误，纠错，ack的包头
                    //if (tcps.headersize > tcps.ip2_min_len)
                    //    tcps.headersize = tcps.ip2_min_len;

                    #endregion

                    #region 关键信息聚合，信令回放
                    tcps.ip_aggre = pd_no_3tcp.Select(e => e.ip_len.Value.ToString()).Aggregate((a, b) => a + "," + b);
                    tcps.ip2_aggre = pd_no_3tcp.Select(e => e.ip2_len.Value.ToString()).Aggregate((a, b) => a + "," + b);
                    tcps.llc_aggre = pd_no_3tcp.Select(e => e.llcgprs_nu.Value.ToString()).Aggregate((a, b) => a + "," + b);

                    #endregion

                    #region seq算法，计算tcp重传和丢包
                    //正确计算seq的包长，
                    //tcps.seq_total = (tcps.seq_nxt > tcps.seq_max ? tcps.seq_nxt : tcps.seq_max) - tcps.seq_min;
                    //不能用seq_max
                    //tcps.seq_total = tcps.seq_nxt - tcps.seq_min;//这个计算有错误
                    //序列号的计算
                    tcps.seq_tcp_min = pd_no_3tcp.Min(e => Convert.ToInt64(e.tcp_seq));
                    //tcps.seq_nxt = pd_no_3tcp.Max(e => Convert.ToInt64(e.tcp_nxtseq));
                    tcps.seq_nxt_max = pd_no_3tcp.Max(e => Convert.ToInt64(e.tcp_nxtseq));
                    //正确计算是，每个包进行计算。
                    tcps.seq_total_aggre = pd_no_3tcp.Sum(e => Convert.ToInt64(e.tcp_nxtseq) - Convert.ToInt64(e.tcp_seq));
                    tcps.ip_total_aggre = pd_no_3tcp.Sum(e => e.ip_len);
                    tcps.seq_total_reduce = tcps.seq_nxt_max - tcps.seq_tcp_min;
                    tcps.seq_total_lost = tcps.seq_total_reduce - tcps.seq_total_aggre;
                    tcps.seq_total_aggre_rate = (double)tcps.seq_total_aggre / tcps.duration;
                    tcps.seq_total_count = pd_no_3tcp.Count();
                    tcps.tcp_seq_aggre = pd_no_3tcp.Select(e => e.tcp_seq).Aggregate((a, b) => a + "," + b);
                    //第1层ip地址，第2层ip地址
                    tcps.ip_addr_aggre = pd_no_3tcp.Select(e => e.ip_src_host + "-" + e.ip_dst_host).Distinct().Aggregate((a, b) => a + "," + b);
                    tcps.ip2_addr_aggre = pd_no_3tcp.Select(e => e.ip2_src_host + "-" + e.ip2_dst_host).Distinct().Aggregate((a, b) => a + "," + b);
                    //ttl
                    tcps.ip_ttl_aggre = pd_no_3tcp.Select(e => e.ip_ttl).Distinct().Aggregate((a, b) => a + "," + b);
                    tcps.ip2_ttl_aggre = pd_no_3tcp.Select(e => e.ip2_ttl).Distinct().Aggregate((a, b) => a + "," + b);
                    //分片
                    tcps.ip_flags_mf = pd_no_3tcp.Select(e => e.ip_flags_mf).Distinct().Aggregate((a, b) => a + "," + b);
                    tcps.ip2_flags_mf = pd_no_3tcp.Select(e => e.ip2_flags_mf).Distinct().Aggregate((a, b) => a + "," + b);
                    tcps.sndcp_m = pd_no_3tcp.Select(e => e.sndcp_m).Distinct().Aggregate((a, b) => a + "," + b);
                    tcps.tcp_need_segment = pd_no_3tcp.Select(e => e.tcp_need_segment).Distinct().Aggregate((a, b) => a + "," + b);
                    //windows_size,CWR
                    tcps.tcp_flags_cwr = pd_no_3tcp.Select(e => e.tcp_flags_cwr).Distinct().Aggregate((a, b) => a + "," + b);
                    tcps.tcp_win_size = pd_no_3tcp.Select(e => e.tcp_window_size).Distinct().Aggregate((a, b) => a + "," + b);
                    //端口
                    tcps.tcp_nxt_aggre = pd_no_3tcp.Select(e => e.tcp_nxtseq).Aggregate((a, b) => a + "," + b);
                    tcps.tcp_port_aggre = pd_no_3tcp.Select(e => Convert.ToString(e.tcp_srcport) + "-" + Convert.ToString(e.tcp_dstport)).Distinct().Aggregate((a, b) => a + "," + b);
                    //消息类型
                    tcps.msg_aggre = pd_no_3tcp.Select(e => e.TCP_MsgType).Distinct().Aggregate((a, b) => a + "," + b);
                    #endregion

                    //包数量和重传的计算
                    tcps.packet_count = pd_no_3tcp.Count();
                    tcps.packet_count_repeat = pd_no_3tcp.Count() - pd_no_3tcp.Select(e => e.tcp_nxtseq).Distinct().Count();

                    //????
                    //序列号和ip包的计算误差
                    //tcps.seq_ip2 = tcps.seq_total + (decimal?)tcps.packet_count * (decimal)tcps.headersize - tcps.ip2_total;
                    //tcps.seq_ip2 = (int)((tcps.seq_total - tcps.ip2_total) / tcps.packet_count);

                    //速率的计算
                    if (tcps.duration != 0)
                    {
                        tcps.ip2_rate = 1.0 * (double)tcps.ip2_total / tcps.duration;
                        //tcps.seq_rate = 1.0 * (double)tcps.seq_total / tcps.duration;
                        tcps.ip_rate = 1.0 * (double)tcps.ip_total / tcps.duration;
                    }

                    //丢包的九三

                    /*
                     * 未出现tcp重传? sndcp_set ?  tcp_segment? 
                     * 但是 seq+包头!=ip？的问题？  这种情况可是tcp丢包？
                     * */

                    int sndcp = pd_no_3tcp.Where(e => e.sndcp_m == "Set").Count();
                    int tcp = pd_no_3tcp.Where(e => e.tcp_need_segment == "Set").Count();

                    if (tcps.packet_count_repeat == 0 && (tcps.seq_ip2 + tcps.headersize) != 0)
                    {
                        if (sndcp + tcp == 0)
                            tcps.packet_discard_total = 1;
                        else
                        {
                            if (sndcp != 0)
                                tcps.packet_discard_total = 9;
                            if (tcp != 0)
                                tcps.packet_discard_total = 11;
                        }
                    }

                    tcps.packet_sack_total = pd_no_3tcp.Where(e => e.tcp_options_sack_se_num > 0).Count();

                    tcps.llc_max = packet_down.Max(e => e.llcgprs_nu);
                    tcps.llc_min = packet_down.Min(e => e.llcgprs_nu);
                    tcps.llc_cnt = packet_down.Count();

                    //llc重传算法设计，通过511这个特殊的帧号处理会话？？？？
                    if (tcps.llc_max == 511)
                    {
                        int pn = packet_down.Where(e => e.llcgprs_nu == 511).Select(e => e.PacketNum).First();
                        tcps.llc_max = packet_down.Where(e => e.PacketNum > pn).Max(e => e.llcgprs_nu + 511);
                        tcps.llc_min = packet_down.Where(e => e.PacketNum <= pn).Min(e => e.llcgprs_nu);
                    }

                    mongo_tts.MongoCol.Insert(tcps);

                }
            }

            string mess = string.Format("OK...direction...{0}...size:{1}...step:{2}...cnt:{3}",
                direction, size, step, packet_cnt);

            Console.WriteLine(mess);

        }
    }
}
