using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;
using System.Data.Objects;
using System.Data.Entity;
using System.Data.Entity.Design;
using EntityFramework.Extensions;
using EntityFramework.Caching;
using EntityFramework.Batch;

using MutliInterfaceGnGi.GbFlowControl;


namespace MutliInterfaceGnGi
{

    public partial class Service1 : IService1
    {

        private DataTable viewTableDetail_gb()
        {
            var cnt_t = gz_gb.Gb_FlowControly.FromCache().Count();
            var size_t = gz_gb.Gb_FlowControly.FromCache().Sum(e => e.ip_len);

            var gngi = from p in gz_gb.Gb_FlowControly.FromCache()
                       group p by p.Flow_Control_MsgType into ttt
                       select new
                       {
                           ttt.Key,
                           cnt = ttt.Count(),
                           cnt_total = cnt_t,
                           cnt_percent = 1.0 * ttt.Count() / cnt_t,
                           size = ttt.Sum(e => e.ip_len),
                           size_total = size_t,
                           size_percent = 1.0 * ttt.Sum(e => e.ip_len) / size_t,


                       };


            var dborder = gngi.OrderByDescending(e => e.cnt).ToList();

            return dborder.ToDataTable();

        }

        string flowcontrol = "BSSGP.FLOW-CONTROL-MS";


        private DataTable viewTableDetail_All_gb()
        {
            var beginframes = gz_gb.Gb_FlowControly.ToLookup(e => e.BeginFrameNum);


            List<FlowControlBeforeMessage> fcBeforMsgs = new List<FlowControlBeforeMessage>();

            foreach (var m in beginframes)
            {
                var fc = m.Where(e => e.Flow_Control_MsgType.StartsWith(flowcontrol)).OrderBy(e => e.PacketNum).FirstOrDefault();
                if (fc != null)
                {
                    var fcb = m
                        .Where(e => e.Flow_Control_MsgType.StartsWith("GMMSM."))
                        .Where(e => e.Flow_Control_MsgType.StartsWith("GMMSM.GMM Information") == false)
                        .Where(e => e.PacketNum < fc.PacketNum)
                        .OrderByDescending(e => e.PacketNum).FirstOrDefault();
                    if (fcb != null)
                    {
                        FlowControlBeforeMessage fcbm = new FlowControlBeforeMessage();
                        fcbm.fc_msg = fc.Flow_Control_MsgType;
                        fcbm.fc_packetnum = fc.PacketNum;
                        fcbm.fc_packettime = fc.PacketTime;

                        fcbm.bssgp_ms_bucket_size = fc.bssgp_ms_bucket_size;
                        fcbm.bssgp_bucket_leak_rate = fc.bssgp_bucket_leak_rate;
                        fcbm.bssgp_bucket_full_ratio = fc.bssgp_bucket_full_ratio;

                        TimeSpan ts = fc.PacketTime.Value - fcb.PacketTime.Value;

                        fcbm.duration = ts.TotalSeconds;

                        fcbm.fcb_msg = fcb.Flow_Control_MsgType;
                        fcbm.fcb_packetnum = fcb.PacketNum;
                        fcbm.fcb_packettime = fcb.PacketTime;


                        fcBeforMsgs.Add(fcbm);
                    }

                }
            }

            var dborder = from p in fcBeforMsgs
                          orderby p.fc_packetnum
                          select new
                          {
                              p.fc_packetnum,
                              p.fc_packettime,
                              p.fc_msg,
                              p.bssgp_ms_bucket_size,
                              p.bssgp_bucket_leak_rate,
                              p.bssgp_bucket_full_ratio,
                              p.fcb_packetnum,
                              p.fcb_packettime,
                              p.fcb_msg,
                              p.duration,
                          };

            return dborder.ToDataTable();
        }



        private DataTable statTableDetail_All_gb()
        {
            var beginframes = gz_gb.Gb_FlowControly.ToLookup(e => e.BeginFrameNum);

            List<FlowControlBeforeMessage> fcBeforMsgs = new List<FlowControlBeforeMessage>();

            foreach (var m in beginframes)
            {
                var fc = m.Where(e => e.Flow_Control_MsgType.StartsWith(flowcontrol)).OrderBy(e => e.PacketNum).FirstOrDefault();
                if (fc != null)
                {
                    var fcb = m
                        .Where(e => e.Flow_Control_MsgType.StartsWith("GMMSM."))
                        .Where(e => e.Flow_Control_MsgType.StartsWith("GMMSM.GMM Information") == false)
                        .Where(e => e.PacketNum < fc.PacketNum)
                        .OrderByDescending(e => e.PacketNum).FirstOrDefault();
                    if (fcb != null)
                    {
                        FlowControlBeforeMessage fcbm = new FlowControlBeforeMessage();
                        fcbm.fc_msg = fc.Flow_Control_MsgType;
                        fcbm.fc_packetnum = fc.PacketNum;
                        fcbm.fc_packettime = fc.PacketTime;

                        fcbm.bssgp_ms_bucket_size = fc.bssgp_ms_bucket_size;
                        fcbm.bssgp_bucket_leak_rate = fc.bssgp_bucket_leak_rate;
                        fcbm.bssgp_bucket_full_ratio = fc.bssgp_bucket_full_ratio;

                        TimeSpan ts = fc.PacketTime.Value - fcb.PacketTime.Value;

                        fcbm.duration = ts.TotalSeconds;

                        fcbm.fcb_msg = fcb.Flow_Control_MsgType;
                        fcbm.fcb_packetnum = fcb.PacketNum;
                        fcbm.fcb_packettime = fcb.PacketTime;


                        fcBeforMsgs.Add(fcbm);
                    }

                }
            }

            var dborders = from p in fcBeforMsgs
                          group p by p.fcb_msg into ttt
                          select new
                          {
                              ttt.Key,
                              cnt = ttt.Count(),
                              cnt_percent =1.0* ttt.Count() / fcBeforMsgs.Count(),
                              duration = ttt.Average(e => e.duration),
                              bucket_size_avg=ttt.Average(e=>e.bssgp_ms_bucket_size),
                              leak_rate=ttt.Average(e=>e.bssgp_bucket_leak_rate),
                              full_rate_avg=ttt.Average(e=>e.bssgp_bucket_full_ratio),

                          };
            var dborder = dborders.OrderByDescending(e => e.cnt);

            return dborder.ToDataTable();
        }

    }
}
