using PdfSharp.Pdf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TimeTech.Core.Common;
using TimeTech.DeviceDLL.Camera;
using TimeTech.DeviceDLL.Light;
using TimeTech.DeviceDLL.Motion;
using TimeTech.DeviceDLL.SMU;
using TimeTech.Core;
using HalconDotNet;
using PerovskiteTest.userControl;
using System.Drawing.Drawing2D;
using NationalInstruments.DAQmx;
using System.IO.Ports;

namespace PerovskiteTest
{

    public partial class Form1 : Form
    {
        HalconProvider halconProvider = new HalconProvider();
        private FileAndFolder fileAndFolder;
        Image imageRed = Image.FromFile("red1.png");
        Image imageGreen = Image.FromFile("green1.png");
        #region 初始化事件
        public Form1()
        {
            InitializeComponent();
            if (LoadData())
            {
                using (InputID form = new InputID())
                {
                    form.StartPosition = FormStartPosition.CenterScreen;
                    form.ShowDialog();
                }
            }
            if (GlobalField.testNum == 1)
            {
                var aa = MessageBox.Show("是否进入参数设置界面？", "Tips", MessageBoxButtons.OKCancel);
                if (aa == DialogResult.OK)
                {
                    using (FmSet form = new FmSet())
                    {
                        form.StartPosition = FormStartPosition.CenterScreen;
                        form.ShowDialog();
                    }
                }
            }
            // LoadData();
            GlobalField.logText += $"系统初始化完成{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\r\n";
        }
        private void ControlInit()
        {
            pictureBox3.Image = pictureBox4.Image = pictureBox5.Image = pictureBox6.Image = pictureBox7.Image = pictureBox8.Image = imageGreen;
            //panel2._bottomWidth = 1;
            //panelEx1._topWidth = 1;
            //panelEx1._bottomWidth = 1;
            tabControl1.Top = 53;
            panel1.Top = 55;
            tabControl1.Left = this.Width / 5 + 20;
            tabControl1.Width = this.Width - this.Width / 5 - 30;// - 170;
            tabControl1.Height = this.Height - 90;
            panel1.Left = panel2.Left = panelEx1.Left = 2;
            panel1.Width = panel2.Width = panelEx1.Width = this.Width / 5;
            panel2.Top = 0;
            panel2.Height = 300;
            panelEx1.Height = 300;
            panelEx1.Top = 299;
            txtShowLog.Top = 615;
            txtShowLog.Height = 300;
            txtShowLog.BorderStyle = BorderStyle.None;
            panel1.Height = tabControl1.Height;
            panel1.BackColor = txtShowLog.BackColor = Color.FromArgb(68, 114, 196);//0,177,255
            //Bitmap newGradientBackImg = new Bitmap(panel1.Width, panel1.Height);
            //LinearGradientBrush brush = new LinearGradientBrush(new PointF(0, 0), new PointF(0, panel1.Height), Color.FromArgb(68, 114, 196), Color.FromArgb(0, 177, 255));
            //Graphics gr = Graphics.FromImage(newGradientBackImg);
            //gr.FillRectangle(brush, new RectangleF(0, 0, panel1.Width, panel1.Height));
            ////btn.BackColor = Color.Transparent;
            //panel1.BackgroundImage = panel2.BackgroundImage = panelEx1.BackgroundImage = newGradientBackImg;
            //txtShowLog.BackColor = Color.Transparent;
            this.tabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl1.DrawItem += new DrawItemEventHandler(this.tabControl1_DrawItem);
            labProID.Text = GlobalField.productInfo.ProID;
            labProSize.Text = GlobalField.productInfo.ProSize;
            labRemark.Text = GlobalField.productInfo.Remark;
            labManufacturer.Text = GlobalField.productInfo.Manufacturer;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (GlobalField.testNum == 99)
            {
                if (GlobalField.nGI != null)
                    GlobalField.nGI.Close();
                this.Close();
                Application.Exit();
                return;
            }
            ControlInit();
        }
        private void tabControl1_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
        {
            Font fntTab;
            Brush bshBack;
            Brush bshFore;
            if (e.Index == this.tabControl1.SelectedIndex)
            {
                fntTab = new Font(e.Font, FontStyle.Bold);
                bshBack = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(237, 125, 49), Color.Yellow, System.Drawing.Drawing2D.LinearGradientMode.Vertical);
                bshFore = Brushes.Black;
            }
            else
            {
                fntTab = e.Font;
                bshBack = new SolidBrush(Color.White);
                bshFore = new SolidBrush(Color.Black);
            }
            string tabName = this.tabControl1.TabPages[e.Index].Text;
            StringFormat sftTab = new StringFormat();
            e.Graphics.FillRectangle(bshBack, e.Bounds);
            Rectangle recTab = e.Bounds;
            recTab = new Rectangle(recTab.X, recTab.Y + 4, recTab.Width, recTab.Height - 4);
            e.Graphics.DrawString(tabName, fntTab, bshFore, recTab, sftTab);
        }
        private void GlobalInit()
        {
            try
            {
                //AI传图方法
                GlobalField.defectIdent1 = new DefectIdent(AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["MODEL_SEG_1"]);  //明场模型
                GlobalField.defectIdent2 = new DefectIdent(AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["MODEL_SEG_2"]);  //PL模型
                GlobalField.defectIdent3 = new DefectIdent(AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["MODEL_SEG_3"]);  //EL模型
                GlobalField. hDevelopExport = new HDevelopExport();
        //Bitmap bitmap = null;
        //GlobalField.defectIdent1.GetDefectIdentResult(ref bitmap);

        //GlobalField.defectIdent2 = new TimeTech.DeviceDLL.OPR.DefectIdent(AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["Model_seg_2"]);
        GlobalField.CameraSN1 = ushort.Parse(ConfigurationManager.AppSettings["CameraSN1"]);
                bool[] isRun = new bool[5] { false, false, false, false, false };
                GlobalField.Camera_IsRun = isRun;
                GlobalField.testNum = 0;

                ////万名位移台
                //GlobalField.nim_Server_Test = new NIM_SERVER_TEST(new string[] { "2", ConfigurationManager.AppSettings["Wanming_stage"], "2000" });
                //LensConnect电动调焦
                //GlobalField.lensConnectBLL = new LensConnectBLL();
                //PLC控制光源和相机轴
                //GlobalField.tcpConnectProvider = new TcpConnectProvider("192.168.0.21", 2000);
                ////光源控制
                //GlobalField.kCS_LightSource_BLL = new KCS_LightSource_BLL();
                //GlobalField.kCS_LightSource_BLL.SetBrightness(KCS_LightSource_BLL.ChannelEnum.A, 255);

                //GlobalField._ACS = new CAiMo300("192.168.0.13");  //爱默300*300mm zmotion位移台
                //GlobalField._ACS = new AiMoBLL2();  //艾默位移台ACS控制器
                //GlobalField._ACS.PowerOn(0);
                //GlobalField._ACS.PowerOn(1);
                //GlobalField.n2600Demo = new N2600Demo("192.168.0.100", 1, 5025);  //吉时利电表
                //if (GlobalField.n2600Demo.OpenConnect()) { GlobalField.n2600Demo.LoadSetupUnit1(); }
                //GlobalField.n2600Demo = new N2600Demo("192.168.5.124", 1, 7000);  //恩智电源表
                //if (GlobalField.n2600Demo.OpenConnect()) { GlobalField.n2600Demo.SetParamMCCF(); }  //设置硬件触发模式

                #region 加载配置文件
                string configFilePath = AppDomain.CurrentDomain.BaseDirectory + "setting.config";
                if (File.Exists(configFilePath))
                {
                    Hashtable ht = Newtonsoft.Json.JsonConvert.DeserializeObject<Hashtable>(File.ReadAllText(configFilePath));
                    ushort usTemp = 0;
                    uint uiTemp = 0;
                    float flTemp = 0;

                    uint.TryParse(ht["BLExpoTime"] + "", out uiTemp);
                    GlobalField.BLExpoTime = uiTemp;
                    ushort.TryParse(ht["BLExpoGain"] + "", out usTemp);
                    GlobalField.BLExpoGain = usTemp;
                    uint.TryParse(ht["PLExpoTime"] + "", out uiTemp);
                    GlobalField.PLExpoTime = uiTemp;
                    ushort.TryParse(ht["PLExpoGain"] + "", out usTemp);
                    GlobalField.PLExpoGain = usTemp;
                    uint.TryParse(ht["ELExpoTime"] + "", out uiTemp);
                    GlobalField.ELExpoTime = uiTemp;
                    ushort.TryParse(ht["ELExpoGain"] + "", out usTemp);
                    GlobalField.ELExpoGain = usTemp;
                    ushort.TryParse(ht["focusInitialVal"] + "", out usTemp);
                    GlobalField.focusInitialVal = usTemp;
                    uint.TryParse(ht["photographDelay"] + "", out uiTemp);
                    GlobalField.photographDelay = uiTemp;

                    float.TryParse(ht["SampleMotionX"] + "", out flTemp);
                    GlobalField.SampleMotionX = flTemp;
                    float.TryParse(ht["SampleMotionY"] + "", out flTemp);
                    GlobalField.SampleMotionY = flTemp;
               
                    float.TryParse(ht["Light1Pos"] + "", out flTemp);
                    GlobalField.Light1Pos = flTemp;
                    float.TryParse(ht["laserPos"] + "", out flTemp);
                    GlobalField.laserPos = flTemp;

                    float.TryParse(ht["Z0Num"] + "", out flTemp);
                    GlobalField.Z0Num = flTemp;
                    float.TryParse(ht["Z1Num"] + "", out flTemp);
                    GlobalField.Z1Num = flTemp;

                    float.TryParse(ht["fieldMoveX"] + "", out flTemp);
                    GlobalField.fieldMoveX = flTemp;
                    float.TryParse(ht["fieldMoveY"] + "", out flTemp);
                    GlobalField.fieldMoveY = flTemp;
                    ushort.TryParse(ht["fieldX"] + "", out usTemp);
                    GlobalField.fieldX = usTemp;
                    ushort.TryParse(ht["fieldY"] + "", out usTemp);
                    GlobalField.fieldY = usTemp;
                    uint.TryParse(ht["eTOMMENSOutputVolt"] + "", out uiTemp);
                    GlobalField.eTOMMENSOutputVolt = uiTemp;
                    uint.TryParse(ht["eTOMMENSOutputCurrent"] + "", out uiTemp);
                    GlobalField.eTOMMENSOutputCurrent = uiTemp;
                    uint.TryParse(ht["lightIntensity"] + "", out uiTemp);
                    GlobalField.lightIntensity = uiTemp;
                }
                #endregion
                //GlobalField.etmmens = new eTOMMENS(); //供电电源
                //if (GlobalField.etmmens.StartOutVolt())
                //{

                //}
                GlobalField.objModeHeatMap=new ModeHeatMap();  
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Exit();
            }
        }

