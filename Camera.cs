using PerovskiteTest.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using TimeTech.Core.DataProvider;
using TimeTech.Core.Common;
using HalconDotNet;
using System.Threading;
using System.Security.Cryptography;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using TimeTech.Core;
using System.Windows.Forms.DataVisualization.Charting;

namespace PerovskiteTest
{
    public partial class Camera : CameraServiceProvider
    {
        ushort cIndex = 0, stepIndex = 0;
        HalconProvider halconProvider = new HalconProvider();
        CheckCameraFocus checkCameraFocus = new CheckCameraFocus();
        bool IsOpenTrigger = false;
        ImageOperateEnum ImageOperate;
        int selectThumbnailReg;
        //  白点      黑点        白线      发亮    发暗    边缘侵入       缺失     条纹亮         条纹暗      麻点     划伤
        //int whitePoint, blackPoint, whiteLine, shiny, darken, edgeIntrusion, missing, brightStripes, darkStripes, pitting, scratch = 0;
        //主界面添加光电压扫描
        public Action<int> AddFrmPhotovoltageScan;
        private void GlobalInit()
        {
            try
            {
                GlobalField.CameraSN1 = ushort.Parse(ConfigurationManager.AppSettings["CameraSN1"]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Exit();
            }
        }

        public Camera(ushort cIndex)
        {
            InitializeComponent();

            this.cIndex = cIndex;
            //GlobalInit();
        }

        #region 放大缩小
        Point mouseDownPoint = new Point();
        bool flagIsMove = false;
        int multiple = 1;
        double Magnification = double.Parse(ConfigurationManager.AppSettings["Magnification"]);

        private void PB_ImageResult_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDownPoint.X = System.Windows.Forms.Cursor.Position.X; //记录鼠标左键按下时位置
                mouseDownPoint.Y = System.Windows.Forms.Cursor.Position.Y;
                flagIsMove = true;
                PB_ImageResult.Focus(); //鼠标滚轮事件(缩放时)需要picturebox有焦点
            }
        }

        private void PB_ImageResult_MouseMove(object sender, MouseEventArgs e)
        {
            
            if (flagIsMove)
            {
                PB_ImageResult.Focus(); //鼠标在picturebox上时才有焦点，此时可以缩放
                int x, y;   //新的PB_ImageResult.Location(x,y)
                int moveX, moveY; //X方向，Y方向移动大小。
                moveX = System.Windows.Forms.Cursor.Position.X - mouseDownPoint.X;  //当前鼠标位置减去鼠标点击时记录的位置
                moveY = System.Windows.Forms.Cursor.Position.Y - mouseDownPoint.Y;
                if (ImageOperate.ToString().Contains("FrameSelect"))
                {
                    x = frameSelectPoint.X + moveX;
                    y = frameSelectPoint.Y + moveY;
                    frameSelectPoint = new Point(x, y);
                    PB_ImageResult.Image = bmpResult;
                    Graphics g = PB_ImageResult.CreateGraphics();
                    g.DrawImage(bmpframeSelect, frameSelectPoint);  //在新位置画框
                }
                if (ImageOperate == ImageOperateEnum.CursorArrow)
                {
                    x = PB_ImageResult.Location.X + moveX;
                    y = PB_ImageResult.Location.Y + moveY;
                    PB_ImageResult.Location = new Point(x, y);
                }
                mouseDownPoint.X = System.Windows.Forms.Cursor.Position.X;
                mouseDownPoint.Y = System.Windows.Forms.Cursor.Position.Y;
            }
        }


