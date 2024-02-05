using Aspose.Cells;
using NationalInstruments.DAQmx;
using PerovskiteTest.userControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TimeTech.Core;

namespace PerovskiteTest
{
    public partial class FrmPhotovoltageScan : Form
    {
        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory + "TestData\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
        CancellationTokenSource cancellationTokenSource;
        ManualResetEvent MRE = new ManualResetEvent(true); //实例化阻塞事件
        public FrmPhotovoltageScan()
        {
            InitializeComponent();
        }

        private void FrmPhotovoltageScan_Load(object sender, EventArgs e)
        {
            panel1.Width= PB_ImageResult.Width = this.Width / 2;
            PB_ImageResult.Height = this.Height - 200;
            panel1.Top = PB_ImageResult.Height + 8;
            panel2.Left = PB_ImageResult.Width + 30;

            //测试信息展示
            tbXStartPos.Text= GlobalField.SampleMotionX+"";
            tbYStartPos.Text = GlobalField.SampleMotionY +"";
            tbTestDis.Text = GlobalField.GTestDis + "";
            tbNextDis.Text = GlobalField.GNextDis + "";
            tbTestCount.Text = GlobalField.GTestCount + "";
        }

        Workbook wb = new Aspose.Cells.Workbook();
        PeiZhiCanShu peizhicanshu = new PeiZhiCanShu();
        private  void StartScanPhotovoltage_Click(object sender, EventArgs e)
        {
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            System.Threading.Tasks.Task task = System.Threading.Tasks.Task.Run(() => 
            {
                YScanDirTest(cancellationToken); 
            }, cancellationToken);
         
            //try
            //{
            //    // 等待任务完成
            //    await task;
            //}
            //catch (OperationCanceledException)
            //{
            //    Console.WriteLine("Task was canceled.");
            //}
        }
        void YScanDirTest(CancellationToken cancellationToken)
        {
            testRow = 0;
            peizhicanshu.AiChannel = "Dev1/ai0";
            peizhicanshu.TriggerChannel = "/Dev1/PFI2";
            peizhicanshu.SampleRate = 100000;
            peizhicanshu.SamplesPerTrigger = 10;  //每个触发信号测试几次？
            peizhicanshu.MinVoltage = -1;
            peizhicanshu.MaxVoltage = 2;
            peizhicanshu.ExtSignalRate = 1000;  //1s触发多少次？
            peizhicanshu.BufferSize = (int)(GlobalField.GTestDis / GlobalField.GNextDis) * peizhicanshu.SamplesPerTrigger;
            peizhicanshu.callbackInterval = (int)(GlobalField.GTestDis / GlobalField.GNextDis) * peizhicanshu.SamplesPerTrigger;
            AcqTask(peizhicanshu);
            GlobalField.XAxisMoveSpeed = peizhicanshu.ExtSignalRate * GlobalField.GNextDis;
            GlobalField._ACS.SetSpeed(0, GlobalField.XAxisMoveSpeed);
            GlobalField._ACS.AbsMove(0, GlobalField.SampleMotionX - 1);
            while (GlobalField._ACS.WaitMoveFinish(0)) { };

            GlobalField._ACS.AbsMove(1, GlobalField.SampleMotionY);
            while (GlobalField._ACS.WaitMoveFinish(1)) { };
            wb.Worksheets.Add();
            if (!Directory.Exists(currentDirectory + "YH\\"))
            {
                Directory.CreateDirectory(currentDirectory + "YH\\");
            }
            string currentDirectoryNow = currentDirectory + $"YH\\{DateTime.Now.ToFileTime()} .xlsx";
            FileLog.AddUserLog("YH测试开始时间：" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));

