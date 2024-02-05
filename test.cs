using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerovskiteTest
{
    public class HeatPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public byte Intensity { get; set; }

        public HeatPoint(int iX, int iY, byte bIntensity)
        {
            X = iX;
            Y = iY;
            Intensity = bIntensity;
        }
    }
    public class test
    {
        List<HeatPoint> aHeatPoints = new List<HeatPoint>();
        public test(List<HeatPoint> points) {
            aHeatPoints = points;
        }   
        public Bitmap CreateBitmap()
        {
            Bitmap Output = new Bitmap(250, 50, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(Output);
            graphics.Clear(System.Drawing.Color.White);
            foreach (HeatPoint point in aHeatPoints)
            {
                //if (point.Intensity * 30 / 180 == 0)
                //    continue;
                //DrawHeatPoint(graphics, point, point.Intensity * 30 / 180);
                DrawHeatPoint(graphics, point);
            }
            return Output;
        }
        public void DrawHeatPoint(Graphics graphics, HeatPoint HeatPoint)
        {
            Rectangle rectangle = new Rectangle(new Point() { X= HeatPoint.X,Y= HeatPoint.Y },new Size() { Width=50,Height=50 });
            GraphicsPath gp = new GraphicsPath();
            gp.AddRectangle(rectangle);
            Brush pgb = new SolidBrush(Color.FromArgb(255, HeatPoint.Intensity, 255, 255));

            //Color[] colors = { Color.FromArgb(0, HeatPoint.Intensity, 255, 255) };
            //pgb.SurroundColors = colors;
            graphics.FillRectangle(pgb, rectangle);
        }
    }
}
