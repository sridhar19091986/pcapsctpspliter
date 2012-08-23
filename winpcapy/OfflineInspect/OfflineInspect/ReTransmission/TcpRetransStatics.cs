﻿/*
 * 
 * 输出目标是给SSAS进行数据挖掘使用。
 * 
 * 
 * 合理增加维度进行计算
 * 
 * 
 * mongo->gridview->exportexcel->etl(SSIS)->ssas
 * 
 * 
 * 2012.8.23
 * 
 * 
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OfflineInspect.CommonTools;
using OfflineInspect.Mongo;

namespace OfflineInspect.ReTransmission
{
    public class TcpRetransStaticsDocument
    {
        #region 维度
        public object _id;
        public string session_id;
        public string imsi;
        public string lac_ci;
        public string ip2_ttl_aggre;
        public string direction;
        #endregion
        #region 度量
        //流量计算
        public decimal? ip_total_aggre;
        public decimal? seq_total_aggre;
        public decimal? seq_total_reduce;
        //数量计算
        public int seq_total_count;
        public int seq_distinct_count;
        public int seq_repeat_cnt;
        //丢包和重传计算
        public decimal? seq_total_lost;
        public decimal? seq_total_repeat;
        //时间计算
        public double duration;
        #endregion
    }
    public class TcpRetransStatics : CommonToolx, IDisposable
    {
        private string mongo_collection = CommonAttribute.TcpRetransStatics[0];
        private string mongo_db = CommonAttribute.TcpRetransStatics[1];
        private string mongo_conn = CommonAttribute.TcpRetransStatics[2];

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
            TlliTcpSession tts = new TlliTcpSession();
            var query = from p in tts.mongo_tts.QueryMongo()
                        select new TcpRetransStaticsDocument
                        {
                            _id = p._id,

                            session_id=p.session_id,
                            imsi = p.imsi,
                            lac_ci = p.lac_ci,
                            ip2_ttl_aggre = p.ip2_ttl_aggre,
                            direction = p.direction,

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
