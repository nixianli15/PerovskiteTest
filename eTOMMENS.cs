using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.ConstrainedExecution;

namespace PerovskiteTest
{
    public class eTOMMENS
    {
        public byte[] Result;
        public byte[] InterResult=new  byte[13];
        private int TimeOut = 2000;
        private readonly SerialPort serialPort;
        public bool READED;
        public eTOMMENS()
        {
            this.serialPort =new SerialPort();
            this.serialPort.PortName = "COM3";
            this.serialPort.BaudRate = 9600;
            this.serialPort.DataBits = 8;
            this.serialPort.StopBits = StopBits.One;
            this.serialPort.Parity = Parity.None;   
            this.serialPort.DataReceived += new SerialDataReceivedEventHandler(Receive);
            this.serialPort.ReceivedBytesThreshold = 8;
            this.serialPort.Open();  
        }

        public void SetVoltCurrentOn()
        {
            try
            {           

                string data = string.Format("01060030{0}", (GlobalField.eTOMMENSOutputVolt * 100).ToString("X4"));
                byte[] sendData = HexStringToBytes(data);
                sendData = CRC16(sendData, Convert.ToByte("A0", 16), Convert.ToByte("01", 16));
                this.serialPort.Write(sendData, 0, sendData.Length);

                //byte[] sendData = HexStringToBytes("01  06  00  30  04  B0  8A B1");
                //this.serialPort.Write(sendData,0,sendData.Length);

                //sendData = HexStringToBytes("01  06  00  31  00  C8  D9 93");
                //this.serialPort.Write(sendData, 0, sendData.Length);
                data = string.Format("01060031{0}", (GlobalField.eTOMMENSOutputCurrent * 100).ToString("X4"));
                sendData = HexStringToBytes(data);
                sendData = CRC16(sendData, Convert.ToByte("A0", 16), Convert.ToByte("01", 16));
                this.serialPort.Write(sendData, 0, sendData.Length);

                Thread.Sleep(1000);
                sendData = HexStringToBytes("01060001000119CA");
                this.serialPort.Write(sendData, 0, sendData.Length);
            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message);
            }
        }


        private void Receive(object sender, SerialDataReceivedEventArgs e)
        {
            int Data = this.serialPort.BytesToRead;
            if (Data >= 8)
            {
                byte[] buuf = new byte[Data];
                this.serialPort.Read(buuf, 0, Data);
                this.Result = buuf; 
            }

        }


        public bool StartOutVolt()
        {
            SetVoltCurrentOn();
            Thread.Sleep(200);
            if (this.IsVoltIn(this.TimeOut))
            {
                return true;
            }
            else
            {
                SetVoltCurrentOn();
                Thread.Sleep(200);
                if (this.IsVoltIn(this.TimeOut))
                {
                    return true;
                }
                return false;
            }
        }



  
        private bool IsVoltIn(int timeOut)
        {
            try
            {
                DateTime startTime = DateTime.Now;
                if (DateTime.Now.Subtract(startTime).TotalMilliseconds > timeOut)
                {
                    return false;
                }
                if (Result != null)
                {
                    if (this.Result[3] == 48 )//&& this.Result[3] == 50 && this.Result[12] == 49)
                    {
                            return true;                        
                    }
                    else { return false; }  
                }
                return false;   
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }

  
        public string Close()
        {
            try
            {

                this.serialPort.Close();
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return null;
        }

        public void CloseOut()
        {
            byte[] sendData = HexStringToBytes("01 06 00 01 00 00 D8 0A");
            this.serialPort.Write(sendData, 0, sendData.Length);
        }

        public void OpenOut()
        {
            byte[] sendData = HexStringToBytes("01060001000119CA");
            this.serialPort.Write(sendData, 0, sendData.Length);
        }

        private byte[] HexStringToBytes(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException("DataError");
            }

            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return returnBytes;
        }

        private byte[] CRC16(byte[] value, byte CH, byte CL)
        {
            byte[] array = new byte[value.Length + 2];
            value.CopyTo(array, 0);
            byte b = 255;
            byte b2 = 255;
            for (int i = 0; i < value.Length; i++)
            {
                b ^= value[i];
                for (int j = 0; j <= 7; j++)
                {
                    byte b3 = b2;
                    byte b4 = b;
                    b2 = (byte)(b2 >> 1);
                    b = (byte)(b >> 1);
                    bool flag = (b3 & 1) == 1;
                    if (flag)
                    {
                        b |= 128;
                    }
                    bool flag2 = (b4 & 1) == 1;
                    if (flag2)
                    {
                        b2 ^= CH;
                        b ^= CL;
                    }
                }
            }
            array[array.Length - 2] = b;
            array[array.Length - 1] = b2;
            return array;
        }
    }
}
