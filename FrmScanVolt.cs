using Aspose.Cells;
using HalconDotNet;
using NationalInstruments.DAQmx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TimeTech.Core;
using TimeTech.DeviceDLL.Motion;
using TimeTech.DeviceDLL.SMU;

namespace PerovskiteTest
{
    public enum XYScanDir
    {
        XDir=0, YDir=1  
    }
    public partial class FrmScanVolt : Form
    {
        bool isPause;  //判断线程是否被暂停可以取消
        bool voltTesting;  //电压测试中

        DevStatusFlag devStatusFlag = DevStatusFlag.dsfIDLE;

        StringBuilder strLog = new StringBuilder();

        int yMoveCount;
        Stopwatch w = new Stopwatch();
        //Thread threadTCP = null;
        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory + "TestData\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
        public int begIXH = 0;
        public int begIYH = 0;
        System.Threading.Tasks.Task longRunningTask =null;
        // 创建一个 CancellationTokenSource 来生成取消令牌
        CancellationTokenSource cts = new CancellationTokenSource();
        // 使用 Token 可以随时取消任务
        CancellationToken token ;
        
        public FrmScanVolt()
        {
            InitializeComponent();
        }
        private System.Timers.Timer timer21=new System.Timers.Timer();
        ManualResetEvent MRE = new ManualResetEvent(true); //实例化阻塞事件
        private void Form1_Load(object sender, EventArgs e)
        {
            //FileLog.AddUserLog("程序启动");
            //GlobalField.nGI = new tcpNGI("192.168.5.124", 7000);  //恩智电源表

            //GlobalField.nim_Server_Test = new NIM_SERVER_TEST(new string[] { "2", "COM7", "2000" });  //万名位移台
            GlobalField._ACS = new AiMoBLL2();  //艾默位移台
            //GlobalField._ACS = new CAiMo300("192.168.0.13");  //艾默位移台
            GlobalField.XAxisMoveSpeed =45;
            GlobalField._ACS.SetSpeed(0, GlobalField.XAxisMoveSpeed);
            GlobalField.YAxisMoveSpeed = 45;
            GlobalField._ACS.SetSpeed(1, GlobalField.YAxisMoveSpeed);
            //GlobalField._ACS.SetMaxSpeed(0);
            //GlobalField._ACS.SetMaxSpeed(1);
            GlobalField.tcpConnectProvider = new TcpConnectProvider("192.168.0.21", 2000);  //plc光源相机位移台
            btnStart.Text = "等待开始";  
            #region 加载配置文件
            string configFilePath = AppDomain.CurrentDomain.BaseDirectory + "scanVoltSetting.config";
            if (File.Exists(configFilePath))
            {
                Hashtable ht = Newtonsoft.Json.JsonConvert.DeserializeObject<Hashtable>(File.ReadAllText(configFilePath));
                int iTemp = 0;
                float flTemp = 0;
                float.TryParse(ht["SampleMotionX"] + "", out flTemp);
                GlobalField.SampleMotionX = flTemp;
                tbXStartPos.Text = flTemp + "";
                float.TryParse(ht["SampleMotionY"] + "", out flTemp);
                GlobalField.SampleMotionY = flTemp;
                tbYStartPos.Text = flTemp + "";

                float.TryParse(ht["GTestDis"] + "", out flTemp);
                GlobalField.GTestDis = flTemp;
                tbTestDis.Text = flTemp + "";
                float.TryParse(ht["GNextDis"] + "", out flTemp);
                GlobalField.GNextDis = flTemp;
                tbNextDis.Text = flTemp + "";

                int.TryParse(ht["GTestCount"] + "", out iTemp);
                GlobalField.GTestCount = iTemp;
                tbTestCount.Text = iTemp+"";
                yMoveCount = GlobalField.GTestCount;
            }
            #endregion

            voltTesting = false;
            timer21.Interval = 15;
            timer21.Elapsed += new System.Timers.ElapsedEventHandler(timer1_Tick);
            //if (threadTCP != null)
            //{
            //    //m_bThreadTCP = false;
            //    Thread.Sleep(200);
            //    if (threadTCP.IsAlive) threadTCP.Abort();
            //}
            //threadTCP = new Thread(startTest);
            //threadTCP.IsBackground = true;//后台线程，随主程序一起结束
            ////m_bThreadTCP = true;
            //threadTCP.Start();

            //GlobalField.n2600Demo = new N2600Demo("192.168.0.124", 1, 7000);  //恩智电源表
            GlobalField.n2600Demo = new N2600Demo("192.168.0.100", 1, 5025);  //吉时利电表
            if (GlobalField.n2600Demo.OpenConnect()) { GlobalField.n2600Demo.LoadSetupUnit1(); }
            //{ GlobalField.n2600Demo.SetParamMCCF(); }

            // 使用 Token 可以随时取消任务
             token = cts.Token;
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
           
            GlobalField.tcpConnectProvider.AxisIndex = "F1";
            GlobalField.tcpConnectProvider.SetHomeZero();
            //Thread.Sleep(100);
            //GlobalField.nim_Server_Test.GoHomeZero(0);
            //Thread.Sleep(100);
            //GlobalField.nim_Server_Test.GoHomeZero(1);
            GlobalField._ACS.AbsMove(0,0);
            GlobalField._ACS.AbsMove(1, 0);
        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem.ToString() == "X1方向")
            {
                float dtemp = 0;
                float.TryParse(distance.Text, out dtemp);
                GlobalField.XAxisMoveSpeed = 20;
                GlobalField._ACS.SetSpeed(0, GlobalField.XAxisMoveSpeed);
                //GlobalField.nim_Server_Test.ASetVel(0, 10.0);
                //GlobalField.nim_Server_Test.MoveAbsolute(0,dtemp);
                GlobalField._ACS.AbsMove(0,dtemp);
                while (GlobalField._ACS.WaitMoveFinish(0)) { };
            }
            if (comboBox1.SelectedItem.ToString() == "Y2方向")
            {
                float dtemp = 0;
                float.TryParse(distance.Text, out dtemp);
                //GlobalField.nim_Server_Test.ASetVel(1, 10.0);
                //GlobalField.nim_Server_Test.MoveAbsolute(1,dtemp);
                GlobalField.YAxisMoveSpeed = 20;
                GlobalField._ACS.SetSpeed(1, GlobalField.YAxisMoveSpeed);
                GlobalField._ACS.AbsMove(1, dtemp);
                while (GlobalField._ACS.WaitMoveFinish(1)) { };
            }
            if (comboBox1.SelectedItem.ToString() == "光源")
            {
                float dtemp = 0;

                float.TryParse(distance.Text, out dtemp);
                GlobalField.tcpConnectProvider.AxisIndex = "F1";                
                GlobalField.tcpConnectProvider.AbsMove(dtemp, 25);               
            }
            if (comboBox1.SelectedItem.ToString() == "相机")
            {
                float dtemp = 0;

                float.TryParse(distance.Text, out dtemp);
                GlobalField.tcpConnectProvider.AxisIndex = "F2";
                GlobalField.tcpConnectProvider.AbsMove(dtemp, 25);
            }
                                  
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            //GlobalField.nim_Server_Test.Stop();
            GlobalField._ACS.StopAll();
        }
        List<float> listf = new List<float>();
        //开始测试
        private void btnStart_Click_1(object sender, EventArgs e)
        {
            btnStart.Text = "运行中";
            devStatusFlag = DevStatusFlag.dsfRunning;
            GlobalField.logText += $"frm开始检测{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\r\n";
            FileLog.AddUserLog("测试开始");
            //光源轴到起始位置
            GlobalField.tcpConnectProvider.AxisIndex = "F1";
            GlobalField.tcpConnectProvider.AbsMove(170.0F, 15);

            var aa = MessageBox.Show("是否接续上次运行", "接续运行", MessageBoxButtons.OKCancel);
            if (aa == DialogResult.OK)
            {
                ReadTestingData();
            }
            else
            {
                begIYH = 0;
            }
            test();
        
        }
        