            for (int i = 0; i < GlobalField.GTestCount; i++)
            {
                MRE.WaitOne();
                setPulseTrig(XYScanDir.YDir);
                GlobalField.XAxisMoveSpeed = peizhicanshu.ExtSignalRate * GlobalField.GNextDis;
                GlobalField._ACS.SetSpeed(0, GlobalField.XAxisMoveSpeed);
                if (i%2==0)
                {                   
                    GlobalField._ACS.AbsMove(0, GlobalField.SampleMotionX + GlobalField.GTestDis + 1);
                    while (GlobalField._ACS.WaitMoveFinish(0)) { };
                }
                else
                {
                    GlobalField._ACS.AbsMove(0, (GlobalField.SampleMotionX - 1));
                    while (GlobalField._ACS.WaitMoveFinish(0)) { };
                }

                GlobalField._ACS.AbsMove(1, GlobalField.SampleMotionY - i * GlobalField.GNextDis);
                while (GlobalField._ACS.WaitMoveFinish(1)) { };
              
                UpdateProgressBar(100*(i/ GlobalField.GTestCount));
            }
            FileLog.AddUserLog("YH测试结束时间：" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));
            wb.Save(currentDirectoryNow);
            GlobalField.objModeHeatMap.ActionModeHeatMap(currentDirectoryNow, PB_ImageResult.Image);

            GlobalField._ACS.AbsMove(0, GlobalField.SampleMotionX);
            while (GlobalField._ACS.WaitMoveFinish(0)) { };
            GlobalField._ACS.AbsMove(1, GlobalField.SampleMotionY);
            while (GlobalField._ACS.WaitMoveFinish(1)) { };
            testRow = 0;
        }
        bool isPause;
        private void suspendScanPhotovoltage_Click(object sender, EventArgs e)
        {
            
            if (!isPause)
            {
                MRE.Reset();
                StartScanPhotovoltage.Text = "暂停";
                StartScanPhotovoltage.BackColor = Color.Red;
                suspendScanPhotovoltage.Text = "GO ON";
                isPause = true;
            }
            else
            {
                MRE.Set();
                StartScanPhotovoltage.Text = "运行中";
                StartScanPhotovoltage.BackColor = Color.FromArgb(69, 220, 38);
                suspendScanPhotovoltage.Text = "PAUSE";
                isPause = false;
            }
        }

        private void cancelScanPhotovoltage_Click(object sender, EventArgs e)
        {
            cancellationTokenSource.Cancel();
        }

        #region 配置PEG位置信息
        private void setPulseTrig(XYScanDir xDirYdir)
        {
            //width  ms  interval//mm
            double width = 0.1, firstPoint = 0, interval = GlobalField.GNextDis, lastPoint = 0;
            PegPkg pegPkg = new PegPkg();           
            if (xDirYdir == XYScanDir.YDir)  //Y方向使用X轴扫描
            {
                pegPkg.engToEncBitCode = 0x00000000;
                pegPkg.outputIndex = 8;
                pegPkg.bitConvertCode = 0b0000;
                firstPoint = GlobalField.SampleMotionX + GlobalField.GNextDis;
                lastPoint = GlobalField.SampleMotionX + GlobalField.GTestDis;
                pegPkg.engToEncBitCode = 0x00000000;
                pegPkg.axis = 0;
            }
            else if (xDirYdir == XYScanDir.XDir)
            {
                pegPkg.engToEncBitCode = 0x00000001;
                pegPkg.outputIndex = 9;
                pegPkg.bitConvertCode = 0b1000;
                firstPoint = GlobalField.SampleMotionY + GlobalField.GNextDis;
                lastPoint = GlobalField.SampleMotionY + GlobalField.GTestDis;
                pegPkg.engToEncBitCode = 0x00000000;
                pegPkg.axis = 1;
            }
            pegPkg.gpOutsBitCode = 0b000;
            pegPkg.pulseWidth = width;
            pegPkg.startPosition = firstPoint;
            pegPkg.pegInterval = interval;
            pegPkg.endPosition = lastPoint;
            GlobalField._ACS.ConfigurePeg(pegPkg);
        }
        #endregion

        #region NI板卡
        NationalInstruments.DAQmx.Task myTask;
        AnalogSingleChannelReader aReader;
         bool bAcqState;
        void AcqTask(PeiZhiCanShu peizhicanshu)
        {
            myTask?.Dispose();
            myTask = null;
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
        private void myCallback(object sender, EveryNSamplesReadEventArgs e)
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
        public void UpdateProgressBar(int value)
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke(new Action<int>(UpdateProgressBar), value);
            }
            else
            {
                progressBar1.Value = value;
            }
        }
    }
}
