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

namespace OfflineInspect.MultiInterface
{
    public class GiGETRateMap : CommonToolx
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
        public string packetnum_aggre;

        private string mongo_db = "MultiInterface";
        private string mongo_collection = "GiGETRateMap";
        private string mongo_conn = "mongodb://192.168.4.209/?safe=true";

        private MongoCrud<GiGETRateMap> mongo_get;

        public GiGETRateMap()
        {
            mongo_get = new MongoCrud<GiGETRateMap>(mongo_conn, mongo_db, mongo_collection);
        }

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
            //CreateCollectionGRE();
        }

        private string ip_itface = "Gi";
        private string ip_proto_tcp = "TCP";
        private string ip_proto_gre = "GRE";

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
                            id = i.ip_id,   //tcp
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
                                itface = this.ip_itface,
                                ip_proto = this.ip_proto_tcp,  //tcp
                                min_time = ttt.Min(e => e.PacketTime),
                                max_time = ttt.Max(e => e.PacketTime),
                                get_cnt = ttt.Count(),
                                reponse_cnt = ttt.Where(e => e.Response != null).Count(),
                                reponse_rate = 1.0 * ttt.Where(e => e.Response != null).Count() / ttt.Count(),
                                reponse_delay = ttt.Where(e => e.Response != null).Average(e => e.Response_delayFirst),
                                packetnum_arr = ttt.Select(e => e.PacketNum),
                            };

            //这里完成聚合？2012.8.2
            Parallel.ForEach(query.ToList(), q =>
            {
                GiGETRateMap get = new GiGETRateMap();
                get._id = GenerateId();
                get.rkey = new RelationKeyWords();
                get.rkey.ip = q.rkey.ip;
                get.get_cnt = q.get_cnt;
                get.packetnum_aggre = q.packetnum_arr.Select(e => e.ToString()).Aggregate((a, b) => a + "," + b);
                GET_col.Insert(get);
            });
        }

        private void CreateCollectionGRE()
        {
            GuangZhou_GiGET gi = new GuangZhou_GiGET();
            gi.ContextOptions.LazyLoadingEnabled = true;
            gi.GnGiGw_Get2x.MergeOption = MergeOption.NoTracking;

            var query = from i in gi.GnGiGw_Get2x
                        where i.ip_proto == this.ip_proto_gre  //gre
                        group i by new
                        {
                            ip = i.ip2_dst_host,  //gre
                            id = i.ip2_id,    //gre
                            i.tcp_seq,
                            i.tcp_nxtseq,
                            i.tcp_ack,
                            i.http_request_uri,
                            i.http_user_agent
                        }
                            into ttt
                            select new GiGETRateMap
                            {
                                rkey = new RelationKeyWords
                                {
                                    ip = ttt.Key.ip,
                                    ip_id = ttt.Key.id,
                                    tcp_seq = ttt.Key.tcp_seq,
                                    tcp_nxtseq = ttt.Key.tcp_nxtseq,
                                    tcp_ack = ttt.Key.tcp_ack,
                                    http_request_uri = ttt.Key.http_request_uri,
                                    http_user_agent = ttt.Key.http_user_agent
                                },
                                itface = this.ip_itface,
                                ip_proto = this.ip_proto_gre,  //gre
                                min_time = ttt.Min(e => e.PacketTime),
                                max_time = ttt.Max(e => e.PacketTime),
                                get_cnt = ttt.Count(),
                                reponse_cnt = ttt.Where(e => e.Response != null).Count(),
                                reponse_rate = 1.0 * ttt.Where(e => e.Response != null).Count() / ttt.Count(),
                                reponse_delay = ttt.Where(e => e.Response != null).Average(e => e.Response_delayFirst),
                            };
            Parallel.ForEach(query.AsEnumerable(), get =>
            {
                get._id = GenerateId();
                GET_col.Insert(get);
            });
        }
    }
}
