using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PerovskiteTest
{
    public partial class FrmspectrumAcquire : Form
    {
        SpectrometerS3000 spectrometerS3000 = new SpectrometerS3000();  //暂时不知道有没有用
        List<UInt16> listCMD = new List<UInt16>();
        Byte[] pBufIN = new Byte[4096 * 8];  
        bool m_bVCP_Frist = true;
        Spectrometer m_sp;  
        int m_iRunning = 0;
        bool m_bTrigMode = false;
        public FrmspectrumAcquire()
        {
            InitializeComponent();
        }

        private void btnConnectVCP_Click(object sender, EventArgs e)
        {
            m_bVCP_Frist = true;
            ConnectSpectrometersVCP();
        }

        private void ConnectSpectrometersVCP()
        {
            if (serialPort1.IsOpen) return;

            string[] PortNames = SerialPort.GetPortNames();    //获取本机串口名称，存入PortNames数组中
            //if (PortNames.Count() < 1) return;
            if (PortNames.Length < 1) return;

            //for 
            {
                serialPort1.BaudRate = 115200;
                serialPort1.PortName = PortNames[PortNames.Length - 1];//连接最后一个串口 //PortNames[0];//连接第一个串口 "COM6"; //"COM59";
                //serialPort1.DiscardInBuffer();//清空上次可能遗留的数据
                //serialPort1.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);//串口接收数据函数,消息驱动
                try
                {
                    serialPort1.Open();
                }
                catch (Exception e)
                {
                    serialPort1.Close();
                    setStatusVCP();
                    return;
                }

                if (serialPort1.IsOpen)
                {
                    serialPort1.DiscardInBuffer();//清空上次可能遗留的数据
                    serialPort1.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);

                    listCMD.Clear();//串口重复连接的情况
                    listCMD.Add(0x3010);
                    listCMD.Add(0x1002);
                    listCMD.Add(0x3020);
                    listCMD.Add(0x4010);
                    EXE_listCMD();
                }
                setStatusVCP();
            }
        }
        private void setStatusVCP()
        {
            if (serialPort1.IsOpen)
            {
                labelStatusVCP.Text = "Connected: " + serialPort1.PortName;
                labelStatusVCP.ForeColor = Color.Blue;
                btnConnectVCP.Enabled = false;
            }
            else
            {
                labelStatusVCP.Text = "unconnected";
                labelStatusVCP.ForeColor = Color.Red;
                btnConnectVCP.Enabled = true;
            }
        }
        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //Async 异步接收数据部分
            try //string str = serialPort1.ReadExisting();     //字符串方式读
            {
                int cbRead = serialPort1.Read(pBufIN, 0, 4096 * 4 + 8);  //读到的字节数
                Data_Dealer(pBufIN, cbRead);
            }
            catch
            {
                //MessageBox.Show(String.Format("Port Error!"));
                ///serialPort1.Close();
                ///serialPort1.DataReceived -= new SerialDataReceivedEventHandler(port_DataReceived);
            }
        }

        bool m_bOpenLAN = false;
        
        private void EXE_listCMD()
        {
            //此函数为多线程执行,不能有GUI相关控件的操作,可以用MessageBox
            if (!serialPort1.IsOpen && !m_bOpenLAN)
            {
                //MessageBox.Show(String.Format("Serial Port Closed"));
                return;
            }
            if (listCMD.Count < 1)
            {
                //MessageBox.Show(String.Format("list count < 1"));
                //if (bOpenLAN) LAN_SendData(new Byte[0], 0, 0); //Read Data IN Trigger Mode
                return;
            }

            if (listCMD[0] == 0x4010)//ReadIntensity
            {
                Byte[] cmd = new Byte[16] { 0xAA, 0x66, 0x40, 0x10, 0x00, 0x08, 0x00, 0x00, 0xFF, 0xFF, 0x00, 0x01, 0x00, 0x01, 0xFF, 0xFF };
                cmd[6] = (Byte)(m_sp.nIntUS >> 24);
                cmd[7] = (Byte)(m_sp.nIntUS >> 16);
                cmd[8] = (Byte)(m_sp.nIntUS >> 8);
                cmd[9] = (Byte)(m_sp.nIntUS);
                cmd[10] = (Byte)(m_sp.nAverage >> 8);
                cmd[11] = (Byte)(m_sp.nAverage);
                cmd[12] = (Byte)(m_sp.nBoxcar >> 8);
                cmd[13] = (Byte)(m_sp.nBoxcar);
                SendData(cmd, 0, 16); // serialPort1.Write(cmd, 0, 16);
                return;
            }

            if (listCMD[0] == 0x3010)//GetNumPixels
            {
                Byte[] cmd = new Byte[8] { 0xAA, 0x66, 0x30, 0x10, 0x00, 0x00, 0xFF, 0xFF };
                SendData(cmd, 0, 8);//serialPort1.Write(cmd, 0, 8);
                return;
            }
            if (listCMD[0] == 0x1002)//GetSerialNO
            {
                Byte[] cmd = new Byte[8] { 0xAA, 0x66, 0x10, 0x02, 0x00, 0x00, 0xFF, 0xFF };
                SendData(cmd, 0, 8);//serialPort1.Write(cmd, 0, 8);
                return;
            }
            if (listCMD[0] == 0x3020)//GetWavelength
            {
                Byte[] cmd = new Byte[8] { 0xAA, 0x66, 0x30, 0x20, 0x00, 0x00, 0xFF, 0xFF };
                SendData(cmd, 0, 8);// serialPort1.Write(cmd, 0, 8);
                return;
            }

            if (listCMD[0] == 0x4051)
            {
                //if (m_bTrigPara)//Enter Trig Mode
                Byte[] cmd = new Byte[12] { 0xAA, 0x66, 0x40, 0x51, 0x00, 0x04, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF };
                cmd[6] = (Byte)(m_sp.nIntUS >> 24);
                cmd[7] = (Byte)(m_sp.nIntUS >> 16);
                cmd[8] = (Byte)(m_sp.nIntUS >> 8);
                cmd[9] = (Byte)(m_sp.nIntUS);
                SendData(cmd, 0, 12); //serialPort1.Write(cmd, 0, 12);
                //MessageBox.Show("Enter Tirg Mode");
                return;
            }

            if (listCMD[0] == 0x4052) //Exit Trig Mode
            {
                Byte[] cmd = new Byte[8] { 0xAA, 0x66, 0x40, 0x52, 0x00, 0x00, 0xFF, 0xFF };
                SendData(cmd, 0, 8); //serialPort1.Write(cmd, 0, 8);
                //MessageBox.Show("Exit Tirg Mode");
                return;
            }
        }
        private void SendData(Byte[] cmd, int offset, int size)
        {
            if (m_bVCP_Frist)
            {
                if (serialPort1.IsOpen) { serialPort1.Write(cmd, offset, size); return; }
             
            }
         
        }

        private void Data_Dealer(Byte[] bufIN, int cbRead)
        {
            //添加新数据到list
            for (int i = 0; i < cbRead; i++)
            {
                m_sp.buf.Add(bufIN[i]);
            }

            //检查起始地址0xAA
            while (m_sp.buf.Count > 1)
            {
                if (m_sp.buf[0] != 0xAA)
                {
                    m_sp.buf.RemoveAt(0);
                }
                else
                {
                    break;
                }
            }
            if (m_sp.buf.Count < 6) return;//收到6个字节就可以确定数据包的大小,

            int cbTotal = m_sp.buf[4] * 256 + m_sp.buf[5] + 8;//payload 大小在[4][5]字节内

            if (m_sp.buf.Count < cbTotal) return;//数据包还未完整接收

            int nCMDType = m_sp.buf[2] * 256 + m_sp.buf[3];

            //MessageBox.Show(String.Format("CMD:0x{0:X}", nCMDType));

            if (nCMDType == 0x4010) //ReadIntensity();
            {
                m_sp.bufToIntensity();
                //m_sp.cntScans++;

                listCMD.RemoveAt(0);//listCMD.Remove(0x4010);//

                m_iRunning--;
                if (m_iRunning > 0)//&& listCMD.Count == 0)//如果命令列表为NULL，那么添加新命令
                {
                    listCMD.Add(0x4010);//继续ReadIntensity()
                }
                m_sp.buf.RemoveRange(0, cbTotal);
                EXE_listCMD();
                return;
                //MessageBox.Show("ReadIntensity");
            }

            //if (m_sp.buf[1] != 0x66) //ErrorCode: can't match Command Code
            //{
            //    listCMD.RemoveAt(0);
            //    m_sp.buf.RemoveRange(0, cbTotal);
            //    EXE_listCMD();
            //    //MessageBox.Show("ReadIntensity");
            //    return;

            //}


            if (nCMDType == 0x4051) //Enter Trig Mode() + receive trigger data
            {
                if (cbTotal == m_sp.nNumPixels * 2 + 8) //receive trig data
                {
                    m_sp.bufToIntensity();
                    m_sp.cntTrigs++;
                }
                else if (cbTotal == 8) //Enter Trig Mode()
                {
                    m_bTrigMode = true;
                    listCMD.Clear();//在这里可以Clear(),在GUI部分不可Clear();//listCMD.RemoveAt(0);//listCMD.Remove(0x4051);//后面必定有ReadIntensity，必须Clear
                    //MessageBox.Show("Enter Trig Mode");
                }
                m_sp.buf.RemoveRange(0, cbTotal);
                EXE_listCMD();
                return;
            }

            if (nCMDType == 0x4052) //Exit Trig Mode()
            {
                m_bTrigMode = false;
                listCMD.Clear();
                listCMD.Add(0x4010);//switch to normal mode : ReadIntensity
                                    //MessageBox.Show("Exit Trig Mode");
                m_sp.buf.RemoveRange(0, cbTotal);
                EXE_listCMD();
                return;
            }

            if (nCMDType == 0x3010) //GetNumPixels();
            {
                m_sp.nNumPixels = (UInt16)(m_sp.buf[6] * 256 + m_sp.buf[7]);
                m_sp.fWavelength = new float[m_sp.nNumPixels];
                m_sp.fIntensity = new float[m_sp.nNumPixels];
                //MessageBox.Show(String.Format("Pixels:{0}", m_sp.nNumPixels));
                listCMD.RemoveAt(0);//命令已完成，删除
                //MessageBox.Show("GetNumPixels");
                for (int i = 0; i < m_sp.fWavelength.Length; i++)
                {
                    m_sp.fWavelength[i] = i;
                }
                m_sp.buf.RemoveRange(0, cbTotal);
                EXE_listCMD();
                return;
            }

            if (nCMDType == 0x1002) //GetSerianNo();
            {
                m_sp.strSN = System.Text.Encoding.UTF8.GetString(m_sp.buf.ToArray(), 6, 32);
                listCMD.RemoveAt(0);//listCMD.Remove(0x1020);//
                //MessageBox.Show("GetSerianNo");
                m_sp.buf.RemoveRange(0, cbTotal);
                EXE_listCMD();
                return;
            }

            if (nCMDType == 0x3020) //ReadWavelength();
            {
                Byte[] data = new Byte[4];
                for (int i = 0; i < m_sp.nNumPixels; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        data[j] = m_sp.buf[6 + 4 * i + j];
                    }
                    m_sp.fWavelength[i] = System.BitConverter.ToSingle(data, 0);
                }
                listCMD.RemoveAt(0);//listCMD.Remove(0x3020); //
                //MessageBox.Show("ReadWavelength");
                m_sp.buf.RemoveRange(0, cbTotal);
                EXE_listCMD();
                return;
            }


        }

        private void FrmspectrumAcquire_Load(object sender, EventArgs e)
        {
            ConnectSpectrometersVCP();
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            m_iRunning = 1;//0x7FFFFFFF
            //ReadIntensityAsync(m_sp.nIntUS, m_sp.nAverage, m_sp.nBoxcar);
            listCMD.Add(0x4010);
            EXE_listCMD();
        }
    }
}
