/*
use  [GuangZhou_Gb]
go
alter TABLE [Gb_LLC_ReTransmission]   alter  COLUMN [PacketNum] int  not null
go
alter TABLE [Gb_LLC_ReTransmission]  alter  COLUMN [FileNum] int  not null
go
alter table [Gb_LLC_ReTransmission] add  PRIMARY KEY (PacketNum,FileNum);
 * */


/*
 * 
 * 
 * 本模块只完成llc重传？
 * 
 * 
 * 2012.8.9
 * 
 * 
 * **/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntitySqlTable.SqlServer.Local.Gb_LLC_ReTransmission;
using System.Data.Objects;
using OfflineInspect.Mongo;
using OfflineInspect.CommonTools;
using System.ComponentModel.DataAnnotations;

namespace OfflineInspect.ReTransmission
{
    public class TlliLLCSessionDocument
    {
        #region 给sqlserver虚构一个主键，good,ef5 code first,/2012.8.29
        [Key]
        public long tlsdID { get; set; }
        #endregion

        #region 维度
        public long _id;//字段不写入sqlserver
        public string session_id { get; set; }
        public string imsi { get; set; }
        public string lac_ci { get; set; }
        public string direction { get; set; }
        #endregion

        public string bsc_bvci { get; set; }
        public string mscbsc_ip_aggre { get; set; }
        public string llc_nu_aggre { get; set; }
        public string msg_aggre { get; set; }

        #region 度量
        public double duration { get; set; }
        public int llc_nu_count { get; set; }
        public int mscbsc_ip_count { get; set; }
        public int msg_distinct_count { get; set; }
        public Int16 llc_nu_discard { get; set; }//llc丢包用户比例
        #endregion

    }

    public class TlliLLCSession : CommonToolx, IDisposable
    {
        private string mongo_collection = CommonAttribute.TlliLLCSession[0];
        private string mongo_db = CommonAttribute.TlliLLCSession[1];
        private string mongo_conn = CommonAttribute.TlliLLCSession[2];
        private int maxfilenum = Int32.Parse(CommonAttribute.TlliLLCSession[3]);
        private int size = Int32.Parse(CommonAttribute.TlliLLCSession[4]);

        public MongoCrud<TlliLLCSessionDocument> mongo_tls;
        private GuangZhou_Gb_LLC_ReTransmission gb;

        public TlliLLCSession()
        {
            mongo_tls = new MongoCrud<TlliLLCSessionDocument>(mongo_conn, mongo_db, mongo_collection);
            gb = new GuangZhou_Gb_LLC_ReTransmission();
            gb.CommandTimeout = 0;
            gb.ContextOptions.LazyLoadingEnabled = true;
            gb.Gb_LLC_ReTransmission.MergeOption = MergeOption.NoTracking;
        }

        #region Implementing IDisposable and the Dispose Pattern Properly
        private bool disposed = false; // to detect redundant calls
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~TlliLLCSession()
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

        public void CreateCollection()
        {
            for (int j = 0; j < maxfilenum; j++)
                CreateCollection(j);
        }

        public void CreateCollection(int filenum)
        {
            var llcsession = gb.Gb_LLC_ReTransmission.Where(e => e.BeginFileNum == filenum);
            CreateTable("Up", llcsession, filenum);
            CreateTable("Down", llcsession, filenum);
        }

        public void CreateTable(string direction, IEnumerable<Gb_LLC_ReTransmission> gb_llc_retrans, int filenum)
        {
            int packet_cnt = gb_llc_retrans.Select(e => e.BeginFrameNum).Distinct().Count();

            int step = packet_cnt / size + 1;

            for (int i = 0; i < step; i++)
            {
                var tlli_session = gb_llc_retrans.Where(e => e.BeginFrameNum >= i * size && e.BeginFrameNum < (i + 1) * size);

                var tlli_sessions = tlli_session.ToLookup(e => e.BeginFrameNum);

                //tcp的会话过程
                foreach (var m in tlli_sessions)
                {
                    #region 会话过滤，filter
                    var gb_packet = m.Where(e => e.bssgp_direction == direction);
                    var pd_llc = gb_packet.Where(e => e.llcgprs_nu != null);//不包含nu编号的包
                    if (pd_llc.Count() == 0) continue;
                    #endregion

                    TlliLLCSessionDocument llcs = new TlliLLCSessionDocument();

                    #region tcp会话的基础信息，callid/imsi/lac/cell/bvci/duration/
                    llcs._id = GenerateId();
                    llcs.session_id = filenum.ToString() + "-" + m.Key.Value.ToString();
                    llcs.direction = direction;
                    llcs.imsi = m.Where(e => e.bssgp_imsi != null).Select(e => e.bssgp_imsi).FirstOrDefault();
                    var src = m.Select(e => e.ip_src_host);
                    var dst = m.Select(e => e.ip_dst_host);
                    var bscip = src.Union(dst);
                    llcs.mscbsc_ip_aggre = string.Join(",", bscip.Distinct());
                    llcs.mscbsc_ip_count = bscip.Distinct().Count();
                    //还需要1个？留意ip的问题?
                    llcs.bsc_bvci = m.Where(e => e.nsip_bvci != null).Select(e => Convert.ToString(e.nsip_bvci)).Distinct().Aggregate((a, b) => a + "," + b);
                    llcs.lac_ci = m.Where(e => e.bssgp_lac != null).Count() == 0 ? "" : m.Where(e => e.bssgp_lac != null).Select(e => Convert.ToString(e.bssgp_lac) + "-" + Convert.ToString(e.bssgp_ci)).Distinct().Aggregate((a, b) => a + "," + b);
                    //下行时延，包含3次握手，取这次会话的总长度吧。
                    TimeSpan? ts = m.Max(e => DateTime.Parse(e.LLC_time)) - pd_llc.Min(e => DateTime.Parse(e.LLC_time));
                    llcs.duration = ts.Value.TotalMilliseconds;
                    #endregion

                    #region 计算llc是否连续及llc_nu的数量
                    var nu = pd_llc.OrderBy(e => e.FileNum).ThenBy(e => e.PacketNum).Select(e => e.llcgprs_nu);
                    llcs.llc_nu_count = nu.Count();
                    llcs.llc_nu_aggre = nu.Select(e => Convert.ToString(e)).Aggregate((a, b) => a + "," + b);
                    Int16 continu = 0;
                    for (int j = 0; j < nu.Count() - 2; j++)
                    {
                        if (nu.ElementAt(j + 1) - nu.ElementAt(j) != 1)
                            if (nu.ElementAt(j + 1) - nu.ElementAt(j) != 0 - 511)
                            {
                                continu = 1; break;
                            }
                    }
                    llcs.llc_nu_discard = continu;
                    #endregion
                    llcs.msg_aggre = pd_llc.Select(e => e.LLC_MsgType).Distinct().Aggregate((a, b) => a + "," + b);                  
                    llcs.msg_distinct_count= pd_llc.Select(e => e.LLC_MsgType).Distinct().Count();
                    mongo_tls.MongoCol.Insert(llcs);
                }
            }

            string mess = string.Format("OK...direction...{0}...size:{1}...step:{2}...cnt:{3}",
                direction, size, step, packet_cnt);
            Console.WriteLine(mess);
        }
    }
}