        private void PB_ImageResult_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                flagIsMove = false;
                //PB_ImageResult.Image = bmpResult;
                if (ImageOperate.ToString().Contains("FrameSelect"))
                {
                    PB_ImageResult.Refresh();
                    Graphics g = PB_ImageResult.CreateGraphics();
                    g.DrawImage(bmpframeSelect, frameSelectPoint);
                    MessageBox.Show($"中心坐标：{frameSelectPoint.X},{frameSelectPoint.Y}");
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                PB_ImageResult.Refresh();
                Graphics g = PB_ImageResult.CreateGraphics();
                if (bmpframeSelect!=null)
                {
                    g.DrawImage(bmpframeSelect, frameSelectPoint);
                    pnChoseOperate.Visible = true;
                    pnChoseOperate.Location = e.Location;
                }
             
            }
        }
        Point frameSelectPoint = new Point(0, 0);  //选择框的起始点
        Image bmpframeSelect = null, bmpResult = null, bmpOrg = null;
        /// <summary>
        /// 框选
        /// </summary>
        private void pbFrameSelect_Click(object sender, EventArgs e)  //画500um*500um框
        {
            PB_ImageResult.Refresh();
            //PB_ImageResult.Image = bmpResult;
            frameSelectPoint = new Point(0, 0);//框选位置清零
            ImageOperate = ImageOperateEnum.FrameSelect;
            double utemp = Magnification * (multiple - 1);
            utemp = 100 * (utemp == 0 ? 1 : utemp);//设置框的大小
            bmpframeSelect = new Bitmap(Convert.ToInt16(utemp), Convert.ToInt16(utemp));
            Graphics g1 = Graphics.FromImage(bmpframeSelect);
            Rectangle ret = new Rectangle(0, 0, bmpframeSelect.Width - 2, bmpframeSelect.Height - 2);
            Pen pen = new Pen(Color.Red);
            g1.DrawRectangle(pen, ret);
            Graphics g = PB_ImageResult.CreateGraphics();
            g.DrawImage(bmpframeSelect, new PointF(0, 0));
            ImageOperateChange();

        }
        private void pbFrameSelect2_Click(object sender, EventArgs e)  //画1*1mm框
        {
            PB_ImageResult.Refresh();
            frameSelectPoint = new Point(0, 0);//框选位置清零
            ImageOperate = ImageOperateEnum.FrameSelect2;//设置当前图片操作类型
            double utemp = Magnification * (multiple - 1);//获取放大倍率
            utemp = 200 * (utemp == 0 ? 1 : utemp);//设置框的大小
            bmpframeSelect = new Bitmap(Convert.ToInt16(utemp), Convert.ToInt16(utemp));
            Graphics g1 = Graphics.FromImage(bmpframeSelect);
            Rectangle ret = new Rectangle(0, 0, bmpframeSelect.Width - 2, bmpframeSelect.Height - 2);
            Pen pen = new Pen(Color.Red);
            g1.DrawRectangle(pen, ret);
            Graphics g = PB_ImageResult.CreateGraphics();
            g.DrawImage(bmpframeSelect, frameSelectPoint);
            ImageOperateChange();
        }
        #endregion

        private void ControlInit()
        {
            panelEx1.BorderColor = Color.Black;
            panelEx1.BorderWidth = 1;
            panelEx1.Width = panelEx2.Width = PB_ImageResult.Width = this.Width / 2;
            chart1.Left = panel1.Left = panelEx1.Width + 30;
            chart1.Height = this.Height / 2 - 20;
            panel1.Height = this.Height / 2 - 120;

            chart1.Top =(this.Height-100) / 2;
            panelEx1.Height = PB_ImageResult.Height = this.Height - 200 > panelEx1.Width ? panelEx1.Width : this.Height - 200;
            //bmpOrg = bmpResult = Image.FromFile("D:\\123123123.png");
            
            //bmpOrg = bmpResult = (Image)picTemp.Clone();
            PB_ImageResult.Image = Image.FromFile("D:\\ORGIMG\\0.png");
            //PB_ImageResult.Width = 1000;//bmpResult.Width;
            //PB_ImageResult.Height = (int)((double)PB_ImageResult.Width / (double)bmpResult.Width * (double)bmpResult.Height);
            PB_ImageResult.Location = new Point(0, 0);
            //label1.BackColor= Color.Transparent;
            //label1.Parent=PB_ImageResult;
            //label1.ForeColor= Color.Red;
            label1.Text = $"———\r\n{1} mm   比例尺";
            pbEnlarge.Image = Image.FromFile("Enlarge.png");
            pbNarrow.Image = Image.FromFile("Narrow.png");
            pbFrameSelect.Image = pbFrameSelect2.Image = Image.FromFile("FrameSelect.png");
            pbCursorArrow.Image = Image.FromFile("cursor-arrow.png");
            panelEx2.Top = panelEx1.Height + 8;
            btChangeImage.ForeColor = Color.FromArgb(68, 114, 196);

            //picBoxThum1.Image = Image.FromFile("D:\\ORGIMG\\0.png");
            //picBoxThum2.Image = Image.FromFile("D:\\ORGIMG\\1.png");
            //picBoxThum3.Image = Image.FromFile("D:\\ORGIMG\\3.png");
            //picBoxThum4.Image = Image.FromFile("D:\\ORGIMG\\2.png");
            updateGraph();

        }
        private void updateGraph()
        {
            chart1.ChartAreas[0].AxisX.Interval = 1;
            ChartArea chartArea = chart1.ChartAreas[0];
            chartArea.AxisY.Minimum = 1;
            chartArea.AxisY.Maximum = 100;
            chartArea.AxisY.Title = "百分比";
            Series series = chart1.Series[0];
            //series.Points.Clear();  
            series.Points.AddXY("白点", base.whitepoint);
            series.Points.AddXY("黑点", base.Blackspot);
            series.Points.AddXY("白线", base.whiteline);
            series.Points.AddXY("发亮", base.shine);
            series.Points.AddXY("发暗", base.dim);
            series.Points.AddXY("边缘侵入", base.invade);
            series.Points.AddXY("缺失", base.Missing);
            series.Points.AddXY("条纹亮", base.Brightstripe);
            series.Points.AddXY("条纹暗", base.Darkstripes);
            series.Points.AddXY("麻点", base.Pockmark);
            series.Points.AddXY("划伤", base.scratch);
            //series.Points[0].Color = Color.Red;
            chart1.Legends[0].Docking = Docking.Bottom;
            chart1.Refresh();
        }

        private void Camera_Load(object sender, EventArgs e)
        {
            ushort expoT = 0;
            ushort expoG = 0;
            ControlInit();
            //int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            //int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            ////PB_ImageResult.Image = Image.FromFile("D:\\work\\需求文档\\MicroLed检测流程手绘图.jpg");
            ////PB_Camera.Image = Image.FromFile("D:\\work\\需求文档\\MicroLed检测流程手绘图.jpg");
            //this.Width = screenWidth - 126;
            //this.Height = screenHeight - 119;

            base.pictureBox = PB_ImageResult;  //显示原图
            base.chart1 = this.chart1;
            base.pictureBoxThum1 = picBoxThum1;  //显示缩略图1
            base.pictureBoxThum2 = picBoxThum2;  //显示缩略图2
            base.pictureBoxThum3 = picBoxThum3;  //显示缩略图3
            base.pictureBoxThum4 = picBoxThum4;  //显示缩略图4
            base.cIndex = this.cIndex;  //区分明场荧光EL                  
           
        }

        private async void AutoFocus()
        {
            await Task.Run(() =>
            {
                List<CameraIsFocusOBJ> objList = new List<CameraIsFocusOBJ>();
                //ushort intiFValue = ushort.Parse(ConfigurationManager.AppSettings["intiFValue"]), //初始值
                ushort intiFValue = (ushort)(GlobalField.focusInitialVal > 150 ? GlobalField.focusInitialVal :200 );
                ushort FocusValue = 0;
                
                for (ushort i = 0; i <= 15; i++)  //往上取15张图
                {
                    FocusValue = (ushort)(intiFValue + i * 10);//当前焦距值
                    GlobalField.lensConnectBLL.FocusOperation(FocusValue);
                    HImage image;
                    do { Thread.Sleep(10); } while (bmpFocus == null);
                    halconProvider.Bitmap2HObject24(bmpFocus, out image);  //取图
                    CameraIsFocusOBJ objReturn = checkCameraFocus.CameraIsFocus(image, FocusValue);
                    objList.Add(objReturn);
                    //bmpFocus.Save("D:\\钙钛矿\\program\\PerovskiteTest\\bin\\Debug\\IMG\\20231221\\"+i+".png");//调试用
                    //bmpFocus = null;
                    UpdateProgressBar(100*i / 30);
                }
                for (ushort i = 1; i <= 15; i++)  //往下取15张图
                {
                    FocusValue = (ushort)(intiFValue - i * 10);
                    GlobalField.lensConnectBLL.FocusOperation(FocusValue);
                    HImage image;
                    do { Thread.Sleep(10); } while (bmpFocus == null);
                    halconProvider.Bitmap2HObject24(bmpFocus, out image);  //取图
                    CameraIsFocusOBJ objReturn = checkCameraFocus.CameraIsFocus(image, FocusValue);
                    objList.Add(objReturn);
                    //bmpFocus.Save("D:\\钙钛矿\\program\\PerovskiteTest\\bin\\Debug\\IMG\\20231221\\"+i+".png");//调试用
                    //bmpFocus = null;
                    UpdateProgressBar(100*(i+15) / 30);
                }
                ushort bestFocusValue = objList.First(f => f.Eval == objList.Max(d => d.Eval)).FocusValue;  //找最清晰的图
                GlobalField.lensConnectBLL.FocusOperation(bestFocusValue); //调焦
            });
            //return;

        }

        private void SaveTestResult(string imgSaveName, List<string> result)
        {
            string strResult = result.Aggregate("", (current, s) => current + (s + "&")).TrimEnd('&');
            AreaInfo areaInfo = new AreaInfo()
            {
                areaCode = 1,
                imgName = imgSaveName,
                serialNum = "test001",
                createTime = DateTime.Now,
                stepId = stepIndex
            };
            int areaId = areaInfo.SaveModelM();
            DefectInfo defectInfo = new DefectInfo()
            {
                areaId = areaId,
                defectInfo = strResult,
            };
            defectInfo.SaveModelM();
        }

        private void pbEnlarge_Click(object sender, EventArgs e)  //放大图片
        {
            if (multiple > 6)
                return;
            multiple++;
            ImageOperate = ImageOperateEnum.Enlarge;
            PB_ImageResult.Width = Convert.ToInt32(PB_ImageResult.Width * Magnification);
            PB_ImageResult.Height = Convert.ToInt32(PB_ImageResult.Height * Magnification);
            int diffX = (int)(PB_ImageResult.Width * ((Magnification - 1) / 2));
            int diffY = (int)(PB_ImageResult.Height * ((Magnification - 1) / 2));
            int diffLX = PB_ImageResult.Left - diffX;
            PB_ImageResult.Left = diffLX;
            int diffLY = PB_ImageResult.Top - diffY;
            PB_ImageResult.Top = diffLY;

            ImageOperateChange();
        }

        private void pbNarrow_Click(object sender, EventArgs e)  //缩小图片
        {
            if (multiple <= 1)
                return;
            multiple--;
            ImageOperate = ImageOperateEnum.Narrow;
            int diffX = (int)(PB_ImageResult.Width * ((Magnification - 1) / 2));
            int diffY = (int)(PB_ImageResult.Height * ((Magnification - 1) / 2));
            int diffLX = PB_ImageResult.Left + diffX;
            PB_ImageResult.Left = diffLX;
            int diffLY = PB_ImageResult.Top + diffY;
            PB_ImageResult.Top = diffLY;
            PB_ImageResult.Width = Convert.ToInt32(PB_ImageResult.Width / Magnification);
            PB_ImageResult.Height = Convert.ToInt32(PB_ImageResult.Height / Magnification);
            
            ImageOperateChange();
        }

        //选中后拖动
        private void pbCursorArrow_Click(object sender, EventArgs e)
        {
            ImageOperate = ImageOperateEnum.CursorArrow;//设置当前图片操作类型
            ImageOperateChange();
        }

       

        private void button1_Click_1(object sender, EventArgs e)
        {
            pnChoseOperate.Visible = false;
            PB_ImageResult.Refresh();
            Graphics g = PB_ImageResult.CreateGraphics();
            g.DrawImage(bmpframeSelect, frameSelectPoint);
        }

        private void btnScanStart_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                GlobalField.SampleMotionX= frameSelectPoint.X;
                GlobalField.SampleMotionY = frameSelectPoint.Y;
                if (ImageOperate== ImageOperateEnum.FrameSelect)
                {
                    GlobalField.GTestDis = 5;
                }
               else if (ImageOperate == ImageOperateEnum.FrameSelect2)
                {
                    GlobalField.GTestDis = 10;

                }
                AddFrmPhotovoltageScan(this.cIndex);
            }
            //pnChoseOperate.Visible = false;
            //PB_ImageResult.Refresh();
            //Graphics g = PB_ImageResult.CreateGraphics();
            //g.DrawImage(bmpframeSelect, frameSelectPoint);
            
        }

        //开始检测
        private void btAutoTest_Click(object sender, EventArgs e)
        {
            GlobalField.tcpConnectProvider.AxisIndex = "F1";
            GlobalField.tcpConnectProvider.AbsMove(18.0F, 15);
            base.objwtUpdateProgressBar =new Action<int>( UpdateProgressBar);     
            if (cIndex==0)
            {
                GlobalField.logText += $"明场开始检测时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\r\n";
                FileLog.AddUserLog("明场测试开始");
            }
            else if (cIndex == 1)
            {
                GlobalField.logText += $"荧光开始检测时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\r\n";
                FileLog.AddUserLog("荧光测试开始");
            }
            else if (cIndex == 2)
            {
                GlobalField.logText += $"EL开始检测时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\r\n";
                FileLog.AddUserLog("EL测试开始");
            }
            //base.PhotoPath = AppDomain.CurrentDomain.BaseDirectory + "TestData\\";
            //base.pictureBox = new PictureBox();
            //bmpOrg = bmpResult = PB_ImageResult.Image = Image.FromFile(PhotoPath + $"ORGIMG\\0 .png");
            base.OpenTrigger(1);  //打开软触发
            base.AutoTest(0);
            if (cIndex == 0)
            {
                GlobalField.logText += $"明场结束检测时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\r\n";
                FileLog.AddUserLog("明场结束检测");
            }
            else if (cIndex == 1)
            {
                GlobalField.logText += $"荧光结束检测时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\r\n";
                FileLog.AddUserLog("荧光测试开始");
            }
            else if (cIndex == 2)
            {
                GlobalField.logText += $"EL结束检测时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\r\n";
                FileLog.AddUserLog("EL测结束检测");
            }
        }

        private void btPause_Click(object sender, EventArgs e)
        {
           base.Pause();
            if (base.isPause)
                btPause.BackColor = Color.Red;
            else
                btPause.BackColor = System.Drawing.SystemColors.ControlLightLight;
        }

        //取消测试流程
        private void btReset_Click(object sender, EventArgs e)
        {
            base.Reset();
        }

        //打开相机
        private void btnOpenCamera_Click(object sender, EventArgs e)
        {
            uint expoT = 0;
            ushort expoG = 0;
            base.OnStop();
            base.OnStart(0);
            try
            {
                if (cIndex == 0)
                {
                    expoT = GlobalField.BLExpoTime == 0 ? (ushort)1000 : GlobalField.BLExpoTime;
                    expoG = GlobalField.BLExpoGain == 0 ? (ushort)1 : GlobalField.BLExpoGain;
                }
                else if (cIndex == 1)
                {
                    expoT = GlobalField.PLExpoTime == 0 ? 1000 : GlobalField.PLExpoTime;
                    expoG = GlobalField.PLExpoGain == 0 ? (ushort)1 : GlobalField.PLExpoGain;
                }
                else if (cIndex == 2)
                {
                    expoT = GlobalField.ELExpoTime == 0 ? 1000 : GlobalField.ELExpoTime;
                    expoG = GlobalField.ELExpoGain == 0 ? (ushort)1 : GlobalField.ELExpoGain;                   
                }
                //设置曝光时间和增益
                if (base.cam_ != null)
                {
                    base.SetExpoTime((int)expoT*1000);
                    base.SetExpoGain(expoG);

                    //base.cam_.put_HFlip(true);
                    //base.ROTATE(90);
                }


            }
            catch (Exception)
            {

                throw;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AutoFocus();
        }

        private void btChangeImage_Click(object sender, EventArgs e)
        {
            bool ORGDef = false;
            if (ORGDef)
            {
                switch (selectThumbnailReg)
                {
                    case 1:
                        PB_ImageResult.Image = Image.FromFile(GlobalField.savePath + $"明场\\ORGIMG\\0 .png");
                        ORGDef = false;
                        break;
                    default: break;
                }
            }
            else
            {
                switch (selectThumbnailReg)
                {
                    case 1:
                        PB_ImageResult.Image = Image.FromFile(GlobalField.savePath + $"明场\\Defect\\0 .png");
                        ORGDef = true;
                        break;
                    default: break;
                }
            }
        }

        private void picBoxThum1_Click(object sender, EventArgs e)
        {
            var img = Image.FromFile(GlobalField.savePath + $"明场\\ORGIMG\\0.png");
            if (img != null) 
            { 
                PB_ImageResult.Image = img;
                bmpResult = img;
            }
        }

        private void picBoxThum2_Click(object sender, EventArgs e)
        {
            var img = Image.FromFile(GlobalField.savePath + $"明场\\ORGIMG\\1.png");
            if (img != null) 
            {
                PB_ImageResult.Image = img;
                bmpResult = img;
            }
            
        }

        private void picBoxThum3_Click(object sender, EventArgs e)
        {
            var img = Image.FromFile(GlobalField.savePath + $"明场\\ORGIMG\\2.png");
            if (img != null)
            {
                PB_ImageResult.Image = img;
                bmpResult = img;
            }
        }

        private void picBoxThum4_Click(object sender, EventArgs e)
        {
            var img = Image.FromFile(GlobalField.savePath + $"明场\\ORGIMG\\3.png");
            if (img != null)
            {
                PB_ImageResult.Image = img;
                bmpResult = img;
            }
        }

        /// <summary>
        /// 图片操作类型转变后更改一些状态
        /// </summary>
        private void ImageOperateChange()
        {
            pbCursorArrow.BackColor = pbEnlarge.BackColor = pbNarrow.BackColor = pbFrameSelect.BackColor = pbFrameSelect2.BackColor = System.Drawing.SystemColors.Control;
            ((PictureBox)this.Controls.Find("pb" + ImageOperate.ToString(), true)[0]).BackColor = Color.Red;//设置当前选中的背景色
            double utemp = Magnification * (multiple - 1);
            utemp = utemp == 0 ? 1 : utemp;
            label1.Text = $"———\r\n{utemp} mm   比例尺";//设置比例尺倍率
        }

        // 在异步任务中更新进度条，确保在 UI 线程上更新
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

        #region 无引用代码
        //拍照
        private void button6_Click(object sender, EventArgs e)
        {
            if (IsOpenTrigger)
                base.Trigger(1);

            string fullPath = "";
            #region 初始化图片保存路径
            string imgSaveName = DateTime.Now.ToFileTime() + ".png";
            string imgSaveDir = AppDomain.CurrentDomain.BaseDirectory + "ORGIMG\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
            string imgDefectSavePath = AppDomain.CurrentDomain.BaseDirectory + "TestResultImg\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
            if (!Directory.Exists(imgSaveDir))
                Directory.CreateDirectory(imgSaveDir);
            if (!Directory.Exists(imgDefectSavePath))
                Directory.CreateDirectory(imgDefectSavePath);
            fullPath = imgSaveDir + imgSaveName;
            base.PhotoPath = fullPath;
            #endregion
            //需要优化
            var _bmp = base.GetCurPhoto;
            List<string> result = new List<string>();
            //if (stepIndex == 1)
            //    result = GlobalField.defectIdent1.GetDefectIdentResult(fullPath, ref _bmp);
            //else if (stepIndex == 2)
            //    result = GlobalField.defectIdent2.GetDefectIdentResult(fullPath, ref _bmp);
            //else if (stepIndex == 3)
            //    result = GlobalField.defectIdent1.GetDefectIdentResult(fullPath, ref _bmp);
            //_bmp.Save(imgDefectSavePath + imgSaveName);
            PB_ImageResult.Image = _bmp;
            //await Task.Run(() => { SaveTestResult(imgSaveName, result); });
            //if (stepIndex == 1)
            //{
            //    base.OnStop();
            //    if (MessageBox.Show("step1检测已完成，请关闭明场照明，打开激发光源！", "明场检测完成", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            //    {
            //        ((Form1)this.Parent.Parent.Parent).DoStep((ushort)(stepIndex + 1));
            //    }
            //    else
            //    {
            //    }
            //}
        }
        #endregion

    }

    enum ImageOperateEnum
    {
        FrameSelect = 1,
        FrameSelect2 = 2,
        CursorArrow = 9,
        Enlarge = 11,
        Narrow = 12,
    }
}
