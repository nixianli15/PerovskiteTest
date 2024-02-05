using HalconDotNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TimeTech.DeviceDLL.OPR;

namespace PerovskiteTest
{
    public partial class testAI : Form
    {
        public testAI()
        {
            InitializeComponent();
            GlobalField.hDevelopExport = new HDevelopExport();
            GlobalField.defectIdent1 = new DefectIdent(AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["MODEL_SEG_1"]);  //明场模型
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap btAIDefect = (Bitmap)Image.FromFile("D:\\ORGIMG\\0001.png"), clipPic = null, bitmap2 = null, bitDefImage = null;
            Bitmap btDefOrg= (Bitmap)btAIDefect.Clone();
            Bitmap bitmapForAi = (Bitmap)btAIDefect.Clone();
            string PhotoPath = @"D:\ORGIMG\hobj\";
            List<string> list0= new List<string>();
            List<Halcon_DefectIdentInfo> list = new List<Halcon_DefectIdentInfo>();
            int[] hv_Area = null;

            GlobalField.hDevelopExport.action(btAIDefect, ref clipPic, PhotoPath + $"{1}.hobj");   //clipPic 扣边旋转图  位置信息文件

            list0 = GlobalField.defectIdent1.GetDefectIdentResultForPerovskite(ref bitmapForAi, ref bitmap2, ref list);  //原图转黑白图bitmapForAi    bitmap2 原图转易渊彩图
            btDefOrg.Save("D:\\ORGIMG\\51.png");
            GlobalField.hDevelopExport.actionShow( btDefOrg, PhotoPath + $"{1}.hobj",  bitmap2, ref bitDefImage);


            GlobalField.hDevelopExport.actionArea(bitmapForAi, ref hv_Area);

            clipPic.Save("D:\\ORGIMG\\11.png");
            bitmapForAi.Save("D:\\ORGIMG\\21.png");
            bitmap2.Save("D:\\ORGIMG\\31.png");
            bitDefImage.Save("D:\\ORGIMG\\41.png");

            btAIDefect.Dispose();
            clipPic.Dispose();
            bitmap2.Dispose();
            bitDefImage.Dispose();
            bitmapForAi.Dispose();
            btDefOrg.Dispose();
        }
    }
}
