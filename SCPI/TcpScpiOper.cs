using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace N2600Demo.SCPI
{
    public class TcpScpiOper: ScpiOper
    {
        public byte EndByte { get; set; } = 0x0A;

        private TcpClient _client = null;

        private NetworkStream _stream = null;

        public string Ip { get; private set; }

        public int Id { get; private set; }

        public int Port { get; private set; } = 0;

        public int OutTime { get; private set; } = 0;

        volatile private bool isReading = false;

        public bool IsReading
        {
            get
            {
                return isReading;
            }
            private set
            {
                isReading = value;
            }
        }

        public TcpScpiOper(string ip, int id, int port, int outTime = 5000)
        {
            Ip = ip;
            Id = id;
            Port = port;
            OutTime = outTime;
        }

        public void Close()
        {
            _stream?.Close();
            _client?.Close();
        }

        public bool Open()
        {
            try
            {
                _client = new TcpClient();
                //_client.NoDelay = true;
                //_client.Client.SendTimeout;
                //_client.Client.ReceiveBufferSize = 1024; 
                IPEndPoint point = new IPEndPoint(IPAddress.Parse(Ip), Port);
                _client.Client.ReceiveTimeout = OutTime;
                _client.Client.SendTimeout = OutTime;
                _client.NoDelay = true;
                //_client.Client.Blocking = false;

                var tc = _client.ConnectAsync(point.Address, point.Port);
                tc.Wait(2000);
                _stream = _client.GetStream();
                return true;
            }
            catch (Exception)
            {
                
            }
            return false;
        }

        public bool Query(string inStr, out string outStr)
        {
            outStr = "";
            if (Write(inStr))
            {
                return Read(out outStr);
            }
            return false;
        }

        public bool Read(out string outStr)
        {
            outStr = "";
            byte[] buff = ReadStream(OutTime);
            if (buff != null)
            {
                outStr = Encoding.ASCII.GetString(buff);
                return true;
            }
            return false;
        }

        public bool TryClearBuff()
        {
            try
            {
                if (_client.Available > 0 && _stream.CanRead)
                {
                    byte[] buff = new byte[_client.Available];
                    _stream.Read(buff, 0, buff.Length);
                    return true;
                }
            }
            catch
            {

            }
            return false;
        }

        //从内存流中按字节读，直到出现EndChar为止
        public byte[] ReadStream(int outTime)
        {
            byte[] buff = null;
            List<byte> listTmp = new List<byte>();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {

                while (true)
                {
                    isReading = true;

                    if (sw.ElapsedMilliseconds > outTime)
                    {
                        if (_client.Available <= 0)
                        {
                            return buff;
                        }
                    }
                    while (_client.Available > 0)
                    {
                        int d = _stream.ReadByte();

                        if (d == -1)
                        {
                            break;
                        }
                        else
                        {
                            byte b = (byte)d;
                            listTmp.Add(b);
                            if (b == EndByte)
                            {
                                buff = listTmp.ToArray();
                                return buff;
                            }
                        }
                    }

                }
            }
            catch
            {
                buff = null;
            }
            isReading = false;
            return buff;
        }

        public bool Write(string inStr)
        {
            try
            {
                if (_stream.CanWrite)
                {
                    byte[] bytes = Encoding.Default.GetBytes(inStr);
                    _stream.Write(bytes, 0, bytes.Length);
                }
                return true;
            }
            catch
            {

            }
            return false;
        }

        public void ResetTransBuffer()
        {
            _stream?.Flush();
        }
    }
}
