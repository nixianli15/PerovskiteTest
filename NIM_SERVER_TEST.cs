using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PerovskiteTest
{
    public class NIM_SERVER_TEST
    {
        int nRe = 0;
        uint hMaster = 0;
        int[] nAddrs = new int[10];
        public bool comState;  //有无通讯上
        public NIM_SERVER_TEST(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: NimServoSDK_Test2 commType commParam [unitFactor]");
                return;
            }

            int nCommType = int.Parse(args[0]);
            // if (0 != NimServoSDK.Nim_init("D:\\03-SDK\\NimServoSDK-MM-bin-Windows-X64\\bin"))
            if (0 != NimServoSDK.Nim_init(""))
            {
                Console.WriteLine("exec Nim_init faild");
            }
            
            if (0 != NimServoSDK.Nim_create_master(nCommType, ref hMaster))
            {
                Console.WriteLine("exec Nim_create_master faild\n");
            }
            if (nCommType < 0 || nCommType > 2)
                return;

            string conn_str = "";
            switch (nCommType)
            {
                case 0:     //CANopen
                    int nDevType = int.Parse(args[1]);
                    switch (nDevType)
                    {
                        case 1001:      //NiMotion USB-CAN Device
                            conn_str = "{\"DevType\": \"1001\", \"DevIndex\": 0, \"Baudrate\": 8,"
                                        + "\"PDOIntervalMS\": 10, \"SyncIntervalMS\": 10}";
                            break;
                        case 1002:      //Zhoulg USB-CAN Device
                            conn_str = "{\"DevType\": \"1002\", \"DevSubType\": 3, \"DevIndex\": 0, \"ChannelIndex\": 0,"
                                        + " \"Baudrate\": 8, \"PDOIntervalMS\": 10, \"SyncIntervalMS\": 10}";
                            break;
                        case 1003:      //Ixxat USB-CAN Device
                            conn_str = "{\"DevType\": \"%s\", \"DevIndex\": 0, \"Baudrate\": 8,"
                                        + " \"PDOIntervalMS\": 10, \"SyncIntervalMS\": 10}";
                            break;
                        case 1004:      //NiMotion TCP-CAN Device
                            conn_str = "{\"DevType\": \"1004\", \"IP\": \"192.168.0.96\", \"Port\": 40001,"
                                        + " \"Baudrate\": 8, \"PDOIntervalMS\": 10, \"SyncIntervalMS\": 10}";
                            break;
                        case 1005:      //Linux SocketCAN Device
                            conn_str = "{\"DevType\": \"%s\", \"DeviceName\": \"can0\","
                                        + " \"Baudrate\": 8, \"PDOIntervalMS\": 10, \"SyncIntervalMS\": 10}";
                            break;
                        default:
                            conn_str = "{\"DevType\": \"1001\", \"DevIndex\": 0, \"Baudrate\": 8,"
                                        + "\"PDOIntervalMS\": 10, \"SyncIntervalMS\": 10}";
                            break;
                    }
                    break;
                case 1:     //EtherCAT
                    conn_str = "{\"NetworkAdapter\": \"" + args[1] + "\", \"OverlappingPDO\": true, \"PDOIntervalMS\": 5}";
                    break;
                case 2:     //Modbus
                    conn_str = "{\"SerialPort\": \"" + args[1] + "\", \"Baudrate\": 115200,"
                               + " \"Parity\": \"N\", \"DataBits\": 8, \"StopBits\": 1,"
                               + " \"PDOIntervalMS\": 10, \"SyncIntervalMS\": 0}";
                    break;
                default:
                    break;
            }
            Console.WriteLine(conn_str);

            //运行主站
            if (0 != NimServoSDK.Nim_master_run(hMaster, conn_str))
            {
                Console.WriteLine("exec Nim_master_run faild");
                return;
            }
            Thread.Sleep(200);

            // 扫描电机
            //int nAddr = 0;
            
            int nSlaveCount = 0;
            NimServoSDK.Nim_scan_nodes(hMaster, 1, 10);
            for (int i = 0; i < 10; i++)
            {
                if (0 != NimServoSDK.Nim_is_online(hMaster, i))
                {
                    nAddrs[nSlaveCount] = i;
                    Console.WriteLine(string.Format("motor {0} is online", i));
                    nSlaveCount++;
                    comState = true;
                }
            }
            //if (nAddr == 0)
            //{
            //    Console.WriteLine("There is no motor online");
            //    NimServoSDK.Nim_master_stop(hMaster);
            //    return;
            //}

            // 设置传动比
            double fUnitFactor = 10000.0;
            if (args.Length >= 3)
            {
                fUnitFactor = double.Parse(args[2]);
            }
            Console.WriteLine(string.Format("UnitFactor = {0}", fUnitFactor));

            // 加载电机参数表
            
            for (int nIndex = 0; nIndex < nSlaveCount; nIndex++)
            {
                if (nCommType == 2)
                    nRe = NimServoSDK.Nim_load_params(hMaster, nAddrs[nIndex], "BLMxx_modbus_zh.db");
                else
                    nRe = NimServoSDK.Nim_load_params(hMaster, nAddrs[nIndex], "Servo_zh.db");
                if (0 != nRe)
                {
                    Console.WriteLine("exec Nim_load_params failed");
                    return;
                }
            }


            //// 读取电机PDO配置  PDO屏蔽
            //nRe = NimServoSDK.Nim_read_PDOConfig(hMaster, nAddr);
            //if (0 != nRe)
            //{
            //    Console.WriteLine("exec Nim_read_PDOConfig failed\r");
            //    return;
            //}

            // 主站切换到OP模式
            // nRe = NimServoSDK.Nim_master_changeToOP(hMaster);
            for (int nIndex = 0; nIndex < nSlaveCount; nIndex++)
            {
                nRe = NimServoSDK.Nim_set_unitsFactor(hMaster, nAddrs[nIndex], fUnitFactor);
                // 设置操作模式并抱机
                Thread.Sleep(200);
                nRe = NimServoSDK.Nim_power_off(hMaster, nAddrs[nIndex], 1);
                Thread.Sleep(200);
                nRe = NimServoSDK.Nim_set_workMode(hMaster, nAddrs[nIndex], 1, 1);
                Thread.Sleep(200);
                nRe = NimServoSDK.Nim_power_on(hMaster, nAddrs[nIndex], 1);
                Thread.Sleep(200);
                nRe = NimServoSDK.Nim_set_profileVelocity(hMaster, nAddrs[nIndex], 10.24);
                nRe = NimServoSDK.Nim_set_profileAccel(hMaster, nAddrs[nIndex], 10.24);
                nRe = NimServoSDK.Nim_set_profileDecel(hMaster, nAddrs[nIndex], 10.24);
            }
        }



        public void MoveRelative(int mIndex,double moveNum) 
        {
            nRe = NimServoSDK.Nim_moveRelative(hMaster, nAddrs[mIndex], moveNum, 0, 1);
            double fPos = 0.0;
            UInt16 statusWord = 0;
            do
            {
                Thread.Sleep(50);
                if (0 == NimServoSDK.Nim_get_currentPosition(hMaster, nAddrs[mIndex], ref fPos, 1)
                    && 0 == NimServoSDK.Nim_get_statusWord(hMaster, nAddrs[mIndex], ref statusWord, 1))
                    Console.Write(string.Format("statusword = {0:x4} \t currentpos = {1}\t\r", statusWord, fPos));
            } while ((statusWord & 0x400) == 0);
        }

        public void MoveAbsolute(int mIndex, double moveNum)
        {
            nRe = NimServoSDK.Nim_moveAbsolute(hMaster, nAddrs[mIndex], moveNum, 0, 1);
            double fPos = 0.0;
            UInt16 statusWord = 0;
            do
            {
                Thread.Sleep(50);
                if (0 == NimServoSDK.Nim_get_currentPosition(hMaster, nAddrs[mIndex], ref fPos, 1)
                    && 0 == NimServoSDK.Nim_get_statusWord(hMaster, nAddrs[mIndex], ref statusWord, 1))
                Console.Write(string.Format("statusword = {0:x4} \t currentpos = {1}\t\r", statusWord, fPos));
            } while ((statusWord & 0x400) == 0);
        }

        public void Stop()
        {
            NimServoSDK.Nim_master_stop(hMaster);
        }

        public void GoHomeZero(int mIndex)
        {
            Thread.Sleep(200);
            nRe = NimServoSDK.Nim_power_off(hMaster, nAddrs[mIndex], 1);
            Thread.Sleep(200);
            nRe = NimServoSDK.Nim_set_workMode(hMaster, nAddrs[mIndex], 6, 1);
            Thread.Sleep(200);
            nRe = NimServoSDK.Nim_power_on(hMaster, nAddrs[mIndex], 1);
            Thread.Sleep(200);
            nRe = NimServoSDK.Nim_set_goHome_velocity(hMaster, nAddrs[mIndex], 10.24, 1.024);
            nRe = NimServoSDK.Nim_set_goHome_accel (hMaster, nAddrs[mIndex], 10.24);
            nRe = NimServoSDK.Nim_set_homeOffset(hMaster, nAddrs[mIndex], 0.0);

            // 控制电机在HM模式下运行
            nRe = NimServoSDK.Nim_goHome(hMaster, nAddrs[mIndex], 27, 1);
            //printf("********************go home*********************\n");
            double fPos = 0.0;
            double fVelocity = 0.0;
            UInt16 statusWord = 0;
            do
            {
                Thread.Sleep(50);
                if (0 == NimServoSDK.Nim_get_currentPosition(hMaster, nAddrs[mIndex], ref fPos, 1)
                        && 0 == NimServoSDK.Nim_get_currentVelocity(hMaster, nAddrs[mIndex], ref fVelocity, 1)
                        && 0 == NimServoSDK.Nim_get_statusWord(hMaster, nAddrs[mIndex], ref statusWord, 1))
                    File.WriteAllText(nAddrs[mIndex].ToString() + ".txt", "回零完成");
                //printf("statusword = %04x \t curVelocity = %f \t curPositon = %f\t\r",
                //       statusWord, fVelocity, fPos);
            } while ((statusWord & 0x1000) == 0);

            //切换回位置模式
            // 设置操作模式并抱机
            Thread.Sleep(200);
            nRe = NimServoSDK.Nim_power_off(hMaster, nAddrs[mIndex], 1);
            Thread.Sleep(200);
            nRe = NimServoSDK.Nim_set_workMode(hMaster, nAddrs[mIndex], 1, 1);
            Thread.Sleep(200);
            nRe = NimServoSDK.Nim_power_on(hMaster, nAddrs[mIndex], 1);
            Thread.Sleep(200);
            nRe = NimServoSDK.Nim_set_profileVelocity(hMaster, nAddrs[mIndex], 10.24);
            nRe = NimServoSDK.Nim_set_profileAccel(hMaster, nAddrs[mIndex], 10.24);
            nRe = NimServoSDK.Nim_set_profileDecel(hMaster, nAddrs[mIndex], 10.24);
        }

        public double GetPos(int mIndex)
        {
            double fPos=0;
            NimServoSDK.Nim_get_currentPosition(hMaster, nAddrs[mIndex], ref fPos, 1);
            return fPos;
        }

        public void ASetVel(int mIndex,double dSpeed)
        {
            nRe = NimServoSDK.Nim_set_profileVelocity(hMaster, nAddrs[mIndex], dSpeed);
        }

    }
}
