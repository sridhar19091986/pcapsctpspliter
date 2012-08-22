using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OfflineInspect.FlowControl;
using OfflineInspect.MultiInterface;
using OfflineInspect.ReTransmission;

namespace ViewInspect
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region FlowControlView
        //FlowControlOneBvc
        private void navBarItem1_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            FlowControlOneBvc fcob = new FlowControlOneBvc();
            var query = from p in fcob.mongo_fcob.QueryMongo()
                        select new
                        {
                            p._id,
                            p.Flow_Control_time,
                            p.lac_cell,
                            p.Flow_Control_MsgType,
                            p.bssgp_direction,
                            p.ip_len,
                            p.bssgp_bmax_default_ms,
                            p.bssgp_bucket_full_ratio,
                            p.bssgp_bucket_leak_rate,
                            p.bssgp_bvc_bucket_size,
                            p.bssgp_ms_bucket_size,
                            p.bssgp_R_default_ms,

                        };

            clearColumns();
            var dborder = query.Take(1000);
            gridControl1.DataSource = dborder.AsParallel().ToList();
            gridView1.OptionsView.ColumnAutoWidth = true;
        }
        //LacCellBvci
        private void navBarItem2_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            OfflineInspect.FlowControl.LacCellBvci lcb = new OfflineInspect.FlowControl.LacCellBvci();
            var query = from p in lcb.mongo_lac_cell_bvci.QueryMongo()
                        select new
                        {
                            p._id,
                            p.lac_cell,
                            p.src,
                            p.dst,
                            p.bvci,
                            p.cnt,
                        };
            clearColumns();
            var dborder = query.OrderBy(e => e.lac_cell);
            gridControl1.DataSource = dborder.AsParallel().ToList();
            gridView1.OptionsView.ColumnAutoWidth = true;
        }
        //FlowControlMapBvc
        private void navBarItem3_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            FlowControlMapBvc fcmb = new FlowControlMapBvc();
            clearColumns();
            var dborder = from p in fcmb.mongo_fcmb.QueryMongo().OrderByDescending(e => e.fcb_cnt)
                          select new
                          {
                              p.lac_cell,
                              p.bvci,
                              p.fcb_cnt,
                              p.packet_cnt,
                              p.tlli_cnt,
                              //radio_status = p.msg_distinct_aggre.CountMessage("BSSGP.RADIO-STATUS"),
                              p.ms_leak_rate,
                              p.ms_bucket_size,
                              p.bssgp_bmax_default_ms,
                              p.bssgp_bucket_full_ratio,
                              p.bssgp_bucket_leak_rate,
                              p.bssgp_bvc_bucket_size,
                              p.bssgp_ms_bucket_size,
                              p.bssgp_R_default_ms,
                              p.down_packet_rate,
                              p.down_total_len,
                              p.fcb_delay_aggre,
                              p.fcb_time_aggre,
                              p.msg_distinct_aggre,
                              p.tlli_distinct_aggre
                          };
            gridControl1.DataSource = dborder.AsParallel().ToList();
            gridView1.OptionsView.ColumnAutoWidth = true;
        }
        //FlowControlMapBvc&& LacCellBvci
        private void navBarItem4_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            clearColumns();
            FlowControlMapBvc fcmb = new FlowControlMapBvc();
            var bvc = fcmb.mongo_fcmb.ListT;
            OfflineInspect.FlowControl.LacCellBvci lcb = new OfflineInspect.FlowControl.LacCellBvci();
            var cell = from p in lcb.mongo_lac_cell_bvci.ListT
                       group p by p.lac_cell into ttt
                       select new
                       {
                           lac_cell = ttt.Key,
                           bvci_aggre = ttt.Select(e => e.bvci).Aggregate((a, b) => a + "," + b),

                       };
            var query = from p in bvc
                        join q in cell on p.lac_cell equals q.lac_cell
                        select new
                        {
                            p.lac_cell,
                            p.fcb_cnt,
                            p.packet_cnt,
                            p.tlli_cnt,
                            q.bvci_aggre,
                            //radio_status = p.msg_distinct_aggre.CountMessage("BSSGP.RADIO-STATUS"),
                            p.ms_leak_rate,
                            p.ms_bucket_size,
                            p.bssgp_bmax_default_ms,
                            p.bssgp_bucket_full_ratio,
                            p.bssgp_bucket_leak_rate,
                            p.bssgp_bvc_bucket_size,
                            p.bssgp_ms_bucket_size,
                            p.bssgp_R_default_ms,
                            p.down_packet_rate,
                            p.down_total_len,
                            p.fcb_delay_aggre,
                            p.fcb_time_aggre,
                            p.msg_distinct_aggre,
                            p.tlli_distinct_aggre
                        };

            gridControl1.DataSource = query.OrderByDescending(e => e.fcb_cnt).AsParallel().ToList();
            gridView1.OptionsView.ColumnAutoWidth = true;
        }
        //FlowControlMapMs 
        private void navBarItem5_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            FlowControlMapMs fcmm = new FlowControlMapMs();
            var query = from p in fcmm.mongo_fcmm.QueryMongo()
                        select new
                        {
                            p.BeginFrameNum,
                            p.fcontrol_cnt,
                            p.packet_cnt,
                            p.leak_rate_avg,
                            p.leak_rate_max,
                            p.leak_rate_min,
                            p.bucket_size_avg,
                            p.bucket_size_max,
                            p.bucket_size_min,

                            p.down_packet_rate,
                            p.down_total_len,
                            p.fcm_time,
                            p.bucket_size,
                            p.first_delay,
                            p.leak_rate,


                        };
            clearColumns();
            var dborder = query.OrderByDescending(e => e.fcontrol_cnt).Take(100);
            gridControl1.DataSource = dborder.AsParallel().ToList();
            gridView1.OptionsView.ColumnAutoWidth = false;
        }
        //FlowControlOneMs
        private void navBarItem6_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            FlowControlOneMs fcom = new FlowControlOneMs();
            var query = from p in fcom.mongo_fcom.QueryMongo()
                        select new
                        {
                            p._id,
                            p.BeginFrameNum,
                            p.Flow_Control_time,
                            p.PacketNum,
                            p.ip_len,
                            p.Flow_Control_MsgType,
                            p.bssgp_direction,
                            p.bucket_size,
                            p.leak_rate,

                        };
            clearColumns();
            var dborder = query.OrderBy(e => e._id).Take(100);
            gridControl1.DataSource = dborder.AsParallel().ToList();
            gridView1.OptionsView.ColumnAutoWidth = true;
        }




        //FlowControlMessageRate.cs
        private void navBarItem7_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            FlowControlMessageRate fcmr = new FlowControlMessageRate();
            var query = from p in fcmr.mongo_fcmr.QueryMongo()
                        select new
                        {
                            p._id,
                            p.cnt,
                            p.cnt_percent,
                            p.cnt_total,
                            p.message_type,
                            p.size,
                            p.size_percent,
                            p.size_total,
                        };
            clearColumns();
            var dborder = query.OrderByDescending(e => e.cnt);
            gridControl1.DataSource = dborder.AsParallel().ToList();
            gridView1.OptionsView.ColumnAutoWidth = true;
        }
        //FlowControlBeforeMessage.cs
        private void navBarItem8_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            FlowControlBeforeMessage fcbm = new FlowControlBeforeMessage();
            var query = from p in fcbm.mongo_fcbm.QueryMongo()
                        select new
                        {
                            p._id,
                            p.bssgp_bucket_full_ratio,
                            p.bssgp_bucket_leak_rate,
                            p.bssgp_ms_bucket_size,
                            p.duration,
                            p.fc_msg,
                            p.fc_packetnum,
                            p.fc_packettime,
                            p.fcb_msg,
                            p.fcb_packetnum,
                            p.fcb_packettime,
                        };
            clearColumns();
            var dborder = query.OrderBy(e => e._id);
            gridControl1.DataSource = dborder.AsParallel().ToList();
            gridView1.OptionsView.ColumnAutoWidth = true;
        }
        //FlowControlBeforeMessageMap.cs
        private void navBarItem9_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            FlowControlBeforeMessageMap fcbmm = new FlowControlBeforeMessageMap();
            var query = from p in fcbmm.mongo_fcbmm.QueryMongo()
                        select new
                        {
                            p._id,
                            p.bucket_size_avg,
                            p.cnt,
                            p.cnt_percent,
                            p.duration,
                            p.fcBeforMsgs,
                            p.full_rate_avg,
                            p.leak_rate,

                        };
            clearColumns();
            var dborder = query.OrderByDescending(e => e.cnt);
            gridControl1.DataSource = dborder.AsParallel().ToList();
            gridView1.OptionsView.ColumnAutoWidth = true;

        }

        #endregion

        #region  MultiInerfaceView




        private void navBarItem10_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            GiGETRateMap giget = new GiGETRateMap();
            var query = from p in giget.mongo_get.QueryMongo()
                        select new
                        {
                            p._id,
                            p.rkey.ip,
                            p.rkey.ip_id,
                            p.rkey.tcp_seq,
                            p.rkey.tcp_nxtseq,
                            p.rkey.tcp_ack,
                            p.rkey.http_request_uri,
                            p.rkey.http_user_agent,
                            p.itface,
                            p.ip_proto,
                            p.min_time,
                            p.max_time,
                            p.get_cnt,
                            p.reponse_cnt,
                            p.reponse_rate,
                            p.reponse_delay,
                            p.packetnum_aggre,
                        };
            clearColumns2();
            var dborder = query.OrderByDescending(e => e.get_cnt).Take(1000);
            gridControl2.DataSource = dborder.AsParallel().ToList();
            gridView2.OptionsView.ColumnAutoWidth = true;
        }

        #endregion

        #region tcp、llc重传和丢包
        private void navBarItem14_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            TlliTcpSession tts = new TlliTcpSession();
            var query = from p in tts.mongo_tts.QueryMongo()
                        select new
                        {
                            p._id,
                            p.session_id,
                            p.imsi,
                            p.bsc_bvci,
                            p.lac_ci,
                            p.mscbsc_ip_count,
                            p.direction,
                            p.ip_total_aggre,
                            p.seq_total_count,
                            p.seq_distinct_count,
                            p.seq_total_aggre,
                            p.seq_total_reduce,
                            p.seq_total_lost,
                            p.seq_repeat_rate,
                            p.duration,
                            p.seq_total_aggre_rate,
                            p.ip_ttl_aggre,
                            p.ip2_ttl_aggre,
                            p.ip_flags_mf,
                            p.ip2_flags_mf,
                            p.tcp_flags_cwr,
                            p.tcp_win_size,
                            p.tcp_port_aggre,
                            p.sndcp_m,
                            p.tcp_need_segment,
                            p.msg_aggre,
                            p.ip_src_aggre,
                            p.ip_dst_aggre,
                            p.ip2_src_aggre,
                            p.ip2_dst_aggre,
                            p.seq_tcp_min,
                            p.seq_nxt_max,
                            p.mscbsc_ip_aggre,
                            p.tcp_seq_aggre,
                            p.tcp_nxt_aggre,
                        };
            clearColumns3();
            var dborder = query.OrderBy(e => e.session_id).Take(1000);
            gridControl3.DataSource = dborder.AsParallel().ToList();
            gridView3.OptionsView.ColumnAutoWidth = false;
            gridView3.BestFitColumns();
        }

        private void navBarItem15_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            OfflineInspect.ReTransmission.LacCellBvci lcb = new OfflineInspect.ReTransmission.LacCellBvci();
            var query = from p in lcb.mongo_lac_cell_bvci.QueryMongo()
                        select new
                        {
                            p._id,
                            p.lac_cell,
                            p.src,
                            p.dst,
                            p.bvci,
                            p.cnt,
                        };
            clearColumns3();
            var dborder = query.OrderByDescending(e => e._id).Take(100);
            gridControl3.DataSource = dborder.AsParallel().ToList();
            gridView3.OptionsView.ColumnAutoWidth = false;
            gridView3.BestFitColumns();
        }

        private void navBarItem16_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            TlliLLCSession tls = new TlliLLCSession();
            var query = from p in tls.mongo_tls.QueryMongo()
                        select new
                        {
                            p._id,
                            p.session_id,
                            p.imsi,
                            p.bsc_bvci,
                            p.lac_ci,
                            p.mscbsc_ip_count,
                            p.direction,
                            p.duration,
                            p.llc_nu_continue,
                            p.llc_nu_count,
                            p.msg_aggre,
                            p.llc_nu_aggre,
                        };
            clearColumns3();
            var dborder = query.OrderBy(e => e.session_id).Take(1000);
            gridControl3.DataSource = dborder.AsParallel().ToList();
            gridView3.OptionsView.ColumnAutoWidth = false;
            gridView3.BestFitColumns();
        }
        #endregion
    }
}

