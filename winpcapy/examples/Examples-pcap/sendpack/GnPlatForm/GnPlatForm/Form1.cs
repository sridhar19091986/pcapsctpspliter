/*
 *

业务请求成功率低－定位异常SP或WAP网关
问题描述：如HTTP业务成功率偏低，存在较大比例下行连接重置等问题和其他错误码；
初步定位： 怀疑SP或网关性能问题；
初步计划：
1、分析总体http、WAP业务连接成功率指标，并分析从总体时延和错误原因
2、按升序分析TOP 100的SP的成功率、时延，寻找业务最差的SP或WAP网关(修改成按照top10的错误代码分析）
3、增值室配合定位错误码和部分业务侧异常问题


SELECT * FROM [GuangZhou_Gn].[dbo].[mpos_gn_common]
  --where my_URI_Main='qq.com' and Abnormal_reason=4906
where my_URI_Main='qq.com' and my_Abnormal_reason is null and my_URI_Main_header='ebook12'

SELECT * FROM [GuangZhou_Gn].[dbo].[mpos_gn_common]
 where my_URI_Main='qq.com' and Abnormal_reason=4901 and my_DEST_IP <> '10.0.0.172'
 --and my_URI_Main_header='ebook12'
--where my_URI_Main='qq.com' and my_Abnormal_reason is null and my_URI_Main_header='ebook12'
 * 

 * 
 * SELECT * FROM [GuangZhou_Gn].[dbo].[mpos_gn_common]
 --where my_URI_Main='baidu.com' and Abnormal_reason=4901 and my_DEST_IP = '10.0.0.172'
 where my_URI_Main='baidu.com' and my_Abnormal_reason is null and my_DEST_IP = '10.0.0.172'
 * 
 * 
 * 
 * 
 * 
 * 
 * */

/*
 * 

1.正确解决企业库使用的问题
    1)记录linq 生成的sql文件，2)最新版本是4.3.1，3)对象是objectquery,4)升级方法,install-package entityframework

2.正确解决控件使用的问题
    1)窗口控件停靠 的问题，dockmanage，2)命令控件堆砌的问题，navbar

3.正确解决 嵌套表格显示的问题？
    1)linq自动->gridview

 
 * 
**/


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GnPlatForm.KeyPerformance;
using GnPlatForm.BusinessLogic;
using GnPlatForm.SqlServer;
using System.Configuration;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using System.IO;
using System.Data.Objects;
using GnPlatForm.Analysis.Out;
using GnPlatForm.MySql;

