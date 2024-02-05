using NationalInstruments.DAQmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerovskiteTest
{
    public struct PeiZhiCanShu
    {
        public string AiChannel;
        public string TriggerChannel;
        public double SampleRate;    //采样率
        public int SamplesPerTrigger;  //每个触发信号测试几次？
        public int MinVoltage;
        public int MaxVoltage;
        public int ExtSignalRate;  //1s触发多少个信号？
        public int BufferSize;
        public int callbackInterval;     
    }
}
