using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;
using System.Data.Objects;
//using System.Data.DataSetExtensions;


namespace MutliInterfaceGnGi
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public partial class Service1 : IService1
    {
        public DataSet GetCase()
        {
            Guangzhou_GnGiEntities gz = new Guangzhou_GnGiEntities();
            var query = from p in gz.GnGi_Get2x_Multi
                        select new { p.Accept, p.BeginFileNum };
            DataTable boundTable = query.ToDataTable();
            //DataTable boundTable = query.CopyToDataTable();
            DataSet ds = new DataSet();
            ds.Tables.Add(boundTable);
            return ds;
        }

        Guangzhou_GnGiEntities gz_gngi = new Guangzhou_GnGiEntities();

        public DataSet GetDataCollection(int value)
        {

            gz_gngi.ContextOptions.LazyLoadingEnabled = true;
            gz_gngi.GnGi_Get2x_Multi.MergeOption = MergeOption.NoTracking;

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
                    dt = viewTableDetail_All_FromCache(); break;
                case 6:
                    dt = statTableGnGiLost(); break;
                case 7:
                    dt = getTableAllRows(); break;
                default:
                    dt =  getTableRandownRows(); break;
            }
            ds.Tables.Add(dt);
            return ds;
        }

        private string viewTableTimeDuration()
        //private void navBarItem7_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            var maxt = gz_gngi.GnGi_Get2x_Multi.Max(e => e.PacketTime);
            var mint = gz_gngi.GnGi_Get2x_Multi.Min(e => e.PacketTime);
            TimeSpan ts = maxt.Value - mint.Value;
            var ttim = mint.Value.ToString() + "-" + maxt.Value.ToString() + "," + ts.TotalSeconds.ToString();
            //richTextBox1.Text = ttim.ToString();
            return ttim.ToString();
        }

        private DataTable compareGnGiTimeError()
        //private void navBarItem1_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            //clearColumns();
            var gnget = gz_gngi.GnGi_Get2x_Multi;
            var giget = gz_gngi.GnGi_Get2x_Multi;

            var get = from p in gnget.ToList()
                      join q in giget.ToList() on new { p.Request_URI, p.User_Agent, p.tcp_seq, p.tcp_nxtseq }
                      equals new { q.Request_URI, q.User_Agent, q.tcp_seq, q.tcp_nxtseq }

                      select new
                      {
                          gn_filenum = p.FileNum,
                          gn_packetnum = p.PacketNum,
                          gn_packettime = p.Get2x_time,
                          gi_filenum = q.FileNum,
                          gi_packetnum = q.PacketNum,
                          gi_packettime = q.Get2x_time,
                          dt = DateTime.Parse(p.Get2x_time) - DateTime.Parse(q.Get2x_time),
                          gn_ip2 = q.Dest_IP2,
                          gi_ip2 = p.Dest_IP2,
                          p.User_Agent,
                          p.Request_URI,
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
