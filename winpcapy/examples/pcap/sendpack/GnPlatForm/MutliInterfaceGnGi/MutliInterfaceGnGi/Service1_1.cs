using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;
using System.Data.Objects;
using System.Data.Entity;
using System.Data.Entity.Design;
using EntityFramework.Extensions;
using EntityFramework.Caching;
using EntityFramework.Batch;

using MutliInterfaceGnGi.ServerEntity.Gi;
using MutliInterfaceGnGi.ServerEntity.Gw;
using MutliInterfaceGnGi.ServerEntity.Gn;
using MutliInterfaceGnGi.ServerEntity.Other;


/*

namespace MutliInterfaceGnGi
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class MulitiInterfaceLostPacket:Service1

    {
        public DataSet GetCase()
        {
            Guangzhou_GnGiEntities gz = new Guangzhou_GnGiEntities();
            var query = from p in gz.GnGiGw_Http_Any_Multi
                        select new { p.DumpFor, p.BeginFileNum };
            DataTable boundTable = query.ToDataTable();
            //DataTable boundTable = query.CopyToDataTable();
            DataSet ds = new DataSet();
            ds.Tables.Add(boundTable);
            return ds;
        }

        Guangzhou_GnGiEntities gz_gngi = new Guangzhou_GnGiEntities();
        GuangZhou_GiEntities1 gz_gi = new GuangZhou_GiEntities1();
        GuangZhou_GiwEntities gz_giw = new GuangZhou_GiwEntities();
        GuangZhou_GnEntities gz_gn = new GuangZhou_GnEntities();

        string tcp = "TCP";
        string gre = "GRE";

        private void EntitySetLazy()
        {
            gz_gn.CommandTimeout = 0;
            gz_gn.ContextOptions.LazyLoadingEnabled = true;

            gz_gi.CommandTimeout = 0;
            gz_gi.ContextOptions.LazyLoadingEnabled = true;

            gz_giw.CommandTimeout = 0;
            gz_giw.ContextOptions.LazyLoadingEnabled = true;
        }
        private void EntityLoadCache_Get()
        {
            gz_gi.GnGiGw_Get2x.MergeOption = MergeOption.NoTracking;
            gz_gi.GnGiGw_Get2x.Load();
            gz_gi.RawFileList.Load();

            gz_gn.GnGiGw_Get2x.MergeOption = MergeOption.NoTracking;
            gz_gn.GnGiGw_Get2x.Load();
            gz_gn.RawFileList.Load();

            gz_giw.GnGiGw_Get2x.MergeOption = MergeOption.NoTracking;
            gz_giw.GnGiGw_Get2x.Load();
        }
        private void EntityLoadCache_Syn()
        {


            gz_gn.GnGiGw_Syn.MergeOption = MergeOption.NoTracking;
            gz_gi.GnGiGw_Syn.MergeOption = MergeOption.NoTracking;
            gz_giw.GnGiGw_Syn.MergeOption = MergeOption.NoTracking;
            gz_gn.GnGiGw_Syn.Load();
            gz_gi.GnGiGw_Syn.Load();
            gz_giw.GnGiGw_Syn.Load();


        }

        public DataSet GetDataCollection(int value)
        {
            EntitySetLazy();

            EntityLoadCache_Syn();

            DataSet ds = new DataSet();

            DataTable dt = new DataTable();

            switch (value)
            {
                case 1:
                    dt = compareGnGiTimeError(); break;
                case 2:
                    dt = viewTableDetail(); break;
                case 3:
                    dt = viewTableDetail_ip2_id(); break;
                case 4:
                    dt = viewTableDetail_All(); break;
                case 5:
                    dt = viewKeyKPI_gn_FromCache(); break;
                case 6:
                    dt = viewKeyKPI_gi_FromCache(); break;
                case 7:
                    dt = viewKeyKPI_giw_FromCache(); break;
                case 8:
                    dt = viewKeyKPI_gi_from_giw_FromCache(tcp); break;
                case 9:
                    dt = viewKeyKPI_gn_from_giw_FromCache(tcp); break;
                case 10:
                    dt = viewKeyKPI_giw_from_giw_FromCache(tcp); break;
                case 11:
                    dt = viewKeyKPI_gi_from_giw_FromCache(gre); break;
                case 12:
                    dt = viewKeyKPI_gn_from_giw_FromCache(gre); break;
                case 13:
                    dt = viewKeyKPI_giw_from_giw_FromCache(gre); break;

                    //
                case 100:
                    dt = viewKPI_upLost_FromCache(gre);break;
                case 99:
                    dt = viewKPI_upLost_FromCache(tcp);break;

                case 14:
                    dt = viewLost_gn_from_giw_FromCache(tcp); break;
                case 15:
                    dt = viewLost_gi_from_giw_FromCache(tcp); break;
                case 16:
                    dt = viewLost_gn_from_giw_FromCache(gre); break;
                case 17:
                    dt = viewLost_gi_from_giw_FromCache(gre); break;
                case 18:

                    //
                    dt = viewAll_gigngw_from_giw_FromCache(); break;
                case 55:
                    dt = viewTableDetail_All_FromCache(); break;
                case 59:
                    dt = viewTableDetail_All_FromCache_simple(); break;
                case 58:
                    dt = viewTableDetail_All_ToLookup(); break;
                case 56:
                    dt = statTableGnGiLost(); break;
                case 57:
                    dt = getTableAllRows(); break;
                default:
                    dt = getTableRandownRows(); break;
            }
            ds.Tables.Add(dt);
            return ds;
        }

        private string viewTableTimeDuration()
        //private void navBarItem7_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            var maxt = gz_gngi.GnGiGw_Http_Any_Multi.FromCache().Max(e => e.PacketTime);
            var mint = gz_gngi.GnGiGw_Http_Any_Multi.FromCache().Min(e => e.PacketTime);
            TimeSpan ts = maxt.Value - mint.Value;
            var ttim = mint.Value.ToString() + "-" + maxt.Value.ToString() + "," + ts.TotalSeconds.ToString();
            //richTextBox1.Text = ttim.ToString();
            return ttim.ToString();
        }

        private DataTable compareGnGiTimeError()
        //private void navBarItem1_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            //clearColumns();
            var gnget = gz_gngi.GnGiGw_Http_Any_Multi.FromCache();
            var giget = gz_gngi.GnGiGw_Http_Any_Multi.FromCache();

            var get = from p in gnget.ToList()
                      join q in giget.ToList() on new { p.http_request_uri, p.http_user_agent, p.tcp_seq, p.tcp_nxtseq }
                      equals new { q.http_request_uri, q.http_user_agent, q.tcp_seq, q.tcp_nxtseq }

                      select new
                      {
                          gn_filenum = p.FileNum,
                          gn_packetnum = p.PacketNum,
                          gn_packettime = p.Http_Any_time,
                          gi_filenum = q.FileNum,
                          gi_packetnum = q.PacketNum,
                          gi_packettime = q.Http_Any_time,
                          dt = DateTime.Parse(p.Http_Any_time) - DateTime.Parse(q.Http_Any_time),
                          gn_ip2 = q.ip2_dst_host,
                          gi_ip2 = p.ip2_dst_host,
                          p.http_user_agent,
                          p.http_request_uri,
                          p.tcp_seq,
                          p.tcp_nxtseq,
                      };

            var dborder = get.OrderBy(e => e.gn_filenum).ThenBy(e => e.gn_packetnum);

            return dborder.ToDataTable();
            //gridControl1.DataSource = dborder.ToList();

            //formatColumns(1);
        }
    }
}
*/