using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TimeTech.DeviceDLL.Camera;
using TimeTech.DeviceDLL.Light;
using TimeTech.DeviceDLL.Motion;
using TimeTech.DeviceDLL.OPR;
using TimeTech.DeviceDLL.SMU;
using NationalInstruments.DAQmx;

namespace PerovskiteTest
{
    public enum DevStatusFlag
    {
        dsfIDLE, dsfPause,dsfRunning,dsfTestEnd,dsfTrouble
    }
    struct Spectrometer
    {
        public UInt32 nIntUS;//积分时间 微秒 接收光的时间
        public UInt16 nAverage;
        public UInt16 nBoxcar;
        public int nNumPixels;
        public string strSN;
        public float[] fWavelength;// = new float[4096];
        public float[] fIntensity;
        public int nStep;
        public int nRead;
        public int nTotal;
        public List<Byte> buf;
        public int cntScans;
        public int cntTrigs;
        public DateTime timeStamp;

        public int MAX_FRAMES;
        public List<UInt16[]> frames; //buf direct to frames
        public List<DateTime> sampleTimes;
        public void bufToIntensity()
        {
            int nOffset;

            timeStamp = DateTime.Now;

            UInt16 wIntensity;
            UInt16[] pNewData = new UInt16[nNumPixels];
            for (int i = 0; i < nNumPixels; i++)
            {
                nOffset = 6 + 2 * i;
                wIntensity = (UInt16)(buf[nOffset] * 256 + buf[nOffset + 1]);
                fIntensity[i] = (float)wIntensity;
                pNewData[i] = wIntensity;
            }
            frames.Add(pNewData);
            sampleTimes.Add(timeStamp);

            //数量太多，删除
            if (frames.Count > MAX_FRAMES) frames.RemoveAt(0);
            if (sampleTimes.Count > MAX_FRAMES) sampleTimes.RemoveAt(0);
            //cntScans = frames.Count;
            cntScans++;
        }
    };
    public class GlobalField
    {
        /// <summary>
        /// 测试次数，用来记录测试到哪个区域
        /// </summary>
        public static ushort testNum { get; set; }
        public static string _operator { get; set; }
        public static string selectFolder { get; set; }
        public static string savePath { get; set; }
        public static string serialNum { get; set; }

        public static ProductInfo productInfo { get; set; }

        public static string logText { get; set; }

        public static Siglen siglen { get; set; }

        public static ushort testSpec { get; set; }

        //苏州爱默电动位移台-检测物位移台（XY轴）
        public static AiMoBLL2 _ACS { get; set; }
        //300mm正运动控制器
        //public static CAiMo300 _ACS { get; set; }

        //万名位移台
        public static NIM_SERVER_TEST nim_Server_Test { get; set; }

        //PLC对象
        public static TcpConnectProvider tcpConnectProvider { get; set; }

        //光源控制器
        public static KCS_LightSource_BLL kCS_LightSource_BLL { get; set; }

        //恩智电源表
        public static tcpNGI nGI { get; set; }
        public static N2600Demo n2600Demo { get; set; } 

        //生成热力图类
        public static ModeHeatMap objModeHeatMap { get; set; }

        #region 相机全局参数和图像识别全局参数
        /// <summary>
        /// 明场图像识别
        /// </summary>
        public static DefectIdent defectIdent1 { get; set; }
        public static DefectIdent defectIdent2 { get; set; }
         public static DefectIdent defectIdent3 { get; set; }

        public static HDevelopExport hDevelopExport {  get; set; }  
        public static bool[] Camera_IsRun { get; set; }


        /// <summary>
        /// 相机曝光时间
        /// </summary>
        public static uint BLExpoTime { get; set; }
        public static uint PLExpoTime { get; set; }
        public static uint ELExpoTime { get; set; }

        /// <summary>
        /// 相机曝光增益
        /// </summary>
        public static ushort BLExpoGain { get; set; }
        public static ushort PLExpoGain { get; set; }
        public static ushort ELExpoGain { get; set; }
        //调焦初始参数
        public static ushort focusInitialVal { get; set; }
        //拍摄延时时间
        public static uint photographDelay { get; set; }
        /// <summary>
        /// 相机
        /// </summary>
        public static ushort CameraSN1 { get; set; }

        /// <summary>
        /// 调焦设备
        /// </summary>
        public static LensConnectBLL lensConnectBLL { get; set; }

        #endregion

        #region Setting Config
        public static float Light1Pos { get; set; }  //名场光源位置
        public static float laserPos { get; set; }  //激光光源位置

        public static float SampleMotionX { get; set; }  //测试X轴起始位置
        public static float SampleMotionY { get; set; }  //测试Y轴起始位置
        public static float Z0Num { get; set; }  //相机z轴位置1                                                                                                                                                                                                                                                                                               
        public static float Z1Num { get; set; }  //相机z轴位置2
        public static float Z2Num { get; set; }  //相机z轴位置3
        public static float fieldMoveX { get; set; }  //拍照照片X长度
        public static float fieldMoveY { get; set; }  //拍照照片Y长度
        public static float OffsetX { get; set; }
        public static float OffsetY { get; set; }

        public static ushort fieldX { get; set; }  //X方向拍照数
        public static ushort fieldY { get; set; }  //Y方向拍照数

        public static int LastZ1Num { get; set; }

        public static float GTestDis { get; set; }    //光电压扫描测试距离
        public static float GNextDis { get; set; }   //光电压扫描换行距离
        public static int GTestCount { get; set; }  //光电压扫描换行次数

        public static double XAxisMoveSpeed { get; set; }   //X轴移动速度
        public static double YAxisMoveSpeed { get; set; }   //Y轴移动速度

        public static uint eTOMMENSOutputVolt { get; set; }

        public static uint eTOMMENSOutputCurrent { get; set; }

        public static uint lightIntensity {  get; set; }

        #endregion

        public static eTOMMENS etmmens { get; set; }
    }

    public class ProductInfo
    {
        public string ProID { get; set; }
        public string Remark { get; set; }
        public string ProSize { get; set; }
        public string Manufacturer { get; set; }
        public string Tester { get; set; }
    }
}
