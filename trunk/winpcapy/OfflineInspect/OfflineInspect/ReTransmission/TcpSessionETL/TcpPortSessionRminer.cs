/*
 * 
 * 数据ETL送给SSAS？
 * 
 * 1.OLAP
 * 2.Model
 * 
 * 
 * 数据ETL送给R data miner？
 * 
 * 1.rattle
 * 
数据可视化超强
数据是否能描述事物的特征？
如果不能，如何进行数据的抽取和转换，提取特征数据？
数据归一化和数据质量等问题
 * 
 * 
 * */

/*
 * 
 * 把数据写入宽表进行数值分析
 * 
 * 分别写入sqlserver和csv，
 * 
 * 考虑使用sqlserver的ETL，SSIS可能会比价快
 * 
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OfflineInspect.ReTransmission.MapReduce;

namespace OfflineInspect.ReTransmission.TcpSessionETL
{
    public class TcpPortSessionRminer
    {

        public void a()
        {
            TcpPortSessionETL trs = new TcpPortSessionETL();
            foreach (var tcp in trs.mongo_TcpPortSessionETL.QueryMongo())
            {
            }
        }
        public void b()
        {
        }

    }
}
