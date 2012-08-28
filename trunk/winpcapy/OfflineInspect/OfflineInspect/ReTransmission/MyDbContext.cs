/*
 * 
 * 主要用户从mongo导出到sqlserver
 * 
 * */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Infrastructure;
using System.Data.Services;
using System.Data.EntityClient;
using System.Data.SqlClient;
//using System.Linq;

namespace OfflineInspect.ReTransmission
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(SqlConnection connection) : base(connection) { }
        public DbSet<TcpRetransStaticsTable> TcpRetransStaticsSet { get; set; }
        public IEnumerable<TcpRetransStaticsTable> getTcpRetransStaticsTableRow()
        {
            TcpRetransStatics trs = new TcpRetransStatics();
            var query = from p in trs.mongo_TcpRetransStatics.QueryMongo()
                        select new TcpRetransStaticsTable
                        {
                            abcId = (Int64)p._id,
                            session_id = p.session_id,
                            ip_total_aggre = p.ip_total_aggre,
                            seq_total_aggre = p.seq_total_aggre,
                            seq_total_reduce = p.seq_total_reduce,
                            seq_total_lost = p.seq_total_lost,
                            seq_total_repeat = p.seq_total_repeat,
                            duration = p.duration,
                            seq_total_count = p.seq_total_count,
                            seq_distinct_count = p.seq_distinct_count,
                            seq_repeat_cnt = p.seq_repeat_cnt,
                            //abcId = p.abcId,
                            //session_id = p.session_id,
                            imsi = p.imsi,
                            lac_ci = p.lac_ci,
                            ip2_ttl_aggre = p.ip2_ttl_aggre,
                            direction = p.direction,

                        };
            foreach (var tcp in query)
                yield return tcp;
        }

        public DbSet<TlliLLCSessionTable> TlliLLCSessionSet { get; set; }
        public IEnumerable<TlliLLCSessionTable> getTlliLLCSessionTableRow()
        {
            TlliLLCSession tls = new TlliLLCSession();
            var query = from p in tls.mongo_tls.QueryMongo()
                        select new TlliLLCSessionTable
                        {
                            abcId =(Int64)p._id,
                            bsc_bvci = p.bsc_bvci,
                            direction = p.direction,
                            duration = p.duration,
                            imsi = p.imsi,
                            lac_ci = p.lac_ci,
                            llc_nu_aggre = p.llc_nu_aggre,
                            llc_nu_count = p.llc_nu_count,
                            llc_nu_discard = p.llc_nu_discard,
                            mscbsc_ip_aggre = p.mscbsc_ip_aggre,
                            mscbsc_ip_count = p.mscbsc_ip_count,
                            msg_aggre = p.msg_aggre,
                            session_id = p.session_id,
                        };
            foreach (var llc in query)
                yield return llc;
        }
    }
}