        private  void test()
        {
            System.Threading.Tasks.Task.Factory.StartNew(() => {
                try
                {
 
                    //XScanDirHomeTest();
                    YScanDirHomeTest();
                }
                catch (Exception ex)
                {                  
                   FileLog.AddErrorLog(ex.Message, ex);
                   GlobalField.logText += $"frm测试异常{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\r\n";
                   devStatusFlag = DevStatusFlag.dsfTrouble;
               }
                //File.WriteAllText(DateTime.Now.ToFileTime() + ".txt", strLog.ToString().Trim(','));
                if (!btnStart.InvokeRequired)  //异步不中断线程
                {
                    btnStart.Text = "等待开始";
                }
                else 
                {
                    Invoke(new Action(() =>  //同步中断线程
                    {
                        btnStart.Text = "等待开始";
                    }));
                }

               GlobalField.logText += $"frm测试完成{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\r\n";
               FileLog.AddUserLog("测试完成");
               devStatusFlag = DevStatusFlag.dsfTestEnd;
           }, TaskCreationOptions.LongRunning);
         }

        //X方向S形测试
        public void XDirSShapeTest()
        {
            for (int i = 0; i < GlobalField.GTestCount; i++)
            {
                MRE.WaitOne();
                strLog = new StringBuilder();

                if (i % 2 == 0)
                {

                    GlobalField.nim_Server_Test.MoveRelative(0, GlobalField.GTestDis);   //移动
                    
                }
                else
                {
                    
                    GlobalField.nim_Server_Test.MoveRelative(0, -GlobalField.GTestDis);   //移动
                    
                }
                Thread.Sleep(100);
                GlobalField.nim_Server_Test.MoveRelative(1, -GlobalField.GNextDis);
                File.WriteAllText(currentDirectory + $"XS\\{DateTime.Now.ToString("yyyyMMdd")}\\{i}.txt", strLog.ToString().Trim(','));
            }
            //回测试起始点
            GlobalField.nim_Server_Test.MoveAbsolute(0, GlobalField.SampleMotionX);
            GlobalField.nim_Server_Test.MoveAbsolute(1, GlobalField.SampleMotionY);
        }

