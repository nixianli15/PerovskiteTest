using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TimeTech.Core;
using TimeTech.Core.Models;
using OfficeOpenXml; // 引用EPPlus库
using System.Diagnostics;
using System.IO;

namespace PerovskiteTest
{
    public partial class HeatMap : Form
    {
        List<HeatRange> heatRanges = new List<HeatRange>();
        private string curExcelFile="";

        public HeatMap()
        {
            InitializeComponent();
        }

        private void btnOutHeatMap_Click(object sender, EventArgs e)
        {
            // 创建 Stopwatch 实例
            Stopwatch stopwatch = new Stopwatch();
            // 启动计时器
            stopwatch.Start();
            try
            {
                heatRanges = new List<HeatRange>();
                CreateHeatRanges();
                FileLog.AddUserLog("---开始读取Excel:" + curExcelFile);
                CreateHeatMap();
            }
            catch (Exception)
            {

                throw;
            }
            stopwatch.Stop();
            // 获取经过的时间（秒）
            double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
            FileLog.AddUserLog($"测试耗时{elapsedSeconds}");
        }

        double minVolt = 0;
        double maxVolt = 0;
        private void CreateHeatRanges()
        {
            
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new System.IO.FileInfo(curExcelFile)))
            {
                // 获取第一个工作表（可以根据需要更改工作表索引）
                var worksheet = package.Workbook.Worksheets[0];
                for (int i = 1; i <= worksheet.Dimension.Columns; i++)
                {
                    // 获取列中的所有值
                    var columnValues = worksheet.Cells[1, i, worksheet.Dimension.End.Row, i]
                                                .Select(cell => Convert.ToDouble(cell.Text));
                    if (columnValues.Min()< minVolt)
                    {
                        if (columnValues.Min()>0) minVolt = columnValues.Min();

                    }

                    if (columnValues.Max()> maxVolt) { maxVolt=columnValues.Max();}
                }

            }
            //for (int i = 1; i <= 5; i++)
            //{
            //    heatRanges.Add(GetHeatRangeMaxMin(i));
            //}
            for (int i = 1; i <= 3; i++)
            {
                heatRanges.Add(GetHeatRangeMaxMin(i));
            }
        }

        private HeatRange GetHeatRange( int i)
        {
            HeatRange heatRange = new HeatRange() { };
            double uTemp = 0;
            double.TryParse(((System.Windows.Forms.TextBox)(this.Controls.Find($"txtVolt{i}", true)[0])).Text, out uTemp);
           
            heatRange.max = uTemp;
            return heatRange;
        }

        private HeatRange GetHeatRangeMaxMin(int i)
        {
            //HeatRange heatRange = new HeatRange() { };
            //double uTemp = 0;
            //uTemp = i*(maxVolt - minVolt) / 5 + minVolt;

            //heatRange.max = uTemp;
            //return heatRange;
            HeatRange heatRange = new HeatRange() { };
            double uTemp = 0;
            uTemp = i * (maxVolt - minVolt) / 3 + minVolt;

            heatRange.max = uTemp;
            return heatRange;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }
        Color[] gradientColors;
        Color[] rainbowColors;
        private void CreateHeatMap()
        {
            // 生成黄蓝二阶渐变色
            gradientColors = GenerateYellowBlueGradient(1024);
            // 生成彩虹渐变色
            rainbowColors = GenerateRainbowColors(1024);
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new System.IO.FileInfo(curExcelFile)))
            {
                // 获取第一个工作表（可以根据需要更改工作表索引）
                var worksheet = package.Workbook.Worksheets[0];
                int rowCount = 0;
                // 获取有数据的行数和列数
                if (worksheet.Dimension.Rows<1000)
                {
                   rowCount = worksheet.Dimension.Rows;
                }
                else
                {
                    rowCount = 1000;
                }
                
                int columnCount = worksheet.Dimension.Columns;
                Color color = Color.FromArgb(25, 24, 188);
                Image bitmap = new System.Drawing.Bitmap(columnCount, rowCount);
                //Image bitmap = new System.Drawing.Bitmap(1000, 1000);
                Graphics g = System.Drawing.Graphics.FromImage(bitmap);
                for (int i = 1; i <= rowCount; i++) 
                { 
                    for (int j = 1; j <= columnCount; j++) 
                    {
                        if (worksheet.Cells[i, j].Text.Length > 0)
                        {
                            color = GetColor4(worksheet.Cells[i, j].Value);
                            SolidBrush brush = new SolidBrush(color);
                            //Rectangle ret = new Rectangle(j * 1000 / columnCount, i * 1000 / rowCount, 1000 / columnCount, 1000 / rowCount);
                            Rectangle ret = new Rectangle(j , i , 1, 1);
                            //Rectangle ret = new Rectangle(j , i , 1, 1);
                            g.FillRectangle(brush, ret);
                        }
                        
                    }
                }

                FileLog.AddUserLog("------------------" + "热力图生成结束---------------------");
                pbTSD.Image = bitmap;
                bitmap.Save(curExcelFile.Substring(0, curExcelFile.LastIndexOf('\\') + 1)  + Path.GetFileName(curExcelFile) + ".png");
            }
        }


        #region 黄蓝二阶RGB算法

        private Color GetColor4(object value)
        {          
            double i = 0.0; // 默认值，可以根据需要修改
            if (double.TryParse(value.ToString(), out i))
            {
            }
            if (i < 0) i = 0.0;
            return gradientColors[1023-(int)(1023 * ((i - minVolt) / maxVolt))];
        }


        static Color[] GenerateYellowBlueGradient(int numColors)
        {
            Color[] colors = new Color[numColors];

            for (int i = 0; i < numColors; i++)
            {
                double t = i / (double)(numColors - 1);

                colors[i] = InterpolateColor(t, Color.Yellow, Color.Blue);
            }

            return colors;
        }

        static Color InterpolateColor(double t, Color color1, Color color2)
        {
            int r = Interpolate(color1.R, color2.R, t);
            int g = Interpolate(color1.G, color2.G, t);
            int b = Interpolate(color1.B, color2.B, t);

            return Color.FromArgb(r, g, b);
        }

        static int Interpolate(int value1, int value2, double t)
        {
            return (int)(value1 + (value2 - value1) * t);
        }

        #endregion


        #region 黄绿蓝三阶rgb算法

        private Color GetColor6(object value)
        {
            // 生成彩虹渐变色
            //Color[] rainbowColors = GenerateRainbowColors(256);

            // 输出一些颜色值
            double i = 0.0; // 默认值，可以根据需要修改
            if (double.TryParse(value.ToString(), out i))
            {
            }
            if (i < 0) i = 0.0;
            return rainbowColors[1023 - (int)(1023 * ((i - minVolt) / maxVolt))];

        }

        static Color[] GenerateRainbowColors(int numColors)
        {
            Color[] colors = new Color[numColors];

            for (int i = 0; i < numColors; i++)
            {
                double t = i / (double)(numColors - 1);

                colors[i] = InterpolateRainbowColor(t);
            }

            return colors;
        }

        static Color InterpolateRainbowColor(double t)
        {
            int r, g, b;

            if (t < 1.0 / 3)
            {
                // Red to Yellow
                r = 255;
                g = Interpolate(0, 255, t * 3);
                b = 0;
            }
            else if (t < 2.0 / 3)
            {
                // Yellow to Green
                r = Interpolate(255, 0, (t - 1.0 / 3) * 3);
                g = 255;
                b = 0;
            }
            else
            {
                // Green to Blue
                r = 0;
                g = 255 - Interpolate(0, 255, (t - 2.0 / 3) * 3);
                b = Interpolate(0, 255, (t - 2.0 / 3) * 3);
            }

            return Color.FromArgb(r, g, b);
        }

        static int Interpolate2(int value1, int value2, double t)
        {
            return (int)(value1 + (value2 - value1) * t);
        }

        #endregion

        private void textBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                curExcelFile = dialog.FileName;
                //GetMaxTotal44(dialog.FileName);                
            }
        }
    }
}
