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
    public class NGI
    {
        public byte[] Result;
        public byte[] InterResult=new  byte[13];
        private int TimeOut = 1000;
        private readonly SerialPort serialPort;
        public bool READED;
        public NGI()
        {
            this.serialPort =new SerialPort();
            this.serialPort.PortName = "COM8";
            this.serialPort.BaudRate = 115200;
            this.serialPort.DataBits = 8;
            this.serialPort.StopBits = StopBits.One;
            this.serialPort.DataReceived += new SerialDataReceivedEventHandler(Receive);
            this.serialPort.ReceivedBytesThreshold = 8;
            this.serialPort.Open();  
        }

        public void SendVoltmand()
        {
            try
            {
                this.serialPort.Write("*RST\n");
                this.serialPort.Write("SOUR:FUNC VOLT\n");
                this.serialPort.Write("SOUR:VOLT:MODE FIXED\n");
                this.serialPort.Write("SENS:FUNC “VOLT”\n");
                this.serialPort.Write("SOUR:VOLT:RANG 2\n");
                this.serialPort.Write("SOUR:VOLT 1.001\n");
                this.serialPort.Write("SENSe:VOLTage:PROT 50\n");
                this.serialPort.Write("SENSe:VOLTage:RANG 200\n");
                this.serialPort.Write("SENS:VOLT:NPLC 1\n");
                this.serialPort.Write("SOUR:DEL 0\n");
                this.serialPort.Write("OUTPut:STATe ON\n");
                this.serialPort.Write("READ?\n");
                Thread.Sleep(130);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            
        }

        //测量前发送
        public void SendMeasModeMand()
        {
            //this.serialPort.Write("OUTPut:STATe OFF\n");
            this.serialPort.Write("*RST\n");
            this.serialPort.Write("SOUR:FUNC CURR\n");
            this.serialPort.Write("SOUR:CURR:MODE FIXED\n");
            this.serialPort.Write("SENS:FUNC VOLT\n");
            this.serialPort.Write("SOUR:CURR:RANG 10E-3\n");
            this.serialPort.Write("SOUR:CURR 0\n");
            this.serialPort.Write("SENSe:VOLTage:PROT 10\n");
            this.serialPort.Write("SENSe:VOLTage:RANG 20\n");
            this.serialPort.Write("SENS:VOLT:NPLC 0.01\n");
            this.serialPort.Write("SOUR:DEL 0\n");
            this.serialPort.Write("OUTPut:STATe ON\n");
            //this.serialPort.Write("READ?\n");        

        }
        private void Receive(object sender, SerialDataReceivedEventArgs e)
        {
            int Data = this.serialPort.BytesToRead;
            if (Data > 71)
            {
                byte[] buuf = new byte[Data];
                this.serialPort.Read(buuf, 0, Data);
                this.Result = buuf; 
            }

        }

        public bool StartOutVolt()
        {
            SendVoltmand();
            this.serialPort.Write("READ?\n");
            Thread.Sleep(130);
            if (this.IsVoltIn(this.TimeOut))
            {
                return true;
            }
            else
            {
                SendVoltmand();
                Thread.Sleep(130);
                if (this.IsVoltIn(this.TimeOut))
                {
                    return true;
                }
                return false;
            }
        }

        public void SendReadMad()
        {
            this.serialPort.Write("READ?\n");
        }
        public string StartMeasVolt()
        {
            this.Result = null;
            this.serialPort.Write("READ?\n");
            do
            {
                Thread.Sleep(2);
            } while (this.Result==null);
            return Encoding.ASCII.GetString(Result).Substring(0, 13);
            //return double.Parse(Encoding.ASCII.GetString(Result).Substring(0, 13)).ToString();
        }

        private double FTestVolt(int timeOut)
        {
            try
            {
                DateTime startTime = DateTime.Now;
                if (DateTime.Now.Subtract(startTime).TotalMilliseconds > timeOut)
                {
                    return 0;
                }
                if (Result != null)
                {
                    return double.Parse(Encoding.ASCII.GetString(Result).Substring(0, 13));
                }
                return 0;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return 0;
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
                    if (this.Result[1] == 49 && this.Result[3] == 50 && this.Result[12] == 49)
                    {
                            return true;                        
                    }
                    else { return false; }  
                }
                return false;   
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message,"2");
                return false;
            }
        }

        public bool StopOut()
        {
            try
            {
                this.serialPort.Write("OUTPut:STATe OFF\n");
                Thread.Sleep(100);
                this.serialPort.Write("TRACe: CLEar\n");
            
            }
            catch (Exception)
            {
                return false;
            }
            return true;    
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
    }
}
