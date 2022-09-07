namespace Save_Window_Position_and_Size
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.Save = new System.Windows.Forms.Button();
            this.lbl_Window_Title = new System.Windows.Forms.Label();
            this.Restore = new System.Windows.Forms.Button();
            this.LogOutput = new System.Windows.Forms.TextBox();
            this.AppsSaved = new System.Windows.Forms.ListBox();
            this.WindowPosX = new System.Windows.Forms.TextBox();
            this.lblWindowPosX = new System.Windows.Forms.Label();
            this.lblWindowPosY = new System.Windows.Forms.Label();
            this.WindowPosY = new System.Windows.Forms.TextBox();
            this.WindowWidth = new System.Windows.Forms.TextBox();
            this.lblWindowWidth = new System.Windows.Forms.Label();
            this.lblWindowHeight = new System.Windows.Forms.Label();
            this.WindowHeight = new System.Windows.Forms.TextBox();
            this.AllRunningApps = new System.Windows.Forms.ListBox();
            this.UpdateTimerInterval = new System.Windows.Forms.TextBox();
            this.lblUpdateTimerInterval = new System.Windows.Forms.Label();
            this.lblTime = new System.Windows.Forms.Label();
            this.Time = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.RefreshAllRunningApps = new System.Windows.Forms.Button();
            this.KeepWindowOnTop = new System.Windows.Forms.CheckBox();
            this.WindowTitle = new System.Windows.Forms.Label();
            this.lblAllRunningApps = new System.Windows.Forms.Label();
            this.lblSavedApps = new System.Windows.Forms.Label();
            this.AutoPosition = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(404, 129);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(75, 23);
            this.Save.TabIndex = 0;
            this.Save.Text = "Save";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // lbl_Window_Title
            // 
            this.lbl_Window_Title.AutoSize = true;
            this.lbl_Window_Title.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lbl_Window_Title.Location = new System.Drawing.Point(376, 16);
            this.lbl_Window_Title.Name = "lbl_Window_Title";
            this.lbl_Window_Title.Size = new System.Drawing.Size(185, 21);
            this.lbl_Window_Title.TabIndex = 2;
            this.lbl_Window_Title.Text = "Selected Window Title:";
            // 
            // Restore
            // 
            this.Restore.Location = new System.Drawing.Point(515, 129);
            this.Restore.Name = "Restore";
            this.Restore.Size = new System.Drawing.Size(75, 23);
            this.Restore.TabIndex = 3;
            this.Restore.Text = "Restore";
            this.Restore.UseVisualStyleBackColor = true;
            this.Restore.Click += new System.EventHandler(this.Restore_Click);
            // 
            // LogOutput
            // 
            this.LogOutput.Location = new System.Drawing.Point(0, 328);
            this.LogOutput.Multiline = true;
            this.LogOutput.Name = "LogOutput";
            this.LogOutput.ReadOnly = true;
            this.LogOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LogOutput.Size = new System.Drawing.Size(515, 67);
            this.LogOutput.TabIndex = 4;
            this.LogOutput.Text = "Output Log:";
            // 
            // AppsSaved
            // 
            this.AppsSaved.FormattingEnabled = true;
            this.AppsSaved.ItemHeight = 15;
            this.AppsSaved.Location = new System.Drawing.Point(713, 54);
            this.AppsSaved.Name = "AppsSaved";
            this.AppsSaved.ScrollAlwaysVisible = true;
            this.AppsSaved.Size = new System.Drawing.Size(233, 259);
            this.AppsSaved.TabIndex = 5;
            this.AppsSaved.SelectedIndexChanged += new System.EventHandler(this.AppsSaved_SelectedIndexChanged);
            this.AppsSaved.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AppsSaved_KeyDown);
            // 
            // WindowPosX
            // 
            this.WindowPosX.Location = new System.Drawing.Point(376, 177);
            this.WindowPosX.Name = "WindowPosX";
            this.WindowPosX.Size = new System.Drawing.Size(100, 23);
            this.WindowPosX.TabIndex = 6;
            // 
            // lblWindowPosX
            // 
            this.lblWindowPosX.AutoSize = true;
            this.lblWindowPosX.Location = new System.Drawing.Point(288, 181);
            this.lblWindowPosX.Name = "lblWindowPosX";
            this.lblWindowPosX.Size = new System.Drawing.Size(86, 15);
            this.lblWindowPosX.TabIndex = 7;
            this.lblWindowPosX.Text = "Window Pos X:";
            // 
            // lblWindowPosY
            // 
            this.lblWindowPosY.AutoSize = true;
            this.lblWindowPosY.Location = new System.Drawing.Point(288, 223);
            this.lblWindowPosY.Name = "lblWindowPosY";
            this.lblWindowPosY.Size = new System.Drawing.Size(86, 15);
            this.lblWindowPosY.TabIndex = 8;
            this.lblWindowPosY.Text = "Window Pos Y:";
            // 
            // WindowPosY
            // 
            this.WindowPosY.Location = new System.Drawing.Point(377, 219);
            this.WindowPosY.Name = "WindowPosY";
            this.WindowPosY.Size = new System.Drawing.Size(100, 23);
            this.WindowPosY.TabIndex = 9;
            // 
            // WindowWidth
            // 
            this.WindowWidth.Location = new System.Drawing.Point(607, 177);
            this.WindowWidth.Name = "WindowWidth";
            this.WindowWidth.Size = new System.Drawing.Size(100, 23);
            this.WindowWidth.TabIndex = 10;
            // 
            // lblWindowWidth
            // 
            this.lblWindowWidth.AutoSize = true;
            this.lblWindowWidth.Location = new System.Drawing.Point(515, 181);
            this.lblWindowWidth.Name = "lblWindowWidth";
            this.lblWindowWidth.Size = new System.Drawing.Size(89, 15);
            this.lblWindowWidth.TabIndex = 11;
            this.lblWindowWidth.Text = "Window Width:";
            // 
            // lblWindowHeight
            // 
            this.lblWindowHeight.AutoSize = true;
            this.lblWindowHeight.Location = new System.Drawing.Point(515, 222);
            this.lblWindowHeight.Name = "lblWindowHeight";
            this.lblWindowHeight.Size = new System.Drawing.Size(93, 15);
            this.lblWindowHeight.TabIndex = 12;
            this.lblWindowHeight.Text = "Window Height:";
            // 
            // WindowHeight
            // 
            this.WindowHeight.Location = new System.Drawing.Point(607, 218);
            this.WindowHeight.Name = "WindowHeight";
            this.WindowHeight.Size = new System.Drawing.Size(100, 23);
            this.WindowHeight.TabIndex = 13;
            // 
            // AllRunningApps
            // 
            this.AllRunningApps.FormattingEnabled = true;
            this.AllRunningApps.ItemHeight = 15;
            this.AllRunningApps.Location = new System.Drawing.Point(12, 54);
            this.AllRunningApps.Name = "AllRunningApps";
            this.AllRunningApps.ScrollAlwaysVisible = true;
            this.AllRunningApps.Size = new System.Drawing.Size(255, 259);
            this.AllRunningApps.TabIndex = 14;
            this.AllRunningApps.SelectedIndexChanged += new System.EventHandler(this.AllRunningApps_SelectedIndexChanged);
            // 
            // UpdateTimerInterval
            // 
            this.UpdateTimerInterval.Location = new System.Drawing.Point(775, 367);
            this.UpdateTimerInterval.Name = "UpdateTimerInterval";
            this.UpdateTimerInterval.Size = new System.Drawing.Size(25, 23);
            this.UpdateTimerInterval.TabIndex = 15;
            this.UpdateTimerInterval.Text = "1";
            this.UpdateTimerInterval.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UpdateTimerInterval_KeyDown);
            // 
            // lblUpdateTimerInterval
            // 
            this.lblUpdateTimerInterval.AutoSize = true;
            this.lblUpdateTimerInterval.Location = new System.Drawing.Point(524, 370);
            this.lblUpdateTimerInterval.Name = "lblUpdateTimerInterval";
            this.lblUpdateTimerInterval.Size = new System.Drawing.Size(251, 15);
            this.lblUpdateTimerInterval.TabIndex = 16;
            this.lblUpdateTimerInterval.Text = "Minutes between checking window positions: ";
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point(851, 370);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(65, 15);
            this.lblTime.TabIndex = 17;
            this.lblTime.Text = "Refresh in: ";
            // 
            // Time
            // 
            this.Time.AutoSize = true;
            this.Time.Location = new System.Drawing.Point(912, 370);
            this.Time.Name = "Time";
            this.Time.Size = new System.Drawing.Size(34, 15);
            this.Time.TabIndex = 18;
            this.Time.Text = "         ";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.RefreshAllRunningApps);
            this.panel1.Controls.Add(this.KeepWindowOnTop);
            this.panel1.Controls.Add(this.WindowTitle);
            this.panel1.Controls.Add(this.lblAllRunningApps);
            this.panel1.Controls.Add(this.lblSavedApps);
            this.panel1.Controls.Add(this.AllRunningApps);
            this.panel1.Controls.Add(this.Save);
            this.panel1.Controls.Add(this.lbl_Window_Title);
            this.panel1.Controls.Add(this.Restore);
            this.panel1.Controls.Add(this.AppsSaved);
            this.panel1.Controls.Add(this.WindowHeight);
            this.panel1.Controls.Add(this.WindowPosX);
            this.panel1.Controls.Add(this.lblWindowHeight);
            this.panel1.Controls.Add(this.lblWindowPosX);
            this.panel1.Controls.Add(this.lblWindowWidth);
            this.panel1.Controls.Add(this.lblWindowPosY);
            this.panel1.Controls.Add(this.WindowWidth);
            this.panel1.Controls.Add(this.WindowPosY);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(958, 328);
            this.panel1.TabIndex = 19;
            // 
            // RefreshAllRunningApps
            // 
            this.RefreshAllRunningApps.Location = new System.Drawing.Point(12, 17);
            this.RefreshAllRunningApps.Name = "RefreshAllRunningApps";
            this.RefreshAllRunningApps.Size = new System.Drawing.Size(75, 23);
            this.RefreshAllRunningApps.TabIndex = 25;
            this.RefreshAllRunningApps.Text = "Refresh";
            this.RefreshAllRunningApps.UseVisualStyleBackColor = true;
            this.RefreshAllRunningApps.Click += new System.EventHandler(this.RefreshAllRunningApps_Click);
            // 
            // KeepWindowOnTop
            // 
            this.KeepWindowOnTop.AutoSize = true;
            this.KeepWindowOnTop.Location = new System.Drawing.Point(421, 283);
            this.KeepWindowOnTop.Name = "KeepWindowOnTop";
            this.KeepWindowOnTop.Size = new System.Drawing.Size(143, 19);
            this.KeepWindowOnTop.TabIndex = 24;
            this.KeepWindowOnTop.Text = "Keep Window on Top?";
            this.KeepWindowOnTop.UseVisualStyleBackColor = true;
            this.KeepWindowOnTop.CheckedChanged += new System.EventHandler(this.KeepWindowOnTop_CheckedChanged);
            // 
            // WindowTitle
            // 
            this.WindowTitle.Location = new System.Drawing.Point(273, 54);
            this.WindowTitle.Name = "WindowTitle";
            this.WindowTitle.Size = new System.Drawing.Size(434, 72);
            this.WindowTitle.TabIndex = 23;
            // 
            // lblAllRunningApps
            // 
            this.lblAllRunningApps.AutoSize = true;
            this.lblAllRunningApps.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblAllRunningApps.Location = new System.Drawing.Point(109, 16);
            this.lblAllRunningApps.Name = "lblAllRunningApps";
            this.lblAllRunningApps.Size = new System.Drawing.Size(142, 21);
            this.lblAllRunningApps.TabIndex = 22;
            this.lblAllRunningApps.Text = "All Running Apps";
            // 
            // lblSavedApps
            // 
            this.lblSavedApps.AutoSize = true;
            this.lblSavedApps.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblSavedApps.Location = new System.Drawing.Point(775, 16);
            this.lblSavedApps.Name = "lblSavedApps";
            this.lblSavedApps.Size = new System.Drawing.Size(98, 21);
            this.lblSavedApps.TabIndex = 21;
            this.lblSavedApps.Text = "Saved Apps";
            // 
            // AutoPosition
            // 
            this.AutoPosition.AutoSize = true;
            this.AutoPosition.Location = new System.Drawing.Point(528, 341);
            this.AutoPosition.Name = "AutoPosition";
            this.AutoPosition.Size = new System.Drawing.Size(98, 19);
            this.AutoPosition.TabIndex = 20;
            this.AutoPosition.Text = "Auto Position";
            this.AutoPosition.UseVisualStyleBackColor = true;
            this.AutoPosition.CheckedChanged += new System.EventHandler(this.AutoPosition_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(958, 395);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.LogOutput);
            this.Controls.Add(this.AutoPosition);
            this.Controls.Add(this.UpdateTimerInterval);
            this.Controls.Add(this.lblUpdateTimerInterval);
            this.Controls.Add(this.Time);
            this.Controls.Add(this.lblTime);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Save Window Size and Position";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button Save;
        private Label lbl_Window_Title;
        private Button Restore;
        private TextBox LogOutput;
        private ListBox AppsSaved;
        private TextBox WindowPosX;
        private Label lblWindowPosX;
        private Label lblWindowPosY;
        private TextBox WindowPosY;
        private TextBox WindowWidth;
        private Label lblWindowWidth;
        private Label lblWindowHeight;
        private TextBox WindowHeight;
        private ListBox AllRunningApps;
        private TextBox UpdateTimerInterval;
        private Label lblUpdateTimerInterval;
        private Label lblTime;
        private Label Time;
        private Panel panel1;
        private CheckBox AutoPosition;
        private Label lblAllRunningApps;
        private Label lblSavedApps;
        private Label WindowTitle;
        private CheckBox KeepWindowOnTop;
        private Button RefreshAllRunningApps;
    }
}