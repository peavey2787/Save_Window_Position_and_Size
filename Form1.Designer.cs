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
            Save = new Button();
            lbl_Window_Title = new Label();
            Restore = new Button();
            LogOutput = new TextBox();
            AppsSaved = new ListBox();
            WindowPosX = new TextBox();
            lblWindowPosX = new Label();
            lblWindowPosY = new Label();
            WindowPosY = new TextBox();
            WindowWidth = new TextBox();
            lblWindowWidth = new Label();
            lblWindowHeight = new Label();
            WindowHeight = new TextBox();
            AllRunningApps = new ListBox();
            UpdateTimerInterval = new TextBox();
            lblUpdateTimerInterval = new Label();
            lblTime = new Label();
            Time = new Label();
            panel1 = new Panel();
            hWnd = new Label();
            RefreshWindowButton = new Button();
            WindowId = new Label();
            WindowDisplayName = new TextBox();
            label1 = new Label();
            AutoPosition = new CheckBox();
            WindowClass = new Label();
            label2 = new Label();
            IgnoreButton = new Button();
            RestoreAll = new Button();
            RefreshAllRunningApps = new Button();
            KeepWindowOnTop = new CheckBox();
            WindowTitle = new Label();
            lblAllRunningApps = new Label();
            lblSavedApps = new Label();
            label3 = new Label();
            ProcessName = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // Save
            // 
            Save.Location = new Point(302, 217);
            Save.Name = "Save";
            Save.Size = new Size(75, 23);
            Save.TabIndex = 0;
            Save.Text = "Save";
            Save.UseVisualStyleBackColor = true;
            Save.Click += Save_Click;
            // 
            // lbl_Window_Title
            // 
            lbl_Window_Title.AutoSize = true;
            lbl_Window_Title.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lbl_Window_Title.Location = new Point(403, 103);
            lbl_Window_Title.Name = "lbl_Window_Title";
            lbl_Window_Title.Size = new Size(112, 21);
            lbl_Window_Title.TabIndex = 2;
            lbl_Window_Title.Text = "Window Title";
            // 
            // Restore
            // 
            Restore.Location = new Point(606, 217);
            Restore.Name = "Restore";
            Restore.Size = new Size(75, 23);
            Restore.TabIndex = 3;
            Restore.Text = "Restore";
            Restore.UseVisualStyleBackColor = true;
            Restore.Click += Restore_Click;
            // 
            // LogOutput
            // 
            LogOutput.Location = new Point(0, 412);
            LogOutput.Multiline = true;
            LogOutput.Name = "LogOutput";
            LogOutput.ReadOnly = true;
            LogOutput.ScrollBars = ScrollBars.Vertical;
            LogOutput.Size = new Size(515, 67);
            LogOutput.TabIndex = 4;
            LogOutput.Text = "Output Log:";
            // 
            // AppsSaved
            // 
            AppsSaved.FormattingEnabled = true;
            AppsSaved.ItemHeight = 15;
            AppsSaved.Location = new Point(713, 54);
            AppsSaved.Name = "AppsSaved";
            AppsSaved.ScrollAlwaysVisible = true;
            AppsSaved.Size = new Size(233, 304);
            AppsSaved.TabIndex = 5;
            AppsSaved.SelectedIndexChanged += AppsSaved_SelectedIndexChanged;
            AppsSaved.KeyDown += AppsSaved_KeyDown;
            // 
            // WindowPosX
            // 
            WindowPosX.Location = new Point(367, 257);
            WindowPosX.Name = "WindowPosX";
            WindowPosX.Size = new Size(100, 23);
            WindowPosX.TabIndex = 6;
            // 
            // lblWindowPosX
            // 
            lblWindowPosX.AutoSize = true;
            lblWindowPosX.Location = new Point(279, 261);
            lblWindowPosX.Name = "lblWindowPosX";
            lblWindowPosX.Size = new Size(86, 15);
            lblWindowPosX.TabIndex = 7;
            lblWindowPosX.Text = "Window Pos X:";
            // 
            // lblWindowPosY
            // 
            lblWindowPosY.AutoSize = true;
            lblWindowPosY.Location = new Point(279, 295);
            lblWindowPosY.Name = "lblWindowPosY";
            lblWindowPosY.Size = new Size(86, 15);
            lblWindowPosY.TabIndex = 8;
            lblWindowPosY.Text = "Window Pos Y:";
            // 
            // WindowPosY
            // 
            WindowPosY.Location = new Point(368, 291);
            WindowPosY.Name = "WindowPosY";
            WindowPosY.Size = new Size(100, 23);
            WindowPosY.TabIndex = 9;
            // 
            // WindowWidth
            // 
            WindowWidth.Location = new Point(598, 257);
            WindowWidth.Name = "WindowWidth";
            WindowWidth.Size = new Size(100, 23);
            WindowWidth.TabIndex = 10;
            // 
            // lblWindowWidth
            // 
            lblWindowWidth.AutoSize = true;
            lblWindowWidth.Location = new Point(506, 261);
            lblWindowWidth.Name = "lblWindowWidth";
            lblWindowWidth.Size = new Size(89, 15);
            lblWindowWidth.TabIndex = 11;
            lblWindowWidth.Text = "Window Width:";
            // 
            // lblWindowHeight
            // 
            lblWindowHeight.AutoSize = true;
            lblWindowHeight.Location = new Point(506, 294);
            lblWindowHeight.Name = "lblWindowHeight";
            lblWindowHeight.Size = new Size(93, 15);
            lblWindowHeight.TabIndex = 12;
            lblWindowHeight.Text = "Window Height:";
            // 
            // WindowHeight
            // 
            WindowHeight.Location = new Point(598, 290);
            WindowHeight.Name = "WindowHeight";
            WindowHeight.Size = new Size(100, 23);
            WindowHeight.TabIndex = 13;
            // 
            // AllRunningApps
            // 
            AllRunningApps.FormattingEnabled = true;
            AllRunningApps.ItemHeight = 15;
            AllRunningApps.Location = new Point(12, 54);
            AllRunningApps.Name = "AllRunningApps";
            AllRunningApps.ScrollAlwaysVisible = true;
            AllRunningApps.Size = new Size(255, 304);
            AllRunningApps.TabIndex = 14;
            AllRunningApps.SelectedIndexChanged += AllRunningApps_SelectedIndexChanged;
            AllRunningApps.MouseDown += AllRunningApps_MouseDown;
            // 
            // UpdateTimerInterval
            // 
            UpdateTimerInterval.Location = new Point(768, 365);
            UpdateTimerInterval.Name = "UpdateTimerInterval";
            UpdateTimerInterval.Size = new Size(25, 23);
            UpdateTimerInterval.TabIndex = 15;
            UpdateTimerInterval.Text = "1";
            UpdateTimerInterval.KeyDown += UpdateTimerInterval_KeyDown;
            // 
            // lblUpdateTimerInterval
            // 
            lblUpdateTimerInterval.AutoSize = true;
            lblUpdateTimerInterval.Location = new Point(638, 368);
            lblUpdateTimerInterval.Name = "lblUpdateTimerInterval";
            lblUpdateTimerInterval.Size = new Size(124, 15);
            lblUpdateTimerInterval.TabIndex = 16;
            lblUpdateTimerInterval.Text = "Auto Position Interval ";
            // 
            // lblTime
            // 
            lblTime.AutoSize = true;
            lblTime.Location = new Point(844, 367);
            lblTime.Name = "lblTime";
            lblTime.Size = new Size(65, 15);
            lblTime.TabIndex = 17;
            lblTime.Text = "Refresh in: ";
            // 
            // Time
            // 
            Time.AutoSize = true;
            Time.Location = new Point(907, 367);
            Time.Name = "Time";
            Time.Size = new Size(28, 15);
            Time.TabIndex = 18;
            Time.Text = "59   ";
            // 
            // panel1
            // 
            panel1.Controls.Add(ProcessName);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(hWnd);
            panel1.Controls.Add(RefreshWindowButton);
            panel1.Controls.Add(WindowId);
            panel1.Controls.Add(WindowDisplayName);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(AutoPosition);
            panel1.Controls.Add(WindowClass);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(IgnoreButton);
            panel1.Controls.Add(RestoreAll);
            panel1.Controls.Add(RefreshAllRunningApps);
            panel1.Controls.Add(KeepWindowOnTop);
            panel1.Controls.Add(WindowTitle);
            panel1.Controls.Add(lblAllRunningApps);
            panel1.Controls.Add(lblSavedApps);
            panel1.Controls.Add(AllRunningApps);
            panel1.Controls.Add(Save);
            panel1.Controls.Add(lbl_Window_Title);
            panel1.Controls.Add(Restore);
            panel1.Controls.Add(AppsSaved);
            panel1.Controls.Add(WindowHeight);
            panel1.Controls.Add(WindowPosX);
            panel1.Controls.Add(lblWindowHeight);
            panel1.Controls.Add(lblWindowPosX);
            panel1.Controls.Add(lblWindowWidth);
            panel1.Controls.Add(lblWindowPosY);
            panel1.Controls.Add(WindowWidth);
            panel1.Controls.Add(WindowPosY);
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(958, 366);
            panel1.TabIndex = 19;
            // 
            // hWnd
            // 
            hWnd.AutoSize = true;
            hWnd.Location = new Point(299, 29);
            hWnd.Name = "hWnd";
            hWnd.Size = new Size(39, 15);
            hWnd.TabIndex = 34;
            hWnd.Text = "hWnd";
            // 
            // RefreshWindowButton
            // 
            RefreshWindowButton.Location = new Point(459, 217);
            RefreshWindowButton.Name = "RefreshWindowButton";
            RefreshWindowButton.Size = new Size(75, 23);
            RefreshWindowButton.TabIndex = 33;
            RefreshWindowButton.Text = "Refresh";
            RefreshWindowButton.UseVisualStyleBackColor = true;
            RefreshWindowButton.Click += RefreshWindowButton_Click;
            // 
            // WindowId
            // 
            WindowId.AutoSize = true;
            WindowId.Location = new Point(299, 9);
            WindowId.Name = "WindowId";
            WindowId.Size = new Size(18, 15);
            WindowId.TabIndex = 32;
            WindowId.Text = "ID";
            // 
            // WindowDisplayName
            // 
            WindowDisplayName.Location = new Point(459, 8);
            WindowDisplayName.Name = "WindowDisplayName";
            WindowDisplayName.Size = new Size(100, 23);
            WindowDisplayName.TabIndex = 30;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(371, 12);
            label1.Name = "label1";
            label1.Size = new Size(61, 15);
            label1.TabIndex = 31;
            label1.Text = "Nickname";
            // 
            // AutoPosition
            // 
            AutoPosition.AutoSize = true;
            AutoPosition.Location = new Point(279, 332);
            AutoPosition.Name = "AutoPosition";
            AutoPosition.Size = new Size(98, 19);
            AutoPosition.TabIndex = 20;
            AutoPosition.Text = "Auto Position";
            AutoPosition.UseVisualStyleBackColor = true;
            AutoPosition.CheckedChanged += AutoPosition_CheckedChanged;
            // 
            // WindowClass
            // 
            WindowClass.Location = new Point(273, 187);
            WindowClass.Name = "WindowClass";
            WindowClass.Size = new Size(434, 28);
            WindowClass.TabIndex = 29;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label2.Location = new Point(406, 166);
            label2.Name = "label2";
            label2.Size = new Size(120, 21);
            label2.TabIndex = 28;
            label2.Text = "Window Class:";
            // 
            // IgnoreButton
            // 
            IgnoreButton.Location = new Point(173, 29);
            IgnoreButton.Name = "IgnoreButton";
            IgnoreButton.Size = new Size(75, 22);
            IgnoreButton.TabIndex = 27;
            IgnoreButton.Text = "Ignore List";
            IgnoreButton.UseVisualStyleBackColor = true;
            IgnoreButton.Click += IgnoreButton_Click;
            // 
            // RestoreAll
            // 
            RestoreAll.Location = new Point(712, 29);
            RestoreAll.Name = "RestoreAll";
            RestoreAll.Size = new Size(75, 23);
            RestoreAll.TabIndex = 26;
            RestoreAll.Text = "Restore All";
            RestoreAll.UseVisualStyleBackColor = true;
            RestoreAll.Click += RestoreAll_Click;
            // 
            // RefreshAllRunningApps
            // 
            RefreshAllRunningApps.Location = new Point(11, 30);
            RefreshAllRunningApps.Name = "RefreshAllRunningApps";
            RefreshAllRunningApps.Size = new Size(75, 23);
            RefreshAllRunningApps.TabIndex = 25;
            RefreshAllRunningApps.Text = "Refresh";
            RefreshAllRunningApps.UseVisualStyleBackColor = true;
            RefreshAllRunningApps.Click += RefreshAllRunningApps_Click;
            // 
            // KeepWindowOnTop
            // 
            KeepWindowOnTop.AutoSize = true;
            KeepWindowOnTop.Location = new Point(532, 332);
            KeepWindowOnTop.Name = "KeepWindowOnTop";
            KeepWindowOnTop.Size = new Size(143, 19);
            KeepWindowOnTop.TabIndex = 24;
            KeepWindowOnTop.Text = "Keep Window on Top?";
            KeepWindowOnTop.UseVisualStyleBackColor = true;
            KeepWindowOnTop.CheckedChanged += KeepWindowOnTop_CheckedChanged;
            // 
            // WindowTitle
            // 
            WindowTitle.Location = new Point(273, 124);
            WindowTitle.Name = "WindowTitle";
            WindowTitle.Size = new Size(434, 38);
            WindowTitle.TabIndex = 23;
            // 
            // lblAllRunningApps
            // 
            lblAllRunningApps.AutoSize = true;
            lblAllRunningApps.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblAllRunningApps.Location = new Point(67, 6);
            lblAllRunningApps.Name = "lblAllRunningApps";
            lblAllRunningApps.Size = new Size(142, 21);
            lblAllRunningApps.TabIndex = 22;
            lblAllRunningApps.Text = "All Running Apps";
            // 
            // lblSavedApps
            // 
            lblSavedApps.AutoSize = true;
            lblSavedApps.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblSavedApps.Location = new Point(794, 6);
            lblSavedApps.Name = "lblSavedApps";
            lblSavedApps.Size = new Size(98, 21);
            lblSavedApps.TabIndex = 21;
            lblSavedApps.Text = "Saved Apps";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label3.Location = new Point(398, 54);
            label3.Name = "label3";
            label3.Size = new Size(117, 21);
            label3.TabIndex = 35;
            label3.Text = "Process Name";
            // 
            // ProcessName
            // 
            ProcessName.AutoSize = true;
            ProcessName.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            ProcessName.Location = new Point(405, 80);
            ProcessName.Name = "ProcessName";
            ProcessName.Size = new Size(110, 15);
            ProcessName.TabIndex = 36;
            ProcessName.Text = "Process Name Here";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(958, 480);
            Controls.Add(panel1);
            Controls.Add(LogOutput);
            Controls.Add(lblUpdateTimerInterval);
            Controls.Add(UpdateTimerInterval);
            Controls.Add(Time);
            Controls.Add(lblTime);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "Save Window Size and Position";
            Load += Form1_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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
        private Button RestoreAll;
        private Label WindowClass;
        private Label label2;
        private Button IgnoreButton;
        private TextBox WindowDisplayName;
        private Label label1;
        private Label WindowId;
        private Button RefreshWindowButton;
        private Label hWnd;
        private Label ProcessName;
        private Label label3;
    }
}