using Aspose.Cells.Drawing;
using HalconDotNet;
using PdfSharp.Charting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using TimeTech.Core;
using TimeTech.Core.Common;
using TimeTech.DeviceDLL.Camera;
using TimeTech.DeviceDLL.Light;
using TimeTech.DeviceDLL.Motion;
using TimeTech.DeviceDLL.OPR;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.MonthCalendar;


namespace PerovskiteTest
{
    public class CameraServiceProvider : Form
    {
        public Toupcam cam_ = null;  //相机对象
        private Bitmap bmp_ = null;
        private uint count_ = 0;
        public bool isPause = false;
        public string PhotoPath { get; set; }
        public PictureBox pictureBox { get; set; }
        public PictureBox pictureBoxThum1 { get; set; }
        public PictureBox pictureBoxThum2 { get; set; }
        public PictureBox pictureBoxThum3 { get; set; }
        public PictureBox pictureBoxThum4 { get; set; }
        public PictureBox pictureBoxResult { get; set; }

        //白点、黑点、白线、发亮、发暗、边沿侵入、缺失、条纹发亮、条纹发暗、麻点、划伤
        public double whitepoint ,Blackspot, whiteline, shine, dim, invade,Missing, Brightstripe, Darkstripes, Pockmark, scratch;
        ManualResetEvent MRE = new ManualResetEvent(true); //实例化阻塞事件

        public ushort cIndex = 0;
        HalconProvider halconProvider = new HalconProvider();

        // 创建一个 CancellationTokenSource 来生成取消令牌
        static  CancellationTokenSource cts = new CancellationTokenSource();

     

        //图像识别结果
        public List<string> list0,list1,list2,list3;

        //更新界面进度条委托
       public Action<int> objwtUpdateProgressBar;

        //更新缺陷信息表
        public Action UpdateDefectChart;

        public System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        /// <summary>
        /// 启动相机
        /// </summary>
        /// <param name="cIndex"></param>
        public void OnStart(ushort cIndex)
        {
            if (cam_ != null)
                return;

            Miicam.DeviceV2[] arr = Miicam.EnumV2();
            if (arr.Length <= 0)
                MessageBox.Show("No camera found.");
            else if (1 == arr.Length)
                startDevice(arr[0].id);
            else if (cIndex >= arr.Length)
            {
                MessageBox.Show("camera index error.");
            }
            else
            {
                startDevice(arr[cIndex].id);
            }
        }
        /// <summary>
        /// 关闭相机
        /// </summary>
        public void OnStop()
        {
            if (cam_ != null)
            {
                cam_.Stop();
                cam_.Close();
                cam_ = null;    
            }
        }

        /// <summary>
        /// 关闭所有相机
        /// </summary>
        public static void StopAllCamera()
        {
            Miicam.DeviceV2[] arr = Miicam.EnumV2();
            for (int i = 0; i < arr.Length; i++)
            {
                var cam1_ = Miicam.Open(arr[i].id);
                if (cam1_ != null)
                {
                    cam1_.Stop();
                    cam1_.Close();
                }
            }
        }
        int blExpoTime = (int)GlobalField.BLExpoTime;
        /// <summary>
        /// 设置相机曝光时间
        /// </summary>
        /// <param name="expoValue"></param>
        public void SetExpoTime(int expoValue)
        {
            //cam_?.put_AutoExpoEnable(false);
            blExpoTime = expoValue;
            bool aa = cam_.put_ExpoTime((uint)expoValue);
            //OnExpoValueChange(sender,e);
        }

        /// <summary>
        /// 设置相机曝光增益
        /// </summary>
        /// <param name="expoGain"></param>
        public void SetExpoGain(ushort expoGain)
        {
            cam_?.put_AutoExpoEnable(false);
            bool aa = cam_.put_ExpoAGain(expoGain);
        }

