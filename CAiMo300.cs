using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PerovskiteTest
{
    public class CAiMo300
    {
        public IntPtr g_handle;         //链接返回的句柄，可以作为卡号
        public bool g_basflag = false;  //BAS文件加载标志位
        public float Bus_type = -1;     //BAS文件中变量判断总线类型，也作为BAS文件是否下载成功判断
        public float [] fPointPos;
        public CAiMo300(string ip) 
        {
            if (g_handle == (IntPtr)0)
            {
                C_Close_Card_Click();
            }
            zmcaux.ZAux_OpenEth(ip, out g_handle);
            if (g_handle != (IntPtr)0) { }
            int ret = 0;

            ret = zmcaux.ZAux_Direct_SetAxisEnable(g_handle, 0, 1);
            ret = zmcaux.ZAux_Direct_SetAxisEnable(g_handle, 1, 1);

            ret = zmcaux.ZAux_Direct_SetUnits(g_handle, 0, 2000);
            ret = zmcaux.ZAux_Direct_SetSpeed(g_handle, 0, 50);
            ret = zmcaux.ZAux_Direct_SetAccel(g_handle, 0, 2000);
            ret = zmcaux.ZAux_Direct_SetDecel(g_handle, 0, 2000);
            //ret = zmcaux.ZAux_Direct_SetSramp(g_handle, 0, 1);

            ret = zmcaux.ZAux_Direct_SetUnits(g_handle, 1, 2000);
            ret = zmcaux.ZAux_Direct_SetSpeed(g_handle, 1, 50);
            ret = zmcaux.ZAux_Direct_SetAccel(g_handle, 1, 2000);
            ret = zmcaux.ZAux_Direct_SetDecel(g_handle, 1, 2000);
            //ret = zmcaux.ZAux_Direct_SetSramp(g_handle, 1, 1);
        }

        public void AbsMove(ushort axis, double dis)
        {
            zmcaux.ZAux_Direct_Single_MoveAbs(g_handle, axis, (float)dis);
        }

        public bool WaitMoveFinish(int axis)
        {
            int m_nMotorState = 0;
            zmcaux.ZAux_Direct_GetIfIdle(g_handle,axis,ref m_nMotorState);
            do { zmcaux.ZAux_Direct_GetIfIdle(g_handle, axis, ref m_nMotorState); Thread.Sleep(5); }
            while (m_nMotorState  != -1);
            return true;
        }

        public void GetPos(int axis, ref float Pos)
        {
            zmcaux.ZAux_Direct_GetDpos(g_handle,axis,ref Pos);
        }
        public void SetMaxSpeed()
        {
            int ret = 0;
            ret = zmcaux.ZAux_Direct_SetUnits(g_handle, 0, 2000);
            ret = zmcaux.ZAux_Direct_SetSpeed(g_handle, 0, 200);
            ret = zmcaux.ZAux_Direct_SetAccel(g_handle, 0, 2000);
            ret = zmcaux.ZAux_Direct_SetSramp(g_handle, 0, 2000);

            ret = zmcaux.ZAux_Direct_SetUnits(g_handle, 1, 2000);
            ret = zmcaux.ZAux_Direct_SetSpeed(g_handle, 1, 200);
            ret = zmcaux.ZAux_Direct_SetAccel(g_handle, 1, 2000);
            ret = zmcaux.ZAux_Direct_SetSramp(g_handle, 1, 2000);
        }

         public void SetSpeed(int axis,double speed)
        {
            zmcaux.ZAux_Direct_SetSpeed(g_handle, axis, (float)speed);
        }
        public void C_Close_Card_Click()     //断开控制器连接
        {
            //断开链接
            zmcaux.ZAux_Close(g_handle);
            g_handle = (IntPtr)0;
        }

        //停止运动
        public void StopAll()
        {
            if (g_handle == (IntPtr)0)
            {
                MessageBox.Show("未链接到控制器!", "提示");
            }
            else
            {
                zmcaux.ZAux_Direct_Single_Cancel(g_handle, 0, 2);
                zmcaux.ZAux_Direct_Single_Cancel(g_handle, 1, 2);
            }
        }

        //设定PEG参数
        public void SetXPegPara(int axisnum, float startPos, float endPos, float nextDis)
        {
            
            int iret = 0;           
            iret = zmcaux.ZAux_Direct_HwPswitch2(g_handle, axisnum, 2, 0, 0, 0, 0, 0, 0);     //清除前面的比较输出指令
            iret = (int)((endPos - startPos) / nextDis);
            fPointPos = new float[iret];
            for (int i = 0; i < iret; i++)
            {
                fPointPos[i] = startPos +nextDis * i;
            }
            iret = zmcaux.ZAux_Direct_SetTable(g_handle, 0, fPointPos.Length - 1, fPointPos);

            iret = zmcaux.ZAux_Direct_HwPswitch2(g_handle, axisnum, 1, 0, 1, 0, fPointPos.Length - 1, 1, 0);
          
            zmcaux.ZAux_Trigger(g_handle);
        }

        public void ClearPegPara(int axisnum)
        {
            int iret = 0;
            iret = zmcaux.ZAux_Direct_HwPswitch2(g_handle, axisnum, 2, 0, 0, 0, 0, 0, 0);     //清除前面的比较输出指令
        }
    }
}
