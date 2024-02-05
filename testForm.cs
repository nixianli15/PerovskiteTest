using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TimeTech.DeviceDLL.Motion;

namespace PerovskiteTest
{
    public partial class testForm : Form  //测试长春吉莹位移台
    {
        public testForm()
        {
            InitializeComponent();
        }

        private void testForm_Load(object sender, EventArgs e)
        {
            //GlobalField.nimServoSDKBLL = new NimServoSDKBLL(new string[] { "2", ConfigurationManager.AppSettings["COM_device_1"] });
            //hothit();
        }

        private void hothit()
        {
            List<HeatPoint> points = new List<HeatPoint>();
            points.Add(new HeatPoint(0, 0, 100));
            points.Add(new HeatPoint(50, 0, 200));
            points.Add(new HeatPoint(100, 0, 150));
            points.Add(new HeatPoint(150, 0, 250));
            points.Add(new HeatPoint(200, 0, 50));

            pictureBox1.Image = new test(points).CreateBitmap();
        }

        private void PdfTest()
        {
            PdfDocument pdfDocument = new PdfDocument();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double temp1 = 0;
            if(double.TryParse(textBoxForNum1.Text,out temp1)) { }
                //GlobalField.nimServoSDKBLL.MoveAbsolute(temp1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            double temp1 = 0;
            if (double.TryParse(textBoxForNum1.Text, out temp1)) { }
                //GlobalField.nimServoSDKBLL.MoveRelative(temp1);
        }
    }
}
