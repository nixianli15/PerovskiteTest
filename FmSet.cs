using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;

namespace PerovskiteTest
{
    public partial class FmSet : Form
    {
        public FmSet()
        {
            InitializeComponent();
        }

        /*
         * 
         */
        private void btSaveSet_Click(object sender, EventArgs e)
        {
            Hashtable ht = new Hashtable();
            ushort usTemp = 0;
            uint uiTemp = 0;
            float flTemp = 0;

            //相机参数
            uint.TryParse(BLExpoTime.Text, out uiTemp);
            GlobalField.BLExpoTime = uiTemp;
            ht.Add("BLExpoTime", uiTemp);
            ushort.TryParse(BLExpoGain.Text, out usTemp);
            GlobalField.BLExpoGain = usTemp;
            ht.Add("BLExpoGain", usTemp);

            uint.TryParse(PLExpoTime.Text, out uiTemp);
            GlobalField.PLExpoTime = uiTemp;
            ht.Add("PLExpoTime", uiTemp);
            ushort.TryParse(PLExpoGain.Text, out usTemp);
            GlobalField.PLExpoGain = usTemp;
            ht.Add("PLExpoGain", usTemp);

            uint.TryParse(ELExpoTime.Text, out uiTemp);
            GlobalField.ELExpoTime = uiTemp;
            ht.Add("ELExpoTime", uiTemp);
            ushort.TryParse(ELExpoGain.Text, out usTemp);
            GlobalField.ELExpoGain = usTemp;
            ht.Add("ELExpoGain", usTemp);

            ushort.TryParse(focusInitialVal.Text, out usTemp);
            GlobalField.focusInitialVal = usTemp;
            ht.Add("focusInitialVal", usTemp);

            ushort.TryParse(photographDelay.Text, out usTemp);
            GlobalField.photographDelay = usTemp;
            ht.Add("photographDelay", usTemp);

            //位移台参数
            float.TryParse(txttz1.Text, out flTemp);
            GlobalField.SampleMotionX = flTemp;
            ht.Add("SampleMotionX", flTemp);
            float.TryParse(txttz2.Text, out flTemp);
            GlobalField.SampleMotionY = flTemp;
            ht.Add("SampleMotionY", flTemp);
            GlobalField.Light1Pos = 18;
            ht.Add("Light1Pos", 18);
            GlobalField.laserPos = 170;
            ht.Add("laserPos", 170);
            GlobalField.Z0Num = 0;
            ht.Add("Z0Num", 0);
            GlobalField.Z1Num = -150;
            ht.Add("Z1Num", -150);

            //移动视野参数
            float.TryParse(txtfieldMoveX.Text, out flTemp);
            GlobalField.fieldMoveX = flTemp;
            ht.Add("fieldMoveX", flTemp);
            float.TryParse(txtfieldMoveY.Text, out flTemp);
            GlobalField.fieldMoveY = flTemp;
            ht.Add("fieldMoveY", flTemp);
            ushort.TryParse(txtfieldX.Text, out usTemp);
            GlobalField.fieldX = usTemp;
            ht.Add("fieldX", usTemp);
            ushort.TryParse(txtfieldY.Text, out usTemp);
            GlobalField.fieldY = usTemp;
            ht.Add("fieldY", usTemp);

            //其他参数
            uint.TryParse(eTOMMENSOutputVolt.Text,out uiTemp);
            GlobalField.eTOMMENSOutputVolt = uiTemp;
            ht.Add("eTOMMENSOutputVolt", uiTemp);
            uint.TryParse(eTOMMENSOutputCurrent.Text, out uiTemp);
            GlobalField.eTOMMENSOutputCurrent = uiTemp;
            ht.Add("eTOMMENSOutputCurrent", uiTemp);
            uint.TryParse(lightIntensity.Text, out uiTemp);
            GlobalField.lightIntensity = uiTemp;
            ht.Add("lightIntensity", uiTemp);

            string strJson = Newtonsoft.Json.JsonConvert.SerializeObject(ht);
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "setting.config", strJson);
            MessageBox.Show("参数设置保存成功！");
            this.Close();
        }

        private void FmSet_Load(object sender, EventArgs e)
        {
            BLExpoTime.Text = GlobalField.BLExpoTime + "";
            BLExpoGain.Text = GlobalField.BLExpoGain + "";
            PLExpoTime.Text = GlobalField.PLExpoTime + "";
            PLExpoGain.Text = GlobalField.PLExpoGain + "";
            ELExpoTime.Text = GlobalField.ELExpoTime + "";
            ELExpoGain.Text = GlobalField.ELExpoGain + "";
            focusInitialVal.Text= GlobalField.focusInitialVal + "";
            photographDelay.Text = GlobalField.photographDelay + "";


            txtfieldMoveX.Text = GlobalField.fieldMoveX + "";
            txtfieldMoveY.Text = GlobalField.fieldMoveY + "";
            txtfieldX.Text = GlobalField.fieldX + "";
            txtfieldY.Text = GlobalField.fieldY + "";
            txttz1.Text = GlobalField.SampleMotionX + "";
            txttz2.Text = GlobalField.SampleMotionY + "";

            eTOMMENSOutputVolt.Text = GlobalField.eTOMMENSOutputVolt+"";
            eTOMMENSOutputCurrent.Text = GlobalField.eTOMMENSOutputCurrent + "";
            lightIntensity.Text = GlobalField.lightIntensity + "";
        }

        private void textBoxForNum2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
