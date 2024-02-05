using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PerovskiteTest.userControl
{
    public class PanelEx : System.Windows.Forms.Panel
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hwnd);
        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);
        private Color _borderColor = Color.Black;
        private int _borderWidth = 0;
        public int _leftWidth = 0, _topWidth = 0, _rightWidth = 0, _bottomWidth = 0;
        //
        // 摘要:
        // 获取或设置控件的边框颜色。
        //
        // 返回结果:
        // 控件的边框颜色 System.Drawing.Color。默认为 System.Drawing.Color.Black
        // 属性的值。
        [Description("组件的边框颜色。"), Category("Appearance")]
        public Color BorderColor
        {
            get
            {
                return _borderColor;
            }
            set
            {
                _borderColor = value;
                this.Invalidate();
            }
        }
        //
        // 摘要:
        // 获取或设置控件的边框宽度。
        //
        // 返回结果:
        // 控件的边框宽度 int。默认为 1
        // 属性的值。
        [Description("组件的边框宽度。"), Category("Appearance")]
        public int BorderWidth
        {
            get
            {
                return _borderWidth;
            }
            set
            {
                _borderWidth = value;
                this.Invalidate();
            }
        }
        public PanelEx()
        {
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, false);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.Paint += new PaintEventHandler(PanelEx_Paint);
        }
        private void PanelEx_Paint(object sender, PaintEventArgs e)
        {
            if (this.BorderStyle == BorderStyle.FixedSingle)
            {
                IntPtr hDC = GetWindowDC(this.Handle);
                Graphics g = Graphics.FromHdc(hDC);
                ControlPaint.DrawBorder(
                g,
                new Rectangle(0, 0, this.Width, this.Height),
                _borderColor,
                _borderWidth > 0 ? _borderWidth : _leftWidth,
                ButtonBorderStyle.Solid,
                _borderColor,
                _borderWidth > 0 ? _borderWidth : _topWidth,
                ButtonBorderStyle.Solid,
                _borderColor,
                _borderWidth > 0 ? _borderWidth : _rightWidth,
                ButtonBorderStyle.Solid,
                _borderColor,
                 _borderWidth > 0 ? _borderWidth : _bottomWidth,
                ButtonBorderStyle.Solid);
                g.Dispose();
                ReleaseDC(Handle, hDC);
            }
        }
    }
}