        public void XScanDirHomeTest()
        {
            Workbook wb = new Aspose.Cells.Workbook();
            wb.Worksheets.Add();
            Worksheet ws = wb.Worksheets[0];
            if (!Directory.Exists(currentDirectory + "XH\\"))
            {
                Directory.CreateDirectory(currentDirectory + "XH\\");
            }
            for (int i = begIXH; i < GlobalField.GTestCount; i++)
            {
                //setPulseTrig(XYScanDir.XDir);   
                MRE.WaitOne();
                //GlobalField.nGI.strLog.Clear();
                //GlobalField.nGI.TestVoltData.Clear();
                //GlobalField.nim_Server_Test.ASetVel(0, 5.12);   
                //GlobalField.nGI.SendMessage("READ?\n");
                //GlobalField.nGI.voltTesting = true;
                //TestStart2();
                //timer21.Start();
                //w.Start();  
                //GlobalField.nGI.voltTesting = true;
                //TestStart();
                //GlobalField.nim_Server_Test.MoveRelative(0, GlobalField.GTestDis);  
                //GlobalField.nGI.voltTesting = false;                
                //w.Stop();
                //timer21.Stop();
                //this.voltTesting = false;
                //GlobalField.nim_Server_Test.ASetVel(0, 20.48);   
                //GlobalField.nim_Server_Test.MoveRelative(1, -GlobalField.GNextDis);  
                //GlobalField.nim_Server_Test.MoveRelative(0, -GlobalField.GTestDis);   
                
                GlobalField.n2600Demo.StartTest();  //电表开始测试
                GlobalField.n2600Demo.btnOff_Click();
                //GlobalField._ACS.RefMove(1, -GlobalField.GTestDis);
                while (GlobalField._ACS.WaitMoveFinish(1)) { };
                Thread.Sleep(110);
                //GlobalField._ACS.RefMove(0, GlobalField.GNextDis);
                while (GlobalField._ACS.WaitMoveFinish(0)) { };
                //GlobalField._ACS.RefMove(1,GlobalField.GTestDis);
                while (GlobalField._ACS.WaitMoveFinish(1)) { };
                GlobalField.n2600Demo.btnOff_Click();
                string rcvData = GlobalField.n2600Demo.ReadCacheData();
                GlobalField.n2600Demo.btnClear_Click();
                if (!string.IsNullOrWhiteSpace(rcvData))
                {
                    string[] array = rcvData.Split(',');
                    
                    for (int j = 0; j < array.Length; j++)
                    {
                        if (j%2==0)
                        {
                            ws.Cells[i, j/2].Value = double.Parse(array[j]);
                        }
                        
                    }
                    rcvData=String.Empty;   
                }
                GlobalField.n2600Demo.btnClear_Click();
            }
                            
            wb.Save(currentDirectory + $"XH\\{DateTime.Now.ToFileTime()} .xlsx");
            //回测试起始点
            //GlobalField.nim_Server_Test.MoveAbsolute(0, GlobalField.SampleMotionX);
            //GlobalField.nim_Server_Test.MoveAbsolute(1, GlobalField.SampleMotionY);
            GlobalField._ACS.AbsMove(0, GlobalField.SampleMotionX);
            while (GlobalField._ACS.WaitMoveFinish(0)) { };
            GlobalField._ACS.AbsMove(1, GlobalField.SampleMotionX);
            while (GlobalField._ACS.WaitMoveFinish(1)) { };
        }

