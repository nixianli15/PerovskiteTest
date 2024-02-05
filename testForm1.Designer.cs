namespace PerovskiteTest
{
    partial class testForm1
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textBoxForNum1 = new PerovskiteTest.userControl.TextBoxForNum();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(208, 42);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(165, 66);
            this.button1.TabIndex = 0;
            this.button1.Text = "连接";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(295, 425);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(165, 66);
            this.button2.TabIndex = 1;
            this.button2.Text = "发送";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBoxForNum1
            // 
            this.textBoxForNum1.Location = new System.Drawing.Point(138, 198);
            this.textBoxForNum1.Name = "textBoxForNum1";
            this.textBoxForNum1.Size = new System.Drawing.Size(100, 35);
            this.textBoxForNum1.TabIndex = 2;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(581, 198);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(603, 399);
            this.textBox1.TabIndex = 3;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(62, 367);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(398, 35);
            this.textBox2.TabIndex = 4;
            // 
            // testForm1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 810);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.textBoxForNum1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "testForm1";
            this.Text = "testForm1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private userControl.TextBoxForNum textBoxForNum1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
    }
}