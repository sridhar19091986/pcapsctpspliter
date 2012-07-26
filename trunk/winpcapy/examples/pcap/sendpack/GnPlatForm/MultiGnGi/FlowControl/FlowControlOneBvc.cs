/*
 * 
 where Flow_Control_MsgType='BSSGP.FLOW-CONTROL-BVC'
 and (nsip_bvci=39 or nsip_bvci=87) and ip_src_host='10.128.24.84'
 order by FileNum
 * 
 * */



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Linq;
using MongoDB.Driver.Linq;
using EntityClass.ServerEntity.Gb;
using EntityClass.ServerEntity.Gb_209;
using System.Data.Objects;
using System.Threading.Tasks;
using System.Configuration;

namespace MultiGnGi
{
    public class LacCellBvci
    {
        public object _id;
        public string src;
        public string dst;
        public string bvci;
        public string lac_cell;


        private string mongodb_collection_name = "LacCellBvci";
        //private string mongo_conn = "mongodb://192.168.4.209/?safe=true";
        private string mongo_conn = ConfigurationManager.AppSettings["LocalMongo"].ToString();


        /*
         * http://eason132.iteye.com/blog/234821
         * 生成数字型唯一ID代码： 
         * 
         * **/
        private long GenerateId()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }

        public void CreatCollection()
        {
            GuangZhou_GbEntities_209 gb = new GuangZhou_GbEntities_209();
            gb.CommandTimeout = 0;
            gb.ContextOptions.LazyLoadingEnabled = true;
            gb.Gb_FlowControly.MergeOption = MergeOption.NoTracking;
            var fc = from p in gb.Gb_FlowControly
                     group p by new
                     {
                         p.bssgp_lac,
                         p.bssgp_ci,
                         p.nsip_bvci,
                         p.ip_src_host,
                         p.ip_dst_host,
                     } into ttt
                     select new
                     {
                         ttt.Key.bssgp_lac,
                         ttt.Key.bssgp_ci,
                         ttt.Key.nsip_bvci,
                         ttt.Key.ip_src_host,
                         ttt.Key.ip_dst_host,
                         cnt = ttt.Count()
                     };

            var bv = from p in fc.ToList()
                     where p.nsip_bvci != null
                     where p.bssgp_ci != null
                     where p.bssgp_lac != null
                     where p.ip_src_host != null
                     where p.ip_dst_host != null
                     select new LacCellBvci
                     {
                         _id = GenerateId(),
                         lac_cell = p.bssgp_lac.ToString() + "-" + p.bssgp_ci.ToString(),
                         src = p.ip_src_host,
                         dst = p.ip_dst_host,
                         bvci = p.nsip_bvci.ToString(),
                     };

            BulkMongo(bv.ToList());
        }

        public void BulkMongo(List<LacCellBvci> lcb)
        {



            var collection = MongoConn.GetMongoCollection(mongo_conn, mongodb_collection_name, true);

            collection.InsertBatch(lcb);
        }

        public IQueryable<LacCellBvci> QueryMongo()
        {

            var collection = MongoConn.GetMongoCollection(mongo_conn, mongodb_collection_name, false);

            var query = from p in collection.AsQueryable<LacCellBvci>()
                        select p;


            return query;
        }

        private List<LacCellBvci> listlaccellbvci = null;
        public List<LacCellBvci> ListLacCellBvci
        {
            get
            {
                if (listlaccellbvci == null)
                {
                    var collection = MongoConn.GetMongoCollection(mongo_conn, mongodb_collection_name, false);

                    listlaccellbvci = collection.AsQueryable<LacCellBvci>().ToList();

                }

                return listlaccellbvci;

            }
            set
            {
                value = listlaccellbvci;
            }
        }

