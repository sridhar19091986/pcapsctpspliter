using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfflineInspect.ReTransmission
{
    public class myTcpSession11
    {
        public int? session_id;
        public int? lac;
        public int? ci;
        public string bsc_ip;
        public int? bsc_bvci;
        public int? ip2_total;
        public string direction;
        public decimal? seq_total;
        public string imsi;

        public int? ip2_min_len;  //ack的包头

        public decimal? seq_min_;
        public decimal? seq_max_;

        public decimal? seq_nxt;

        public double duration;

        public int packet_count;

        public int? seq_ip2;
        public double? down_rate;

        public int packet_count_repeat;

        public decimal? headersize;

        public decimal? seq_totals;
        public void CreateTable(string direction)
        {
            Guangzhou_GbEntities gb = new Guangzhou_GbEntities();

            int packet_cnt = gb_cell_repeat.Select(e => e.BeginFrameNum).Distinct().Count();
            int size = 5000;
            int step = packet_cnt / size + 1;

            progressBar1.Maximum = step;

            for (int i = 0; i < step; i++)
            {
                progressBar1.Value = i;
                textBox1.Text = i.ToString();
                Application.DoEvents();

                var tcp_session = gb_cell_repeat
                    .Where(e => e.BeginFrameNum >= i * size && e.BeginFrameNum < (i + 1) * size);

                var tcp_sessions = tcp_session.ToLookup(e => e.BeginFrameNum);

                List<myTcpSession> sessions = new List<myTcpSession>();

                //tcp的会话过程
                foreach (var m in tcp_sessions)
                {
                    var packet_down = m.Where(e => e.bssgp_direction == direction); //本次只计算下行速率

                    var pd_no_3tcp = packet_down.Where(e => e.tcp_nxtseq != null);//不是3次握手的包

                    if (pd_no_3tcp == null) continue;
                    if (pd_no_3tcp.Count() < 2) continue;

                    myTcpSession tcps = new myTcpSession();

                    tcps.direction = direction;
                    tcps.session_id = (int)m.Key;
                    tcps.bsc_ip = packet_down.Select(e => e.ip_dst_host).FirstOrDefault();
                    tcps.bsc_bvci = m.Where(e => e.nsip_bvci != null).Select(e => e.nsip_bvci).FirstOrDefault();
                    tcps.lac = m.Where(e => e.bssgp_lac != null).Select(e => e.bssgp_lac).FirstOrDefault();
                    tcps.ci = m.Where(e => e.bssgp_ci != null).Select(e => e.bssgp_ci).FirstOrDefault();
                    tcps.imsi = packet_down.Select(e => e.bssgp_imsi).FirstOrDefault();

                    //下行时延，包含3次握手
                    TimeSpan? ts = pd_no_3tcp.Max(e => DateTime.Parse(e.TCP_time))
                        - pd_no_3tcp.Min(e => DateTime.Parse(e.TCP_time));
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

                    //序列号的计算
                    tcps.seq_min = pd_no_3tcp.Min(e => Convert.ToInt64(e.tcp_seq));
                    tcps.seq_nxt = pd_no_3tcp.Max(e => Convert.ToInt64(e.tcp_nxtseq));
                    tcps.seq_max = pd_no_3tcp.Max(e => Convert.ToInt64(e.tcp_seq));

                    //????
                    //正确计算seq的包长，
                    //tcps.seq_total = (tcps.seq_nxt > tcps.seq_max ? tcps.seq_nxt : tcps.seq_max) - tcps.seq_min;
                    //不能用 seq_max
                    //tcps.seq_total = tcps.seq_nxt - tcps.seq_min;//这个计算有错误

                    //正确计算是，每个包进行计算。
                    tcps.seq_total = pd_no_3tcp.Sum(e => Convert.ToInt64(e.tcp_nxtseq) - Convert.ToInt64(e.tcp_seq));

                    //包数量和重传的计算
                    tcps.packet_count = pd_no_3tcp.Count();
                    tcps.packet_count_repeat = pd_no_3tcp.Count()
                        - pd_no_3tcp.Select(e => e.tcp_nxtseq).Distinct().Count();

                    //????
                    //序列号和ip包的计算误差
                    //tcps.seq_ip2 = tcps.seq_total + (decimal?)tcps.packet_count * (decimal)tcps.headersize - tcps.ip2_total;
                    tcps.seq_ip2 = (int)((tcps.seq_total - tcps.ip2_total) / tcps.packet_count);

                    //速率的计算
                    if (tcps.duration != 0)
                    {
                        tcps.ip2_rate = 1.0 * (double)tcps.ip2_total / tcps.duration;
                        tcps.seq_rate = 1.0 * (double)tcps.seq_total / tcps.duration;
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

                    //sessions.Add(tcps);
                    gb.myTcpSession.AddObject(tcps);

                }

                gb.SaveChanges();

                //容易出错？？？
                //ThreadPool.QueueUserWorkItem(e => gb.SaveChanges());


            }

            string mess = string.Format("OK...size:{0}...step:{1}...cnt:{2}", size, step, packet_cnt);
            MessageBox.Show(mess);

        }




    }
    class TcpSession
    {
    }
}
