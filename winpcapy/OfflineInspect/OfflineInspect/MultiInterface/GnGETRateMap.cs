
/*
 * 
 * 1.关于group by big table的问题？
 * 
 * 解决办法：分页->group by ->merge table(insert mongo->group by)
 * 
 * 
 * 2.关于linq to sql 不支持aggregate的问题？
 * 
 * 解决办法：group by ->tolist->aggregate?
 * 
 * 
 * 3.关于添加聚合索引和非聚合索引的问题？
 * 
 * 解决办法：1个聚合+99个非聚合，可以解决group by 、分页、where查询速度的问题
 * 
 * 
 * 4.Implementing IDisposable and the Dispose Pattern Properly
 * 
 * 
 * 5.上述办法可以解决大型数据集合挖掘的问题。
 * 
 * 
 * 2012.8.3日，weihp
 * 
 * */



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntitySqlTable.SqlServer.ip209.GuangZhou.GnGET;
using System.Data.Objects;
using OfflineInspect.CommonTools;
using OfflineInspect.Mongo;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.ComponentModel;

namespace OfflineInspect.MultiInterface
{
    public class GnGETRateMap : CommonToolx, IDisposable
    {
        public object _id;
        public RelationKeyWords rkey;
        public string itface;
        public string ip_proto;
        public DateTime? min_time;
        public DateTime? max_time;
        public int get_cnt;
        public double reponse_cnt;
        public double reponse_rate;
        public double? reponse_delay;

        //信令回放
        public string packetnum_aggre;
        //端口、分片、ttl、get_len等需要参与计算的聚合
        public string ip_segment_aggre;
        public string ip2_segment_aggre;
        public string tcp_segment_aggre;
        public string port_aggre;
        public string ttl_aggre;
        public int get_len;
        public int respone_len;

        private string mongo_collection = CommonAttribute.GnGETRateMap[0];
        private string mongo_db = CommonAttribute.GnGETRateMap[1];
        private string mongo_conn = CommonAttribute.GnGETRateMap[2];
        private string ip_itface = CommonAttribute.GnGETRateMap[3];
        private string ip_proto_tcp = CommonAttribute.GnGETRateMap[4];
        private string ip_proto_gtp = CommonAttribute.GnGETRateMap[5];

        private MongoCrud<GiGETRateMap> mongo_get;

        public GnGETRateMap()
        {
            mongo_get = new MongoCrud<GiGETRateMap>(mongo_conn, mongo_db, mongo_collection);
        }

        #region Implementing IDisposable and the Dispose Pattern Properly
        private bool disposed = false; // to detect redundant calls
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~GnGETRateMap()
        {
            Dispose(false);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Free other state (managed objects).
                }
                // Free your own state (unmanaged objects).
                // Set large fields to null.
                disposed = true;
            }
        }
        #endregion

        public void CreateCollection()
        {
            GuangZhou_GnGET gn = new GuangZhou_GnGET();
            gn.CommandTimeout = 0;
            gn.ContextOptions.LazyLoadingEnabled = true;
            gn.GnGiGw_Get2x.MergeOption = MergeOption.NoTracking;

            var query = from i in gn.GnGiGw_Get2x
                        where i.ip_proto == this.ip_proto_gtp  //gtp
                        group i by new
                        {
                            ip = i.ip2_dst_host,  //gtp
                            ip_id = i.ip2_id,  //gtp
                            i.tcp_seq,
                            i.tcp_nxtseq,
                            i.tcp_ack,
                            i.http_request_uri,
                            i.http_user_agent
                        }
                            into ttt
                            select new
                            {
                                rkey = ttt.Key,
                                min_time = ttt.Min(e => e.PacketTime),
                                max_time = ttt.Max(e => e.PacketTime),
                                get_cnt = ttt.Count(),
                                reponse_cnt = ttt.Where(e => e.Response != null).Count(),
                                reponse_rate = 1.0 * ttt.Where(e => e.Response != null).Count() / ttt.Count(),
                                reponse_delay = ttt.Where(e => e.Response != null).Average(e => e.Response_delayFirst),
                                packetnum_arr = ttt.Select(e => new { e.FileNum, e.PacketNum }),//这里返回的是集合                                
                            };

            //这里完成聚合？2012.8.3
            Parallel.ForEach(query.ToList(), q =>
            {
                GiGETRateMap get = new GiGETRateMap();
                get._id = GenerateId();
                get.rkey = new RelationKeyWords();
                get.rkey.ip = q.rkey.ip;
                get.rkey.ip_id = q.rkey.ip_id;
                get.rkey.tcp_seq = q.rkey.tcp_seq;
                get.rkey.tcp_nxtseq = q.rkey.tcp_nxtseq;
                get.rkey.tcp_ack = q.rkey.tcp_ack;
                get.rkey.http_request_uri = q.rkey.http_request_uri;
                get.rkey.http_user_agent = q.rkey.http_user_agent;
                get.itface = this.ip_itface;
                get.ip_proto = this.ip_proto_gtp;//gtp
                get.min_time = q.min_time;
                get.max_time = q.max_time;
                get.get_cnt = q.get_cnt;
                get.reponse_cnt = q.reponse_cnt;
                get.reponse_rate = q.reponse_rate;
                get.reponse_delay = q.reponse_delay;
                //信令回放
                get.packetnum_aggre = q.packetnum_arr
                    .Select(e => e.FileNum.ToString() + "-" + e.PacketNum.ToString()).Aggregate((a, b) => a + "," + b);//这里进行集合aggre
                //GET_col.Insert(get);
                mongo_get.MongoCol.Insert(get);
            });
        }
    }
}
