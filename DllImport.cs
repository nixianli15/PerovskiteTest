using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Px00Test
{
    internal class DllImport
    {
        internal enum SweepType
        {
            Type_Mode_Sweep,    // 序列扫描
            Type_Mode_List,     // 自定义扫描
        };

        const string dllPath = "PssSx00Dll_C.dll";

        /* 1 通过网络打开设备并返回设备ID(>=0) */
        [DllImport(dllPath, EntryPoint = "Pss_Sx_OpenNetDevice", CallingConvention = CallingConvention.StdCall)]
        public static extern int Pss_Sx_OpenNetDevice(string devIP);

        /* 2 通过串口打开设备并返回设备ID(>=0) */
        [DllImport(dllPath, EntryPoint = "Pss_Sx_OpenUartDevice", CallingConvention = CallingConvention.StdCall)]
        public static extern int Pss_Sx_OpenUartDevice(string uartName, int baudRate);

        /* 3 关闭设备 */
        [DllImport(dllPath, EntryPoint = "Pss_Sx_CloseDevice", CallingConvention = CallingConvention.StdCall)]
        public static extern int Pss_Sx_CloseDevice(int devID);

        /* 4 获取设备IDN */
        [DllImport(dllPath, EntryPoint = "Pss_Sx_GetIDN", CallingConvention = CallingConvention.StdCall)]
        public static extern int Pss_Sx_GetIDN(int devID, StringBuilder outIDN, ref int outIDNSize);

        /* 5 设置源类型 */
        [DllImport(dllPath, EntryPoint = "Pss_Sx_SetSourceV", CallingConvention = CallingConvention.StdCall)]
        public static extern int Pss_Sx_SetSourceV(int devID, bool isVSrc);

        /* 6 设置源量程 */
        [DllImport(dllPath, EntryPoint = "Pss_Sx_SetSrcRange", CallingConvention = CallingConvention.StdCall)]
        public static extern int Pss_Sx_SetSrcRange(int devID, bool isVSrc, double range);

        /* 7 设置源值 */
        [DllImport(dllPath, EntryPoint = "Pss_Sx_SetSrcVal", CallingConvention = CallingConvention.StdCall)]
        public static extern int Pss_Sx_SetSrcVal(int devID, bool isVSrc, double value);

        /* 8 设置限量程 */
        [DllImport(dllPath, EntryPoint = "Pss_Sx_SetLmtRange", CallingConvention = CallingConvention.StdCall)]
        public static extern int Pss_Sx_SetLmtRange(int devID, bool isVSrc, double range);

        /* 9 设置限值 */
        [DllImport(dllPath, EntryPoint = "Pss_Sx_SetLmtVal", CallingConvention = CallingConvention.StdCall)]
        public static extern int Pss_Sx_SetLmtVal(int devID, bool isVSrc, double value);

        /* 10 设置2/4线 */
        [DllImport(dllPath, EntryPoint = "Pss_Sx_Set4Wire", CallingConvention = CallingConvention.StdCall)]
        public static extern int Pss_Sx_Set4Wire(int devID, bool is4Wire);

        /* 设置输出前后面板 */
        [DllImport(dllPath, EntryPoint = "Pss_Sx_SetFront", CallingConvention = CallingConvention.StdCall)]
        public static extern int Pss_Sx_SetFront(int devID, bool isFront);

        /* 11 设置采样延时 */
        [DllImport(dllPath, EntryPoint = "Pss_Sx_SetSampleDelay", CallingConvention = CallingConvention.StdCall)]
        public static extern int Pss_Sx_SetSampleDelay(int devID, int delayUs);

        /* 13 设置nplc */
        [DllImport(dllPath, EntryPoint = "Pss_Sx_SetNPLC", CallingConvention = CallingConvention.StdCall)]
        public static extern int Pss_Sx_SetNPLC(int devID, bool isVSrc, double nplc);

        /* 14 开启输出 */
        [DllImport(dllPath, EntryPoint = "Pss_Sx_SetOutput", CallingConvention = CallingConvention.StdCall)]
        public static extern int Pss_Sx_SetOutput(int devID, bool on);

        /* 27 读取测量结果 */
        [DllImport(dllPath, EntryPoint = "Pss_Sx_GetMeasureResult", CallingConvention = CallingConvention.StdCall)]
        public static extern int Pss_Sx_GetMeasureResult(int devID, ref double rstV, ref double rstI);

        /* 18 设置扫描模式 */
        [DllImport(dllPath, EntryPoint = "Pss_Sx_SetSweepMode", CallingConvention = CallingConvention.StdCall)]
        public static extern int Pss_Sx_SetSweepMode(int devID, bool isSourceV, int sweepType);

        /* 19 设置扫描开始值(V/A) */
        [DllImport(dllPath, EntryPoint = "Pss_Sx_SetSweepStartValue", CallingConvention = CallingConvention.StdCall)]
        public static extern int Pss_Sx_SetSweepStartValue(int devID, bool isSourceV, double startVal);

        /* 20 设置扫描结束值(V/A) */
        [DllImport(dllPath, EntryPoint = "Pss_Sx_SetSweepStopValue", CallingConvention = CallingConvention.StdCall)]
        public static extern int Pss_Sx_SetSweepStopValue(int devID, bool isSourceV, double stopVal);

        /* 21 设置扫描点数 */
        [DllImport(dllPath, EntryPoint = "Pss_Sx_SetSweepPoints", CallingConvention = CallingConvention.StdCall)]
        public static extern int Pss_Sx_SetSweepPoints(int devID, int points);

        /* 22 设置自定义扫描数据 */
        [DllImport(dllPath, EntryPoint = "Pss_Sx_SetCustomSweepData", CallingConvention = CallingConvention.StdCall)]
        public static extern int Pss_Sx_SetCustomSweepData(int devID, bool isSourceV, double[] data, int points);

        /* 28 读取扫描或自定义扫描结果 */
        [DllImport(dllPath, EntryPoint = "Pss_Sx_GetSweepResult", CallingConvention = CallingConvention.StdCall)]
        public static extern int Pss_Sx_GetSweepResult(int devID, double[] vResList, double[] iResList, ref int points, int timeout = 30000);
    }
}
