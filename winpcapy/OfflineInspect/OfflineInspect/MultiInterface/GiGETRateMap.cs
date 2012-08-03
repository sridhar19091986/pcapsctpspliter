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
using EntitySqlTable.SqlServer.ip209.GuangZhou.GiGET;
using System.Data.Objects;
using OfflineInspect.CommonTools;
using OfflineInspect.Mongo;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.ComponentModel;

namespace OfflineInspect.MultiInterface
{
    public class GiGETRateMap : CommonToolx, IDisposable
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

        private string mongo_db = "MultiInterface";
        private string mongo_collection = "GiGETRateMap";
        private string mongo_conn = "mongodb://192.168.4.209/?safe=true";

        private MongoCrud<GiGETRateMap> mongo_get;

        public GiGETRateMap()
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
        ~GiGETRateMap()
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

        private MongoCollection get_col = null;
        private MongoCollection GET_col
        {
            get
            {
                if (get_col == null)
                {
                    get_col = mongo_get.GetMongoCollection(true);
                }
                return get_col;
            }
            set
            {
                value = get_col;
            }
        }

        public IQueryable<GiGETRateMap> QueryMongo()
        {
            return mongo_get.QueryMongo();
        }

        public void CreateCollection()
        {
            CreateCollectionTCP();
            CreateCollectionGRE();
        }

        private string ip_itface = "Gi";
        private string ip_proto_tcp = "TCP";
        private string ip_proto_gre = "GRE";

        /*
         * 
         * ALTER TABLE [dbo].[GnGiGw_Get2x] ADD PRIMARY KEY CLUSTERED ([PacketNum] ASC,[FileNum] ASC)ON [PRIMARY]
         * 
         alter TABLE [Gi_Get2x_Multi]   alter  COLUMN [PacketNum] int  not null
        alter TABLE [Gi_Get2x_Multi]  alter  COLUMN [FileNum] int  not null
        alter table [Gi_Get2x_Multi] add  PRIMARY KEY (PacketNum,FileNum);
         *    
         * CREATE INDEX <name> ON <table> (KeyColList) INCLUDE (NonKeyColList) 
         *    
        CREATE  NONCLUSTERED  INDEX GnGiGw_Get2x_tcp 
        ON dbo.GnGiGw_Get2x (ip_dst_host,ip_id,tcp_seq,tcp_nxtseq,tcp_ack,http_request_uri,http_user_agent) 
        INCLUDE (PacketTime,Response,Response_delayFirst,PacketNum) 
         * 
         * 警告! 最大键长度为 900 个字节。索引 'GnGiGw_Get2x_gre' 的最大长度为 2800 个字节。对于某些大值组合，插入/更新操作将失败。
        * */

        private void CreateCollectionTCP()
        {
            GuangZhou_GiGET gi = new GuangZhou_GiGET();
            gi.CommandTimeout = 0;
            gi.ContextOptions.LazyLoadingEnabled = true;
            gi.GnGiGw_Get2x.MergeOption = MergeOption.NoTracking;

            var query = from i in gi.GnGiGw_Get2x
                        where i.ip_proto == this.ip_proto_tcp  //tcp
                        group i by new
                        {
                            ip = i.ip_dst_host,  //tcp
                            ip_id = i.ip_id,   //tcp
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
                get.ip_proto = this.ip_proto_tcp; //tcp
                get.min_time = q.min_time;
                get.max_time = q.max_time;
                get.get_cnt = q.get_cnt;
                get.reponse_cnt = q.reponse_cnt;
                get.reponse_rate = q.reponse_rate;
                get.reponse_delay = q.reponse_delay;
                //信令回放
                get.packetnum_aggre = q.packetnum_arr
                    .Select(e => e.FileNum.ToString() + "-" + e.PacketNum.ToString()).Aggregate((a, b) => a + "," + b);//这里进行集合aggre
                GET_col.Insert(get);
            });
        }

        /*
         *    
         * CREATE INDEX <name> ON <table> (KeyColList) INCLUDE (NonKeyColList) 
         *    
        CREATE  NONCLUSTERED  INDEX GnGiGw_Get2x_gre 
       ON dbo.GnGiGw_Get2x (ip2_dst_host,ip2_id,tcp_seq,tcp_nxtseq,tcp_ack,http_request_uri,http_user_agent) 
       INCLUDE (PacketTime,Response,Response_delayFirst,PacketNum) 
         * 
         * 警告! 最大键长度为 900 个字节。索引 'GnGiGw_Get2x_gre' 的最大长度为 2800 个字节。对于某些大值组合，插入/更新操作将失败。
        * */

        private void CreateCollectionGRE()
        {
            GuangZhou_GiGET gi = new GuangZhou_GiGET();
            gi.CommandTimeout = 0;
            gi.ContextOptions.LazyLoadingEnabled = true;
            gi.GnGiGw_Get2x.MergeOption = MergeOption.NoTracking;

            var query = from i in gi.GnGiGw_Get2x
                        where i.ip_proto == this.ip_proto_gre  //gre
                        group i by new
                        {
                            ip = i.ip2_dst_host,  //gre
                            ip_id = i.ip2_id,    //gre
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
                get.ip_proto = this.ip_proto_gre;//gre
                get.min_time = q.min_time;
                get.max_time = q.max_time;
                get.get_cnt = q.get_cnt;
                get.reponse_cnt = q.reponse_cnt;
                get.reponse_rate = q.reponse_rate;
                get.reponse_delay = q.reponse_delay;
                //信令回放
                get.packetnum_aggre = q.packetnum_arr
                    .Select(e => e.FileNum.ToString() + "-" + e.PacketNum.ToString()).Aggregate((a, b) => a + "," + b);//这里进行集合aggre
                GET_col.Insert(get);

            });
        }
    }
}
