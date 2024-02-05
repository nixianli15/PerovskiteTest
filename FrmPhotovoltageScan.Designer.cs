namespace PerovskiteTest
{
    partial class FrmPhotovoltageScan
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
            this.PB_ImageResult = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cancelScanPhotovoltage = new System.Windows.Forms.Button();
            this.suspendScanPhotovoltage = new System.Windows.Forms.Button();
            this.StartScanPhotovoltage = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbNextDis = new System.Windows.Forms.TextBox();
            this.tbTestDis = new System.Windows.Forms.TextBox();
            this.tbYStartPos = new System.Windows.Forms.TextBox();
            this.tbXStartPos = new System.Windows.Forms.TextBox();
            this.tbTestCount = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PB_ImageResult)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // PB_ImageResult
            // 
            this.PB_ImageResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PB_ImageResult.Location = new System.Drawing.Point(3, 3);
            this.PB_ImageResult.Name = "PB_ImageResult";
            this.PB_ImageResult.Size = new System.Drawing.Size(636, 562);
            this.PB_ImageResult.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PB_ImageResult.TabIndex = 10;
            this.PB_ImageResult.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cancelScanPhotovoltage);
            this.panel1.Controls.Add(this.suspendScanPhotovoltage);
            this.panel1.Controls.Add(this.StartScanPhotovoltage);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.progressBar1);
            this.panel1.Location = new System.Drawing.Point(6, 584);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(648, 144);
            this.panel1.TabIndex = 11;
            // 
            // cancelScanPhotovoltage
            // 
            this.cancelScanPhotovoltage.Location = new System.Drawing.Point(459, 26);
            this.cancelScanPhotovoltage.Name = "cancelScanPhotovoltage";
            this.cancelScanPhotovoltage.Size = new System.Drawing.Size(75, 23);
            this.cancelScanPhotovoltage.TabIndex = 59;
            this.cancelScanPhotovoltage.Text = "重置";
            this.cancelScanPhotovoltage.UseVisualStyleBackColor = true;
            this.cancelScanPhotovoltage.Click += new System.EventHandler(this.cancelScanPhotovoltage_Click);
            // 
            // suspendScanPhotovoltage
            // 
            this.suspendScanPhotovoltage.Location = new System.Drawing.Point(372, 27);
            this.suspendScanPhotovoltage.Name = "suspendScanPhotovoltage";
            this.suspendScanPhotovoltage.Size = new System.Drawing.Size(75, 23);
            this.suspendScanPhotovoltage.TabIndex = 58;
            this.suspendScanPhotovoltage.Text = "暂停";
            this.suspendScanPhotovoltage.UseVisualStyleBackColor = true;
            this.suspendScanPhotovoltage.Click += new System.EventHandler(this.suspendScanPhotovoltage_Click);
            // 
            // StartScanPhotovoltage
            // 
            this.StartScanPhotovoltage.Location = new System.Drawing.Point(285, 27);
            this.StartScanPhotovoltage.Name = "StartScanPhotovoltage";
            this.StartScanPhotovoltage.Size = new System.Drawing.Size(75, 23);
            this.StartScanPhotovoltage.TabIndex = 57;
            this.StartScanPhotovoltage.Text = "开始";
            this.StartScanPhotovoltage.UseVisualStyleBackColor = true;
            this.StartScanPhotovoltage.Click += new System.EventHandler(this.StartScanPhotovoltage_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(478, 107);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 22);
            this.label4.TabIndex = 56;
            this.label4.Text = "进度条";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(13, 106);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(432, 23);
            this.progressBar1.TabIndex = 12;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tbTestCount);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.tbNextDis);
            this.panel2.Controls.Add(this.tbTestDis);
            this.panel2.Controls.Add(this.tbYStartPos);
            this.panel2.Controls.Add(this.tbXStartPos);
            this.panel2.Location = new System.Drawing.Point(659, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(254, 222);
            this.panel2.TabIndex = 40;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(42, 144);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 47;
            this.label6.Text = "换行距离：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(42, 107);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 46;
            this.label5.Text = "测试距离：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 45;
            this.label1.Text = "y起始位置：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(42, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 12);
            this.label3.TabIndex = 44;
            this.label3.Text = "x起始位置：";
            // 
            // tbNextDis
            // 
            this.tbNextDis.Location = new System.Drawing.Point(111, 136);
            this.tbNextDis.Name = "tbNextDis";
            this.tbNextDis.Size = new System.Drawing.Size(100, 21);
            this.tbNextDis.TabIndex = 43;
            // 
            // tbTestDis
            // 
            this.tbTestDis.Location = new System.Drawing.Point(111, 100);
            this.tbTestDis.Name = "tbTestDis";
            this.tbTestDis.Size = new System.Drawing.Size(100, 21);
            this.tbTestDis.TabIndex = 42;
            // 
            // tbYStartPos
            // 
            this.tbYStartPos.Location = new System.Drawing.Point(111, 64);
            this.tbYStartPos.Name = "tbYStartPos";
            this.tbYStartPos.Size = new System.Drawing.Size(100, 21);
            this.tbYStartPos.TabIndex = 41;
            // 
            // tbXStartPos
            // 
            this.tbXStartPos.Location = new System.Drawing.Point(111, 28);
            this.tbXStartPos.Name = "tbXStartPos";
            this.tbXStartPos.Size = new System.Drawing.Size(100, 21);
            this.tbXStartPos.TabIndex = 40;
            // 
            // tbTestCount
            // 
            this.tbTestCount.Location = new System.Drawing.Point(113, 172);
            this.tbTestCount.Name = "tbTestCount";
            this.tbTestCount.Size = new System.Drawing.Size(100, 21);
            this.tbTestCount.TabIndex = 49;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(42, 181);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 48;
            this.label7.Text = "测试次数：";
            // 
            // FrmPhotovoltageScan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1314, 740);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.PB_ImageResult);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(785, 503);
            this.Name = "FrmPhotovoltageScan";
            this.Text = "光电压扫描";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FrmPhotovoltageScan_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PB_ImageResult)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox PB_ImageResult;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button cancelScanPhotovoltage;
        private System.Windows.Forms.Button suspendScanPhotovoltage;
        private System.Windows.Forms.Button StartScanPhotovoltage;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbNextDis;
        private System.Windows.Forms.TextBox tbTestDis;
        private System.Windows.Forms.TextBox tbYStartPos;
        private System.Windows.Forms.TextBox tbXStartPos;
        private System.Windows.Forms.TextBox tbTestCount;
        private System.Windows.Forms.Label label7;
    }
}