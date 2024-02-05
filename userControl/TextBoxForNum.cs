using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PerovskiteTest.userControl
{
    public class TextBoxForNum: TextBox
    {
        private NumTextBoxType inputType = NumTextBoxType.Double;  
        public enum NumTextBoxType
        {
            String,//是这个的时候，什么都不处理，跟正常TextBox一样  
            Numeric,//只要是数字就行  
            Currency,
            Decimal,
            Float,
            Double,
            Short,
            Int,
            Long
        }

        public TextBoxForNum()
        {
            this.ContextMenu = new ContextMenu();
        }

        public TextBoxForNum(NumTextBoxType _InputType)
        {
            this.ContextMenu = new ContextMenu();
            this.inputType = _InputType;
        }

        [
        Category("专用设置"),
        DefaultValue(NumTextBoxType.Double),
        Description("设置允许类型：\nString跟普通TextBox功能一样\nNumeric只要是数字就可以")
        ]
        public NumTextBoxType InputType
        {
            get { return inputType; }
            set { inputType = value; }
        }

        public override string Text
        {
            get { return base.Text; }
            set
            {
                if (IsValid(value, true))
                {
                    base.Text = value;
                }
            }
        }

        private bool IsValid(string val, bool use)
        {
            bool ret = true;

            if (string.IsNullOrEmpty(val))
            {
                return ret;
            }

            if (use)
            {
                if (val.Equals("-") && inputType != NumTextBoxType.Numeric)
                {
                    return ret;
                }
            }


            try
            {
                switch (inputType)
                {
                    case NumTextBoxType.String:
                        break;
                    case NumTextBoxType.Numeric:
                        if (!Regex.IsMatch(val, @"^(-)?\d*$"))
                        {
                            ret = false;
                        }
                        break;
                    case NumTextBoxType.Currency:
                        decimal dec = decimal.Parse(val);
                        int pos = val.IndexOf(".");
                        if (pos != -1)
                        {
                            ret = val.Substring(pos).Length <= 3;
                        }
                        break;
                    case NumTextBoxType.Float:
                        float flt = float.Parse(val);
                        break;
                    case NumTextBoxType.Double:
                        double dbl = double.Parse(val);
                        break;
                    case NumTextBoxType.Decimal:
                        decimal dec2 = decimal.Parse(val);
                        break;
                    case NumTextBoxType.Short:
                        short s = short.Parse(val);
                        break;
                    case NumTextBoxType.Int:
                        int i = int.Parse(val);
                        break;
                    case NumTextBoxType.Long:
                        long l = long.Parse(val);
                        break;
                    default:
                        throw new ApplicationException();
                }
            }
            catch
            {
                ret = false;
            }
            return ret;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {

            if (keyData == (Keys)Shortcut.CtrlV || keyData == (Keys)Shortcut.ShiftIns)
            {
                IDataObject iData = Clipboard.GetDataObject();


                string newText;
                newText = base.Text.Substring(0, base.SelectionStart)
                    + (string)iData.GetData(DataFormats.Text)
                    + base.Text.Substring(base.SelectionStart + base.SelectionLength);

                if (!IsValid(newText, true))
                {
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnLeave(EventArgs e)
        {
            if (!(inputType == NumTextBoxType.Numeric || inputType == NumTextBoxType.String))
            {
                if (base.Text != "")
                {
                    if (!IsValid(base.Text, false))
                    {
                        base.Text = "";
                    }
                    else if (Double.Parse(base.Text) == 0)
                    {
                        base.Text = "0";
                    }
                }

            }
            base.OnLeave(e);

        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (inputType != NumTextBoxType.String)
            {
                char c = e.KeyChar;
                if (!Char.IsControl(c))
                {
                    if (c.ToString() == " ")
                    {
                        e.Handled = true;
                        return;
                    }

                    string newText = base.Text.Substring(0, base.SelectionStart)
                        + c.ToString() + base.Text.Substring(base.SelectionStart + base.SelectionLength);

                    if (!IsValid(newText, true))
                    {
                        e.Handled = true;
                    }
                }
            }
            base.OnKeyPress(e);
        }
    }
}