        /// <summary>
        /// 镜头旋转
        /// </summary>
        /// <param name="angleNum">旋转度数</param>
        public void ROTATE(ushort angleNum)
        {
            cam_.put_Option(Toupcam.eOPTION.OPTION_ROTATE, angleNum);
            //cam_.StartPullModeWithCallback(new Miicam.DelegateEventCallback(DelegateOnEventCallback));
        }

        public int GetOption()
        {
            int iValue = 0;
            cam_.get_Option(Toupcam.eOPTION.OPTION_RAW, out iValue);
            return iValue;
        }

       
        public void OpenTrigger(int iValue)  //软触发
        {
            cam_.put_Option(Toupcam.eOPTION.OPTION_TRIGGER, iValue);
        }

        public void Trigger(ushort iNum)
        {
            cam_.Trigger(iNum);
        }
        /// <summary>
        /// 启动相机设备
        /// </summary>
        /// <param name="camId"></param>
        private void startDevice(string camId)
        {
            cam_ = Toupcam.Open(camId);

            if (cam_ != null)
            {
                //OpenTrigger(1);
                cam_?.put_AutoExpoEnable(false);
                InitExpoTimeRange();
                if (cam_.MonoMode)
                {
                }
                else
                {
                    OnEventTempTint();
                }

                uint resnum = cam_.ResolutionNumber;
                uint eSize = 0;
                if (cam_.get_eSize(out eSize))
                {

                    int width = 0, height = 0;
                    if (cam_.get_Size(out width, out height))
                    {
                        /* The backend of Winform is GDI, which is different from WPF/UWP/WinUI's backend Direct3D/Direct2D.
                         * We use their respective native formats, Bgr24 in Winform, and Bgr32 in WPF/UWP/WinUI
                         */
                        bmp_ = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                        if (!cam_.StartPullModeWithCallback(new Toupcam.DelegateEventCallback(DelegateOnEventCallback)))  
                            MessageBox.Show("Failed to start camera.");
                        else
                        {
                            //bool autoexpo = true;
                            //cam_.get_AutoExpoEnable(out autoexpo);
                            cam_?.put_AutoExpoEnable(true);
                        }
                    }
                }
            }
        }
        private void DelegateOnEventCallback(Toupcam.eEVENT evt)
        {
            /* this is call by internal thread of miicam.dll which is NOT the same of UI thread.
             * Why we use BeginInvoke, Please see:
             * http://msdn.microsoft.com/en-us/magazine/cc300429.aspx
             * http://msdn.microsoft.com/en-us/magazine/cc188732.aspx
             * http://stackoverflow.com/questions/1364116/avoiding-the-woes-of-invoke-begininvoke-in-cross-thread-winform-event-handling
             */
            BeginInvoke((Action)(() =>
            {
                /* this run in the UI thread */
                if (cam_ != null)
                {
                    switch (evt)
                    {
                        case Toupcam.eEVENT.EVENT_ERROR:
                            OnEventError();
                            break;
                        case Toupcam.eEVENT.EVENT_DISCONNECTED:
                            OnEventDisconnected();
                            break;
                        case Toupcam.eEVENT.EVENT_EXPOSURE:
                            OnEventExposure();
                            break;
                        case Toupcam.eEVENT.EVENT_IMAGE:  //视频数据到达
                            OnEventImage();
                            break;
                        case Toupcam.eEVENT.EVENT_STILLIMAGE:
                            OnEventStillImage();
                            break;
                        case Toupcam.eEVENT.EVENT_TEMPTINT:
                            OnEventTempTint();
                            break;
                        default:
                            break;
                    }
                }
            }));
           
        }

        private void InitExpoTimeRange()
        {
            OnEventExposure();
        }

        private void OnEventError()
        {
            cam_.Close();
            cam_ = null;
            MessageBox.Show("Generic error.");
        }

