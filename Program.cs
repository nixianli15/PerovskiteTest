using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PerovskiteTest
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new testAI());
            //Application.Run(new FrmScanVolt());
            //Application.Run(new HeatMap());
            //Application.Run(new InputID());
            //Application.Run(new Camera(0));
            //Application.Run(new testForm1());

        }
    }
}
