namespace MEMZEffectTest
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
            this.effectListBox = new System.Windows.Forms.ListBox();
            this.startButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.speedTrackBar = new System.Windows.Forms.TrackBar();
            this.speedLabel = new System.Windows.Forms.Label();
            this.randomEffectCheckBox = new System.Windows.Forms.CheckBox();
            this.effectTimer = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.speedTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // effectListBox
            // 
            this.effectListBox.FormattingEnabled = true;
            this.effectListBox.ItemHeight = 12;
            this.effectListBox.Location = new System.Drawing.Point(12, 34);
            this.effectListBox.Name = "effectListBox";
            this.effectListBox.Size = new System.Drawing.Size(257, 280);
            this.effectListBox.TabIndex = 0;
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(275, 34);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 1;
            this.startButton.Text = "开始";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(275, 63);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 2;
            this.stopButton.Text = "停止";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // speedTrackBar
            // 
            this.speedTrackBar.Location = new System.Drawing.Point(275, 119);
            this.speedTrackBar.Maximum = 5000;
            this.speedTrackBar.Minimum = 100;
            this.speedTrackBar.Name = "speedTrackBar";
            this.speedTrackBar.Size = new System.Drawing.Size(104, 45);
            this.speedTrackBar.TabIndex = 3;
            this.speedTrackBar.Value = 1000;
            this.speedTrackBar.Scroll += new System.EventHandler(this.speedTrackBar_Scroll);
            // 
            // speedLabel
            // 
            this.speedLabel.AutoSize = true;
            this.speedLabel.Location = new System.Drawing.Point(272, 103);
            this.speedLabel.Name = "speedLabel";
            this.speedLabel.Size = new System.Drawing.Size(83, 12);
            this.speedLabel.TabIndex = 4;
            this.speedLabel.Text = "速度 (1000ms)";
            // 
            // randomEffectCheckBox
            // 
            this.randomEffectCheckBox.AutoSize = true;
            this.randomEffectCheckBox.Location = new System.Drawing.Point(275, 170);
            this.randomEffectCheckBox.Name = "randomEffectCheckBox";
            this.randomEffectCheckBox.Size = new System.Drawing.Size(96, 16);
            this.randomEffectCheckBox.TabIndex = 5;
            this.randomEffectCheckBox.Text = "随机效果序列";
            this.randomEffectCheckBox.UseVisualStyleBackColor = true;
            // 
            // effectTimer
            // 
            this.effectTimer.Interval = 1000;
            this.effectTimer.Tick += new System.EventHandler(this.effectTimer_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "选择效果:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 335);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.randomEffectCheckBox);
            this.Controls.Add(this.speedLabel);
            this.Controls.Add(this.speedTrackBar);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.effectListBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "MEMZ效果测试工具";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.speedTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox effectListBox;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.TrackBar speedTrackBar;
        private System.Windows.Forms.Label speedLabel;
        private System.Windows.Forms.CheckBox randomEffectCheckBox;
        private System.Windows.Forms.Timer effectTimer;
        private System.Windows.Forms.Label label1;
    }
}