using N2600Demo.SCPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PerovskiteTest
{
    public class N2600Demo
    {
        public static TcpScpiOper comScpiOper;
        public N2600Demo(string ip, int id, int port) 
        {
            comScpiOper = new TcpScpiOper(ip, id, port);  //联机
        }
        public bool OpenConnect()
        {            
            return comScpiOper.Open();  //打开通讯
        }
        public void CloseConnect()
        {
            comScpiOper?.Close();
        }

        //查询错误
        private string QuerryError()
        {
            if (comScpiOper.Query(":syst:err?\n", out string errStr))
            {

                if (!errStr.StartsWith("0"))
                {
                    return errStr.Replace("\r\n", " ") + "," + DateTime.Now + "\n";

                }
                else return "";
            }
            else return "syst:err";
        }

        //设置脉冲触发
        public void SetParamMCCF()
        {
            int testCount = (int)(GlobalField.GTestDis  / GlobalField.GNextDis) - 100;
            comScpiOper.Write($":*RST\n");
            comScpiOper.Write($":SYST:RSEN 0\n");
            comScpiOper.Write($":ROUT:TERM FRONt\n");
            comScpiOper.Write($":SOUR:FUNC CURR\n");
            comScpiOper.Write($":SOUR:CURR:RANG 0.0001\n");
            comScpiOper.Write($":SOUR:CURR 0\n");
            comScpiOper.Write($":SENS:FUNC 'VOLT'\n");
            //comScpiOper.Write($":SENS:VOLT:PROT 10\n");
            //comScpiOper.Write($":SENS:VOLT:RANG 10\n");
            comScpiOper.Write($":SENS:VOLT:PROT 2E-1\n");
            comScpiOper.Write($":SENS:VOLT:RANG 2\n");
            comScpiOper.Write($":FORMAT:ELEM VOLTage,TIME\n");
            comScpiOper.Write($":TRACe:TSTamp:FORMat DELTa\n");
            comScpiOper.Write($":SYSTem:AZERo:STATe 0\n");
            comScpiOper.Write($":SOUR:DELAY 0\n");
            comScpiOper.Write($":SENS:VOLT:NPLC 1\n");
            comScpiOper.Write($":ARM:SOUR BUS\n");
            comScpiOper.Write($":ARM:DIR ACC\n");
            comScpiOper.Write($":ARM:COUN 1\n");
            comScpiOper.Write($":TRIG:SOUR TLINK\n");
            comScpiOper.Write($":TRIG:COUN {testCount}\n");
            comScpiOper.Write($":TRIG:ILIN 1\n");
            comScpiOper.Write($":TRIG:OLIN 2\n");
            comScpiOper.Write($":TRIG:INPUT SOUR\n");
            comScpiOper.Write($":TRIG:OUTP DEL\n");
            comScpiOper.Write($":TRIG:DIR SOUR\n");
            comScpiOper.Write($":TRIG:DELAY 0\n");
            comScpiOper.Write($":TRACE:CLEAR\n");
            comScpiOper.Write($":TRACE:POINTS {testCount}\n");

            comScpiOper.Write($":TRACE:FEED SENS\n");
        }

        //开始测量
        public void StartTest()
        {
            comScpiOper.Write($":TRACE:FEED:CONTROL NEXT\n");
            if(comScpiOper.Write($":OUTP ON\n"))
            {
                //if (comScpiOper.Query(":syst:err?\n", out string errStr2))
                //{
                //    QuerryError();

                //}
            }
          
            if (comScpiOper.Write($":INIT\n"))
            {
                comScpiOper.Write($"*TRG\n");
                comScpiOper.Write($"*WAI\n");
                //if (comScpiOper.Query(":syst:err?\n", out string errStr))
                //{
                //    QuerryError();
                //}
            }
        }

        //读取缓存数据
        public string ReadCacheData()
        {

            if (comScpiOper.Query(":TRACe:DATA?\n", out string buffStr))
            {
                return buffStr;
            }
            return "";
        }

        //清除缓存数据
        public void btnClear_Click()
        {
            if (comScpiOper.Write("TRACe:CLEar\n"))
            {
                //QuerryError();
            }
        }

        //关闭
        public void btnOff_Click()
        {
            if (comScpiOper.Write("OUTP OFF\n"))
            {
                //QuerryError();
            }
        }

        #region 吉时利电表

        //开始测试
        public void btnSetParamJSL_Click()
        {
            //comScpiOper.Write($":TRACe:CLEar \"defbuffer1\"\n");
            //comScpiOper.Write($":INIT\n"); 
            comScpiOper.Write($"trigger.model.abort()\n");
            comScpiOper.Write($"defbuffer1.clear()\n");
            Thread.Sleep(100);
            comScpiOper.Write($"trigger.model.initiate()\n");

        }

        //读取数据
        public string btnReadDataJSL_Click()
        {
            //if (comScpiOper.Query($":TRAC:DATA? 1, 999, \"defbuffer1\", READ\n", out string buffStr))
            //{
            //    return buffStr;
            //}
            if (comScpiOper.Query($"printbuffer(1, {(int)(GlobalField.GTestDis / GlobalField.GNextDis) -1 }, defbuffer1.readings)\n", out string buffStr))
            {
                return buffStr;
            }
            
            return "";

        }
        public void LoadSetupUnit1()
        {
            comScpiOper.Write($"SetupUnit1()\n");
        }
    #endregion

}
}