        public string GetLacCell(string src, string dst, string bvci)
        {

            var query = from p in ListLacCellBvci
                        where (p.src == src && p.dst == dst && p.bvci == bvci) || (p.dst == src && p.src == dst && p.bvci == bvci)
                        select p;

            return query.Select(e => e.lac_cell).FirstOrDefault();
        }
    }

    public class FlowControlOneBvc
    {
        public object _id;
        //public int FileNum;
        //public int PacketNum;
        public DateTime Flow_Control_time;
        public string lac_cell;
        public string Flow_Control_MsgType;
        public int ip_len;
        public string tlli;
        public string bssgp_direction;
        public double bssgp_bmax_default_ms;
        public int? bssgp_bucket_full_ratio;
        public double bssgp_bucket_leak_rate;
        public double bssgp_bvc_bucket_size;
        public double bssgp_ms_bucket_size;
        public double bssgp_R_default_ms;
        public string bvci;

        //private string mongo_conn = "mongodb://localhost/?safe=true";
        //private string mongo_conn = "mongodb://192.168.4.209/?safe=true";

        private string mongodb_collection_name = "FlowControlOneBvc";
        //private string mongo_conn = "mongodb://192.168.4.209/?safe=true";
        private string mongo_conn = ConfigurationManager.AppSettings["RemoteMongo"].ToString();

        public void CreateCollection()
        {
            for (int j = 0; j < 3; j++)
                CreateCollection(j);
            //for (int i = 0; i < 5000; i++)
            //CreateCollection(i, 1000, i);
        }

        public void CreateCollection(int filenum)
        //public void CreateCollection(int step, int bulksize,int filenum)
        {
            GuangZhou_GbEntities_209 gb = new GuangZhou_GbEntities_209();
            gb.CommandTimeout = 0;
            gb.ContextOptions.LazyLoadingEnabled = true;
            gb.Gb_FlowControly.MergeOption = MergeOption.NoTracking;
            LacCellBvci lcb = new LacCellBvci();
            //List<FlowControlOneBvc> fcob_list = new List<FlowControlOneBvc>();
            //查询
            var fc = from p in gb.Gb_FlowControly.Where(e => e.FileNum == filenum)
                     //.OrderBy(e => e.PacketNum ).Skip(step * bulksize).Take(bulksize)
                     //var fc = from p in gb.Gb_FlowControly
                     select new
                     {
                         p.FileNum,
                         p.PacketNum,
                         p.bssgp_tlli,
                         p.Flow_Control_time,
                         p.ip_src_host,
                         p.ip_dst_host,
                         p.nsip_bvci,
                         p.Flow_Control_MsgType,
                         p.ip_len,
                         p.bssgp_direction,
                         p.bssgp_bmax_default_ms,
                         p.bssgp_bucket_full_ratio,
                         p.bssgp_bucket_leak_rate,
                         p.bssgp_bvc_bucket_size,
                         p.bssgp_ms_bucket_size,
                         p.bssgp_R_default_ms,
                     };

            Parallel.ForEach(fc, p =>
            //foreach (var p in fc.AsParallel().ToList())
            {
                FlowControlOneBvc fcob = new FlowControlOneBvc();
                fcob._id = p.FileNum * 100000000 + p.PacketNum;
                //fcob.FileNum = p.FileNum;
                //fcob.PacketNum = p.PacketNum;
                fcob.tlli = p.bssgp_tlli;
                fcob.Flow_Control_time = DateTime.Parse(p.Flow_Control_time);
                fcob.lac_cell = lcb.GetLacCell(p.ip_src_host, p.ip_dst_host, p.nsip_bvci.ToString());
                fcob.bvci = p.nsip_bvci.ToString();
                fcob.Flow_Control_MsgType = p.Flow_Control_MsgType;
                fcob.ip_len = (int)p.ip_len;
                fcob.bssgp_direction = p.bssgp_direction;
                fcob.bssgp_bmax_default_ms = Convert.ToDouble(p.bssgp_bmax_default_ms) / 1000.0;
                fcob.bssgp_bucket_full_ratio = p.bssgp_bucket_full_ratio;
                fcob.bssgp_bucket_leak_rate = Convert.ToDouble(p.bssgp_bucket_leak_rate) / 1000.0;
                fcob.bssgp_bvc_bucket_size = Convert.ToDouble(p.bssgp_bvc_bucket_size) / 1000.0;
                fcob.bssgp_ms_bucket_size = Convert.ToDouble(p.bssgp_ms_bucket_size) / 1000.0;
                fcob.bssgp_R_default_ms = Convert.ToDouble(p.bssgp_R_default_ms) / 1000.0;

                //Task.Factory.StartNew(() => FCOB_col.Insert(fcob));
                //fcob_list.Add(fcob);
                FCOB_col.Insert(fcob);
                //}
            });

            /*
             * 
             * tolist 修改成  asienumble,预计可以解决内存不足的问题
             * */

            //BulkMongo(fcob_list);
            //fcob_list.Clear();
            //var fclook = fc.AsParallel().ToList();

            //var fcl = from p in fclook.AsParallel()
            //          select new FlowControlOneBvc
            //          {
            //              _id = p.PacketNum,
            //              PacketNum = p.PacketNum,
            //              Flow_Control_time = DateTime.Parse(p.Flow_Control_time),
            //              lac_cell = lcb.GetLacCell(p.ip_src_host, p.ip_dst_host, p.nsip_bvci.ToString()),
            //              Flow_Control_MsgType = p.Flow_Control_MsgType,
            //              ip_len = (int)p.ip_len,
            //              bssgp_direction = p.bssgp_direction,
            //              bssgp_bmax_default_ms = Convert.ToDouble(p.bssgp_bmax_default_ms) / 1000.0,
            //              bssgp_bucket_full_ratio = p.bssgp_bucket_full_ratio,
            //              bssgp_bucket_leak_rate = Convert.ToDouble(p.bssgp_bucket_leak_rate) / 1000.0,
            //              bssgp_bvc_bucket_size = Convert.ToDouble(p.bssgp_bvc_bucket_size) / 1000.0,
            //              bssgp_ms_bucket_size = Convert.ToDouble(p.bssgp_ms_bucket_size) / 1000.0,
            //              bssgp_R_default_ms = Convert.ToDouble(p.bssgp_R_default_ms) / 1000.0,
            //          };

            //BulkMongo(fcl.AsParallel().ToList());
        }

        private MongoCollection fcob_col = null;
        private MongoCollection FCOB_col
        {
            get
            {
                if (fcob_col == null)
                {

                    fcob_col = MongoConn.GetMongoCollection(mongo_conn, mongodb_collection_name, true);//这个是提供给写操作的
                }
                return fcob_col;
            }
            set
            {

                value = fcob_col;
            }
        }

        public void BulkMongo(List<FlowControlOneBvc> fcob)
        {

            var collection = MongoConn.GetMongoCollection(mongo_conn, mongodb_collection_name, true);

            collection.InsertBatch(fcob);
        }

        public IQueryable<FlowControlOneBvc> QueryMongo()
        {
            var collection = MongoConn.GetMongoCollection(mongo_conn, mongodb_collection_name, false);

            var query = from p in collection.AsQueryable<FlowControlOneBvc>()
                        select p;


            return query;
        }
    }

    public class FlowControlMapBvc
    {
        // public int FileNum;
        // public int PacketNum;
        // public DateTime Flow_Control_time;
        public object _id;
        public string lac_cell;
        public string bvci;
        public int fcb_cnt;
        public int packet_cnt;
        public int tlli_cnt;
        // public string Flow_Control_MsgType;
        // public int ip_len;
        // public string bssgp_direction;
        public double bssgp_bmax_default_ms;
        public double? bssgp_bucket_full_ratio;
        public string bssgp_bucket_leak_rate;
        public string bssgp_bvc_bucket_size;
        public double bssgp_ms_bucket_size;
        public double bssgp_R_default_ms;
        // public int? BeginFrameNum; 
        public double ms_bucket_size;
        public double ms_leak_rate;
        //public int lac;
        //public int ci;
        public string tlli_distinct_aggre;
        public string msg_distinct_aggre;
        // public double bucket_size_avg;
        // public double bucket_size_min;
        // public double bucket_size_max;
        // public double leak_rate_avg;
        // public double leak_rate_min;
        // public double leak_rate_max;
        public string fcb_delay_aggre;
        // public string leak_rate;
        // public string bucket_size;
        public string down_total_len;
        public string down_packet_rate;
        public string fcb_time_aggre;

        //private string mongo_conn = "mongodb://localhost/?safe=true";
        //private string mongo_conn = "mongodb://192.168.4.209/?safe=true";

        private string mongodb_collection_name = "FlowControlMapBvc";
        //private string mongo_conn = "mongodb://192.168.4.209/?safe=true";
        private string mongo_conn = ConfigurationManager.AppSettings["LocalMongo"].ToString();

        public void BulkMongo(List<FlowControlMapBvc> fcmb)
        {
            var collection = MongoConn.GetMongoCollection(mongo_conn, mongodb_collection_name, true);

            collection.InsertBatch(fcmb);
        }

        public IQueryable<FlowControlMapBvc> QueryMongo()
        {

            var collection = MongoConn.GetMongoCollection(mongo_conn, mongodb_collection_name, false);

            var query = from p in collection.AsQueryable<FlowControlMapBvc>()
                        select p;


            return query;
        }
    }
}