        protected bool LoadData()
        {
            bool bret;
            using (FmLoading fl = new FmLoading())
            {
                var tempData = LoadDataAsync(fl);
                fl.ShowDialog();
                bret = tempData.Result;
            }

            return bret;
        }
        private async Task<bool> LoadDataAsync(FmLoading fl)
        {
            var result = await System.Threading.Tasks.Task.Run(() =>
            {
                Thread.Sleep(1500);
                return true;
            });
            await System.Threading.Tasks.Task.Run(() =>
            {
                GlobalInit();
            });

            fl.Close();
            return result;
        }
        #endregion

        #region 菜单栏事件
        private void BeginTest_Click(object sender, EventArgs e)
        {

        }

        private void ViewTestResult_Click(object sender, EventArgs e)
        {
            fileAndFolder.OpenFolder(AppDomain.CurrentDomain.BaseDirectory + "TestResult");
        }

        private void ViewLog_Click(object sender, EventArgs e)
        {
            fileAndFolder.OpenFolder(AppDomain.CurrentDomain.BaseDirectory + "Log");
        }

        private void SetParam_Click(object sender, EventArgs e)
        {

        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #endregion

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
        }

        #region tabControl 相关事件

        private bool tabControlCheckHave(System.Windows.Forms.TabControl tab, String tabName)
        {
            for (int i = 0; i < tab.TabCount; i++)
            {
                if (tab.TabPages[i].Text == tabName)
                {
                    tab.SelectedIndex = i;
                    return true;
                }
            }
            return false;
        }

