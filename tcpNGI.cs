using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TimeTech.Core;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PerovskiteTest
{
    public class tcpNGI
    {
        Socket socket;
        public bool isConnected;
        private byte[] recvData = new byte[70000];
        int rcvCount = 0;

        //Byte[] pBufIN_LAN = new Byte[4096 * 4 + 8 + 128];
        Byte[] pBufIN_LAN = new Byte[72];
        TcpClient client = null;      
        bool m_bOpenLAN = false;
        NetworkStream TcpStream = null;
        bool m_bThreadTCP = false;
        Thread threadTCP = null;
        IAsyncResult resultTCP;
        public bool voltTesting;
        public StringBuilder strLog = new StringBuilder();
        public List<string>  TestVoltData = new List<string>();
        int readSize=0;
        public tcpNGI(string serverIp, int serverPort)
        {
            //try
            //{
            //    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //    socket.Connect(serverIp, serverPort);
            //    this.isConnected = true;
            //}
            //catch (Exception)
            //{
            //    this.isConnected = false;
            //}
            client = new TcpClient();
            resultTCP = client.BeginConnect(serverIp, serverPort, null, null);
            var success = resultTCP.AsyncWaitHandle.WaitOne(100, true);
            if (success && client.Connected)// we have connected
            {
                m_bOpenLAN = true;
            }
            else
            {
                m_bOpenLAN = false;  //throw new Exception("Failed to connect.");
            }
            if (m_bOpenLAN)
            {
                TcpStream = client.GetStream();
                if (threadTCP != null)
                {
                    m_bThreadTCP = false;
                    Thread.Sleep(200);
                    if (threadTCP.IsAlive) threadTCP.Abort();
                }
                threadTCP = new Thread(TCP_Thread_Listener);
                threadTCP.IsBackground = true;//后台线程，随主程序一起结束
                m_bThreadTCP = true;
                threadTCP.Start();

                //Task.Factory.StartNew(TCP_Thread_Listener, TaskCreationOptions.LongRunning);
            }
        }

       

        private void TCP_Write(byte[] data, int offset, int size)
        {

            if (m_bOpenLAN)
            {
                if (TcpStream.CanWrite)
                {
                    TcpStream.Write(data, offset, size);
                }
                //TcpStream.Write(data, offset, size);
                
            }
        }
        private void TCP_Thread_Listener()
        {
            
            do
            {
                try
                {

                    if (!voltTesting) continue;
                    if (!m_bOpenLAN) continue;

                    lock (TcpStream) //不需要lock 因为就这里用到
                    {
                        if (TcpStream.CanRead)
                        {
                            readSize = TcpStream.Read(pBufIN_LAN, 0, pBufIN_LAN.Length);
                            if (readSize > 0)
                            {
                                Data_Dealer(pBufIN_LAN, readSize);
                                readSize = 0;
                                Array.Clear(pBufIN_LAN, 0, pBufIN_LAN.Length);
                            }
                        }

                    }

                }
                catch
                {
                    m_bOpenLAN = false;
                    return;
                }
            } while (m_bThreadTCP);

        }
      
        
        private void Data_Dealer(byte[] ipBufIN_LAN, int readSize)
        {
            if (ipBufIN_LAN[0].ToString() == "43")
            {
                strLog.Append(Encoding.ASCII.GetString(ipBufIN_LAN).Substring(0, 13) + "\n");
                TestVoltData.Add(Encoding.ASCII.GetString(ipBufIN_LAN).Substring(0, 13));
            }
            //strLog.Append(Encoding.ASCII.GetString(ipBufIN_LAN) + "\n");

            //SendMessage("READ?\n");

        }

        public void SendMessage(string Mand)
        {
            //if (isConnected)
            //{
            //    byte[] utf8Bytes = Encoding.ASCII.GetBytes(Mand);
            //    this.socket.Send(utf8Bytes);
            //}
            byte[] utf8Bytes = Encoding.ASCII.GetBytes(Mand);
            TCP_Write(utf8Bytes, 0, utf8Bytes.Length);

        }

        //测量前发送
        public void SendMeasModeMand()
        {
            SendMessage("*RST\n");
            SendMessage("SOUR:FUNC CURR\n");
            SendMessage("SOUR:CURR:MODE FIXED\n");
            SendMessage("SENS:FUNC VOLT\n");
            SendMessage("SOUR:CURR:RANG 10E-3\n");
            SendMessage("SOUR:CURR 0\n");
            SendMessage("SENSe:VOLTage:PROT 10\n");
            SendMessage("SENSe:VOLTage:RANG 20\n");
            SendMessage("SENS:VOLT:NPLC 0.01\n");
            SendMessage("SOUR:DEL 0\n");
            SendMessage("OUTPut:STATe ON\n");
            SendMessage("SYSTem:AZERo:STATe 0\n");

            //SendMessage("TRACe:FEED:CONTrol NEXT\n");
            //SendMessage("TRACe:POINts 1500\n");
            //SendMessage("TRACe:CLEar\n");
            //SendMessage("READ?\n");
            Thread.Sleep(3);
            //socket.Receive(recvData);
            
        }

        public  void StartMeasVolt()
        {
          
            rcvCount = 0;
            Array.Clear(recvData, 0, recvData.Length);
            Thread.Sleep(1);
            //SendMessage("READ?\n");
            DateTime startTime = DateTime.Now;
            do
            {
                if (DateTime.Now.Subtract(startTime).TotalMilliseconds > 100)
                    return;

                Thread.Sleep(0);
                rcvCount = socket.Receive(recvData);
            } while (rcvCount < 1);
            this.strLog.Append(Encoding.ASCII.GetString(recvData) + "\n");

            //return Encoding.ASCII.GetString(recvData);
            //if (this.recvData[0].ToString() == "43")   //确定表测到数据没问题
            //{
            //    return Encoding.ASCII.GetString(recvData).Substring(0, 13)+"\n";
            //}
            //return "";
        }

        public async void StartMeasVoltHC()
        {

            await Task.Run(() => { SendMessage("READ?\n"); });
            
        }

        public string GetStartMeasVoltHC()
        {
            Thread.Sleep(5);
            SendMessage("TRACe: DATA?\n");
            Thread.Sleep(5);
            DateTime startTime = DateTime.Now;
            do
            {
                if (DateTime.Now.Subtract(startTime).TotalMilliseconds > 10000)
                    return "";
                Thread.Sleep(3);
                rcvCount = socket.Receive(recvData);
            } while (rcvCount < 100);
            return Encoding.ASCII.GetString(recvData);
        }

        public void CLEar()
        {
            SendMessage("TRACe:CLEar\n");
        }

        public void Close()
        {
            if (this.socket != null)
            {
                this.socket.Close();
                this.socket = null;
            }
        }
    }
}
