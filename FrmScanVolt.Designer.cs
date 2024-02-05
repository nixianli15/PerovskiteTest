namespace PerovskiteTest
{
    partial class FrmScanVolt
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                myTask?.Dispose();
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnStop = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.distance = new System.Windows.Forms.TextBox();
            this.btnMove = new System.Windows.Forms.Button();
            this.btnHome = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.tbXStartPos = new System.Windows.Forms.TextBox();
            this.tbYStartPos = new System.Windows.Forms.TextBox();
            this.tbTestDis = new System.Windows.Forms.TextBox();
            this.tbNextDis = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnHeatMap = new System.Windows.Forms.Button();
            this.txtShowLog = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tbTestCount = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(233, 169);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 13;
            this.btnStop.Text = "停止";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "X1方向",
            "Y2方向",
            "光源",
            "相机"});
            this.comboBox1.Location = new System.Drawing.Point(233, 82);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 20);
            this.comboBox1.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(46, 124);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 11;
            this.label1.Text = "距离：";
            // 
            // distance
            // 
            this.distance.Location = new System.Drawing.Point(93, 121);
            this.distance.Name = "distance";
            this.distance.Size = new System.Drawing.Size(100, 21);
            this.distance.TabIndex = 10;
            // 
            // btnMove
            // 
            this.btnMove.Location = new System.Drawing.Point(233, 121);
            this.btnMove.Name = "btnMove";
            this.btnMove.Size = new System.Drawing.Size(75, 23);
            this.btnMove.TabIndex = 9;
            this.btnMove.Text = "移动";
            this.btnMove.UseVisualStyleBackColor = true;
            this.btnMove.Click += new System.EventHandler(this.btnMove_Click);
            // 
            // btnHome
            // 
            this.btnHome.Location = new System.Drawing.Point(378, 33);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(75, 23);
            this.btnHome.TabIndex = 8;
            this.btnHome.Text = "回原点";
            this.btnHome.UseVisualStyleBackColor = true;
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(82, 33);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 7;
            this.btnStart.Text = "开始测试";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click_1);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(142, 227);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 14;
            this.label2.Text = "label2";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(48, 222);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 15;
            this.button2.Text = "显示位置";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(479, 33);
            this.button3.Margin = new System.Windows.Forms.Padding(2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 17;
            this.button3.Text = "X1回原点";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(479, 74);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 18;
            this.button4.Text = "Y2回原点";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // tbXStartPos
            // 
            this.tbXStartPos.Location = new System.Drawing.Point(451, 164);
            this.tbXStartPos.Name = "tbXStartPos";
            this.tbXStartPos.Size = new System.Drawing.Size(100, 21);
            this.tbXStartPos.TabIndex = 19;
            // 
            // tbYStartPos
            // 
            this.tbYStartPos.Location = new System.Drawing.Point(451, 196);
            this.tbYStartPos.Name = "tbYStartPos";
            this.tbYStartPos.Size = new System.Drawing.Size(100, 21);
            this.tbYStartPos.TabIndex = 20;
            // 
            // tbTestDis
            // 
            this.tbTestDis.Location = new System.Drawing.Point(449, 237);
            this.tbTestDis.Name = "tbTestDis";
            this.tbTestDis.Size = new System.Drawing.Size(100, 21);
            this.tbTestDis.TabIndex = 21;
            // 
            // tbNextDis
            // 
            this.tbNextDis.Location = new System.Drawing.Point(449, 274);
            this.tbNextDis.Name = "tbNextDis";
            this.tbNextDis.Size = new System.Drawing.Size(100, 21);
            this.tbNextDis.TabIndex = 22;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(382, 169);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 12);
            this.label3.TabIndex = 23;
            this.label3.Text = "x起始位置：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(382, 199);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 12);
            this.label4.TabIndex = 24;
            this.label4.Text = "y起始位置：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(382, 245);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 25;
            this.label5.Text = "测试距离：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(384, 282);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 26;
            this.label6.Text = "换行距离：";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(464, 374);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 27;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(82, 74);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(75, 23);
            this.btnPause.TabIndex = 30;
            this.btnPause.Text = "暂停或继续";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnHeatMap
            // 
            this.btnHeatMap.Location = new System.Drawing.Point(479, 124);
            this.btnHeatMap.Name = "btnHeatMap";
            this.btnHeatMap.Size = new System.Drawing.Size(75, 23);
            this.btnHeatMap.TabIndex = 31;
            this.btnHeatMap.Text = "打开热力图";
            this.btnHeatMap.UseVisualStyleBackColor = true;
            this.btnHeatMap.Click += new System.EventHandler(this.btnHeatMap_Click);
            // 
            // txtShowLog
            // 
            this.txtShowLog.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtShowLog.Location = new System.Drawing.Point(29, 447);
            this.txtShowLog.Margin = new System.Windows.Forms.Padding(2);
            this.txtShowLog.Multiline = true;
            this.txtShowLog.Name = "txtShowLog";
            this.txtShowLog.Size = new System.Drawing.Size(266, 133);
            this.txtShowLog.TabIndex = 32;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick_1);
            // 
            // tbTestCount
            // 
            this.tbTestCount.Location = new System.Drawing.Point(451, 314);
            this.tbTestCount.Name = "tbTestCount";
            this.tbTestCount.Size = new System.Drawing.Size(100, 21);
            this.tbTestCount.TabIndex = 29;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(386, 324);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 28;
            this.label7.Text = "测试次数";
            // 
            // FrmScanVolt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1020, 772);
            this.Controls.Add(this.txtShowLog);
            this.Controls.Add(this.btnHeatMap);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.tbTestCount);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbNextDis);
            this.Controls.Add(this.tbTestDis);
            this.Controls.Add(this.tbYStartPos);
            this.Controls.Add(this.tbXStartPos);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.distance);
            this.Controls.Add(this.btnMove);
            this.Controls.Add(this.btnHome);
            this.Controls.Add(this.btnStart);
            this.Name = "FrmScanVolt";
            this.Text = "FrmScanVolt";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmScanVolt_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox distance;
        private System.Windows.Forms.Button btnMove;
        private System.Windows.Forms.Button btnHome;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TextBox tbXStartPos;
        private System.Windows.Forms.TextBox tbYStartPos;
        private System.Windows.Forms.TextBox tbTestDis;
        private System.Windows.Forms.TextBox tbNextDis;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnHeatMap;
        private System.Windows.Forms.TextBox txtShowLog;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox tbTestCount;
        private System.Windows.Forms.Label label7;
    }
}