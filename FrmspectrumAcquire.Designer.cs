namespace PerovskiteTest
{
    partial class FrmspectrumAcquire
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
            this.components = new System.ComponentModel.Container();
            this.btnConnectVCP = new System.Windows.Forms.Button();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.labelStatusVCP = new System.Windows.Forms.Label();
            this.btnScan = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnConnectVCP
            // 
            this.btnConnectVCP.Location = new System.Drawing.Point(615, 425);
            this.btnConnectVCP.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnConnectVCP.Name = "btnConnectVCP";
            this.btnConnectVCP.Size = new System.Drawing.Size(86, 22);
            this.btnConnectVCP.TabIndex = 7;
            this.btnConnectVCP.Text = "连接通信";
            this.btnConnectVCP.UseVisualStyleBackColor = true;
            this.btnConnectVCP.Click += new System.EventHandler(this.btnConnectVCP_Click);
            // 
            // serialPort1
            // 
            this.serialPort1.BaudRate = 115200;
            this.serialPort1.ReadBufferSize = 262144;
            this.serialPort1.WriteBufferSize = 4096;
            // 
            // labelStatusVCP
            // 
            this.labelStatusVCP.AutoSize = true;
            this.labelStatusVCP.ForeColor = System.Drawing.Color.Red;
            this.labelStatusVCP.Location = new System.Drawing.Point(526, 430);
            this.labelStatusVCP.Name = "labelStatusVCP";
            this.labelStatusVCP.Size = new System.Drawing.Size(71, 12);
            this.labelStatusVCP.TabIndex = 28;
            this.labelStatusVCP.Text = "unconnected";
            // 
            // btnScan
            // 
            this.btnScan.Enabled = false;
            this.btnScan.Location = new System.Drawing.Point(615, 493);
            this.btnScan.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(86, 22);
            this.btnScan.TabIndex = 29;
            this.btnScan.Text = "获取数据";
            this.btnScan.UseVisualStyleBackColor = true;
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
            // 
            // FrmspectrumAcquire
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1317, 872);
            this.Controls.Add(this.btnScan);
            this.Controls.Add(this.labelStatusVCP);
            this.Controls.Add(this.btnConnectVCP);
            this.MinimumSize = new System.Drawing.Size(785, 503);
            this.Name = "FrmspectrumAcquire";
            this.Text = "光谱采集";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FrmspectrumAcquire_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnConnectVCP;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Label labelStatusVCP;
        private System.Windows.Forms.Button btnScan;
    }
}