using Aspose.Cells.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TimeTech.Core.DataProvider;

namespace PerovskiteTest
{
    public partial class InputID : Form
    {
        public InputID()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GlobalField.testNum = 1;
            GlobalField.productInfo.ProID =txtSerialNum.Text.Trim();  //电池序列号
            GlobalField.productInfo.Manufacturer = txtOperator.Text.Trim();
            GlobalField.productInfo.Remark = txtRemark.Text.Trim();
            GlobalField.savePath = AppDomain.CurrentDomain.BaseDirectory + $"IMG\\{txtSerialNum.Text}\\";

            if (!Directory.Exists(GlobalField.savePath+ "明场\\ORGIMG\\"))
                Directory.CreateDirectory(GlobalField.savePath+ "明场\\ORGIMG\\");
            if (!Directory.Exists(GlobalField.savePath + "明场\\Thumbnail\\"))
                Directory.CreateDirectory(GlobalField.savePath + "明场\\Thumbnail\\");
            if (!Directory.Exists(GlobalField.savePath + "明场\\Defect\\"))
                Directory.CreateDirectory(GlobalField.savePath + "明场\\Defect\\");
            if (!Directory.Exists(GlobalField.savePath + "明场\\AI\\"))
                Directory.CreateDirectory(GlobalField.savePath + "明场\\AI\\");

            if (!Directory.Exists(GlobalField.savePath + "荧光\\ORGIMG\\"))
                Directory.CreateDirectory(GlobalField.savePath + "荧光\\ORGIMG\\");
            if (!Directory.Exists(GlobalField.savePath + "荧光\\Thumbnail\\"))
                Directory.CreateDirectory(GlobalField.savePath + "荧光\\Thumbnail\\");
            if (!Directory.Exists(GlobalField.savePath + "荧光\\Defect\\"))
                Directory.CreateDirectory(GlobalField.savePath + "荧光\\Defect\\");
            if (!Directory.Exists(GlobalField.savePath + "荧光\\AI\\"))
                Directory.CreateDirectory(GlobalField.savePath + "荧光\\AI\\");

            if (!Directory.Exists(GlobalField.savePath + "EL\\ORGIMG\\"))
                Directory.CreateDirectory(GlobalField.savePath + "EL\\ORGIMG\\");
            if (!Directory.Exists(GlobalField.savePath + "EL\\Thumbnail\\"))
                Directory.CreateDirectory(GlobalField.savePath + "EL\\Thumbnail\\");
            if (!Directory.Exists(GlobalField.savePath + "EL\\Defect\\"))
                Directory.CreateDirectory(GlobalField.savePath + "EL\\Defect\\");
            if (!Directory.Exists(GlobalField.savePath + "EL\\AI\\"))
                Directory.CreateDirectory(GlobalField.savePath + "EL\\AI\\");
            //EntityHelper.ExecSqlString($"INSERT INTO sicinfo(serialNum,sicinfo,Operator,SavePath) VALUES ('{GlobalField.serialNum}','{txtRemark.Text.Trim()}','{txtOperator.Text.Trim()}','{GlobalField.savePath.Replace("\\", "\\\\")}') ON DUPLICATE KEY UPDATE sicinfo = values(sicinfo),Operator = values(Operator),SavePath = values(SavePath);");
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GlobalField.testNum = 99;
            this.Close();
        }

        private void InputID_Load(object sender, EventArgs e)
        {
            txtSerialNum.Text = DateTime.Now.ToString("yyyyMMdd");
            GlobalField.productInfo = new ProductInfo();
            radioButton1.Checked = true; radioButton2.Checked = false; GlobalField.productInfo.ProSize = radioButton1.Text;
            //string savePath = $"IMG/{txtSerialNum.Text}/";
            //if (!Directory.Exists(savePath))
            //    Directory.CreateDirectory(savePath);
            //txtSavePath.Text = GlobalField.savePath = AppDomain.CurrentDomain.BaseDirectory + $"IMG\\{txtSerialNum.Text}\\";
        }

        private void txtSavePath_DoubleClick(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                String foldPath = dialog.SelectedPath.Trim('\\') + "\\";
                txtSavePath.Text = foldPath;
            }
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            radioButton1.Checked = true; radioButton2.Checked = false; GlobalField.productInfo.ProSize = radioButton1.Text;
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            radioButton2.Checked = true; radioButton1.Checked = false; GlobalField.productInfo.ProSize = radioButton2.Text;
        }

    }
}