namespace GnPlatForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void refreshGrid(IQueryable linqQuery)
        {
            gridView1.PopulateColumns();
            gridControl1.Refresh();
            gridView1.BestFitColumns();
            gridView1.OptionsView.AllowCellMerge = true;
            for (int i = 5; i < gridView1.Columns.Count; i++)
                gridView1.Columns[i].OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;

            QueryLogFile qlf = new QueryLogFile();
            qlf.QueryLog(linqQuery);
        }

        private void refreshGrid(IQueryable linqQuery ,int begin)
        {
            gridView1.PopulateColumns();
            gridControl1.Refresh();
            gridView1.BestFitColumns();
            gridView1.OptionsView.AllowCellMerge = true;
            for (int i = begin; i < gridView1.Columns.Count; i++)
                gridView1.Columns[i].OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;

            QueryLogFile qlf = new QueryLogFile();
            qlf.QueryLog(linqQuery);
        }

        //Gn接口各指标统计分析,按照终端分类，按照uri分类，维度？？？？
        //计算各URI通过网关的成功率和时延,也可以指定uri
        string wap = "10.0.0.172";
        // linq to sql 向企业库代码的迁移？
        //企业库、linq、linq to sql 等库特征的区分？
        guangzhou_0410Entities mysqlcontext;
        private GuangZhou_GnEntities servercontext;
        private GuangZhou_GnEntities1 servercontext1;
        private GuangZhou_GnEntities2 servercontext2;
        private GuangZhou_GnEntities3 servercontext3;

        //private IEnumerable<mpos_gn_common> sqlserver_ienumerable;
        private ObjectQuery<mpos_gn_common> sqlserver_objectquery;
        private ObjectQuery<Gn_IP_Fragment> sqlerver_segment;
        private ObjectQuery<Gb_IP_Fragment> gb_ip_fragment;
        public HashSet<string> url_list;

        private void Form1_Load(object sender, EventArgs ee)
        {
            if (File.Exists(@"LogFile.xml")) File.Delete(@"LogFile.xml");

            //string serverconnstring = ConfigurationManager.ConnectionStrings["GuangZhou_GnEntities"].ToString();

            servercontext = new GuangZhou_GnEntities();
            servercontext1 = new GuangZhou_GnEntities1();
            servercontext2 = new GuangZhou_GnEntities2();
            servercontext3 = new GuangZhou_GnEntities3();

            servercontext.CommandTimeout = 0;
            servercontext1.CommandTimeout = 0;
            servercontext2.CommandTimeout = 0;
            servercontext3.CommandTimeout = 0;

            sqlserver_objectquery = servercontext.mpos_gn_common;
            sqlerver_segment = servercontext1.Gn_IP_Fragment;
            gb_ip_fragment = servercontext2.Gb_IP_Fragment;
            url_list = new HashSet<string>();

            var url_lists = servercontext3.url_result.Select(e => e.url);
            foreach (string url in url_lists) url_list.Add(url);

            string mysqlconnstring = ConfigurationManager.ConnectionStrings["guangzhou_0410Entities"].ToString();
            mysqlcontext = new guangzhou_0410Entities(mysqlconnstring);

        }



        private void navBarItem1_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {

            var suma = from p in sqlserver_objectquery
                       where p.SynDirection == 0
                       where p.Event_Type == 4 || p.Event_Type == 5
                       group p by p.my_Event_Type into ttt
                       select new
                       {
                           my_Event_Type = ttt.Key,
                           sumary = ttt.Count()
                       };

            var db = from p in sqlserver_objectquery
                     where p.SynDirection == 0
                     where p.Event_Type == 4 || p.Event_Type == 5
                     group p by
                                   new
                                   {
                                       my_Event_Type = p.my_Event_Type,
                                       my_DEST_IP = p.my_DEST_IP.IndexOf(wap) != -1,
                                       my_URI_Main_header = p.my_URI_Main_header == null ? false : p.my_URI_Main_header.IndexOf(wap) != -1
                                   }
                         into ttt
                         select new
                         {
                             my_Event_Type = ttt.Key.my_Event_Type,
                             my_DEST_IP = ttt.Key.my_DEST_IP == true ? "toWap" : "toPub",
                             my_URI_Main_header = ttt.Key.my_URI_Main_header == true ? "toWap" : "toPub",
                             //summary = suma[ttt.Key.my_Event_Type].Count(),
                             //cnt_percent = 1.0 * ttt.Count() / suma[ttt.Key.my_Event_Type].Count(),
                             cnt = ttt.Count(),
                             suc = ttt.Where(e => e.Resp != null).Count(),
                             percent = 1.0 * ttt.Where(e => e.Resp != null).Count() / ttt.Count(),
                             uri_size = ttt.Average(e => e.my_URI_Len),
                             size = ttt.Average(e => e.IP_LEN_DL),
                             rate = ttt.Average(e => e.Duration) == null ? 0 : 1.0 * ttt.Average(e => e.IP_LEN_DL) / ttt.Average(e => (double)e.Duration),
                             delay = ttt.Average(e => e.Duration),
                             resp_delay = ttt.Average(e => e.Result_DelayFirst)
                         };

            var dbss = from p in suma
                       join q in db on p.my_Event_Type equals q.my_Event_Type
                       select new
                       {
                           q.my_Event_Type,
                           q.my_DEST_IP,
                           q.my_URI_Main_header,
                           summary = p.sumary,
                           cnt_percnet = 1.0 * q.cnt / p.sumary,
                           q.cnt,
                           q.suc,
                           q.percent,
                           q.uri_size,
                           q.size,
                           q.rate,
                           q.delay,
                           q.resp_delay
                       };
            var dborder = dbss.OrderByDescending(e => e.summary).ThenBy(e => e.my_DEST_IP);
            gridControl1.DataSource = dborder.ToList();

            refreshGrid(dborder);
        }

        private void navBarItem2_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            var suma = from p in sqlserver_objectquery
                       where p.SynDirection == 0
                       where p.Event_Type == 4 || p.Event_Type == 5
                       where p.my_Abnormal_reason != null
                       group p by p.my_Event_Type into ttt
                       select new
                       {
                           my_Event_Type = ttt.Key,
                           sumary = ttt.Count()
                       };

            var db = from p in sqlserver_objectquery
                     where p.SynDirection == 0
                     where p.Event_Type == 4 || p.Event_Type == 5
                     where p.my_Abnormal_reason != null
                     group p by
                                   new
                                   {
                                       my_Event_Type = p.my_Event_Type,
                                       my_DEST_IP = p.my_DEST_IP.IndexOf(wap) != -1,
                                       my_URI_Main_header = p.my_URI_Main_header == null ? false : p.my_URI_Main_header.IndexOf(wap) != -1,
                                       my_URI_Main = p.my_URI_Main,
                                       p.Abnormal_reason,
                                       p.my_Abnormal_reason
                                   }
                         into ttt
                         select new
                         {
                             my_Event_Type = ttt.Key.my_Event_Type,
                             my_DEST_IP = ttt.Key.my_DEST_IP == true ? "toWap" : "toPub",
                             my_URI_Main_header = ttt.Key.my_URI_Main_header == true ? "toWap" : "toPub",
                             Abnormal_reason = ttt.Key.Abnormal_reason,
                             my_Abnormal_reason = ttt.Key.my_Abnormal_reason,
                             my_URI_Main = ttt.Key.my_URI_Main,
                             //summary = suma[ttt.Key.my_Event_Type].Count(),
                             //cnt_percent = 1.0 * ttt.Count() / suma[ttt.Key.my_Event_Type].Count(),
                             cnt = ttt.Count(),
                         };

            var dbss = from p in suma
                       join q in db on p.my_Event_Type equals q.my_Event_Type
                       select new
                       {
                           q.my_Event_Type,
                           q.my_DEST_IP,
                           q.my_URI_Main_header,
                           q.Abnormal_reason,
                           q.my_Abnormal_reason,
                           q.my_URI_Main,
                           summary = p.sumary,
                           cnt_percent = 1.0 * q.cnt / p.sumary,
                           q.cnt
                       };

            var dborder = dbss.OrderByDescending(e => e.cnt);
            //OrderByDescending(e => e.summary).ThenBy(e => e.my_DEST_IP).
            gridControl1.DataSource = dborder.ToList();
            refreshGrid(dbss);
        }

        private void navBarItem3_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Excel2007文件(*.xlsx) |*.xlsx";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {

                gridView1.ExportToXlsx(saveFileDialog1.FileName);
            }
        }

        private void navBarItem4_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();

            var suma = sqlserver_objectquery
                       .Where(e => e.SynDirection == 0)
                       .Where(e => e.Event_Type == 4 || e.Event_Type == 5)
                       .Where(e => e.my_Abnormal_reason != null)
                       .Count();
            //.ToLookup(e => e.my_Event_Type, e => e.gn_id);

            var db = from p in sqlserver_objectquery
                     where p.SynDirection == 0
                     where p.Event_Type == 4 || p.Event_Type == 5
                     where p.my_Abnormal_reason != null
                     group p by
                                   new
                                   {
                                       //my_Event_Type = p.my_Event_Type,
                                       //my_DEST_IP = p.my_DEST_IP.IndexOf(wap) != -1,
                                       //my_URI_Main_header = p.my_URI_Main_header == null ? false : p.my_URI_Main_header.IndexOf(wap) != -1,
                                       //p.Abnormal_reason,
                                       p.my_Abnormal_reason
                                   }
                         into ttt
                         select new
                         {
                             //my_Event_Type = ttt.Key.my_Event_Type,
                             //my_DEST_IP = ttt.Key.my_DEST_IP == true ? "toWap" : "toPub",
                             //my_URI_Main_header = ttt.Key.my_URI_Main_header == true ? "toWap" : "toPub",
                             //Abnormal_reason = ttt.Key.Abnormal_reason,
                             my_Abnormal_reason = ttt.Key.my_Abnormal_reason,
                             //summary = suma[ttt.Key.my_Event_Type].Count(),
                             cnt_percent = 1.0 * ttt.Count() / suma,
                             cnt = ttt.Count(),
                         };

            //var dborder = db.OrderByDescending(e => e.summary).ThenBy(e => e.my_DEST_IP).ThenByDescending(e => e.cnt);
            var dborder = db.OrderByDescending(e => e.cnt);
            gridControl1.DataSource = dborder.ToList();
            refreshGrid(dborder);
        }

        private void navBarItem5_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();

            var suma = sqlserver_objectquery
            .Where(e => e.SynDirection == 0)
            .Where(e => e.Event_Type == 4 || e.Event_Type == 5)
            .Where(e => e.my_Abnormal_reason != null)
                   .Where(e => e.my_Abnormal_reason == "业务请求网络无响应")
            .Count();
            //.ToLookup(e => e.my_Event_Type, e => e.gn_id);

            var db = from p in sqlserver_objectquery
                     where p.SynDirection == 0
                     where p.Event_Type == 4 || p.Event_Type == 5
                     where p.my_Abnormal_reason != null
                     where p.my_Abnormal_reason == "业务请求网络无响应"
                     group p by
                                   new
                                   {
                                       my_URI_Main = p.my_URI_Main,
                                       //my_Event_Type = p.my_Event_Type,
                                       //my_DEST_IP = p.my_DEST_IP.IndexOf(wap) != -1,
                                       //my_URI_Main_header = p.my_URI_Main_header == null ? false : p.my_URI_Main_header.IndexOf(wap) != -1,
                                       //p.Abnormal_reason,
                                       p.my_Abnormal_reason
                                   }
                         into ttt
                         select new
                         {
                             //my_Event_Type = ttt.Key.my_Event_Type,
                             //my_DEST_IP = ttt.Key.my_DEST_IP == true ? "toWap" : "toPub",
                             //my_URI_Main_header = ttt.Key.my_URI_Main_header == true ? "toWap" : "toPub",
                             my_URI_Main = ttt.Key.my_URI_Main,
                             //Abnormal_reason = ttt.Key.Abnormal_reason,
                             my_Abnormal_reason = ttt.Key.my_Abnormal_reason,
                             //summary = suma[ttt.Key.my_Event_Type].Count(),
                             sumary = suma,
                             cnt = ttt.Count(),
                             cnt_percent = 1.0 * ttt.Count() / suma,
                         };

            //var dborder = db.OrderByDescending(e => e.summary).ThenBy(e => e.my_DEST_IP).ThenByDescending(e => e.cnt);
            var dborder = db.OrderByDescending(e => e.cnt);
            gridControl1.DataSource = dborder.ToList();
            refreshGrid(dborder);
        }

        private void navBarItem6_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();

            var suma = sqlserver_objectquery
            .Where(e => e.SynDirection == 0)
            .Where(e => e.my_URI_Main == "baidu.com")
            .Where(e => e.Event_Type == 4 || e.Event_Type == 5)
            .Where(e => e.my_Abnormal_reason != null)
             .Where(e => e.my_Abnormal_reason == "业务请求网络无响应")
            .Count();

            var db = from p in sqlserver_objectquery
                     where p.SynDirection == 0
                     where p.my_URI_Main == "baidu.com"
                     where p.Event_Type == 4 || p.Event_Type == 5
                     where p.my_Abnormal_reason != null
                     where p.my_Abnormal_reason == "业务请求网络无响应"
                     group p by
                                   new
                                   {
                                       my_URI_Main = p.my_URI_Main,
                                       //p.my_Event_Type,
                                       p.my_DEST_IP,
                                       //p.Abnormal_reason,
                                       p.my_Abnormal_reason
                                   }
                         into ttt
                         select new
                         {
                             my_URI_Main = ttt.Key.my_URI_Main,
                             // my_Event_Type = ttt.Key.my_Event_Type,
                             my_DEST_IP = ttt.Key.my_DEST_IP,
                             //Abnormal_reason = ttt.Key.Abnormal_reason,
                             my_Abnormal_reason = ttt.Key.my_Abnormal_reason,
                             sumary = suma,
                             cnt = ttt.Count(),
                             cnt_percent = 1.0 * ttt.Count() / suma,
                         };

            var dborder = db.OrderByDescending(e => e.cnt);

            //WriteToFile((dborder as ObjectQuery<mpos_gn_common>).ToTraceString());



            //sqlserver_gn_table.lo
            //WriteToFile(dborder.GetType().GetMethod("ToTraceString").Invoke(dborder, null));
            gridControl1.DataSource = dborder.ToList();

            refreshGrid(dborder);
        }

        //增加APN维度
        private void navBarItem7_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();

            var sp_ip = "111.13.12.15";
            var toWap = "toWap";
            var toPub = "toPub";

            var suma = from p in sqlserver_objectquery
                       where p.SynDirection == 0
                       //where p.APN !=string.Empty
                       where p.my_DEST_IP == sp_ip
                       where p.Event_Type == 4 || p.Event_Type == 5
                       group p by p.my_Event_Type into ttt
                       select new
                       {
                           my_Event_Type = ttt.Key,
                           sumary = ttt.Count()
                       };

            var db = from p in sqlserver_objectquery
                     where p.SynDirection == 0
                     //where p.APN !=string.Empty
                     where p.my_DEST_IP == sp_ip
                     where p.Event_Type == 4 || p.Event_Type == 5
                     group p by
                                   new
                                   {
                                       p.APN,
                                       my_Event_Type = p.my_Event_Type,
                                       my_DEST_IP = p.my_DEST_IP.IndexOf(wap) != -1,
                                       my_URI_Main_header = p.my_URI_Main_header == null ? false : p.my_URI_Main_header.IndexOf(wap) != -1
                                   }
                         into ttt
                         select new
                         {
                             ttt.Key.APN,
                             my_Event_Type = ttt.Key.my_Event_Type,
                             my_DEST_IP = ttt.Key.my_DEST_IP == true ? toWap : toPub,
                             my_URI_Main_header = ttt.Key.my_URI_Main_header == true ? toWap : toPub,
                             cnt = ttt.Count(),
                             suc = ttt.Where(e => e.Resp != null).Count(),
                             percent = 1.0 * ttt.Where(e => e.Resp != null).Count() / ttt.Count(),
                             uri_size = ttt.Average(e => e.my_URI_Len),
                             size = ttt.Average(e => e.IP_LEN_DL),
                             rate = ttt.Average(e => e.Duration) == null ? 0 : 1.0 * ttt.Average(e => e.IP_LEN_DL) / ttt.Average(e => (double)e.Duration),
                             delay = ttt.Average(e => e.Duration),
                             resp_delay = ttt.Average(e => e.Result_DelayFirst)
                         };

            var dbss = from p in suma
                       join q in db on p.my_Event_Type equals q.my_Event_Type
                       select new
                       {
                           q.my_Event_Type,
                           q.APN,
                           summary = p.sumary,
                           cnt_percent = 1.0 * q.cnt / p.sumary,
                           q.cnt,
                           q.suc,
                           q.percent,
                           q.uri_size,
                           q.size,
                           q.rate,
                           q.delay,
                           q.resp_delay
                       };

            var dborder = dbss.OrderByDescending(e => e.summary);
            gridControl1.DataSource = dborder.ToList();
            refreshGrid(dborder);
        }

        private void navBarItem8_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();

            var suma = sqlserver_objectquery
                    .Where(e => e.SynDirection == 0)
                //.Where(e => e.my_URI_Main == "baidu.com")
                    .Where(e => e.Event_Type == 4 || e.Event_Type == 5)
                    .Where(e => e.my_Abnormal_reason != null)
                    .Where(e => e.my_Abnormal_reason == "业务请求网络无响应")
                //.Where(e => e.Abnormal_reason == 4901)
                    .Count();

            var db = from p in sqlserver_objectquery
                     where p.SynDirection == 0
                     //where p.my_URI_Main == "baidu.com"
                     where p.Event_Type == 4 || p.Event_Type == 5
                     where p.my_Abnormal_reason != null
                     where p.my_Abnormal_reason == "业务请求网络无响应"
                     //where p.Abnormal_reason == 4901
                     group p by
                                   new
                                   {
                                       //my_URI_Main = p.my_URI_Main,
                                       //p.my_Event_Type,
                                       p.my_DEST_IP,
                                       //p.Abnormal_reason,
                                       p.my_Abnormal_reason
                                   }
                         into ttt
                         select new
                         {
                             //my_URI_Main = ttt.Key.my_URI_Main,
                             //my_Event_Type = ttt.Key.my_Event_Type,
                             my_DEST_IP = ttt.Key.my_DEST_IP,
                             //Abnormal_reason = ttt.Key.Abnormal_reason,
                             my_Abnormal_reason = ttt.Key.my_Abnormal_reason,
                             sumary = suma,
                             cnt = ttt.Count(),
                             cnt_percent = 1.0 * ttt.Count() / suma,
                         };

            var dborder = db.OrderByDescending(e => e.cnt);
            gridControl1.DataSource = dborder.ToList();
            refreshGrid(dborder);
        }

        private void navBarItem9_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {

            var suma = sqlserver_objectquery
                .Where(e => e.SynDirection == 0)
                .Where(e => e.Event_Type == 4)
                .ToLookup(e => e.my_URI_Main, e => e.gn_id);
            var gn = from p in sqlserver_objectquery
                     where p.SynDirection == 0
                     where p.Event_Type == 4
                     group p by
                     new
                     {
                         p.my_URI_Main,
                         my_DEST_IP = p.my_DEST_IP.IndexOf(wap) != -1,
                         my_URI_Main_header = p.my_URI_Main_header == null ? false : p.my_URI_Main_header.IndexOf(wap) != -1
                     }
                         into ttt
                         select new
                         {
                             my_URI_Main = ttt.Key.my_URI_Main,
                             my_DEST_IP = ttt.Key.my_DEST_IP == true ? "toWap" : "toPub",
                             my_URI_Main_header = ttt.Key.my_URI_Main_header == true ? "toWap" : "toPub",
                             summary = suma[ttt.Key.my_URI_Main].Count(),
                             cnt_percent = 1.0 * ttt.Count() / suma[ttt.Key.my_URI_Main].Count(),
                             cnt = ttt.Count(),
                             suc = ttt.Where(e => e.Resp != null).Count(),
                             percent = 1.0 * ttt.Where(e => e.Resp != null).Count() / ttt.Count(),
                             uri_size = ttt.Average(e => e.my_URI_Len),
                             size = ttt.Average(e => e.IP_LEN_DL),
                             rate = ttt.Average(e => e.Duration) == null ? 0 :
                             1.0 * ttt.Average(e => e.IP_LEN_DL) / ttt.Average(e => (double)e.Duration),
                             delay = ttt.Average(e => e.Duration),
                             resp_delay = ttt.Average(e => e.Result_DelayFirst)
                         };
            gridControl1.DataSource = gn.OrderByDescending(e => e.summary).ThenBy(e => e.my_DEST_IP).ToList();
            refreshGrid(gn.OrderByDescending(e => e.summary).ThenBy(e => e.my_DEST_IP));
        }

        private void navBarItem10_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            var suma = sqlserver_objectquery
                   .Where(e => e.SynDirection == 0)
                   .Where(e => e.Event_Type == 5)
                   .ToLookup(e => e.my_URI_Main, e => e.gn_id);

            var gn = from p in sqlserver_objectquery
                     where p.SynDirection == 0
                     where p.Event_Type == 5
                     group p by
                     new
                     {
                         p.my_URI_Main,
                         my_DEST_IP = p.my_DEST_IP.IndexOf(wap) != -1,
                         my_URI_Main_header = p.my_URI_Main_header == null ? false : p.my_URI_Main_header.IndexOf(wap) != -1
                     }
                         into ttt
                         select new
                         {
                             my_URI_Main = ttt.Key.my_URI_Main,
                             my_DEST_IP = ttt.Key.my_DEST_IP == true ? "toWap" : "toPub",
                             my_URI_Main_header = ttt.Key.my_URI_Main_header == true ? "toWap" : "toPub",
                             summary = suma[ttt.Key.my_URI_Main].Count(),
                             cnt_percent = 1.0 * ttt.Count() / suma[ttt.Key.my_URI_Main].Count(),
                             cnt = ttt.Count(),
                             suc = ttt.Where(e => e.Resp != null).Count(),
                             percent = 1.0 * ttt.Where(e => e.Resp != null).Count() / ttt.Count(),
                             uri_size = ttt.Average(e => e.my_URI_Len),
                             size = ttt.Average(e => e.IP_LEN_DL),
                             rate = ttt.Average(e => e.Duration) == null ? 0 :
                             1.0 * ttt.Average(e => e.IP_LEN_DL) / ttt.Average(e => (double)e.Duration),
                             delay = ttt.Average(e => e.Duration),
                             resp_delay = ttt.Average(e => e.Result_DelayFirst)
                         };
            gridControl1.DataSource = gn.OrderByDescending(e => e.summary).ThenBy(e => e.my_DEST_IP).ToList();
            refreshGrid(gn.OrderByDescending(e => e.summary).ThenBy(e => e.my_DEST_IP));
        }

        private void navBarItem11_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            //servercontext.ContextOptions.LazyLoadingEnabled = false;
            var db = from p in sqlserver_objectquery
                     where p.gn_id == 2
                     select p.APN;
            //db.ToList();
            //var dblist = db.ToList();
            QueryLogFile qlf = new QueryLogFile();
            qlf.QueryLog(db);
        }

        private void navBarItem12_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            var db = from p in sqlserver_objectquery
                     where p.SynDirection == 0
                     where p.Count_Packet_DL > 0
                     where p.Resp == true
                     where p.Event_Type == 4 || p.Event_Type == 5
                     group p by
                                   new
                                   {
                                       //my_Event_Type = p.my_Event_Type,
                                       my_Packet_Size_Down = (p.IP_LEN_DL / p.Count_Packet_DL) / 100,
                                   }
                         into ttt
                         select new
                         {
                             //my_Event_Type = ttt.Key.my_Event_Type,
                             my_Packet_Size_Down = ttt.Key.my_Packet_Size_Down,
                             cnt = ttt.Count(),
                             suc = ttt.Where(e => e.Resp != null).Count(),
                             percent = 1.0 * ttt.Where(e => e.Resp != null).Count() / ttt.Count(),
                             uri_size = ttt.Average(e => e.my_URI_Len),
                             size = ttt.Average(e => e.IP_LEN_DL),
                             rate = ttt.Average(e => e.Duration) == null ? 0 : 1.0 * ttt.Average(e => e.IP_LEN_DL) / ttt.Average(e => (double)e.Duration),
                             delay = ttt.Average(e => e.Duration),
                             resp_delay = ttt.Average(e => e.Result_DelayFirst)
                         };

            var dborder = db.OrderByDescending(e => e.my_Packet_Size_Down);
            gridControl1.DataSource = dborder.ToList();
            refreshGrid(dborder);
        }

        private void navBarItem14_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            gridView1.PopulateColumns();
            gridControl1.Refresh();

            var totalsize = sqlerver_segment.Sum(e => e.ip_len);
            var ip = from p in sqlerver_segment
                     group p by new
                     {
                         //p.ip_flags,
                         //p.ip_flags_df,
                         p.ip_flags_mf,
                         //p.ip_frag_offset
                     }
                         into ttt
                         select new
                             {
                                 //ttt.Key.ip_flags,
                                 //ttt.Key.ip_flags_df,
                                 ttt.Key.ip_flags_mf,
                                 //ttt.Key.ip_frag_offset,
                                 cnt = ttt.Count(),
                                 item_total_size = ttt.Sum(e => e.ip_len),
                                 item_avg_size = 1.0 * ttt.Sum(e => e.ip_len) / ttt.Count(),
                                 item_max_sieze = ttt.Max(e => e.ip_len),
                                 total_size = totalsize,
                                 item_rate = 1.0 * ttt.Sum(e => e.ip_len) / totalsize,
                             };
            var dborder = ip.OrderByDescending(e => e.cnt);
            gridControl1.DataSource = dborder.ToList();
            //refreshGrid(dborder);
        }

        private void navBarItem15_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            gridView1.PopulateColumns();
            gridControl1.Refresh();

            var totalsize = sqlerver_segment.Sum(e => e.ip2_len);
            var ip = from p in sqlerver_segment
                     group p by new
                     {
                         //p.ip2_flags,
                         //p.ip2_flags_df,
                         p.ip2_flags_mf,
                         //p.ip2_frag_offset
                     }
                         into ttt
                         select new
                         {
                             //ttt.Key.ip2_flags,
                             //ttt.Key.ip2_flags_df,
                             ttt.Key.ip2_flags_mf,
                             //ttt.Key.ip2_frag_offset,
                             cnt = ttt.Count(),
                             item_total_size = ttt.Sum(e => e.ip2_len),
                             item_avg_size = 1.0 * ttt.Sum(e => e.ip2_len) / ttt.Count(),
                             item_max_sieze = ttt.Max(e => e.ip2_len),
                             total_size = totalsize,
                             item_rate = 1.0 * ttt.Sum(e => e.ip2_len) / totalsize,
                         };
            var dborder = ip.OrderByDescending(e => e.cnt);
            gridControl1.DataSource = dborder.ToList();
            //refreshGrid(dborder);

        }

        private void navBarItem16_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            gridView1.PopulateColumns();
            gridControl1.Refresh();

            var total_cnt = sqlerver_segment.Where(e => e.IP_Fragment_MsgType == "HTTP.Response").Count();
            var ip = from p in sqlerver_segment
                     where p.IP_Fragment_MsgType == "HTTP.Response"
                     group p by new
                     {
                         //p.IP_Fragment_MsgType  ,
                         p.tcp_need_segment,
                     }
                         into ttt
                         select new
                         {
                             //ttt.Key.IP_Fragment_MsgType,
                             ttt.Key.tcp_need_segment,
                             item_avg_size = ttt.Average(e => e.ip2_len),
                             item_min_size = ttt.Min(e => e.ip2_len),
                             item_max_size = ttt.Max(e => e.ip2_len),
                             cnt = ttt.Count(),
                             //item_total_size = ttt.Sum(e => e.ip2_len),
                             //item_avg_size = 1.0 * ttt.Sum(e => e.ip2_len) / ttt.Count(),
                             //item_max_sieze = ttt.Max(e => e.ip2_len),
                             total_cnt = total_cnt,
                             item_rate = 1.0 * ttt.Count() / total_cnt,
                         };
            var dborder = ip.OrderByDescending(e => e.cnt);
            gridControl1.DataSource = dborder.ToList();
            //refreshGrid(dborder);
        }

        private void navBarItem17_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            var maxt = sqlerver_segment.Max(e => e.PacketTime);
            var mint = sqlerver_segment.Min(e => e.PacketTime);
            TimeSpan ts = maxt.Value - mint.Value;
            var ttim = mint.Value.ToString() + "-" + maxt.Value.ToString() + "," + ts.Seconds.ToString();
            textBox1.Text = ttim.ToString();
        }

        private void navBarItem18_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            gridView1.PopulateColumns();
            gridControl1.Refresh();

            var totalsize = gb_ip_fragment.Sum(e => e.ip_len);
            var ip = from p in gb_ip_fragment
                     group p by new
                     {
                         p.ip_flags,
                         p.ip_flags_df,
                         p.ip_flags_mf,
                         p.ip_frag_offset
                     }
                         into ttt
                         select new
                         {
                             ttt.Key.ip_flags,
                             ttt.Key.ip_flags_df,
                             ttt.Key.ip_flags_mf,
                             ttt.Key.ip_frag_offset,
                             ip_cnt = ttt.Count(),
                             ip_min_size = ttt.Min(e => e.ip_len),
                             ip_avg_size = 1.0 * ttt.Sum(e => e.ip_len) / ttt.Count(),
                             ip_max_size = ttt.Max(e => e.ip_len),
                             ip_total_size = ttt.Sum(e => e.ip_len),
                             total_ip_size = totalsize,
                             ip_size_rate = 1.0 * ttt.Sum(e => e.ip_len) / totalsize,
                         };
            var dborder = ip.OrderByDescending(e => e.ip_cnt);
            gridControl1.DataSource = dborder.ToList();
            //refreshGrid(dborder);
        }

        private void navBarItem19_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            gridView1.PopulateColumns();
            gridControl1.Refresh();

            var totalsize = gb_ip_fragment.Sum(e => e.ip2_len);
            var ip = from p in gb_ip_fragment
                     group p by new
                     {
                         p.ip2_flags,
                         p.ip2_flags_df,
                         p.ip2_flags_mf,
                         p.ip2_frag_offset
                     }
                         into ttt
                         select new
                         {
                             ttt.Key.ip2_flags,
                             ttt.Key.ip2_flags_df,
                             ttt.Key.ip2_flags_mf,
                             ttt.Key.ip2_frag_offset,
                             ip2_cnt = ttt.Count(),
                             ip2_min_size = ttt.Min(e => e.ip2_len),
                             ip2_avg_size = 1.0 * ttt.Sum(e => e.ip2_len) / ttt.Count(),
                             ip2_max_size = ttt.Max(e => e.ip2_len),
                             ip2_total_size = ttt.Sum(e => e.ip2_len),
                             total_ip2_size = totalsize,
                             ip2_size_rate = 1.0 * ttt.Sum(e => e.ip2_len) / totalsize,
                         };
            var dborder = ip.OrderByDescending(e => e.ip2_cnt);
            gridControl1.DataSource = dborder.ToList();
            //refreshGrid(dborder);
        }

        private void navBarItem20_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            gridView1.PopulateColumns();
            gridControl1.Refresh();

            var total_cnt = gb_ip_fragment.Where(e => e.IP_Fragment_MsgType == "HTTP.Response").Count();
            var ip = from p in gb_ip_fragment
                     where p.IP_Fragment_MsgType == "HTTP.Response"
                     group p by new
                     {
                         p.IP_Fragment_MsgType,
                         p.tcp_need_segment,
                     }
                         into ttt
                         select new
                         {
                             ttt.Key.IP_Fragment_MsgType,
                             ttt.Key.tcp_need_segment,
                             ip2_avg_size = ttt.Average(e => e.ip2_len),
                             ip2_min_size = ttt.Min(e => e.ip2_len),
                             ip2_max_size = ttt.Max(e => e.ip2_len),
                             ip2_cnt = ttt.Count(),
                             total_ip2_cnt = total_cnt,
                             ip2_cnt_rate = 1.0 * ttt.Count() / total_cnt,
                         };
            var dborder = ip.OrderByDescending(e => e.ip2_cnt);
            gridControl1.DataSource = dborder.ToList();
            //refreshGrid(dborder);
        }

        private void navBarItem21_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            var maxt = gb_ip_fragment.Max(e => e.PacketTime);
            var mint = gb_ip_fragment.Min(e => e.PacketTime);
            TimeSpan ts = maxt.Value - mint.Value;
            var ttim = mint.Value.ToString() + "-" + maxt.Value.ToString() + "," + ts.TotalSeconds.ToString();
            textBox1.Text = ttim.ToString();
        }

        private void navBarItem22_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            gridView1.PopulateColumns();
            gridControl1.Refresh();

            //var ipsplit = gb_ip_fragment.Where(e => e.ip_flags_mf == "Set").Select(e => e.IP_Fragment_MsgType).Distinct();
            var total_cnt = gb_ip_fragment.Where(e => e.bssgp_direction == "Down")
                //.Where(e=>e.ip2_len==1500)

                //.Where(e=>ipsplit.Contains(e.IP_Fragment_MsgType)).Count();
                .Count();
            var ip = from p in gb_ip_fragment
                     where p.bssgp_direction == "Down"
                     //where p.ip2_len==1500
                     //where ipsplit.Contains(p.IP_Fragment_MsgType)
                     //where p.ip_len !=null && p.ip2_len !=null
                     group p by new
                     {
                         sndcp_segment = p.sndcp_segment == null ? false : p.sndcp_segment > 0,
                         //p.IP_Fragment_MsgType,
                         //p.ip_flags_mf,
                         // p.tcp_checksum_good,
                         //sndcp_split=p.ip2_len> p.ip_len
                         //p.sndcp_dcomp,
                         //p.bssgp_drx_timer,
                         ////p.bssgp_drx_ccch,
                         ////p.bssgp_drx_coefficient,
                         //p.bssgp_drx_cycle,
                         //p.IP_Fragment_MsgType,
                         //p.tcp_need_segment,
                         //p.bssgp_lac,
                         //p.bssgp_ci,
                         // p.nsip_bvci,
                         // p.tcp_window_size,
                         // p.ip_len,
                     }
                         into ttt
                         select new
                         {
                             // ttt.Key.IP_Fragment_MsgType,
                             ttt.Key.sndcp_segment,
                             //ttt.Key.tcp_checksum_good,
                             //ttt.Key.sndcp_split,
                             //ttt.Key.bssgp_drx_timer,
                             ////ttt.Key.bssgp_drx_ccch,
                             ////ttt.Key.bssgp_drx_coefficient,
                             //ttt.Key.bssgp_drx_cycle,
                             ////ttt.Key.IP_Fragment_MsgType,
                             //ttt.Key.tcp_need_segment,
                             //ttt.Key.bssgp_lac,
                             //ttt.Key.bssgp_ci,
                             //ttt.Key.nsip_bvci,
                             //ttt.Key.tcp_window_size,
                             //ttt.Key.ip_len,
                             ip_avg_size = ttt.Average(e => e.ip_len),
                             ip_min_size = ttt.Min(e => e.ip_len),
                             ip_max_size = ttt.Max(e => e.ip_len),
                             ip_cnt = ttt.Count(),
                             total_ip_cnt = total_cnt,
                             ip_cnt_rate = 1.0 * ttt.Count() / total_cnt,
                         };
            var dborder = ip.OrderByDescending(e => e.ip_cnt);
            gridControl1.DataSource = dborder.ToList();

        }

        private void navBarItem23_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            var total_cnt = gb_ip_fragment.Where(e => e.bssgp_direction == "Down").Count();
            var ip = from p in gb_ip_fragment
                     where p.bssgp_direction == "Down"
                     group p by new
                     {

                         p.ip_flags_mf,

                         sndcp_segment = p.sndcp_segment == null ? false : p.sndcp_segment > 0,
                     }
                         into ttt
                         select new
                         {


                             ttt.Key.ip_flags_mf,
                             ttt.Key.sndcp_segment,

                             ip_avg_size = ttt.Average(e => e.ip_len),
                             ip_min_size = ttt.Min(e => e.ip_len),
                             ip_max_size = ttt.Max(e => e.ip_len),
                             ip_cnt = ttt.Count(),
                             total_ip_cnt = total_cnt,
                             ip_cnt_rate = 1.0 * ttt.Count() / total_cnt,

                             //ip_cnt = ttt.Count(),
                             //ip_min_size = ttt.Min(e => e.ip_len),
                             //ip_avg_size = 1.0 * ttt.Sum(e => e.ip_len) / ttt.Count(),
                             //ip_max_size = ttt.Max(e => e.ip_len),
                             //ip_total_size = ttt.Sum(e => e.ip_len),
                             //total_ip_size = totalsize,
                             //ip_size_rate = 1.0 * ttt.Sum(e => e.ip_len) / totalsize,
                         };
            var dborder = ip.OrderByDescending(e => e.ip_cnt);
            gridControl1.DataSource = dborder.ToList();
        }

        //按照apn的维度，只分析get的成功率
        private void navBarItem22_LinkClicked_1(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            var suma = from p in mysqlcontext.gn_common_201204181300
                       where p.SynDirection == 0
                       //where p.Event_Type == 4 || p.Event_Type == 5
                       //group p by p.my_Event_Type into ttt
                       where url_list.Contains(p.URI_Main)
                       where p.APN != string.Empty
                       where p.Event_Type == 4
                       group p by new { p.APN } into ttt
                       select new
                       {
                           ttt.Key.APN,
                           //my_Event_Type = ttt.Key,
                           total_tcp_reset = ttt.Where(e => e.Abnormal_reason == 4906).Count(),
                           total_cnt = ttt.Count()
                       };

            var db = from p in mysqlcontext.gn_common_201204181300
                     where p.SynDirection == 0
                     where p.Event_Type == 4
                     where url_list.Contains(p.URI_Main)
                     where p.APN != string.Empty
                     // where p.Event_Type == 4 || p.Event_Type == 5
                     group p by
                                   new
                                   {
                                       p.APN,
                                       //p.my_URI_Main,
                                       //p.URI_Main,
                                       //my_Event_Type = p.my_Event_Type,
                                       my_DEST_IP = p.DEST_IP== 2885681162,
                                       my_URI_Main = p.URI_Main == null ? false : p.URI_Main.IndexOf(wap) != -1
                                   }
                         into ttt
                         select new
                         {

                             //ttt.Key.my_URI_Main,
                             //ttt.Key.URI_Main,
                             ttt.Key.APN,
                             //my_Event_Type = ttt.Key.my_Event_Type,
                             my_DEST_IP = ttt.Key.my_DEST_IP == true ? "toWap" : "toPub",
                             my_URI_Main = ttt.Key.my_URI_Main == true ? "toWap" : "toPub",
                             //summary = suma[ttt.Key.my_Event_Type].Count(),
                             //cnt_percent = 1.0 * ttt.Count() / suma[ttt.Key.my_Event_Type].Count(),
                             tcp_reset = ttt.Where(e => e.Abnormal_reason == 4906).Count(),
                             cnt = ttt.Count(),
                             suc = ttt.Where(e => e.Resp != null).Count(),
                             suc_percent = ttt.Count() == 0 ? 0 : 1.0 * ttt.Where(e => e.Resp != null).Count() / ttt.Count(),
                             //uri_size = ttt.Average(e => e.my_URI_Len),
                             dl_size = ttt.Average(e => e.IP_LEN_DL),
                             down_rate = ttt.Average(e => e.Duration) == 0 ? 0 : 1.0 * ttt.Average(e => e.IP_LEN_DL) / ttt.Average(e => (double)e.Duration),
                             delay = ttt.Average(e => e.Duration),
                             resp_delay = ttt.Average(e => e.Result_DelayFirst)
                         };

            var dbss = from p in suma
                       join q in db on new { p.APN } equals new { q.APN }
                       select new
                       {

                           q.APN,
                           //q.my_Event_Type,
                           q.my_DEST_IP,
                           q.my_URI_Main,
                           total_cnt = p.total_cnt,
                           cnt_percnet = p.total_cnt == 0 ? 0 : 1.0 * q.cnt / p.total_cnt,
                           q.cnt,
                           q.suc,
                           q.suc_percent,
                           p.total_tcp_reset,
                           q.tcp_reset,
                           tcp_reset_percent = p.total_tcp_reset == 0 ? 0 : 1.0 * q.tcp_reset / p.total_tcp_reset,
                           //q.uri_size,
                           q.dl_size,
                           q.down_rate,
                           q.delay,
                           q.resp_delay
                       };
            var dborder = dbss.OrderByDescending(e => e.total_cnt).ThenBy(e => e.my_DEST_IP);
            //gridControl1.DataSource = dborder.ToList();

            refreshGrid(dborder);
        }

        //按照APN维度，统计 tcp reset 问题？,只针对get
        private void navBarItem24_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            mysqlcontext.CommandTimeout = 0;

            var dbss = from p in mysqlcontext.gn_common_201204181300
                       where p.SynDirection == 0
                       where url_list.Contains(p.URI_Main)
                       where p.APN != string.Empty
                       where p.Event_Type == 4
                       where p.Abnormal_reason != 0
                       select p;

            //int all = dbss.Count();

            var suma = from p in mysqlcontext.gn_common_201204181300
                       where p.SynDirection == 0
                       where url_list.Contains(p.URI_Main)
                       where p.APN != string.Empty
                       where p.Event_Type == 4
                       where p.Abnormal_reason != 0
                       group p by new { p.Abnormal_reason } into ttt
                       select new
                       {
                           ttt.Key.Abnormal_reason,
                           cnt = ttt.Count(),
                           //sumary = all,
                           //cnt_percent = 1.0 * ttt.Count() / all,
                       };

            var dborder = suma.OrderByDescending(e => e.cnt);
            //gridControl1.DataSource = dborder.ToList();
            refreshGrid(dborder, 0);
        }

        //过滤APN为空
        //按照apn的维度，只分析get的成功率
        private void navBarItem25_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            var suma = from p in sqlserver_objectquery
                       where p.SynDirection == 0
                       //where p.Event_Type == 4 || p.Event_Type == 5
                       //group p by p.my_Event_Type into ttt
                       //where url_list.Contains(p.URI_Main)
                       where p.APN != string.Empty
                       where p.Event_Type == 4
                       group p by new { p.URI_Main, p.my_URI_Main } into ttt
                       select new
                       {
                           ttt.Key.URI_Main,
                           ttt.Key.my_URI_Main,
                           total_tcp_reset = ttt.Where(e => e.Abnormal_reason == 4906).Count(),
                           total_no_resp = ttt.Where(e => e.Abnormal_reason == 4901).Count(),
                           //my_Event_Type = ttt.Key,
                           total_cnt = ttt.Count()
                       };

            var db = from p in sqlserver_objectquery
                     where p.SynDirection == 0
                     where p.Event_Type == 4
                     //where url_list.Contains(p.URI_Main)
                     where p.APN != string.Empty
                     // where p.Event_Type == 4 || p.Event_Type == 5
                     group p by
                                   new
                                   {
                                       p.APN,
                                       p.my_URI_Main,
                                       p.URI_Main,
                                       //my_Event_Type = p.my_Event_Type,
                                       my_DEST_IP = p.my_DEST_IP.IndexOf(wap) != -1,
                                       my_URI_Main_header = p.my_URI_Main_header == null ? false : p.my_URI_Main_header.IndexOf(wap) != -1
                                   }
                         into ttt
                         select new
                         {

                             ttt.Key.my_URI_Main,
                             ttt.Key.URI_Main,
                             ttt.Key.APN,
                             //my_Event_Type = ttt.Key.my_Event_Type,
                             my_DEST_IP = ttt.Key.my_DEST_IP == true ? "toWap" : "toPub",
                             my_URI_Main_header = ttt.Key.my_URI_Main_header == true ? "toWap" : "toPub",
                             //summary = suma[ttt.Key.my_Event_Type].Count(),
                             //cnt_percent = 1.0 * ttt.Count() / suma[ttt.Key.my_Event_Type].Count(),
                             cnt = ttt.Count(),
                             tcp_reset = ttt.Where(e => e.Abnormal_reason == 4906).Count(),
                             no_resp = ttt.Where(e => e.Abnormal_reason == 4901).Count(),
                             suc = ttt.Where(e => e.Resp != null).Count(),
                             suc_percent = ttt.Count() == 0 ? 0 : 1.0 * ttt.Where(e => e.Resp != null).Count() / ttt.Count(),
                             uri_size = ttt.Average(e => e.my_URI_Len),
                             down_size = ttt.Average(e => e.IP_LEN_DL),
                             down_rate = ttt.Average(e => e.Duration) == 0 ? 0 : 1.0 * ttt.Average(e => e.IP_LEN_DL) / ttt.Average(e => (double)e.Duration),
                             delay = ttt.Average(e => e.Duration),
                             resp_delay = ttt.Average(e => e.Result_DelayFirst)
                         };

            var dbss = from p in suma
                       join q in db on new { p.URI_Main, p.my_URI_Main } equals new { q.URI_Main, q.my_URI_Main }
                       select new
                       {

                           q.my_URI_Main,
                           q.URI_Main,
                           q.APN,
                           //q.my_Event_Type,
                           q.my_DEST_IP,
                           q.my_URI_Main_header,
                           total_cnt = p.total_cnt,
                           cnt_percnet = p.total_cnt == 0 ? 0 : 1.0 * q.cnt / p.total_cnt,
                           q.cnt,
                           q.suc,
                           q.suc_percent,
                           p.total_tcp_reset,
                           q.tcp_reset,
                           tcp_reset_percent = p.total_tcp_reset == 0 ? 0 : 1.0 * q.tcp_reset / p.total_tcp_reset,
                           p.total_no_resp,
                           q.no_resp,
                           no_resp_percent = p.total_no_resp == 0 ? 0 : 1.0 * q.no_resp / p.total_no_resp,
                           //q.uri_size,
                           //q.down_size,
                           //q.down_rate,
                           //q.delay,
                           //q.resp_delay
                       };
            var dborder = dbss.OrderByDescending(e => e.my_URI_Main).ThenBy(e => e.URI_Main);
            gridControl1.DataSource = dborder.ToList();

            refreshGrid(dborder, 2);
        }

        private void navBarItem26_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            var suma = from p in sqlserver_objectquery
                       where p.SynDirection == 0
                       //where p.Event_Type == 4 || p.Event_Type == 5
                       //group p by p.my_Event_Type into ttt
                       //where url_list.Contains(p.URI_Main)
                       where p.APN != string.Empty
                       where p.Event_Type == 4
                       group p by new { p.APN } into ttt
                       select new
                       {
                           ttt.Key.APN,
                           //my_Event_Type = ttt.Key,
                           total_tcp_reset = ttt.Where(e => e.Abnormal_reason == 4906).Count(),
                           total_no_resp = ttt.Where(e => e.Abnormal_reason == 4901).Count(),
                           total_cnt = ttt.Count()
                       };

            var db = from p in sqlserver_objectquery
                     where p.SynDirection == 0
                     where p.Event_Type == 4
                     //where url_list.Contains(p.URI_Main)
                     where p.APN != string.Empty
                     // where p.Event_Type == 4 || p.Event_Type == 5
                     group p by
                                   new
                                   {
                                       p.APN,
                                       //p.my_URI_Main,
                                       //p.URI_Main,
                                       //my_Event_Type = p.my_Event_Type,
                                       my_DEST_IP = p.my_DEST_IP.IndexOf(wap) != -1,
                                       my_URI_Main = p.URI_Main == null ? false : p.URI_Main.IndexOf(wap) != -1
                                   }
                         into ttt
                         select new
                         {

                             //ttt.Key.my_URI_Main,
                             //ttt.Key.URI_Main,
                             ttt.Key.APN,
                             //my_Event_Type = ttt.Key.my_Event_Type,
                             my_DEST_IP = ttt.Key.my_DEST_IP == true ? "toWap" : "toPub",
                             my_URI_Main = ttt.Key.my_URI_Main == true ? "toWap" : "toPub",
                             //summary = suma[ttt.Key.my_Event_Type].Count(),
                             //cnt_percent = 1.0 * ttt.Count() / suma[ttt.Key.my_Event_Type].Count(),
                             tcp_reset = ttt.Where(e => e.Abnormal_reason == 4906).Count(),
                             no_resp = ttt.Where(e => e.Abnormal_reason == 4901).Count(),
                             cnt = ttt.Count(),
                             suc = ttt.Where(e => e.Resp != null).Count(),
                             suc_percent = ttt.Count() == 0 ? 0 : 1.0 * ttt.Where(e => e.Resp != null).Count() / ttt.Count(),
                             uri_size = ttt.Average(e => e.my_URI_Len),
                             dl_size = ttt.Average(e => e.IP_LEN_DL),
                             down_rate = ttt.Average(e => e.Duration) == 0 ? 0 : 1.0 * ttt.Average(e => e.IP_LEN_DL) / ttt.Average(e => (double)e.Duration),
                             delay = ttt.Average(e => e.Duration),
                             resp_delay = ttt.Average(e => e.Result_DelayFirst)
                         };

            var dbss = from p in suma
                       join q in db on new { p.APN } equals new { q.APN }
                       select new
                       {

                           q.APN,
                           //q.my_Event_Type,
                           q.my_DEST_IP,
                           q.my_URI_Main,
                           total_cnt = p.total_cnt,
                           cnt_percnet = p.total_cnt == 0 ? 0 : 1.0 * q.cnt / p.total_cnt,
                           q.cnt,
                           q.suc,
                           q.suc_percent,
                           p.total_tcp_reset,
                           q.tcp_reset,
                           tcp_reset_percent = p.total_tcp_reset == 0 ? 0 : 1.0 * q.tcp_reset / p.total_tcp_reset,
                           p.total_no_resp,
                           q.no_resp,
                           no_resp_percent = p.total_no_resp == 0 ? 0 : 1.0 * q.no_resp / p.total_no_resp,
                           //q.uri_size,
                           //q.dl_size,
                           //q.down_rate,
                           //q.delay,
                           //q.resp_delay
                       };
            var dborder = dbss.OrderByDescending(e => e.total_cnt).ThenBy(e => e.my_DEST_IP);
            gridControl1.DataSource = dborder.ToList();

            refreshGrid(dborder);
        }

        private void navBarItem27_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            var suma = from p in sqlserver_objectquery
                       where p.SynDirection == 0
                       //where p.Event_Type == 4 || p.Event_Type == 5
                       //group p by p.my_Event_Type into ttt
                       //where url_list.Contains(p.URI_Main)
                       where p.APN != string.Empty
                       where p.Event_Type == 4
                       where p.Abnormal_reason !=0
                       group p by new { p.my_Abnormal_reason } into ttt
                       select new
                       {
                           ttt.Key.my_Abnormal_reason,
                           sumary = ttt.Count()
                       };

            var db = from p in sqlserver_objectquery
                     where p.SynDirection == 0
                     //where p.Event_Type == 4 || p.Event_Type == 5
                     //group p by p.my_Event_Type into ttt
                     //where url_list.Contains(p.URI_Main)
                     where p.APN != string.Empty
                     where p.Event_Type == 4
                     where p.Abnormal_reason != 0
                     group p by
                                   new
                                   {
                                       p.my_Abnormal_reason,
                                       p.my_URI_Main,
                                       p.URI_Main,
                                       p.APN,
                                       my_DEST_IP = p.my_DEST_IP.IndexOf(wap) != -1,
                                       my_URI_Main_header = p.my_URI_Main_header == null ? false : p.my_URI_Main_header.IndexOf(wap) != -1,
                                   }
                         into ttt
                         select new
                         {
                            ttt.Key.my_Abnormal_reason,
                            ttt.Key.my_URI_Main,
                            ttt.Key.APN,
                            ttt.Key.URI_Main,
                             my_DEST_IP = ttt.Key.my_DEST_IP == true ? "toWap" : "toPub",
                             my_URI_Main_header = ttt.Key.my_URI_Main_header == true ? "toWap" : "toPub",
                             cnt = ttt.Count(),
                         };

            var dbss = from p in suma
                       join q in db on p.my_Abnormal_reason equals q.my_Abnormal_reason
                       select new
                       {
                           q.my_Abnormal_reason,
                           q.my_URI_Main,
                           q.URI_Main,
                           q.APN,
                           q.my_DEST_IP,
                           q.my_URI_Main_header,
                           q.cnt,
                           summary = p.sumary,
                           cnt_percent = 1.0 * q.cnt / p.sumary,
                        
                       };

            var dborder = dbss.OrderByDescending(e => e.summary)
                .ThenBy(e=>e.my_URI_Main).ThenBy(e=>e.URI_Main).ThenByDescending(e=>e.cnt);
            //OrderByDescending(e => e.summary).ThenBy(e => e.my_DEST_IP).
            gridControl1.DataSource = dborder.ToList();
            refreshGrid(dborder, 3);
        }

        private void navBarItem28_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs ee)
        {
            var dbss = from p in sqlserver_objectquery
                       where p.SynDirection == 0
                       //where url_list.Contains(p.URI_Main)
                       where p.APN != string.Empty
                       where p.Event_Type == 4
                       where p.Abnormal_reason != 0
                       select p;

            int all = dbss.Count();

            var suma = from p in sqlserver_objectquery
                       where p.SynDirection == 0
                       //where url_list.Contains(p.URI_Main)
                       where p.APN != string.Empty
                       where p.Event_Type == 4
                       where p.Abnormal_reason != 0
                       group p by new { p.my_Abnormal_reason } into ttt
                       select new
                       {
                           ttt.Key.my_Abnormal_reason,
                           cnt = ttt.Count(),
                           sumary=all,
                           cnt_percent=1.0*ttt.Count()/all,
                       };

            var dborder =suma.OrderByDescending(e => e.cnt);
            gridControl1.DataSource = dborder.ToList();
            refreshGrid(dborder, 0);
        }
    }
}
