using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraCharts;
using System.Drawing;
using OfflineInspect.CommonTools;

namespace ViewInspect
{
    public partial class Form1 : Form
    {
        private void button1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox7.Text + "\r\n" +
                         textBox1.Text + "\r\n" +
                  textBox2.Text + "\r\n" +
                  textBox3.Text + "\r\n" +
                  textBox4.Text + "\r\n" +
                  textBox5.Text + "\r\n" +
                  textBox6.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox7.Text + "\r\n" +
          textBox1.Text.GetHeader(",时间序列") + "\r\n" +
             textBox2.Text.GetHeader(",时间序列") + "\r\n" +
          textBox3.Text.GetHeader(",时间序列") + "\r\n" +
          textBox4.Text.GetHeader(",时间序列") + "\r\n" +
          textBox5.Text.GetHeader(",时间序列") + "\r\n" +
          textBox6.Text.GetHeader(",时间序列"));
        }


        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Series series in chartControl1.Series)
                if (series.Label != null)
                    series.Label.Visible = checkBox1.Checked;
        }

        private void gridControl1_Click(object sender, EventArgs ee)
        {
            if (checkBox1.Checked)
                show_graphic_fc_ms();
            if (checkBox2.Checked)
                show_graphic_fc_bvc();
            return;
        }

        private void ClearChart()
        {
            var diagram = chartControl1.Diagram as XYDiagram;
            if (diagram != null)
            {
                diagram.SecondaryAxesY.Clear();
                diagram.SecondaryAxesX.Clear();
                diagram.Panes.Clear();
                diagram.AxisY.Range.Auto = true;
                diagram.AxisY.Range.Auto = true;

            }
            chartControl1.Series.Clear();
            chartControl1.Titles.Clear();
        }

        private void show_graphic_fc_ms()
        {
            ClearChart();

            int[] a = this.gridView1.GetSelectedRows(); //传递实体类过去 获取选中的行
            string fd = this.gridView1.GetRowCellValue(a[0], "first_delay").ToString();//获取选中行的内容
            string lr = this.gridView1.GetRowCellValue(a[0], "leak_rate").ToString();//获取选中行的内容
            string bs = this.gridView1.GetRowCellValue(a[0], "bucket_size").ToString();//获取选中行的内容
            string len = this.gridView1.GetRowCellValue(a[0], "down_total_len").ToString();//获取选中行的内容
            string pr = this.gridView1.GetRowCellValue(a[0], "down_packet_rate").ToString();//获取选中行的内容
            string times = this.gridView1.GetRowCellValue(a[0], "fcm_time").ToString();//获取选中行的内容
            string callid = this.gridView1.GetRowCellValue(a[0], "BeginFrameNum").ToString();//获取选中行的内容

            var arrfd = fd.Split(',');
            var arrlr = lr.Split(',');
            var arrbs = bs.Split(',');
            var arrlen = len.Split(',');
            var arrpr = pr.Split(',');
            var arrts = times.Split(',');

            textBox7.Text = "Callid=" + callid;

            textBox1.Text = "first_delay(flow-control-ms发送时间序列second)：平均值=" + arrfd.Where(e => e != "").Average(e => double.Parse(e)).ToString("f1") + "," +
                                        "最大值=" + arrfd.Where(e => e != "").Max(e => double.Parse(e)).ToString("f1") + "," +
                                        "最小值" + arrfd.Where(e => e != "").Min(e => double.Parse(e)).ToString("f1") + "," +
                                        "时间序列=" + fd;
            textBox2.Text = "leak_rate(flow-control-ms参数值kbps)：平均值=" + arrlr.Where(e => e != "").Average(e => double.Parse(e)).ToString("f1") + "," +
                                        "最大值=" + arrlr.Where(e => e != "").Max(e => double.Parse(e)).ToString("f1") + "," +
                                        "最小值" + arrlr.Where(e => e != "").Min(e => double.Parse(e)).ToString("f1") + "," +
                                        "时间序列=" + lr;
            textBox3.Text = "bucket_size(flow-control-ms参数值Kbyte)：平均值=" + arrbs.Where(e => e != "").Average(e => double.Parse(e)).ToString("f1") + "," +
                                        "最大值=" + arrbs.Where(e => e != "").Max(e => double.Parse(e)).ToString("f1") + "," +
                                        "最小值" + arrbs.Where(e => e != "").Min(e => double.Parse(e)).ToString("f1") + "," +
                                        "时间序列=" + bs;
            textBox4.Text = "down_total_len(flow-control-ms之间MS下行包总长度Kbyte)：平均值=" + arrlen.Where(e => e != "").Average(e => double.Parse(e)).ToString("f1") + "," +
                                        "最大值=" + arrlen.Where(e => e != "").Max(e => double.Parse(e)).ToString("f1") + "," +
                                        "最小值" + arrlen.Where(e => e != "").Min(e => double.Parse(e)).ToString("f1") + "," +
                                        "时间序列=" + len;
            textBox5.Text = "fcm_time(flow-control-ms发送时间间隔second)：平均值=" + arrts.Where(e => e != "").Average(e => double.Parse(e)).ToString("f1") + "," +
                                        "最大值=" + arrts.Where(e => e != "").Max(e => double.Parse(e)).ToString("f1") + "," +
                                        "最小值" + arrts.Where(e => e != "").Min(e => double.Parse(e)).ToString("f1") + "," +
                                        "时间序列=" + times;
            textBox6.Text = "down_packet_rate(flow-control-ms之间MS下行速率kbps)：平均值=" + arrpr.Where(e => e != "").Average(e => double.Parse(e)).ToString("f1") + "," +
                                        "最大值=" + arrpr.Where(e => e != "").Max(e => double.Parse(e)).ToString("f1") + "," +
                                        "最小值" + arrpr.Where(e => e != "").Min(e => double.Parse(e)).ToString("f1") + "," +
                                        "时间序列=" + pr;


            Series series1 = new Series("leak_rate(kbps)", ViewType.Line);
            Series series4 = new Series("down_packet_rate(kbps)", ViewType.Line);

            Series series2 = new Series("bucket_size(KByte)", ViewType.Line);
            Series series3 = new Series("down_total_len(KByte)", ViewType.Line);

            Series series5 = new Series("fcm_time(seconds)", ViewType.Line);

            double x, y, z, m, n, k;
            for (int i = 0; i < arrfd.Count(); i++)
            {
                x = double.Parse(arrfd[i]);
                y = double.Parse(arrlr[i]);
                z = double.Parse(arrbs[i]);
                m = double.Parse(arrlen[i]);
                n = double.Parse(arrpr[i]);
                k = double.Parse(arrts[i]);
                series1.Points.Add(new SeriesPoint(x, y));
                series2.Points.Add(new SeriesPoint(x, z));
                series3.Points.Add(new SeriesPoint(x, m));
                series4.Points.Add(new SeriesPoint(x, n));
                series5.Points.Add(new SeriesPoint(x, k));
            }

            chartControl1.Series.Add(series1);
            chartControl1.Series.Add(series4);


            chartControl1.Series.Add(series3);
            chartControl1.Series.Add(series2);

            chartControl1.Series.Add(series5);


            ChartTitle chartTitle1 = new ChartTitle();
            chartTitle1.Antialiasing = true;
            chartTitle1.Font = new Font("Tahoma", 12, FontStyle.Bold);
            chartTitle1.Text = string.Format("Callid={0} down_packet_rate和FLOW-CONTROL-MS的Bucket_Size、Leak_Rate时间走势", callid);
            chartControl1.Titles.Add(chartTitle1);


            series1.Label.Visible = true;
            series2.Label.Visible = true;
            series3.Label.Visible = true;
            series4.Label.Visible = true;
            series5.Label.Visible = true;


            //chartControl1.Legend.TextColor = series1.Label.LineColor;
            chartControl1.Legend.Antialiasing = true;
            chartControl1.Legend.Font = new Font("Tahoma", 10, FontStyle.Bold);

            XYDiagram diagram = (XYDiagram)chartControl1.Diagram;

            // Customize the appearance of the Y-axis title.
            diagram.AxisY.Title.Visible = true;
            diagram.AxisY.Title.Alignment = StringAlignment.Center;
            diagram.AxisY.Title.Text = "kbps";
            diagram.AxisY.Title.TextColor = Color.Blue;
            diagram.AxisY.Title.Antialiasing = true;
            diagram.AxisY.Title.Font = new Font("Tahoma", 10, FontStyle.Bold);

            diagram.AxisX.Visible = false;


            // Add secondary axes to the diagram, and adjust their options.
            diagram.SecondaryAxesX.Add(new SecondaryAxisX("My Axis X"));
            diagram.SecondaryAxesY.Add(new SecondaryAxisY("My Axis Y"));
            diagram.SecondaryAxesX[0].Alignment = AxisAlignment.Near;
            diagram.SecondaryAxesY[0].Alignment = AxisAlignment.Near;

            // Add a new additional pane to the diagram.
            diagram.Panes.Add(new XYDiagramPane("My Pane"));

            // Assign both the additional pane and, if required,
            // the secondary axes to the second series. 
            LineSeriesView myView = (LineSeriesView)series2.View;
            myView.AxisX = diagram.SecondaryAxesX[0];
            myView.AxisY = diagram.SecondaryAxesY[0];
            // Note that the created pane has the zero index in the collection,
            // because the existing Default pane is a separate entity.
            myView.Pane = diagram.Panes[0];

            // Assign both the additional pane and, if required,
            // the secondary axes to the second series. 
            myView = (LineSeriesView)series3.View;
            myView.AxisX = diagram.SecondaryAxesX[0];
            myView.AxisY = diagram.SecondaryAxesY[0];
            // Note that the created pane has the zero index in the collection,
            // because the existing Default pane is a separate entity.
            myView.Pane = diagram.Panes[0];

            // Customize the appearance of the Y-axis title.
            myView.AxisY.Title.Visible = true;
            myView.AxisY.Title.Alignment = StringAlignment.Center;
            myView.AxisY.Title.Text = "KByte";
            myView.AxisY.Title.TextColor = Color.Blue;
            myView.AxisY.Title.Antialiasing = true;
            myView.AxisY.Title.Font = new Font("Tahoma", 10, FontStyle.Bold);

            myView.AxisX.Visible = false;

            // Add secondary axes to the diagram, and adjust their options.
            diagram.SecondaryAxesX.Add(new SecondaryAxisX("My Axis X1"));
            diagram.SecondaryAxesY.Add(new SecondaryAxisY("My Axis Y1"));
            diagram.SecondaryAxesX[1].Alignment = AxisAlignment.Near;
            diagram.SecondaryAxesY[1].Alignment = AxisAlignment.Near;

            // Add a new additional pane to the diagram.
            diagram.Panes.Add(new XYDiagramPane("My Pane1"));
            // Assign both the additional pane and, if required,
            // the secondary axes to the second series. 
            LineSeriesView myView1 = (LineSeriesView)series5.View;
            myView1.AxisX = diagram.SecondaryAxesX[1];
            myView1.AxisY = diagram.SecondaryAxesY[1];
            // Note that the created pane has the zero index in the collection,
            // because the existing Default pane is a separate entity.
            myView1.Pane = diagram.Panes[1];


            // Customize the appearance of the Y-axis title.
            myView1.AxisY.Title.Visible = true;
            myView1.AxisY.Title.Alignment = StringAlignment.Center;
            myView1.AxisY.Title.Text = "second";
            myView1.AxisY.Title.TextColor = Color.Blue;
            myView1.AxisY.Title.Antialiasing = true;
            myView1.AxisY.Title.Font = new Font("Tahoma", 10, FontStyle.Bold);

            // Customize the appearance of the X-axis title.
            myView1.AxisX.Title.Visible = true;
            myView1.AxisX.Title.Alignment = StringAlignment.Center;
            myView1.AxisX.Title.Text = "FLOW-CONTROL-MS delay(seconds)";
            myView1.AxisX.Title.TextColor = Color.Red;
            myView1.AxisX.Title.Antialiasing = true;
            myView1.AxisX.Title.Font = new Font("Tahoma", 10, FontStyle.Bold);
            myView1.AxisX.Label.Staggered = true;

            // Customize the layout of the diagram's panes.
            diagram.PaneDistance = 10;
            diagram.PaneLayoutDirection = PaneLayoutDirection.Vertical;
            diagram.DefaultPane.SizeMode = PaneSizeMode.UseWeight;
            diagram.DefaultPane.Weight = 1.2;

            diagram.SecondaryAxesY[0].LogarithmicBase = 2;
            diagram.SecondaryAxesY[0].Logarithmic = true;
            diagram.AxisY.LogarithmicBase = 2;
            diagram.AxisY.Logarithmic = true;
        }

        private void show_graphic_fc_bvc()
        {
            ClearChart();

            int[] a = this.gridView1.GetSelectedRows(); //传递实体类过去 获取选中的行
            string fd = this.gridView1.GetRowCellValue(a[0], "fcb_delay_aggre").ToString();//获取选中行的内容
            string lr = this.gridView1.GetRowCellValue(a[0], "bssgp_bucket_leak_rate").ToString();//获取选中行的内容
            string bs = this.gridView1.GetRowCellValue(a[0], "bssgp_bvc_bucket_size").ToString();//获取选中行的内容
            string len = this.gridView1.GetRowCellValue(a[0], "down_total_len").ToString();//获取选中行的内容
            string pr = this.gridView1.GetRowCellValue(a[0], "down_packet_rate").ToString();//获取选中行的内容
            string times = this.gridView1.GetRowCellValue(a[0], "fcb_time_aggre").ToString();//获取选中行的内容
            string callid = this.gridView1.GetRowCellValue(a[0], "lac_cell").ToString();//获取选中行的内容
            string bvci = this.gridView1.GetRowCellValue(a[0], "bvci_aggre").ToString();//获取选中行的内容

            var arrfd = fd.Split(',');
            var arrlr = lr.Split(',');
            var arrbs = bs.Split(',');
            var arrlen = len.Split(',');
            var arrpr = pr.Split(',');
            var arrts = times.Split(',');

            textBox7.Text = "Cellid=" + callid + ",BVCI=" + bvci;

            textBox1.Text = "fcb_delay(flow-control-bvc发送时间序列second)：平均值=" + arrfd.Where(e => e != "").Average(e => double.Parse(e)).ToString("f1") + "," +
                                        "最大值=" + arrfd.Where(e => e != "").Max(e => double.Parse(e)).ToString("f1") + "," +
                                        "最小值" + arrfd.Where(e => e != "").Min(e => double.Parse(e)).ToString("f1") + "," +
                                        "时间序列=" + fd;
            textBox2.Text = "bvc_leak_rate(flow-control-bvc参数值kbps)：平均值=" + arrlr.Where(e => e != "").Average(e => double.Parse(e)).ToString("f1") + "," +
                                        "最大值=" + arrlr.Where(e => e != "").Max(e => double.Parse(e)).ToString("f1") + "," +
                                        "最小值" + arrlr.Where(e => e != "").Min(e => double.Parse(e)).ToString("f1") + "," +
                                        "时间序列=" + lr;
            textBox3.Text = "bvc_bucket_size(flow-control-bvc参数值Kbyte)：平均值=" + arrbs.Where(e => e != "").Average(e => double.Parse(e)).ToString("f1") + "," +
                                        "最大值=" + arrbs.Where(e => e != "").Max(e => double.Parse(e)).ToString("f1") + "," +
                                        "最小值" + arrbs.Where(e => e != "").Min(e => double.Parse(e)).ToString("f1") + "," +
                                        "时间序列=" + bs;
            textBox4.Text = "down_total_len(flow-control-bvc之间bvc下行包总长度Kbyte)：平均值=" + arrlen.Where(e => e != "").Average(e => double.Parse(e)).ToString("f1") + "," +
                                        "最大值=" + arrlen.Where(e => e != "").Max(e => double.Parse(e)).ToString("f1") + "," +
                                        "最小值" + arrlen.Where(e => e != "").Min(e => double.Parse(e)).ToString("f1") + "," +
                                        "时间序列=" + len;
            textBox5.Text = "fcb_time(flow-control-bvc发送时间间隔second)：平均值=" + arrts.Where(e => e != "").Average(e => double.Parse(e)).ToString("f1") + "," +
                                        "最大值=" + arrts.Where(e => e != "").Max(e => double.Parse(e)).ToString("f1") + "," +
                                        "最小值" + arrts.Where(e => e != "").Min(e => double.Parse(e)).ToString("f1") + "," +
                                        "时间序列=" + times;
            textBox6.Text = "down_packet_rate(flow-control-bvc之间bvc下行速率kbps)：平均值=" + arrpr.Where(e => e != "").Average(e => double.Parse(e)).ToString("f1") + "," +
                                        "最大值=" + arrpr.Where(e => e != "").Max(e => double.Parse(e)).ToString("f1") + "," +
                                        "最小值" + arrpr.Where(e => e != "").Min(e => double.Parse(e)).ToString("f1") + "," +
                                        "时间序列=" + pr;

            Series series1 = new Series("bvc_leak_rate(kbps)", ViewType.Line);
            Series series4 = new Series("down_packet_rate(kbps)", ViewType.Line);

            Series series2 = new Series("bvc_bucket_size(KByte)", ViewType.Line);
            Series series3 = new Series("down_total_len(KByte)", ViewType.Line);

            Series series5 = new Series("fcb_time(seconds)", ViewType.Line);

            double x, y, z, m, n, k;
            for (int i = 0; i < arrfd.Count(); i++)
            {
                x = double.Parse(arrfd[i]);
                y = double.Parse(arrlr[i]);
                z = double.Parse(arrbs[i]);
                m = double.Parse(arrlen[i]);
                n = double.Parse(arrpr[i]);
                k = double.Parse(arrts[i]);
                series1.Points.Add(new SeriesPoint(x, y));
                series2.Points.Add(new SeriesPoint(x, z));
                series3.Points.Add(new SeriesPoint(x, m));
                series4.Points.Add(new SeriesPoint(x, n));
                series5.Points.Add(new SeriesPoint(x, k));
            }

            chartControl1.Series.Add(series1);
            chartControl1.Series.Add(series4);


            chartControl1.Series.Add(series3);
            chartControl1.Series.Add(series2);

            chartControl1.Series.Add(series5);

            ChartTitle chartTitle1 = new ChartTitle();
            chartTitle1.Antialiasing = true;
            chartTitle1.Font = new Font("Tahoma", 12, FontStyle.Bold);
            chartTitle1.Text = string.Format("Cellid={0},BVCI={1} down_packet_rate和FLOW-CONTROL-BVC的Bucket_Size、Leak_Rate时间走势", callid, bvci);
            chartControl1.Titles.Add(chartTitle1);

            series1.Label.Visible = true;
            series2.Label.Visible = true;
            series3.Label.Visible = true;
            series4.Label.Visible = true;
            series5.Label.Visible = true;

            //chartControl1.Legend.TextColor = series1.Label.LineColor;
            chartControl1.Legend.Antialiasing = true;
            chartControl1.Legend.Font = new Font("Tahoma", 10, FontStyle.Bold);

            XYDiagram diagram = (XYDiagram)chartControl1.Diagram;

            // Customize the appearance of the Y-axis title.
            diagram.AxisY.Title.Visible = true;
            diagram.AxisY.Title.Alignment = StringAlignment.Center;
            diagram.AxisY.Title.Text = "kbps";
            diagram.AxisY.Title.TextColor = Color.Blue;
            diagram.AxisY.Title.Antialiasing = true;
            diagram.AxisY.Title.Font = new Font("Tahoma", 10, FontStyle.Bold);

            diagram.AxisX.Visible = false;

            // Add secondary axes to the diagram, and adjust their options.
            diagram.SecondaryAxesX.Add(new SecondaryAxisX("My Axis X"));
            diagram.SecondaryAxesY.Add(new SecondaryAxisY("My Axis Y"));
            diagram.SecondaryAxesX[0].Alignment = AxisAlignment.Near;
            diagram.SecondaryAxesY[0].Alignment = AxisAlignment.Near;

            // Add a new additional pane to the diagram.
            diagram.Panes.Add(new XYDiagramPane("My Pane"));

            // Assign both the additional pane and, if required,
            // the secondary axes to the second series. 
            LineSeriesView myView = (LineSeriesView)series2.View;
            myView.AxisX = diagram.SecondaryAxesX[0];
            myView.AxisY = diagram.SecondaryAxesY[0];
            // Note that the created pane has the zero index in the collection,
            // because the existing Default pane is a separate entity.
            myView.Pane = diagram.Panes[0];

            // Assign both the additional pane and, if required,
            // the secondary axes to the second series. 
            myView = (LineSeriesView)series3.View;
            myView.AxisX = diagram.SecondaryAxesX[0];
            myView.AxisY = diagram.SecondaryAxesY[0];
            // Note that the created pane has the zero index in the collection,
            // because the existing Default pane is a separate entity.
            myView.Pane = diagram.Panes[0];

            // Customize the appearance of the Y-axis title.
            myView.AxisY.Title.Visible = true;
            myView.AxisY.Title.Alignment = StringAlignment.Center;
            myView.AxisY.Title.Text = "KByte";
            myView.AxisY.Title.TextColor = Color.Blue;
            myView.AxisY.Title.Antialiasing = true;
            myView.AxisY.Title.Font = new Font("Tahoma", 10, FontStyle.Bold);

            myView.AxisX.Visible = false;

            // Add secondary axes to the diagram, and adjust their options.
            diagram.SecondaryAxesX.Add(new SecondaryAxisX("My Axis X1"));
            diagram.SecondaryAxesY.Add(new SecondaryAxisY("My Axis Y1"));
            diagram.SecondaryAxesX[1].Alignment = AxisAlignment.Near;
            diagram.SecondaryAxesY[1].Alignment = AxisAlignment.Near;

            // Add a new additional pane to the diagram.
            diagram.Panes.Add(new XYDiagramPane("My Pane1"));
            // Assign both the additional pane and, if required,
            // the secondary axes to the second series. 
            LineSeriesView myView1 = (LineSeriesView)series5.View;
            myView1.AxisX = diagram.SecondaryAxesX[1];
            myView1.AxisY = diagram.SecondaryAxesY[1];
            // Note that the created pane has the zero index in the collection,
            // because the existing Default pane is a separate entity.
            myView1.Pane = diagram.Panes[1];

            // Customize the appearance of the Y-axis title.
            myView1.AxisY.Title.Visible = true;
            myView1.AxisY.Title.Alignment = StringAlignment.Center;
            myView1.AxisY.Title.Text = "second";
            myView1.AxisY.Title.TextColor = Color.Blue;
            myView1.AxisY.Title.Antialiasing = true;
            myView1.AxisY.Title.Font = new Font("Tahoma", 10, FontStyle.Bold);

            // Customize the appearance of the X-axis title.
            myView1.AxisX.Title.Visible = true;
            myView1.AxisX.Title.Alignment = StringAlignment.Center;
            myView1.AxisX.Title.Text = "FLOW-CONTROL-BVC delay(seconds)";
            myView1.AxisX.Title.TextColor = Color.Red;
            myView1.AxisX.Title.Antialiasing = true;
            myView1.AxisX.Title.Font = new Font("Tahoma", 10, FontStyle.Bold);
            myView1.AxisX.Label.Staggered = true;

            // Customize the layout of the diagram's panes.
            diagram.PaneDistance = 10;
            diagram.PaneLayoutDirection = PaneLayoutDirection.Vertical;
            diagram.DefaultPane.SizeMode = PaneSizeMode.UseWeight;
            diagram.DefaultPane.Weight = 1.2;

            diagram.SecondaryAxesY[0].LogarithmicBase = 2;
            diagram.SecondaryAxesY[0].Logarithmic = true;
            diagram.AxisY.LogarithmicBase = 2;
            diagram.AxisY.Logarithmic = true;

        }
    }
}