        private void OnEventDisconnected()
        {
            cam_.Close();
            cam_ = null;
            MessageBox.Show("Camera disconnect.");
        }
        private int Exposure = 10000;
        private void OnEventExposure()
        {
            uint nTime = 0;
            if (blExpoTime <= 0)
            {
                if (cam_.get_ExpoTime(out nTime))
                {
                    Exposure = (int)nTime;
                }
            }
            else
                Exposure = blExpoTime;
        }

        private void OnEventImage()
        {
            int num = 0;
            if (bmp_ != null)
            {
                Toupcam.FrameInfoV3 info = new Toupcam.FrameInfoV3();
                bool bOK = false;
                try
                {
                    BitmapData bmpdata = bmp_.LockBits(new Rectangle(0, 0, bmp_.Width, bmp_.Height), ImageLockMode.WriteOnly, bmp_.PixelFormat);
                    try
                    {
                        bOK = cam_.PullImageV3(bmpdata.Scan0, 0, 24, bmpdata.Stride, out info); // check the return value 获得照片
                        int iValue = 0;
                        cam_.get_Option(Toupcam.eOPTION.OPTION_TRIGGER, out iValue);
                    }
                    finally
                    {
                        bmp_.UnlockBits(bmpdata);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                if (bOK)
                {
                    if(bmpFocus == null)
                        bmpFocus = (Bitmap)bmp_.Clone();
                    bTempORG = (Bitmap)bmp_.Clone();
                    btThumbnail = (Bitmap)bmp_.Clone();
                    btAIDefect = (Bitmap)bmp_.Clone();
                    if (this.pictureBox != null)
                    {
                        pictureBox.Image = bmp_;                       
                    }
                    if (!string.IsNullOrWhiteSpace(PhotoPath))
                    {
                        switch (testIndex)
                        {
                            case 0: num = 0; break;
                            case 1: num = 2; break;
                            case 2: num = 3; break;
                            case 3: num = 1; break;
                            default:
                                break;
                        }
                        SaveORGImage(PhotoPath + $"ORGIMG\\{num}.png");
                        SaveThumbnail(PhotoPath + $"Thumbnail\\{num}.png");
                        SaveDefectImage(PhotoPath + $"Defect\\{num}.png");
                        
                    }
                }
            }
        }

      
        public Bitmap bmpFocus { get; set; }

        private void OnEventStillImage()
        {

            Toupcam.FrameInfoV3 info = new Toupcam.FrameInfoV3();
            if (cam_.PullImageV3(IntPtr.Zero, 1, 24, 0, out info))   /* peek the width and height */
            {
                Bitmap sbmp = new Bitmap((int)info.width, (int)info.height, PixelFormat.Format24bppRgb);
                bool bOK = false;
                try
                {
                    BitmapData bmpdata = sbmp.LockBits(new Rectangle(0, 0, sbmp.Width, sbmp.Height), ImageLockMode.WriteOnly, sbmp.PixelFormat);
                    try
                    {
                        bOK = cam_.PullImageV3(bmpdata.Scan0, 1, 24, bmpdata.Stride, out info); // check the return value
                    }
                    finally
                    {
                        sbmp.UnlockBits(bmpdata);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                if (bOK)
                    sbmp.Save(string.Format("demowinformcs_{0}.jpg", ++count_), ImageFormat.Jpeg);
            }
        }

        private void OnExpoValueChange(object sender, EventArgs e)
        {
            bool aa = cam_.put_ExpoTime((uint)(blExpoTime <= 0 ? Exposure : blExpoTime));
            MessageBox.Show(aa.ToString());
        }

        private void OnEventTempTint()
        {
            int nTemp = 0, nTint = 0;
            if (cam_.get_TempTint(out nTemp, out nTint))
            {
            }
        }
        private void OnWhiteBalanceOnce(object sender, EventArgs e)
        {
            cam_?.AwbOnce();
        }

       
        public Bitmap GetCurPhoto { get { return bTempORG; } }
        Bitmap bTempORG = null, btThumbnail= null,btAIDefect=null;

        //保存原图
        public async void SaveORGImage(string savePath)
        {
            await Task.Run(() =>
            {
                try
                {
                    bTempORG.Save(savePath);
                    bTempORG.Dispose();
                    IsSaveedORG = true;
                }
                catch (Exception ex) { FileLog.AddErrorLog($"{testIndex}...存储原图片报错：", ex);
                    GlobalField.logText += $"存储原图片{testIndex}报错：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\r\n";}
            });
        }

        //生成缩略图并保存，画缩略图到界面
        private async void SaveThumbnail(string savePath)
        {        
            await Task.Run(() =>
            {
                try
                {
                    TimeTech.Core.Common.ImageOperate imageOperate = new TimeTech.Core.Common.ImageOperate();
                    var retBMP = imageOperate.MakeLowQualityThumbnail(btThumbnail, 200, 200, TimeTech.Core.Common.ImageOperate.mode.MaxHW, savePath);
                    switch (testIndex)
                    {
                        case 0:
                            pictureBoxThum1.Image = retBMP;
                            break;
                        case 1:
                            pictureBoxThum2.Image = retBMP;
                            break;
                        case 2:
                            pictureBoxThum3.Image = retBMP;
                            break;
                        case 3:
                            pictureBoxThum4.Image = retBMP;
                            break;
                        default:
                            break;
                    }
                    IsSaveedThu = true;
                }
                catch (Exception ex)
                {
                    FileLog.AddErrorLog($"{testIndex}...存储缩略图片报错：", ex);
                    GlobalField.logText += $"存储缩略图片报错：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\r\n";
                }
               
            });
           
        }

        //保存AI识别缺陷图片
        private async void SaveDefectImage(string savePath)
        {
            Bitmap bitmap2 = null, bitDefImage=null, clipPic=null;
            Bitmap btAIDefectConvert = (Bitmap)btAIDefect.Clone();
            List<Halcon_DefectIdentInfo> list = new List<Halcon_DefectIdentInfo>();
            int[] hv_Area = null;
            await Task.Run(() =>
            {
                try
                {
                    switch (cIndex)  //保存路径
                    {
                        case 0:  //明场
                            switch (testIndex)
                            {
                                case 0:

                                    GlobalField.hDevelopExport.actionBL(btAIDefect, ref clipPic, PhotoPath+ $"{ testIndex}.hobj");   //扣边旋转图  位置信息文件                                                                                                                                                            

                                    list0 = GlobalField.defectIdent1.GetDefectIdentResultForPerovskite(ref btAIDefectConvert, ref bitmap2, ref list);  //原图转黑白图bitmapForAi    bitmap2 原图转易渊彩图
                                    GlobalField.hDevelopExport.actionShow(btAIDefect, PhotoPath + $"{testIndex}.hobj", bitmap2, ref bitDefImage);
                                    GlobalField.hDevelopExport.actionArea(btAIDefectConvert, ref hv_Area);
                                    updateGraph(hv_Area);
                                    break;
                                case 1:
                                    
                                    GlobalField.hDevelopExport.actionBL(btAIDefect, ref clipPic, PhotoPath + $"{testIndex}.hobj");   //扣边旋转图  位置信息文件

                                    list1 = GlobalField.defectIdent1.GetDefectIdentResultForPerovskite(ref btAIDefectConvert, ref bitmap2, ref list);  //原图转黑白图bitmapForAi    bitmap2 原图转易渊彩图
                                    GlobalField.hDevelopExport.actionShow(btAIDefect, PhotoPath + $"{testIndex}.hobj", bitmap2, ref bitDefImage);
                                    GlobalField.hDevelopExport.actionArea(btAIDefectConvert, ref hv_Area);
                                    updateGraph(hv_Area);
                                    break;
                                case 2:
                                   
                                    GlobalField.hDevelopExport.actionBL(btAIDefect,ref  clipPic, PhotoPath + $"{testIndex}.hobj");   //扣边旋转图  位置信息文件

                                    list2 = GlobalField.defectIdent1.GetDefectIdentResultForPerovskite(ref btAIDefectConvert, ref bitmap2, ref list);  //原图转黑白图bitmapForAi    bitmap2 原图转易渊彩图
                                    GlobalField.hDevelopExport.actionShow(btAIDefect, PhotoPath + $"{testIndex}.hobj", bitmap2, ref bitDefImage);
                                    GlobalField.hDevelopExport.actionArea(btAIDefectConvert, ref hv_Area);
                                    updateGraph(hv_Area);
                                    break;
                                case 3:
                                    
                                    GlobalField.hDevelopExport.actionBL(btAIDefect, ref clipPic, PhotoPath + $"{testIndex}.hobj");   //扣边旋转图  位置信息文件

                                    list3 = GlobalField.defectIdent1.GetDefectIdentResultForPerovskite(ref btAIDefectConvert, ref bitmap2, ref list);  //原图转黑白图bitmapForAi    bitmap2 原图转易渊彩图
                                    GlobalField.hDevelopExport.actionShow(btAIDefect, PhotoPath + $"{testIndex}.hobj", bitmap2, ref bitDefImage);  //生成缺陷图
                                    GlobalField.hDevelopExport.actionArea(btAIDefectConvert, ref hv_Area);
                                    updateGraph(hv_Area);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case 1:  //荧光
                            switch (testIndex)
                            {
                                case 0:

                                    GlobalField.hDevelopExport.action(btAIDefect,ref clipPic, PhotoPath + $"{testIndex}.hobj");   //扣边旋转图  位置信息文件 
                                    list1 = GlobalField.defectIdent2.GetDefectIdentResultForPerovskite(ref btAIDefectConvert, ref bitmap2, ref list);  //原图转黑白图bitmapForAi    bitmap2 原图转易渊彩图
                                    GlobalField.hDevelopExport.actionShow(btAIDefect, PhotoPath + $"{testIndex}.hobj", bitmap2, ref bitDefImage);  //生成缺陷图
                                    
                                    break;
                                case 1:

                                    GlobalField.hDevelopExport.action(btAIDefect, ref clipPic, PhotoPath + $"{testIndex}.hobj");   //扣边旋转图  位置信息文件 
                                    list1 = GlobalField.defectIdent2.GetDefectIdentResultForPerovskite(ref btAIDefectConvert, ref bitmap2, ref list);  //原图转黑白图bitmapForAi    bitmap2 原图转易渊彩图
                                    GlobalField.hDevelopExport.actionShow(btAIDefect, PhotoPath + $"{testIndex}.hobj", bitmap2, ref bitDefImage);  //生成缺陷图
                                    
                                    break;
                                case 2:
  
                                    GlobalField.hDevelopExport.action(btAIDefect, ref clipPic, PhotoPath + $"{testIndex}.hobj");   //扣边旋转图  位置信息文件 
                                    list1 = GlobalField.defectIdent2.GetDefectIdentResultForPerovskite(ref btAIDefectConvert, ref bitmap2, ref list);  //原图转黑白图bitmapForAi    bitmap2 原图转易渊彩图
                                    GlobalField.hDevelopExport.actionShow(btAIDefect, PhotoPath + $"{testIndex}.hobj", bitmap2, ref bitDefImage);  //生成缺陷图

                                    break;
                                case 3:
     
                                    GlobalField.hDevelopExport.action(btAIDefect, ref clipPic, PhotoPath + $"{testIndex}.hobj");   //扣边旋转图  位置信息文件 
                                    list1 = GlobalField.defectIdent2.GetDefectIdentResultForPerovskite(ref btAIDefectConvert, ref bitmap2, ref list);  //原图转黑白图bitmapForAi    bitmap2 原图转易渊彩图
                                    GlobalField.hDevelopExport.actionShow(btAIDefect, PhotoPath + $"{testIndex}.hobj", bitmap2, ref bitDefImage);  //生成缺陷图

                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                                break;
                    }
                    bitDefImage.Save(savePath);
                    btAIDefect.Dispose();
                    bitmap2.Dispose ();
                    bitDefImage.Dispose();
                    IsSaveedDef = true;
                    btAIDefectConvert.Dispose();    
                }
                catch (Exception ex) { FileLog.AddErrorLog($"{testIndex}...存储AI图片报错：", ex);
                    GlobalField.logText += $"存储AI图片报错：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\r\n";}
            });
        }

        #region 自动拍照
        public int testIndex = 0;
        public ushort stepIndex = 1;
        float tagPosX = 0, tagPosY = 0;
        System.Timers.Timer timerPhoto = new System.Timers.Timer();
        bool IsSaveedORG,IsSaveedThu,IsSaveedDef=false;

        public async void AutoTest(ushort _stepIndex)
        {
            // 使用 Token 可以随时取消任务
            CancellationToken token = cts.Token;
           
                Task t1= Task.Run(() => { 
                    objwtUpdateProgressBar(0);
                   
                    //设置最大速度和加速度，移动到初始位置
                    GlobalField._ACS.AbsMove(0, GlobalField.SampleMotionX);
                    while (GlobalField._ACS.WaitMoveFinish(0)) { };

                    GlobalField._ACS.AbsMove(1, GlobalField.SampleMotionY);
                    while (GlobalField._ACS.WaitMoveFinish(1)) { };
                    testIndex = 0;
                    switch (cIndex)  //保存路径
                    {
                            case 0:
                            PhotoPath = GlobalField.savePath + "明场\\";
                                GlobalField.kCS_LightSource_BLL.SetBrightness(KCS_LightSource_BLL.ChannelEnum.A, (int)GlobalField.lightIntensity);
                                GlobalField.etmmens.CloseOut(); break;                           
                            case 1:
                            PhotoPath = GlobalField.savePath + "荧光\\";
                                GlobalField.kCS_LightSource_BLL.SetBrightness(KCS_LightSource_BLL.ChannelEnum.A, (int)GlobalField.lightIntensity);
                                GlobalField.etmmens.CloseOut(); break;
                            case 2:
                            PhotoPath = GlobalField.savePath + "EL\\";
                                GlobalField.kCS_LightSource_BLL.SetBrightness(KCS_LightSource_BLL.ChannelEnum.A, 0);
                                GlobalField.etmmens.StartOutVolt(); break;
                            default:
                            break;
                    }  
                    Trigger(1);  //触发拍照           
                    do { Thread.Sleep(100); }
                    while (!(IsSaveedORG&IsSaveedThu&IsSaveedDef));  //等待保存图片完成

                    Thread.Sleep(100);
                    testIndex++;
                    IsSaveedORG=IsSaveedThu = IsSaveedDef = false;
                    
                        MoveSample(token);
                        PhotoPath = null;


                       switch (cIndex)  //保存路径{
                        {
                            case 0: GlobalField.kCS_LightSource_BLL.SetBrightness(KCS_LightSource_BLL.ChannelEnum.A, 0); break;
                            case 1: GlobalField.kCS_LightSource_BLL.SetBrightness(KCS_LightSource_BLL.ChannelEnum.A, 0); break;
                            case 2: GlobalField.etmmens.CloseOut(); break;
                            default: break; 
                        }
                }, token);

            try
            {
                t1.Wait();
            }
            catch (AggregateException ex)
            {
                if (cIndex == 0)
                {
                    GlobalField.logText += $"明场取消测试时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\r\n";
                    FileLog.AddUserLog($"明场取消测试:" + ex.Message);
                }
                PhotoPath = null;
            }
        }

        private void MoveSample(CancellationToken token)
        {
            int i=0, j=0;      
            while (true)
            {
                i = testIndex % GlobalField.fieldX;
                j = testIndex / GlobalField.fieldX;
                objwtUpdateProgressBar(25 * (cIndex+1));
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                    break;       
                }
                MRE.WaitOne();
                if (testIndex >= GlobalField.fieldX * GlobalField.fieldY)  //
                {
                    GlobalField._ACS.AbsMove(0, GlobalField.SampleMotionX);
                    while (GlobalField._ACS.WaitMoveFinish(0)) { };
                    GlobalField._ACS.AbsMove(1, GlobalField.SampleMotionY);
                    while (GlobalField._ACS.WaitMoveFinish(1)) { };
                    PhotoPath = "";
                    return;
                }
                

                if (testIndex / GlobalField.fieldX  % 2 == 0) //偶数行
                {
                    GlobalField._ACS.AbsMove(1, GlobalField.SampleMotionY - i* GlobalField.fieldMoveX);
                    while (GlobalField._ACS.WaitMoveFinish(1)) { };
                    GlobalField._ACS.AbsMove(0, (GlobalField.SampleMotionX + j*GlobalField.fieldMoveY));
                    while (GlobalField._ACS.WaitMoveFinish(0)) { };

                    Trigger(1);
                    do { Thread.Sleep(100); }
                    while (!(IsSaveedORG & IsSaveedThu & IsSaveedDef));
                    testIndex++;
                    IsSaveedORG = IsSaveedThu = IsSaveedDef = false;
                }
                else //奇数行
                {
                    GlobalField._ACS.AbsMove(1, GlobalField.SampleMotionY - (GlobalField.fieldX -1- i )* GlobalField.fieldMoveX);
                    while (GlobalField._ACS.WaitMoveFinish(1)) { };
                    GlobalField._ACS.AbsMove(0, (GlobalField.SampleMotionX + j * GlobalField.fieldMoveY));
                    while (GlobalField._ACS.WaitMoveFinish(0)) { };

                    Trigger(1);  //触发拍照
                    do { Thread.Sleep(100); }
                    while (!(IsSaveedORG & IsSaveedThu & IsSaveedDef));
                    testIndex++;
                    IsSaveedORG = IsSaveedThu = IsSaveedDef = false;
                }
                
            }          
        }
        #endregion
        public void Pause()
        {
            if (!isPause)
            {
                MRE.Reset();
                isPause = true;
            }
            else
            {
                MRE.Set();
                isPause = false;
            }
        }

        #region 无引用代码
        private void makeLowQualityThumbnail()
        {
            TimeTech.Core.Common.ImageOperate imageOperate = new TimeTech.Core.Common.ImageOperate();
            string[] filePath = Directory.GetFiles(GlobalField.selectFolder);
            if (!Directory.Exists(GlobalField.selectFolder + "Thumbnail"))
            {
                Directory.CreateDirectory(GlobalField.selectFolder + "Thumbnail");
            }
            foreach (var item in filePath)
            {
                imageOperate.MakeLowQualityThumbnail(System.Drawing.Image.FromFile(item), 200, 200, TimeTech.Core.Common.ImageOperate.mode.MaxHW, item.Replace(GlobalField.selectFolder, GlobalField.selectFolder + "Thumbnail\\"));
            }
        }
        public void Merge()
        {
            List<List<System.Drawing.Image>> imageListList = new List<List<System.Drawing.Image>>();
            for (int i = 0; i < GlobalField.fieldY; i++)
            {
                List<System.Drawing.Image> listTemp = new List<System.Drawing.Image>();
                if (i % 2 == 0)
                {
                    for (int j = 0; j < GlobalField.fieldX; j++)
                    {
                        string filename = null;
                        //if (j==0)
                        //{
                        //    filename = savePath + $"缩略图\\{0}.png";
                        //}
                        //else
                        //{
                        //    filename = savePath + $"缩略图\\{3}.png";
                        //}
                        filename = GlobalField.selectFolder + $"Thumbnail\\{j + GlobalField.fieldX * i}.png";

                        if (File.Exists(filename))
                            listTemp.Add(System.Drawing.Image.FromFile(filename));
                        else
                            listTemp.Add(new Bitmap(100, 100));
                    }
                }
                else
                {
                    for (int j = GlobalField.fieldX; j > 0; j--)
                    {

                        string filename = GlobalField.selectFolder + $"Thumbnail\\{j + GlobalField.fieldX - 1}.png";
                        if (File.Exists(filename))
                            listTemp.Add(System.Drawing.Image.FromFile(filename));
                        else
                            listTemp.Add(new Bitmap(100, 100));
                    }
                }

                imageListList.Add(listTemp);
            }

            System.Drawing.Image retImage = new ImageOperate().JoinImage(imageListList);

            retImage.Save(GlobalField.selectFolder + $"Thumbnail\\big{DateTime.Now.ToString("yyyMMdd")}.png");
            foreach (var item in imageListList)
            {
                foreach (var item1 in item)
                {
                    item1.Dispose();
                }
            }
            pictureBoxResult.Image = retImage;
            //retImage.Dispose();
            GC.Collect();
            GC.WaitForFullGCComplete();
            GC.WaitForPendingFinalizers();
        }
        //把一张图片分割成4张图片
        private System.Drawing.Image[] SplitAndDisplayImage(Bitmap originalImage)
        {
            int width = originalImage.Width / 2;
            int height = originalImage.Height / 2;
            PictureBox[] pictureBoxes = null;
            System.Drawing.Image[] images = new System.Drawing.Image[width * height];
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    int index = i * 2 + j;
                    Rectangle sourceRect = new Rectangle(j * width, i * height, width, height);
                    Bitmap croppedImage = originalImage.Clone(sourceRect, originalImage.PixelFormat);
                    //pictureBoxes[index].Image = croppedImage;
                    //pictureBoxes[index].Size = new Size(width, height);
                    //pictureBoxes[index].Location = new Point(j * width, i * height);
                    images[index] = croppedImage;
                }
            }
            return images;
        }
        #endregion

        internal void Reset()
        {
            cts.Cancel();
        }
        private void updateGraph(int[] Defectareaarray)
        {
            chart1.ChartAreas[0].AxisX.Interval = 1;
            ChartArea chartArea = chart1.ChartAreas[0];
            chartArea.AxisY.Minimum = 1;
            chartArea.AxisY.Maximum = 100;
            chartArea.AxisY.Title = "百分比";
            System.Windows.Forms.DataVisualization.Charting.Series series = chart1.Series[0];
            series.Points.Clear();
            //series.Points.AddXY("白点", whitepoint);
            //series.Points.AddXY("黑点", Blackspot);
            //series.Points.AddXY("白线", whiteline);
            //series.Points.AddXY("发亮", shine);
            //series.Points.AddXY("发暗", dim);
            //series.Points.AddXY("边缘侵入", invade);
            //series.Points.AddXY("缺失", Missing);
            //series.Points.AddXY("条纹亮", Brightstripe);
            //series.Points.AddXY("条纹暗", Darkstripes);
            //series.Points.AddXY("麻点", Pockmark);
            //series.Points.AddXY("划伤", scratch);
            for (int i = 0; i < Defectareaarray.Length;i++)
            {
                series.Points.AddXY((defectType)i, (Defectareaarray[i]*1000)/4928*4928);
            }     
            //series.Points[0].Color = Color.Red;
            chart1.Legends[0].Docking = Docking.Bottom;
            chart1.Refresh();
        }

    }
    public enum defectType
    {
        白点,
        黑点,
        白线,
        发亮,
        发暗,
        边缘侵入,
        缺失,
        条纹亮,
        条纹暗,
        麻点,
        划伤,
        未知1,
        未知2,
        未知3        
    }
}
