using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PerovskiteTest
{
    public partial class frmtest : Form
    {
        public frmtest()
        {
            InitializeComponent();
        }

        bool openOrClose;
        private void button1_Click(object sender, EventArgs e)
        {
            //电磁铁滤光片不挡住镜头
            if (openOrClose)
            {
                GlobalField.tcpConnectProvider.AxisIndex = "F2";
                GlobalField.tcpConnectProvider.SendFunCode("07");
                openOrClose = false;
            }
            else
            {
                GlobalField.tcpConnectProvider.AxisIndex = "F2";
                GlobalField.tcpConnectProvider.SendFunCode("06");
                openOrClose = true;
            }
                


        }
    }
}
