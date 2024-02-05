using ACS.SPiiPlusNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TimeTech.Core;

namespace PerovskiteTest
{
    public class AiMoBLL2
    {
        private Api _ACS;
        private MotorStates m_nMotorState;
        public bool result;

        void _ACS_PHYSICALMOTIONEND(AxisMasks axis)
        {
            int bit = 0x01;
            int axisNo = 0;
            // Param value is bit number 
            // Bit Number = Axis Number
            for (int i = 0; i < 64; i++)
            {
                if ((int)axis == bit)
                {
                    axisNo = i;
                    break;
                }
                bit = bit << 1;
            }
        }

        void _ACS_PROGRAMEND(BufferMasks buffer)
        {
            int bit = 0x01;
            int bufferNo = 0;
            // Param value is bit number 
            // Bit Number = Axis Number
            for (int i = 0; i < 32; i++)
            {
                if ((int)buffer == bit)
                {
                    bufferNo = i;
                    break;
                }
                bit = bit << 1;
            }
        }


        private void TernminateUMD_Connection()
        {
            try
            {
                string terminateExceptionConnName = "ACS.Framework.exe";

                ACSC_CONNECTION_DESC[] connectionList = _ACS.GetConnectionsList();
                for (int index = 0; index < connectionList.Length; index++)
                {

                    if (terminateExceptionConnName.CompareTo((string)connectionList[index].Application) != 0)
                        _ACS.TerminateConnection(connectionList[index]);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
        private void AiMoConnection()
        {
            try
            {
                _ACS.OpenCommEthernetTCP(
                       "192.168.0.13",                             // IP Address (Default : 10.0.0.100)
                      701    // TCP/IP Port nubmer (default : 701)
                       );
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
        }
        public AiMoBLL2()
        {
            _ACS = new Api();

            // Register Event
            _ACS.PHYSICALMOTIONEND += _ACS_PHYSICALMOTIONEND;
            _ACS.PROGRAMEND += _ACS_PROGRAMEND;
            TernminateUMD_Connection();
            AiMoConnection();
            _ACS.Enable((Axis)0);
            _ACS.Enable((Axis)1);
            _ACS.SetVelocityImm((Axis)0, 5);
            _ACS.SetAccelerationImm((Axis)0, 100);
            _ACS.SetDecelerationImm((Axis)0, 100);
            _ACS.SetKillDecelerationImm((Axis)0, 100);
            _ACS.SetJerkImm((Axis)0, 100);

            _ACS.SetVelocityImm((Axis)1, 5);
            _ACS.SetAccelerationImm((Axis)1, 100);
            _ACS.SetDecelerationImm((Axis)1, 100);
            _ACS.SetKillDecelerationImm((Axis)1, 100);
            _ACS.SetJerkImm((Axis)1, 100);
        }

        public void SetMaxSpeed(ushort axis)
        {
            _ACS.SetVelocityImm((Axis)axis, 300);
            _ACS.SetAccelerationImm((Axis)axis, 9000);
            _ACS.SetDecelerationImm((Axis)axis, 9000);
            _ACS.SetKillDecelerationImm((Axis)axis, 30000);
            _ACS.SetJerkImm((Axis)axis, 10000);

            _ACS.SetVelocityImm((Axis)axis, 300);
            _ACS.SetAccelerationImm((Axis)axis, 9000);
            _ACS.SetDecelerationImm((Axis)axis, 9000);
            _ACS.SetKillDecelerationImm((Axis)axis, 30000);
            _ACS.SetJerkImm((Axis)axis, 10000);
        }

        public void RefMove(ushort axis, double dis)
        {
            try
            {
                _ACS.ToPoint(MotionFlags.ACSC_AMF_RELATIVE, (Axis)axis, dis);
            }
            catch (Exception ex)
            {

                throw ex;
            }
           
        }

        public void Stop(ushort axis)
        {
            _ACS.Halt((Axis)axis);
        }

        public void StopAll()
        {
            Axis[] axes = new Axis[] { (Axis)0, (Axis)1 };
            _ACS.HaltM(axes);
        }


        public void AbsMove(ushort axis, double dis)
        {
            try
            {
                _ACS.ToPoint(0, (Axis)axis, dis);

            }
            catch (Exception err)
            {
                FileLog.AddErrorLog($"艾默位移台移动指令发送失败！axis:{axis};distance:{dis};", err);
                throw;
            }
            
        }

        public void GetMotorState(ushort axis)
        {
            _ACS.GetMotorState((Axis)axis);
        }

        public double GetFPosition(ushort axis)
        {
            return _ACS.GetFPosition((Axis)axis);
        }

        public bool WaitMoveFinish(ushort axis)
        {
            int ret = (int)_ACS.GetMotorState((Axis)axis);
            if ((ret & 32) != 0)
                return true;
            else
                return false;
        }

        public void SetSpeed(ushort axis, double Velocity)
        {
            if (Velocity>200)
            {
                Velocity = 200;
            }
            _ACS.SetVelocityImm((Axis)axis, Velocity);
        }
        public int ConfigurePeg(PegPkg pkg)
        {
            try
            {
                if (pkg.axis <= (int)Axis.ACSC_NONE)
                {
                    return 0;
                }
                _ACS.AssignPegNT((Axis)pkg.axis, pkg.engToEncBitCode, pkg.gpOutsBitCode);
                _ACS.AssignPegOutputsNT((Axis)pkg.axis, pkg.outputIndex, pkg.bitConvertCode);
                _ACS.PegIncNT(MotionFlags.ACSC_NONE,
                    (Axis)pkg.axis,
                    pkg.pulseWidth,
                    pkg.startPosition,
                    pkg.pegInterval,
                    pkg.endPosition,
                    Api.ACSC_NONE, Api.ACSC_NONE);
                _ACS.WaitPegReadyNT((Axis)pkg.axis, 2000);
                _ACS.StartPegNT((Axis)pkg.axis);
                return 1;
            }
            catch (ACSException ex)
            {
                return ex.ErrorCode;
            }
        }
        public int StopPeg(int axis)
        {
            _ACS.StopPegNT((Axis)axis);
            Console.WriteLine("PEG已停止");
            return 1;
        }
        public double GetPos(int axis)
        {
            try
            {
                if (axis <= (int)Axis.ACSC_NONE)
                    return 0;
                return _ACS.GetRPosition((Axis)axis);
            }
            catch (ACSException ex)
            {
                return ex.ErrorCode;
            }
        }
        public bool WaitMoveFinish2(ushort axis)
        {
            m_nMotorState = _ACS.GetMotorState((Axis)axis);
            do { m_nMotorState = _ACS.GetMotorState((Axis)0); Thread.Sleep(2); } while ((m_nMotorState & MotorStates.ACSC_MST_INPOS) != 0);
            return true;
        }
    }
    public struct PegPkg
    {
        public int engToEncBitCode;
        public int gpOutsBitCode;
        public int outputIndex;
        public int bitConvertCode;

        public int axis;
        public double startPosition;
        public double endPosition;
        public double pegInterval;
        public double pulseWidth;
        public MotionFlags mf;
    }
}
