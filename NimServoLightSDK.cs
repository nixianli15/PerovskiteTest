using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerovskiteTest
{
    /// <summary>
    /// 万名光源位移台
    /// </summary>
    public class NimServoLightSDK
    {
        SerialPort serialPort1;
        public NimServoLightSDK(string SerialPortName) 
        {
            serialPort1 = new SerialPort();
            //serialPort1.PortName = "COM5";
            serialPort1.PortName = SerialPortName;
            serialPort1.BaudRate = 115200;
            serialPort1.Parity = System.IO.Ports.Parity.None;
            serialPort1.DataBits = 8;
            serialPort1.StopBits = System.IO.Ports.StopBits.One;
            serialPort1.Open();
        }

        //public void 

        private string Read()
        {
            int len = serialPort1.BytesToRead;//获取可以读取的字节数
            byte[] buff = new byte[len];//创建缓存数据数组
            serialPort1.Read(buff, 0, len);//把数据读取到buff数组
            string strRet = ToHexStrFromByte(buff);
            return strRet;
        }


        private void Write(string strContent)
        {
            try
            {
                if (strContent.Length > 0)
                {
                    byte[] buff = ConvertHexStringToBytes(strContent.Replace(" ", ""));
                    serialPort1.Write(buff, 0, buff.Count());//串口发送数据
                    Read();
                }
            }
            catch (Exception )
            {
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
