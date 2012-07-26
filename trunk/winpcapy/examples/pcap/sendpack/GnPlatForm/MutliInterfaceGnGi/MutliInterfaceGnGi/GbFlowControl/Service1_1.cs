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

using EntityClass.ServerEntity.Gi;
using EntityClass.ServerEntity.Gw;
using EntityClass.ServerEntity.Gn;
using EntityClass.ServerEntity.Other;
using EntityClass.ServerEntity.Gb;




namespace MutliInterfaceGnGi
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public partial class Service1 : IService1
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

        GuangZhou_GnGiEntities gz_gngi = new GuangZhou_GnGiEntities();
        GuangZhou_GiEntities gz_gi = new GuangZhou_GiEntities();
        GuangZhou_GnGiEntities gz_giw = new GuangZhou_GnGiEntities();
        GuangZhou_GnGiEntities gz_gn = new GuangZhou_GnGiEntities();
        Guangzhou_GbEntities gz_gb = new Guangzhou_GbEntities();

       
        //Guangzhou_GnGiEntities gz_gngi = new Guangzhou_GnGiEntities();
        //GuangZhou_GiEntities1 gz_gi = new GuangZhou_GiEntities1();
        //GuangZhou_GiwEntities gz_giw = new GuangZhou_GiwEntities();
        //GuangZhou_GnEntities gz_gn = new GuangZhou_GnEntities();
        //GuangZhou_GbEntities gz_gb = new GuangZhou_GbEntities();

        string tcp = "TCP";
        string gre = "GRE";

        private void EntitySetLazy()
        {
            gz_gb.CommandTimeout = 0;
            gz_gb.ContextOptions.LazyLoadingEnabled = true;

            gz_gn.CommandTimeout = 0;
            gz_gn.ContextOptions.LazyLoadingEnabled = true;

            gz_gi.CommandTimeout = 0;
            gz_gi.ContextOptions.LazyLoadingEnabled = true;

            gz_giw.CommandTimeout = 0;
            gz_giw.ContextOptions.LazyLoadingEnabled = true;
        }
        private void EntityLoadCache_Gb()
        {
            gz_gb.Gb_FlowControly.MergeOption = MergeOption.NoTracking;
            //gz_gb.Gb_FlowControly.Load();
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

            //EntityLoadCache_Gb();

            //EntityLoadCache_Syn();

            DataSet ds = new DataSet();

            DataTable dt = new DataTable();

            switch (value)
            {

                case 5:
                    dt = viewTableDetail_gb(); break;
                case 6:
                    dt = viewTableDetail_All_gb(); break;
                case 7:
                    dt = statTableDetail_All_gb(); break;
                default:
                    dt = getTableRandownRows(); break;
            }
            ds.Tables.Add(dt);
            return ds;
        }

        private string viewTableTimeDuration()
        //private void navBarItem7_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            EntitySetLazy();
            var maxt = gz_gb.Gb_FlowControly.Max(e => e.PacketTime);
            var mint = gz_gb.Gb_FlowControly.Min(e => e.PacketTime);
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
