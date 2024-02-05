using System;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PerovskiteTest
{
    public partial class testForm1 : Form
    {
        public testForm1()
        {
            InitializeComponent();
            serialPort1 = new SerialPort();
            serialPort1.PortName = "COM5";
            serialPort1.BaudRate = 115200;
            serialPort1.Parity = System.IO.Ports.Parity.None;
            serialPort1.DataBits = 8;
            serialPort1.StopBits = System.IO.Ports.StopBits.One;
            //serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);
            
        }
        SerialPort serialPort1;
        private void button1_Click(object sender, EventArgs e)
        {
            serialPort1.ReadTimeout = 1000;
            serialPort1.Open();
            Thread readThread = new Thread(Read);
            readThread.Start();
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int len = serialPort1.BytesToRead;//获取可以读取的字节数
            byte[] buff = new byte[len];//创建缓存数据数组
            serialPort1.Read(buff, 0, len);//把数据读取到buff数组
            Invoke((new Action(() =>
            {//C# 3.0以后代替委托的新方法
                textBox1.AppendText(Encoding.Default.GetString(buff));//对话框追加显示数据
            })));
        }

        private void Read()
        {
            int len = serialPort1.BytesToRead;//获取可以读取的字节数
            byte[] buff = new byte[len];//创建缓存数据数组
            serialPort1.Read(buff, 0, len);//把数据读取到buff数组
            Invoke((new Action(() =>
            {//C# 3.0以后代替委托的新方法
                string strRet = ToHexStrFromByte(buff);
                textBox1.AppendText(strRet);//对话框追加显示数据
            })));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            String Str = textBox2.Text.ToString();//获取发送文本框里面的数据
            try
            {
                if (Str.Length > 0)
                {
                    byte[] buff = ConvertHexStringToBytes(Str.Replace(" ", ""));
                    serialPort1.Write(buff,0, buff.Count());//串口发送数据
                    Read();
                }
            }
            catch (Exception err) {
                textBox1.AppendText(err.Message);//对话框追加显示数据
            }
        }

        public byte[] ConvertHexStringToBytes(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException("参数长度不正确,必须是偶数位。");
            }
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return returnBytes;
        }

        public string ToHexStrFromByte(byte[] byteDatas)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < byteDatas.Length; i++)
            {
                builder.Append(string.Format("{0:X2} ", byteDatas[i]));
            }
            return builder.ToString().Trim();
        }
    }
}
