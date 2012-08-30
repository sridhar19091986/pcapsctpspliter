﻿/*
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

namespace OfflineInspect.ReTransmission
{
    public class TcpRetransStaticsDocument
    {
        #region 给sqlserver虚构一个主键，good,ef5 code first,/2012.8.29
        [Key]
        public long trsdID { get; set; }
        #endregion

        #region 维度
        public long _id;
        public string session_id { get; set; }
        public string imsi { get; set; }
        public string lac_ci { get; set; }
        public string direction { get; set; }
        public string msg_distinct_aggre { get; set; }

        public string http_method { get; set; }
        public string absolute_uri { get; set; }//生成uri的绝对路径
        public string user_agent { get; set; }//提取用户的代理

        public string ip_bsc_aggre { get; set; }
        public string ip2_sp_aggre { get; set; }
        public string ip2ttl_sp_aggre { get; set; }

        public string ip_flags_mf { get; set; }
        public string sndcp_m { get; set; }
        public string ip2_flags_mf { get; set; }
        public string tcp_need_segment { get; set; }

        //注入apn、imei维度等
        #endregion

        #region 度量
        //流量计算
        public decimal? ip_total_aggre { get; set; }
        public decimal? seq_total_aggre { get; set; }
        public decimal? seq_total_reduce { get; set; }
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
    }

    public class TcpRetransStatics : CommonToolx, IDisposable
    {
        private string mongo_collection = CommonAttribute.TcpRetransStatics[0];
        private string mongo_db = CommonAttribute.TcpRetransStatics[1];
        private string mongo_conn = CommonAttribute.TcpRetransStatics[2];
        private string directiondown = "Down";

        public MongoCrud<TcpRetransStaticsDocument> mongo_TcpRetransStatics;

        public TcpRetransStatics()
        {
            mongo_TcpRetransStatics = new MongoCrud<TcpRetransStaticsDocument>(mongo_conn, mongo_db, mongo_collection);
        }
        #region Implementing IDisposable and the Dispose Pattern Properly
        private bool disposed = false; // to detect redundant calls
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~TcpRetransStatics()
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
        public void CreatCollection()
        {
            TcpPortSession tts = new TcpPortSession();
            var query = from p in tts.mongo_tts.QueryMongo()
                        select new TcpRetransStaticsDocument
                        {
                            _id = p._id,

                            session_id = p.session_id,
                            imsi = p.imsi,
                            lac_ci = p.lac_ci,
                            msg_distinct_aggre = p.msg_distinct_aggre,
                            direction = p.direction,
                            http_method = p.http_method,
                            user_agent = p.user_agent,
                            absolute_uri = p.absolute_uri,

                            ip_bsc_aggre = p.direction == directiondown ? p.ip_dst_aggre : p.ip_src_aggre,
                            ip2_sp_aggre = p.direction == directiondown ? p.ip2_src_aggre : p.ip2_dst_aggre,
                            ip2ttl_sp_aggre = p.direction == directiondown ? p.ip2_ttl_aggre : "",

                            ip_flags_mf = p.ip_flags_mf,
                            sndcp_m = p.sndcp_m,
                            ip2_flags_mf = p.ip2_flags_mf,
                            tcp_need_segment = p.tcp_need_segment,


                            ip_total_aggre = p.ip_total_aggre,
                            seq_total_aggre = p.seq_total_aggre,
                            seq_total_reduce = p.seq_total_reduce,

                            seq_total_count = p.seq_total_count,
                            seq_distinct_count = p.seq_distinct_count,
                            seq_repeat_cnt = p.seq_repeat_cnt,

                            seq_total_lost = p.seq_total_lost > 0 ? p.seq_total_lost : 0,
                            seq_total_repeat = p.seq_total_lost < 0 ? -1 * p.seq_total_lost : 0,

                            duration = p.duration,
                        };
            mongo_TcpRetransStatics.BulkMongo(query.ToList(), true);
        }
    }
}

