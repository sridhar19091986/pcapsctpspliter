using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GbPlatForm
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
        public double ? down_rate;

        public int packet_count_repeat;

        public decimal? headersize;

        public decimal? seq_totals;


    }
}