        public void YDirSShapeTest()
        {
            for (int i = 0; i < GlobalField.GTestCount; i++)
            {
                MRE.WaitOne();
                strLog = new StringBuilder();

                if (i % 2 == 0)
                {
                    timer21.Start();
                    GlobalField.nim_Server_Test.MoveRelative(1, -GlobalField.GTestDis);   
                    timer21.Stop();
                }
                else
                {
                    timer21.Start();
                    GlobalField.nim_Server_Test.MoveRelative(1, GlobalField.GTestDis);   
                    timer21.Stop();
                }
                Thread.Sleep(10);
                GlobalField.nim_Server_Test.MoveRelative(0, GlobalField.GNextDis);

                File.WriteAllText(currentDirectory + $"YS\\{DateTime.Now.ToString("yyyyMMdd")}\\{i}.txt", strLog.ToString().Trim(','));
            }
  
            GlobalField.nim_Server_Test.MoveAbsolute(0, GlobalField.SampleMotionX);
            GlobalField.nim_Server_Test.MoveAbsolute(1, GlobalField.SampleMotionY);
        }

        Workbook wb = new Aspose.Cells.Workbook();
        PeiZhiCanShu peizhicanshu= new PeiZhiCanShu();  
        public void YScanDirHomeTest()
        {
            testRow = 0;
            peizhicanshu.AiChannel = "Dev1/ai0";
            peizhicanshu.TriggerChannel = "/Dev1/PFI2";
            peizhicanshu.SampleRate = 100000;
            peizhicanshu.SamplesPerTrigger = 10;  //每个触发信号测试几次？
            peizhicanshu.MinVoltage = -10;
            peizhicanshu.MaxVoltage = 10;
            peizhicanshu.ExtSignalRate = 200;  //1s触发多少次？
            peizhicanshu.BufferSize =  (int)(GlobalField.GTestDis / GlobalField.GNextDis)* peizhicanshu.SamplesPerTrigger;
            peizhicanshu.callbackInterval =  (int)(GlobalField.GTestDis / GlobalField.GNextDis)* peizhicanshu.SamplesPerTrigger;
                                                                                         
            try
            {
                AcqTask(peizhicanshu);
                GlobalField.XAxisMoveSpeed = peizhicanshu.ExtSignalRate * GlobalField.GNextDis;
                GlobalField._ACS.SetSpeed(0, GlobalField.XAxisMoveSpeed);
                GlobalField._ACS.AbsMove(0, GlobalField.SampleMotionX-1);
                while (GlobalField._ACS.WaitMoveFinish(0)) { };

                GlobalField._ACS.AbsMove(1, GlobalField.SampleMotionY);
                while (GlobalField._ACS.WaitMoveFinish(1)) { };
                wb.Worksheets.Add();
                if (!Directory.Exists(currentDirectory + "YH\\"))
                {
                    Directory.CreateDirectory(currentDirectory + "YH\\");
                }
                FileLog.AddUserLog("YH测试开始时间：" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));
      

                for (int i = begIYH; i < GlobalField.GTestCount; i++)
                {
                    MRE.WaitOne();
                    setPulseTrig(XYScanDir.YDir);

                    GlobalField.XAxisMoveSpeed = peizhicanshu.ExtSignalRate * GlobalField.GNextDis;
                    GlobalField._ACS.SetSpeed(0, GlobalField.XAxisMoveSpeed);
                    Thread.Sleep(1000);
                    GlobalField._ACS.AbsMove(0, GlobalField.SampleMotionX + GlobalField.GTestDis+1);
                    while (GlobalField._ACS.WaitMoveFinish(0)) { };

                    GlobalField._ACS.AbsMove(1, GlobalField.SampleMotionY - i * GlobalField.GNextDis);
                    while (GlobalField._ACS.WaitMoveFinish(1)) { };

                    //GlobalField.XAxisMoveSpeed = 100;
                    GlobalField._ACS.SetSpeed(0, GlobalField.XAxisMoveSpeed);
                    GlobalField._ACS.AbsMove(0, (GlobalField.SampleMotionX - 1));
                    while (GlobalField._ACS.WaitMoveFinish(0)) { };                   

                    begIYH = i;
                    SaveTestingData();
                    //while (myCallbacked == false)
                    //{
                    //    Thread.Sleep(10);
                    //}
                }
                begIYH = 0;
                SaveTestingData();
                FileLog.AddUserLog("YH测试结束时间：" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));
                wb.Save(currentDirectory + $"YH\\{DateTime.Now.ToFileTime()} .xlsx");
                Thread.Sleep(1000);

