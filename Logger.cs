using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace SMT {
    namespace OEM 
    {

        public class Logger
        {
            private TextBox textBox = null;
            private List<string> bufferedMessages = new List<string>();

            // instantiate a logger with no TextBox (Console only)
            public Logger()
            {
            }

            // instantiate a logger with the TextBox form control to receive updates
            public Logger(TextBox tb)
            {
                textBox = tb;
            }

            // assign or change textbox after construction
            public void setTextBox(TextBox tb)
            {
                textBox = tb;
            }

            // write (not append) log to textfile
            public void save(string pathname)
            {
                if (textBox != null)
                {
                    try
                    {
                        TextWriter tw = new StreamWriter(pathname);
                        tw.WriteLine(textBox.Text);
                        tw.Close();
                    }
                    catch (Exception e)
                    {
                        display("Error saving log to {0}: {1}", pathname, e.Message);
                    }
                }
            }

            // log to textbox and console
            public void display(string fmt, params Object[] obj)
            {
                string msg = DateTime.Now.ToString("<yyyy-MM-dd_HHmmss.fff>  ");
                msg += String.Format(fmt, obj) + Environment.NewLine;

                //Console.Write("DISPLAY: {0}", msg);
                if (textBox != null)
                {
                    StringBuilder sb = new StringBuilder(textBox.Text);
                    sb.Insert(0, msg);
                    //textBox.AppendText(msg);
                    textBox.Text = sb.ToString();
                    
                    // textBox.Parent.Refresh();
                }
            }

            public void push(string fmt, params Object[] obj)
            {
                string msg = String.Format(fmt, obj) + Environment.NewLine;
                bufferedMessages.Add(msg);                
            }

            public void flush()
            {
                foreach (string msg in bufferedMessages)
                {
                    display(msg);
                }
                bufferedMessages.Clear();
            }

            // just log to console
            public void log(string fmt, params Object[] obj)
            {
                string msg = String.Format(fmt, obj) + Environment.NewLine;
                Console.Write("LOG: {0}", msg);
            }
        }


        public class DataProcess
        {
            //Single[] fWavelengths = null;  //波长
            //Dictionary<int, Single[]> fIntensity = null;
            List<Single[]> listIntensity = new List<Single[]>();
            UInt16 m_wAverage = 1;
            UInt16 m_wBoxcar = 0;

            private void RemoveOldData()
            {
                //如果Average 比 List.Count小，那么删除多余数据
                int nCount = listIntensity.Count;
                if (nCount > m_wAverage)
                {
                    listIntensity.RemoveRange(0, nCount - m_wAverage);
                }
            }

            public void SetAverageBoxcar(UInt16 wAverage, UInt16 wBoxcar)
            {
                if (wAverage == 0) return;
                m_wAverage = wAverage;
                m_wBoxcar = wBoxcar;

                RemoveOldData();
            }
            private void NewSpectrumClone(Single[] fRaw)
            {
                Single[] fNew = new Single[fRaw.Length];
                for (int i = 0; i < fRaw.Length; i++)
                {
                    fNew[i] = fRaw[i];
                }
                listIntensity.Add(fNew);

                RemoveOldData();
                //Single a = fIntensity[0][0];
            }

            private Single[] CalcAverageBoxcar(UInt16 wAverage, UInt16 wBoxcar)
            {
                return null;
            }

            public double WavelengthToIntensity(float fWavelength,float[] fX,float[] fY)
            {
                if (fX.Length != fY.Length) return 0;
                if (fX.Length <= 10) return 0;

                int nSize = fX.Length;
	            int nFirst = 0;
	            int nMid;
                int nLast  = nSize - 1;


                if (fWavelength >= fX[nSize - 1])
                {
                    return fY[nSize - 1];
                }

                if (fWavelength <= fX[0])
                {
                    return fY[0];
                }
	
                while (nLast - nFirst > 1 )
                {
                    nMid = (nFirst + nLast) / 2;  // compute mid point.
                    if (fWavelength >= fX[nMid])
                        nFirst = nMid;  // repeat search in top half.
                    else
                        nLast  = nMid; // repeat search in bottom half.
                }
	
	            float fIntensityFirst = (float)fY[nFirst];
	            float fIntensityLast  = (float)fY[nLast];
                //插值法
                float fIntensity = fIntensityFirst + (fIntensityLast - fIntensityFirst) / (fX[nLast] - fX[nFirst]) *(fWavelength - fX[nFirst]);
                return fIntensity;
            }

        }

    }
}