        public void Add_TabPage(string str, Form myForm)
        {
            if (tabControlCheckHave(this.tabControl1, str)) { return; }
            else
            {
                tabControl1.TabPages.Add(str);
                tabControl1.SelectTab(tabControl1.TabPages.Count - 1);
                myForm.FormBorderStyle = FormBorderStyle.None;
                myForm.TopLevel = false;
                myForm.Parent = tabControl1.SelectedTab;
                tabControl1.SelectedTab.AutoScroll = true;
                myForm.Show();
            }
        }

        private void tabControl1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (tabControl1.TabPages[tabControl1.SelectedIndex].Controls[0] is CameraServiceProvider || tabControl1.TabPages[tabControl1.SelectedIndex].Controls[0] is Camera)
                    ((CameraServiceProvider)tabControl1.TabPages[tabControl1.SelectedIndex].Controls[0]).OnStop();
                tabControl1.TabPages.RemoveAt(tabControl1.SelectedIndex);
            }
        }
        #endregion

        //添加明场界面
        private void button1_Click(object sender, EventArgs e)
        {
            foreach (TabPage tabPage in tabControl1.TabPages)
            {
                if (tabPage != null)
                {
                    foreach (Camera control in tabPage.Controls)
                    {
                        ((Camera)control).AddFrmPhotovoltageScan = null;
                        if (control.cam_ != null)
                        {
                            control.cam_.Stop();
                            control.cam_.Close();
                            control.cam_ = null;
                        }

                    }
                }
            }
            Add_TabPage("明场拍摄", new Camera(0));
            GlobalField.logText += $"打开明场拍摄界面{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\r\n";
            TabPage selectedTab = tabControl1.SelectedTab;          
                if (selectedTab != null)
                {
                    foreach (Camera control in selectedTab.Controls)
                    {
                        ((Camera)control).AddFrmPhotovoltageScan = new Action<int>(addFrmPhotovoltageScan);

                    }
                }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (TabPage tabPage in tabControl1.TabPages)
            {
                if (tabPage != null)
                {
                    foreach (Camera control in tabPage.Controls)
                    {
                        if (control.cam_ != null)
                        {
                            control.cam_.Stop();
                            control.cam_.Close();
                            control.cam_ = null;
                        }

                    }
                }
            }
            Add_TabPage("荧光拍摄", new Camera(1));
            GlobalField.logText += $"打开荧光拍摄界面{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\r\n";
        }

        private void btnELPic_Click(object sender, EventArgs e)
        {
            foreach (TabPage tabPage in tabControl1.TabPages)
            {
                if (tabPage != null)
                {
                    foreach (Camera control in tabPage.Controls)
                    {
                        if (control.cam_ != null)
                        {
                            control.cam_.Stop();
                            control.cam_.Close();
                            control.cam_ = null;
                        }

                    }
                }
            }
            Add_TabPage("EL拍摄", new Camera(2));
            GlobalField.logText += $"打开EL拍摄界面{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\r\n";
        }

        public void addFrmPhotovoltageScan(int index)
        {
            Add_TabPage("光电压扫描", new FrmPhotovoltageScan());
            GlobalField.logText += $"打开光电压扫描界面{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\r\n";
        }

        private void spectrumAcquire_Click(object sender, EventArgs e)
        {
            Add_TabPage("光谱采集", new FrmspectrumAcquire());
            GlobalField.logText += $"打开光谱采集界面{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\r\n";
        }

        //光电压扫描
        private void button3_Click(object sender, EventArgs e)
        {
            /*
             * 设置扫描范围
            */
            Add_TabPage("光电压扫描", new FrmPhotovoltageScan());
            GlobalField.logText += $"打开光电压扫描界面{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\r\n";
        }
        private void btInit_Click(object sender, EventArgs e)
        {
            //PLC轴回原点
            GlobalField.tcpConnectProvider.AxisIndex = "F1";
            GlobalField.tcpConnectProvider.SetHomeZero();
            GlobalField.tcpConnectProvider.AxisIndex = "F2";
            GlobalField.tcpConnectProvider.SetHomeZero();
            //光源位移台移动至绿光正对相机位置
            GlobalField.tcpConnectProvider.AxisIndex = "F1";
            GlobalField.tcpConnectProvider.AbsMove(GlobalField.Light1Pos);
            ////万名位移台回原点
            //GlobalField.nim_Server_Test.GoHomeZero(0);
            //GlobalField.nim_Server_Test.GoHomeZero(1);
            //艾默位移台回原点
            GlobalField._ACS.AbsMove(0,0);
            GlobalField._ACS.AbsMove(1, 0);
            //电磁铁滤光片不挡住镜头
            GlobalField.tcpConnectProvider.SendFunCode("07");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (GlobalField.nGI != null)
            {
                GlobalField.nGI.Close();
            }
            Application.Exit();
        }

        private void btSetting_Click(object sender, EventArgs e)
        {
            FmSet fmSet = new FmSet();
            fmSet.ShowDialog();
        }
        bool spectrumVCPConnect = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
           
            if (!string.IsNullOrWhiteSpace(GlobalField.logText))
            {
                txtShowLog.Text = GlobalField.logText;
                txtShowLog.SelectionStart = txtShowLog.Text.Length - 1;
                txtShowLog.ScrollToCaret();
            }
            if (GlobalField.tcpConnectProvider!=null && GlobalField.tcpConnectProvider.result)
            {
                  pictureBox3.Image = pictureBox5.Image = imageGreen;
            }
            else
            {
                pictureBox3.Image = pictureBox5.Image = imageRed;
            }


            if (GlobalField._ACS!=null&& GlobalField._ACS.result)
            {
               pictureBox4.Image = imageGreen; 
            }
            else {  pictureBox4.Image = imageRed; }

            string[] strarr = DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.AI, PhysicalChannelAccess.External);
            if (strarr.Length>0)
            {
                pictureBox8.Image = imageGreen;
            }
            else { pictureBox8.Image = imageRed; }

            if (GlobalField.kCS_LightSource_BLL!=null&& GlobalField.kCS_LightSource_BLL.result)
            {
               pictureBox6.Image = imageGreen; 
            }
            else
            {
                pictureBox6.Image = imageRed;
            }
            SerialPort serialPort1;
            if (!spectrumVCPConnect)
            {
                try
                {
                    serialPort1 = new SerialPort();
                    serialPort1.PortName = "COM4";
                    serialPort1.BaudRate = 115200;
                    serialPort1.DataBits = 8;
                    serialPort1.StopBits = StopBits.One;
                    serialPort1.ReceivedBytesThreshold = 8;
                    Thread.Sleep(100);
                    serialPort1.Open();
                    if (serialPort1.IsOpen) { spectrumVCPConnect = true; }
                }
                catch (Exception)
                {

                    spectrumVCPConnect = false;
                }
               
            }
            
            if (spectrumVCPConnect)
            {
                pictureBox7.Image = imageGreen;
            }
            else { pictureBox7.Image = imageRed; }  
        }

        //打开调试画面
        private void button4_Click(object sender, EventArgs e)
        {
            //Form objfrmtest = new frmtest();
            //objfrmtest.ShowDialog();
            GlobalField.etmmens.OpenOut();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (TabPage tabPage in tabControl1.TabPages)
            {
                if (tabPage != null)
                {
                
                    foreach (Control control in tabPage.Controls)
                    {
                        if (control is Camera)
                        {
                            if (((Camera)control).cam_ != null)
                            {
                                ((Camera)control).cam_.Stop();
                                ((Camera)control).cam_.Close();
                                ((Camera)control).cam_ = null;
                            }
                        }
                       

                    }
                    //tabControl1.TabPages.Remove(tabPage);
                }
            }
        }


    }
}
