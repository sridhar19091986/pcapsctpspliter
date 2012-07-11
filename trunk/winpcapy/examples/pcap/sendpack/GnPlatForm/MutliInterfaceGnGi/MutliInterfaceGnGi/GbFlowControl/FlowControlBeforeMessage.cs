using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MutliInterfaceGnGi.GbFlowControl
{
    public class FlowControlBeforeMessage
    {
        public int? bssgp_ms_bucket_size;
        public int? bssgp_bucket_leak_rate;

        public int? bssgp_bucket_full_ratio;


        public double duration;

        public int fcb_packetnum;
        public DateTime? fcb_packettime;
        public string fcb_msg;

        public int fc_packetnum;
        public DateTime? fc_packettime;
        public string fc_msg;

    }
}
