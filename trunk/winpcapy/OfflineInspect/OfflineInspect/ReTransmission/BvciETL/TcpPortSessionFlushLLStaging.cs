/*
 * 以小区切换作为ETL的最小粒度？
 *  
 * 
 * 替换以tcp会话这个维度。
 * 
 * 如何按照bvci序列分割tcp会话
 * 
 * 
 * 进行集合分裂，多级树，tcp会话按照bvci分裂成子树，
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntitySqlTable.SqlServer.ip249.tcp_data;
using System.Threading.Tasks;
using OfflineInspect.CommonTools;

namespace OfflineInspect.ReTransmission.MapReduce
{
    public class TcpPortSessionFlushLLStaging
    {
        private int size = Int32.Parse(CommonAttribute.TcpPortSessionFlushLLStaging[5]);

        public void CreateTable(string[] directions, IQueryable<Gb_TCP_ReTransmission> gb_tcp_retrans, int filenum, int step)
        {
            //执行帧号分页
            for (int i = 0; i < step; i++)
            //Parallel.For(0, step, (i) =>
            {
                IQueryable<Gb_TCP_ReTransmission> iq_tcp_session = gb_tcp_retrans
                    .Where(e => e.BeginFrameNum >= i * size && e.BeginFrameNum < (i + 1) * size);

                var tcp_sessions = iq_tcp_session.ToLookup(e => e.BeginFrameNum);

                Console.Write("step...{0},", i);

                foreach (var direction in directions)
                    //帧号分页中的每个tcp的会话过程
                    //foreach (var m in tcp_sessions)
                    Parallel.ForEach(tcp_sessions, (per_session) =>
                    {
                        var BvciSequenceCount = per_session.IEnumBvciSequenceCollectionIndex();
                        var BvciSequenceCollection = per_session.IEnumBvciSequenceCollection(BvciSequenceCount.ToArray());
                        foreach (var bvciSequence in BvciSequenceCollection)
                        {
                            var bvciSequenceIndex = bvciSequence.Key;
                            var bvciSession = bvciSequence.Value;

                            /*
                             * 按照TcpPortSessionStaging.cs。
                             * 
                             * 实现把list<Gb_TCP_ReTransmission>写入mongodb。
                             * 
                             * 存在比较复杂的聚合逻辑。
                             * 
                             * */
                        }
                    });
            }
        }
    }
}