                GlobalField._ACS.AbsMove(0, GlobalField.SampleMotionX);
                while (GlobalField._ACS.WaitMoveFinish(0)) { };
                GlobalField._ACS.AbsMove(1, GlobalField.SampleMotionY);
                while (GlobalField._ACS.WaitMoveFinish(1)) { };
                testRow = 0;

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                FileLog.AddErrorLog(ex.Message, ex);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            float pos = 0.0F;
            switch (comboBox1.SelectedItem + "")
            {
                case "X1方向":
                    //GlobalField._ACS.GetPos(0,ref pos) ;//GlobalField.nim_Server_Test.GetPos(0) + "";
                    //label2.Text = pos + "";
                    label2.Text = GlobalField._ACS.GetPos(0) + "";
                    break;
                case "Y2方向":
                    //GlobalField._ACS.GetPos(1,ref pos) ;//GlobalField.nim_Server_Test.GetPos(1) + "";
                    //label2.Text = pos + "";
                    label2.Text = GlobalField._ACS.GetPos(1) + "";
                    break;
                case "光源":
                    GlobalField.tcpConnectProvider.AxisIndex = "F1";
                    label2.Text = GlobalField.tcpConnectProvider.GetPos() + "";
                    break;
                case "相机":
                    GlobalField.tcpConnectProvider.AxisIndex = "F2";
                    label2.Text = GlobalField.tcpConnectProvider.GetPos() + "";
                    break;


            }
        }
        List<string> list = new List<string>();
        private void timer1_Tick(object sender, EventArgs e)
        {
            //DateTime dt1 = DateTime.Now;
            //strLog.Append(GlobalField.nGI.StartMeasVolt() + ",");
            //DateTime dt2 = DateTime.Now;
            //FileLog.AddUserLog("测试一个数据耗时：" + (dt2 - dt1).TotalMilliseconds.ToString());
            //GlobalField.nGI.StartMeasVoltHC();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //GlobalField.nim_Server_Test.GoHomeZero(0);
            GlobalField._ACS.AbsMove(0, 0);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //GlobalField.nim_Server_Test.GoHomeZero(1);
            GlobalField._ACS.AbsMove(1, 0);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Hashtable ht = new Hashtable();
            int iTemp = 0;
            float flTemp = 0;
            float.TryParse(tbXStartPos.Text, out flTemp);
            GlobalField.SampleMotionX = flTemp;
            ht.Add("SampleMotionX", flTemp);

            float.TryParse(tbYStartPos.Text, out flTemp);
            GlobalField.SampleMotionY = flTemp;
            ht.Add("SampleMotionY", flTemp);

            float.TryParse(tbTestDis.Text, out flTemp);
            GlobalField.GTestDis = flTemp;
            ht.Add("GTestDis", flTemp);

            float.TryParse(tbNextDis.Text, out flTemp);
            GlobalField.GNextDis = flTemp;
            ht.Add("GNextDis", flTemp);

            int.TryParse(tbTestCount.Text, out iTemp);
            GlobalField.GTestCount = iTemp;
            ht.Add("GTestCount", iTemp);

            string strJson = Newtonsoft.Json.JsonConvert.SerializeObject(ht);
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "scanVoltSetting.config", strJson);
            //GlobalField.n2600Demo.SetParamMCCF();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (!isPause)
            {
                MRE.Reset();
                btnStart.Text = "暂停";
                btnStart.BackColor = Color.Red;
                btnPause.Text = "GO ON";
                isPause = true;
                devStatusFlag = DevStatusFlag.dsfPause;
            }
            else
            {
                MRE.Set();
                btnStart.Text = "运行中";
                btnStart.BackColor = Color.FromArgb(69,220,38);
                btnPause.Text = "PAUSE";
                isPause = false;
                devStatusFlag = DevStatusFlag.dsfRunning;
            }
        }


        private void FrmScanVolt_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (GlobalField.nGI != null)
            {
                GlobalField.nGI.Close();
            }
            if (GlobalField._ACS!=null)
            {
                //GlobalField._ACS.Close();
            }
            myTask?.Dispose();
        }

        private void btnHeatMap_Click(object sender, EventArgs e)
        {
            HeatMap form = new HeatMap();
            form.ShowDialog();
        }

        private void setPulseTrig(XYScanDir xDirYdir)
        {
            //width  ms  interval//mm
            double width = 0.1, firstPoint=0 , interval = GlobalField.GNextDis,lastPoint = 0;
            PegPkg pegPkg = new PegPkg();
            //AiMoPegEntity aiMoPegEntity = new AiMoPegEntity();
            pegPkg.axis = 0;
            if (xDirYdir== XYScanDir.YDir)
            {
                pegPkg.engToEncBitCode = 0x00000000;
                pegPkg.outputIndex = 8;
                pegPkg.bitConvertCode = 0b0000;
                firstPoint = GlobalField.SampleMotionX+ GlobalField.GNextDis;
                lastPoint = GlobalField.SampleMotionX + GlobalField.GTestDis;
                pegPkg.engToEncBitCode= 0x00000000; 
            }
            else if (xDirYdir == XYScanDir.XDir)
            {
                pegPkg.engToEncBitCode = 0x00000001;
                pegPkg.outputIndex = 9;
                pegPkg.bitConvertCode = 0b1000;
                firstPoint = GlobalField.SampleMotionY+ GlobalField.GNextDis;
                lastPoint = GlobalField.SampleMotionY+ GlobalField.GTestDis;
                pegPkg.engToEncBitCode = 0x00000000;
            }
            pegPkg.gpOutsBitCode = 0b000;
            pegPkg.pulseWidth = width;
            pegPkg.startPosition = firstPoint;
            pegPkg.pegInterval = interval;
            pegPkg.endPosition = lastPoint;
       
            //GlobalField._ACS.StartPegNT(aiMoPegEntity);

            GlobalField._ACS.ConfigurePeg(pegPkg);
            
        }

        private void SaveTestingData()
        {
            Hashtable ht = new Hashtable();
            ht.Add("testCurStep", begIYH);
            string strJson = Newtonsoft.Json.JsonConvert.SerializeObject(ht);
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "testCurStep.config", strJson);
        }
        private void ReadTestingData()
        {
            string configFilePath = AppDomain.CurrentDomain.BaseDirectory + "testCurStep.config";
            if (File.Exists(configFilePath))
            {
                Hashtable ht = Newtonsoft.Json.JsonConvert.DeserializeObject<Hashtable>(File.ReadAllText(configFilePath));
                int.TryParse(ht["testCurStep"] + "", out begIYH);
            }
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(GlobalField.logText))
            {
                txtShowLog.Text = GlobalField.logText;
                txtShowLog.SelectionStart = txtShowLog.Text.Length - 1;
                txtShowLog.ScrollToCaret();
            }
        }

        #region NI板卡
        NationalInstruments.DAQmx.Task myTask;
        AnalogSingleChannelReader aReader;
        private bool bAcqState;
        void AcqTask(PeiZhiCanShu peizhicanshu)
        {
            
            try
            {
                myTask = new NationalInstruments.DAQmx.Task();

                myTask.AIChannels.CreateVoltageChannel(peizhicanshu.AiChannel, "",
                    AITerminalConfiguration.Differential, peizhicanshu.MinVoltage, peizhicanshu.MaxVoltage, AIVoltageUnits.Volts);

                myTask.Timing.ConfigureSampleClock("", peizhicanshu.SampleRate, SampleClockActiveEdge.Rising,
                    SampleQuantityMode.FiniteSamples, peizhicanshu.SamplesPerTrigger);

                myTask.Stream.ConfigureInputBuffer(peizhicanshu.BufferSize);

                myTask.Triggers.StartTrigger.ConfigureDigitalEdgeTrigger(peizhicanshu.TriggerChannel, DigitalEdgeStartTriggerEdge.Rising);

                //可重触发
                myTask.Triggers.StartTrigger.Retriggerable = true;

                //产生回调样本数
                myTask.EveryNSamplesReadEventInterval = peizhicanshu.callbackInterval;

                //注册回调函数
                myTask.EveryNSamplesRead += myCallback;

                myTask.Control(TaskAction.Verify);

                aReader = new AnalogSingleChannelReader(myTask.Stream)
                {
                    SynchronizeCallbacks = false
                };
                myTask.Start();
                bAcqState = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                myTask.Dispose();
            }
        }


        int testRow = 0;
        private bool myCallbacked;
        private  void myCallback(object sender, EveryNSamplesReadEventArgs e)
        {
            Worksheet ws = wb.Worksheets[0];
            double[] data2 = aReader.ReadMultiSample(peizhicanshu.callbackInterval);
            List<double> data = data2.ToList();
            int count = data.Count / peizhicanshu.SamplesPerTrigger;
            for (int i = 0; i < count; i++)
            {
                ws.Cells[testRow, i].Value = $"{data.GetRange(i * peizhicanshu.SamplesPerTrigger, peizhicanshu.SamplesPerTrigger).Average()}";
            }
            testRow++;
            myCallbacked = true;
        }
        #endregion

   
    }
}
