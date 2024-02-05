using PerovskiteTest.userControl;
using System.Drawing;

namespace PerovskiteTest
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.btInit = new System.Windows.Forms.Button();
            this.btnELPic = new System.Windows.Forms.Button();
            this.btSetting = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panelEx1 = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.label15 = new System.Windows.Forms.Label();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.label16 = new System.Windows.Forms.Label();
            this.pictureBox8 = new System.Windows.Forms.PictureBox();
            this.label17 = new System.Windows.Forms.Label();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.label14 = new System.Windows.Forms.Label();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.label11 = new System.Windows.Forms.Label();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtShowLog = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.labManufacturer = new System.Windows.Forms.Label();
            this.labProSize = new System.Windows.Forms.Label();
            this.labRemark = new System.Windows.Forms.Label();
            this.labProID = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labOperator = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button4 = new System.Windows.Forms.Button();
            this.spectrumAcquire = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panelEx1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(273, 12);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(64, 26);
            this.button1.TabIndex = 3;
            this.button1.Text = "明场拍摄";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button2.Location = new System.Drawing.Point(353, 12);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(64, 26);
            this.button2.TabIndex = 4;
            this.button2.Text = "荧光拍摄";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button3.Location = new System.Drawing.Point(513, 12);
            this.button3.Margin = new System.Windows.Forms.Padding(2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(76, 26);
            this.button3.TabIndex = 5;
            this.button3.Text = "光电压扫描";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.AllowDrop = true;
            this.tabControl1.Location = new System.Drawing.Point(342, 49);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(10);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(602, 398);
            this.tabControl1.TabIndex = 6;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            this.tabControl1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.tabControl1_MouseDoubleClick);
            // 
            // btInit
            // 
            this.btInit.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btInit.Location = new System.Drawing.Point(120, 12);
            this.btInit.Margin = new System.Windows.Forms.Padding(2);
            this.btInit.Name = "btInit";
            this.btInit.Size = new System.Drawing.Size(64, 26);
            this.btInit.TabIndex = 32;
            this.btInit.Text = "初始化";
            this.btInit.UseVisualStyleBackColor = true;
            this.btInit.Click += new System.EventHandler(this.btInit_Click);
            // 
            // btnELPic
            // 
            this.btnELPic.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnELPic.Location = new System.Drawing.Point(433, 12);
            this.btnELPic.Name = "btnELPic";
            this.btnELPic.Size = new System.Drawing.Size(64, 26);
            this.btnELPic.TabIndex = 40;
            this.btnELPic.Text = "EL拍摄";
            this.btnELPic.UseVisualStyleBackColor = true;
            this.btnELPic.Click += new System.EventHandler(this.btnELPic_Click);
            // 
            // btSetting
            // 
            this.btSetting.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btSetting.Location = new System.Drawing.Point(196, 12);
            this.btSetting.Margin = new System.Windows.Forms.Padding(2);
            this.btSetting.Name = "btSetting";
            this.btSetting.Size = new System.Drawing.Size(64, 26);
            this.btSetting.TabIndex = 41;
            this.btSetting.Text = "配置";
            this.btSetting.UseVisualStyleBackColor = true;
            this.btSetting.Click += new System.EventHandler(this.btSetting_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.panelEx1);
            this.panel1.Controls.Add(this.txtShowLog);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Location = new System.Drawing.Point(6, 58);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(325, 654);
            this.panel1.TabIndex = 42;
            // 
            // panelEx1
            // 
            this.panelEx1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelEx1.Controls.Add(this.label9);
            this.panelEx1.Controls.Add(this.pictureBox6);
            this.panelEx1.Controls.Add(this.label15);
            this.panelEx1.Controls.Add(this.pictureBox7);
            this.panelEx1.Controls.Add(this.label16);
            this.panelEx1.Controls.Add(this.pictureBox8);
            this.panelEx1.Controls.Add(this.label17);
            this.panelEx1.Controls.Add(this.pictureBox5);
            this.panelEx1.Controls.Add(this.label14);
            this.panelEx1.Controls.Add(this.pictureBox4);
            this.panelEx1.Controls.Add(this.label11);
            this.panelEx1.Controls.Add(this.pictureBox3);
            this.panelEx1.Controls.Add(this.label10);
            this.panelEx1.Location = new System.Drawing.Point(2, 380);
            this.panelEx1.Margin = new System.Windows.Forms.Padding(2);
            this.panelEx1.Name = "panelEx1";
            this.panelEx1.Size = new System.Drawing.Size(313, 234);
            this.panelEx1.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(4, 16);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(104, 34);
            this.label9.TabIndex = 152;
            this.label9.Text = "通讯状态";
            // 
            // pictureBox6
            // 
            this.pictureBox6.Location = new System.Drawing.Point(160, 174);
            this.pictureBox6.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(28, 24);
            this.pictureBox6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox6.TabIndex = 150;
            this.pictureBox6.TabStop = false;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(196, 179);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(41, 12);
            this.label15.TabIndex = 151;
            this.label15.Text = "大光源";
            // 
            // pictureBox7
            // 
            this.pictureBox7.Location = new System.Drawing.Point(160, 118);
            this.pictureBox7.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(28, 24);
            this.pictureBox7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox7.TabIndex = 148;
            this.pictureBox7.TabStop = false;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label16.Location = new System.Drawing.Point(196, 122);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(44, 17);
            this.label16.TabIndex = 149;
            this.label16.Text = "光谱仪";
            // 
            // pictureBox8
            // 
            this.pictureBox8.Location = new System.Drawing.Point(160, 63);
            this.pictureBox8.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox8.Name = "pictureBox8";
            this.pictureBox8.Size = new System.Drawing.Size(28, 24);
            this.pictureBox8.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox8.TabIndex = 146;
            this.pictureBox8.TabStop = false;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label17.Location = new System.Drawing.Point(196, 68);
            this.label17.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(34, 17);
            this.label17.TabIndex = 147;
            this.label17.Text = "NI表";
            // 
            // pictureBox5
            // 
            this.pictureBox5.Location = new System.Drawing.Point(28, 174);
            this.pictureBox5.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(28, 24);
            this.pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox5.TabIndex = 144;
            this.pictureBox5.TabStop = false;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(65, 179);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(65, 12);
            this.label14.TabIndex = 145;
            this.label14.Text = "相机位移台";
            // 
            // pictureBox4
            // 
            this.pictureBox4.Location = new System.Drawing.Point(28, 118);
            this.pictureBox4.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(28, 24);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox4.TabIndex = 142;
            this.pictureBox4.TabStop = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.Location = new System.Drawing.Point(65, 122);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(68, 17);
            this.label11.TabIndex = 143;
            this.label11.Text = "样品位移台";
            // 
            // pictureBox3
            // 
            this.pictureBox3.Location = new System.Drawing.Point(28, 63);
            this.pictureBox3.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(28, 24);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 140;
            this.pictureBox3.TabStop = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(65, 68);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(68, 17);
            this.label10.TabIndex = 141;
            this.label10.Text = "光源位移台";
            // 
            // txtShowLog
            // 
            this.txtShowLog.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtShowLog.Location = new System.Drawing.Point(6, 246);
            this.txtShowLog.Margin = new System.Windows.Forms.Padding(2);
            this.txtShowLog.Multiline = true;
            this.txtShowLog.Name = "txtShowLog";
            this.txtShowLog.Size = new System.Drawing.Size(266, 133);
            this.txtShowLog.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.labManufacturer);
            this.panel2.Controls.Add(this.labProSize);
            this.panel2.Controls.Add(this.labRemark);
            this.panel2.Controls.Add(this.labProID);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.labOperator);
            this.panel2.Location = new System.Drawing.Point(2, 2);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(320, 242);
            this.panel2.TabIndex = 0;
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(4, 10);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(104, 34);
            this.label8.TabIndex = 149;
            this.label8.Text = "产品信息";
            // 
            // labManufacturer
            // 
            this.labManufacturer.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labManufacturer.Location = new System.Drawing.Point(83, 174);
            this.labManufacturer.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labManufacturer.Name = "labManufacturer";
            this.labManufacturer.Size = new System.Drawing.Size(82, 20);
            this.labManufacturer.TabIndex = 148;
            this.labManufacturer.Text = "1111111111";
            // 
            // labProSize
            // 
            this.labProSize.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labProSize.Location = new System.Drawing.Point(83, 136);
            this.labProSize.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labProSize.Name = "labProSize";
            this.labProSize.Size = new System.Drawing.Size(82, 20);
            this.labProSize.TabIndex = 147;
            this.labProSize.Text = "1111111111";
            // 
            // labRemark
            // 
            this.labRemark.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labRemark.Location = new System.Drawing.Point(83, 96);
            this.labRemark.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labRemark.Name = "labRemark";
            this.labRemark.Size = new System.Drawing.Size(82, 20);
            this.labRemark.TabIndex = 146;
            this.labRemark.Text = "1111111111";
            // 
            // labProID
            // 
            this.labProID.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labProID.Location = new System.Drawing.Point(83, 58);
            this.labProID.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labProID.Name = "labProID";
            this.labProID.Size = new System.Drawing.Size(82, 20);
            this.labProID.TabIndex = 145;
            this.labProID.Text = "1111111111";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(15, 174);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 20);
            this.label3.TabIndex = 144;
            this.label3.Text = "生产商：";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(15, 136);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 20);
            this.label2.TabIndex = 143;
            this.label2.Text = "片源大小：";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(15, 96);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 20);
            this.label1.TabIndex = 142;
            this.label1.Text = "备注：";
            // 
            // labOperator
            // 
            this.labOperator.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labOperator.Location = new System.Drawing.Point(15, 58);
            this.labOperator.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labOperator.Name = "labOperator";
            this.labOperator.Size = new System.Drawing.Size(82, 20);
            this.labOperator.TabIndex = 141;
            this.labOperator.Text = "ID号：";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(969, 49);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 43;
            this.button4.Text = "调试东西";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // spectrumAcquire
            // 
            this.spectrumAcquire.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.spectrumAcquire.Location = new System.Drawing.Point(607, 12);
            this.spectrumAcquire.Margin = new System.Windows.Forms.Padding(2);
            this.spectrumAcquire.Name = "spectrumAcquire";
            this.spectrumAcquire.Size = new System.Drawing.Size(76, 26);
            this.spectrumAcquire.TabIndex = 45;
            this.spectrumAcquire.Text = "光谱采集";
            this.spectrumAcquire.UseVisualStyleBackColor = true;
            this.spectrumAcquire.Click += new System.EventHandler(this.spectrumAcquire_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1236, 681);
            this.Controls.Add(this.spectrumAcquire);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btSetting);
            this.Controls.Add(this.btnELPic);
            this.Controls.Add(this.btInit);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panelEx1.ResumeLayout(false);
            this.panelEx1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Button btInit;
        private System.Windows.Forms.Button btnELPic;
        private System.Windows.Forms.Button btSetting;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox txtShowLog;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panelEx1;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.PictureBox pictureBox7;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.PictureBox pictureBox8;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label labManufacturer;
        private System.Windows.Forms.Label labProSize;
        private System.Windows.Forms.Label labRemark;
        private System.Windows.Forms.Label labProID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labOperator;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button spectrumAcquire;
    }
}

