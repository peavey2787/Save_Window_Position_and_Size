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
            components = new System.ComponentModel.Container();
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
            profileComboBox = new ComboBox();
            WinTitleRadioButton = new RadioButton();
            ProcNameRadioButton = new RadioButton();
            label2 = new Label();
            WindowTitle = new Label();
            ProcessName = new Label();
            label3 = new Label();
            hWnd = new Label();
            RefreshWindowButton = new Button();
            WindowId = new Label();
            WindowDisplayName = new TextBox();
            label1 = new Label();
            AutoPosition = new CheckBox();
            IgnoreButton = new Button();
            RestoreAll = new Button();
            RefreshAllRunningApps = new Button();
            KeepWindowOnTop = new CheckBox();
            lblAllRunningApps = new Label();
            lblSavedApps = new Label();
            toolTip1 = new ToolTip(components);
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // Save
            // 
            Save.BackgroundImage = Properties.Resources.floppy_disk;
            Save.BackgroundImageLayout = ImageLayout.Zoom;
            Save.Location = new Point(816, 648);
            Save.Margin = new Padding(5, 6, 5, 6);
            Save.Name = "Save";
            Save.Size = new Size(62, 68);
            Save.TabIndex = 0;
            Save.UseVisualStyleBackColor = true;
            Save.Click += Save_Click;
            // 
            // lbl_Window_Title
            // 
            lbl_Window_Title.AutoSize = true;
            lbl_Window_Title.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lbl_Window_Title.Location = new Point(758, 286);
            lbl_Window_Title.Margin = new Padding(5, 0, 5, 0);
            lbl_Window_Title.Name = "lbl_Window_Title";
            lbl_Window_Title.Size = new Size(192, 38);
            lbl_Window_Title.TabIndex = 2;
            lbl_Window_Title.Text = "Window Title";
            // 
            // Restore
            // 
            Restore.BackgroundImage = Properties.Resources.redo;
            Restore.BackgroundImageLayout = ImageLayout.Zoom;
            Restore.Location = new Point(1061, 648);
            Restore.Margin = new Padding(5, 6, 5, 6);
            Restore.Name = "Restore";
            Restore.Size = new Size(62, 68);
            Restore.TabIndex = 3;
            Restore.UseVisualStyleBackColor = true;
            Restore.Click += Restore_Click;
            // 
            // LogOutput
            // 
            LogOutput.Location = new Point(0, 824);
            LogOutput.Margin = new Padding(5, 6, 5, 6);
            LogOutput.Multiline = true;
            LogOutput.Name = "LogOutput";
            LogOutput.ReadOnly = true;
            LogOutput.ScrollBars = ScrollBars.Vertical;
            LogOutput.Size = new Size(1639, 130);
            LogOutput.TabIndex = 4;
            LogOutput.Text = "Output Log:";
            // 
            // AppsSaved
            // 
            AppsSaved.BackColor = Color.FromArgb(11, 83, 144);
            AppsSaved.ForeColor = SystemColors.Window;
            AppsSaved.FormattingEnabled = true;
            AppsSaved.ItemHeight = 30;
            AppsSaved.Location = new Point(1222, 108);
            AppsSaved.Margin = new Padding(5, 6, 5, 6);
            AppsSaved.Name = "AppsSaved";
            AppsSaved.ScrollAlwaysVisible = true;
            AppsSaved.Size = new Size(397, 604);
            AppsSaved.TabIndex = 5;
            AppsSaved.SelectedIndexChanged += AppsSaved_SelectedIndexChanged;
            AppsSaved.KeyDown += AppsSaved_KeyDown;
            // 
            // WindowPosX
            // 
            WindowPosX.Location = new Point(706, 412);
            WindowPosX.Margin = new Padding(5, 6, 5, 6);
            WindowPosX.Name = "WindowPosX";
            WindowPosX.Size = new Size(83, 35);
            WindowPosX.TabIndex = 6;
            WindowPosX.KeyDown += WindowPosX_KeyDown;
            // 
            // lblWindowPosX
            // 
            lblWindowPosX.AutoSize = true;
            lblWindowPosX.Location = new Point(585, 418);
            lblWindowPosX.Margin = new Padding(5, 0, 5, 0);
            lblWindowPosX.Name = "lblWindowPosX";
            lblWindowPosX.Size = new Size(109, 30);
            lblWindowPosX.TabIndex = 7;
            lblWindowPosX.Text = "Position X:";
            // 
            // lblWindowPosY
            // 
            lblWindowPosY.AutoSize = true;
            lblWindowPosY.Location = new Point(585, 486);
            lblWindowPosY.Margin = new Padding(5, 0, 5, 0);
            lblWindowPosY.Name = "lblWindowPosY";
            lblWindowPosY.Size = new Size(109, 30);
            lblWindowPosY.TabIndex = 8;
            lblWindowPosY.Text = "Position Y:";
            // 
            // WindowPosY
            // 
            WindowPosY.Location = new Point(705, 480);
            WindowPosY.Margin = new Padding(5, 6, 5, 6);
            WindowPosY.Name = "WindowPosY";
            WindowPosY.Size = new Size(81, 35);
            WindowPosY.TabIndex = 9;
            WindowPosY.KeyDown += WindowPosY_KeyDown;
            // 
            // WindowWidth
            // 
            WindowWidth.Location = new Point(974, 412);
            WindowWidth.Margin = new Padding(5, 6, 5, 6);
            WindowWidth.Name = "WindowWidth";
            WindowWidth.Size = new Size(83, 35);
            WindowWidth.TabIndex = 10;
            WindowWidth.KeyDown += WindowWidth_KeyDown;
            // 
            // lblWindowWidth
            // 
            lblWindowWidth.AutoSize = true;
            lblWindowWidth.Location = new Point(893, 418);
            lblWindowWidth.Margin = new Padding(5, 0, 5, 0);
            lblWindowWidth.Name = "lblWindowWidth";
            lblWindowWidth.Size = new Size(74, 30);
            lblWindowWidth.TabIndex = 11;
            lblWindowWidth.Text = "Width:";
            // 
            // lblWindowHeight
            // 
            lblWindowHeight.AutoSize = true;
            lblWindowHeight.Location = new Point(885, 486);
            lblWindowHeight.Margin = new Padding(5, 0, 5, 0);
            lblWindowHeight.Name = "lblWindowHeight";
            lblWindowHeight.Size = new Size(80, 30);
            lblWindowHeight.TabIndex = 12;
            lblWindowHeight.Text = "Height:";
            // 
            // WindowHeight
            // 
            WindowHeight.Location = new Point(974, 480);
            WindowHeight.Margin = new Padding(5, 6, 5, 6);
            WindowHeight.Name = "WindowHeight";
            WindowHeight.Size = new Size(83, 35);
            WindowHeight.TabIndex = 13;
            WindowHeight.KeyDown += WindowHeight_KeyDown;
            // 
            // AllRunningApps
            // 
            AllRunningApps.BackColor = Color.FromArgb(11, 83, 144);
            AllRunningApps.ForeColor = SystemColors.Window;
            AllRunningApps.FormattingEnabled = true;
            AllRunningApps.ItemHeight = 30;
            AllRunningApps.Location = new Point(21, 108);
            AllRunningApps.Margin = new Padding(5, 6, 5, 6);
            AllRunningApps.Name = "AllRunningApps";
            AllRunningApps.ScrollAlwaysVisible = true;
            AllRunningApps.Size = new Size(434, 604);
            AllRunningApps.TabIndex = 14;
            AllRunningApps.SelectedIndexChanged += AllRunningApps_SelectedIndexChanged;
            AllRunningApps.MouseDown += AllRunningApps_MouseDown;
            // 
            // UpdateTimerInterval
            // 
            UpdateTimerInterval.Location = new Point(351, 732);
            UpdateTimerInterval.Margin = new Padding(5, 6, 5, 6);
            UpdateTimerInterval.Name = "UpdateTimerInterval";
            UpdateTimerInterval.Size = new Size(40, 35);
            UpdateTimerInterval.TabIndex = 15;
            UpdateTimerInterval.Text = "1";
            UpdateTimerInterval.KeyDown += UpdateTimerInterval_KeyDown;
            // 
            // lblUpdateTimerInterval
            // 
            lblUpdateTimerInterval.AutoSize = true;
            lblUpdateTimerInterval.BackColor = Color.Transparent;
            lblUpdateTimerInterval.ForeColor = SystemColors.ControlLightLight;
            lblUpdateTimerInterval.Location = new Point(70, 738);
            lblUpdateTimerInterval.Margin = new Padding(5, 0, 5, 0);
            lblUpdateTimerInterval.Name = "lblUpdateTimerInterval";
            lblUpdateTimerInterval.Size = new Size(285, 30);
            lblUpdateTimerInterval.TabIndex = 16;
            lblUpdateTimerInterval.Text = "Auto Position Interval (mins): ";
            // 
            // lblTime
            // 
            lblTime.AutoSize = true;
            lblTime.BackColor = Color.Transparent;
            lblTime.ForeColor = SystemColors.ControlLightLight;
            lblTime.Location = new Point(778, 744);
            lblTime.Margin = new Padding(5, 0, 5, 0);
            lblTime.Name = "lblTime";
            lblTime.Size = new Size(116, 30);
            lblTime.TabIndex = 17;
            lblTime.Text = "Refresh in: ";
            // 
            // Time
            // 
            Time.AutoSize = true;
            Time.BackColor = Color.Transparent;
            Time.ForeColor = SystemColors.ControlLightLight;
            Time.Location = new Point(886, 744);
            Time.Margin = new Padding(5, 0, 5, 0);
            Time.Name = "Time";
            Time.Size = new Size(42, 30);
            Time.TabIndex = 18;
            Time.Text = "0   ";
            // 
            // panel1
            // 
            panel1.AutoSize = true;
            panel1.BackColor = Color.Transparent;
            panel1.Controls.Add(profileComboBox);
            panel1.Controls.Add(WinTitleRadioButton);
            panel1.Controls.Add(ProcNameRadioButton);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(WindowTitle);
            panel1.Controls.Add(ProcessName);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(hWnd);
            panel1.Controls.Add(RefreshWindowButton);
            panel1.Controls.Add(WindowId);
            panel1.Controls.Add(WindowDisplayName);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(Restore);
            panel1.Controls.Add(Save);
            panel1.Controls.Add(AutoPosition);
            panel1.Controls.Add(IgnoreButton);
            panel1.Controls.Add(RestoreAll);
            panel1.Controls.Add(RefreshAllRunningApps);
            panel1.Controls.Add(KeepWindowOnTop);
            panel1.Controls.Add(lblAllRunningApps);
            panel1.Controls.Add(lblSavedApps);
            panel1.Controls.Add(AllRunningApps);
            panel1.Controls.Add(lbl_Window_Title);
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
            panel1.Margin = new Padding(5, 6, 5, 6);
            panel1.Name = "panel1";
            panel1.Size = new Size(1642, 732);
            panel1.TabIndex = 19;
            // 
            // profileComboBox
            // 
            profileComboBox.FormattingEnabled = true;
            profileComboBox.Items.AddRange(new object[] { "Profile 1", "Profile 2", "Profile 3", "Profile 4", "Profile 5" });
            profileComboBox.Location = new Point(1313, 58);
            profileComboBox.Margin = new Padding(5, 6, 5, 6);
            profileComboBox.Name = "profileComboBox";
            profileComboBox.Size = new Size(174, 38);
            profileComboBox.TabIndex = 41;
            profileComboBox.SelectedIndexChanged += profileComboBox_SelectedIndexChanged;
            // 
            // WinTitleRadioButton
            // 
            WinTitleRadioButton.AutoSize = true;
            WinTitleRadioButton.Checked = true;
            WinTitleRadioButton.Location = new Point(723, 298);
            WinTitleRadioButton.Margin = new Padding(5, 6, 5, 6);
            WinTitleRadioButton.Name = "WinTitleRadioButton";
            WinTitleRadioButton.Size = new Size(21, 20);
            WinTitleRadioButton.TabIndex = 40;
            WinTitleRadioButton.TabStop = true;
            WinTitleRadioButton.UseVisualStyleBackColor = true;
            WinTitleRadioButton.CheckedChanged += WinTitleRadioButton_CheckedChanged;
            // 
            // ProcNameRadioButton
            // 
            ProcNameRadioButton.AutoSize = true;
            ProcNameRadioButton.Location = new Point(723, 190);
            ProcNameRadioButton.Margin = new Padding(5, 6, 5, 6);
            ProcNameRadioButton.Name = "ProcNameRadioButton";
            ProcNameRadioButton.Size = new Size(21, 20);
            ProcNameRadioButton.TabIndex = 39;
            ProcNameRadioButton.UseVisualStyleBackColor = true;
            ProcNameRadioButton.CheckedChanged += ProcNameRadioButton_CheckedChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label2.Location = new Point(759, 30);
            label2.Margin = new Padding(5, 0, 5, 0);
            label2.Name = "label2";
            label2.Size = new Size(189, 38);
            label2.TabIndex = 38;
            label2.Text = "Selected App";
            // 
            // WindowTitle
            // 
            WindowTitle.AutoSize = true;
            WindowTitle.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            WindowTitle.Location = new Point(758, 342);
            WindowTitle.Margin = new Padding(5, 0, 5, 0);
            WindowTitle.Name = "WindowTitle";
            WindowTitle.Size = new Size(246, 30);
            WindowTitle.TabIndex = 37;
            WindowTitle.Text = "Window Title Name Here";
            // 
            // ProcessName
            // 
            ProcessName.AutoSize = true;
            ProcessName.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            ProcessName.Location = new Point(758, 234);
            ProcessName.Margin = new Padding(5, 0, 5, 0);
            ProcessName.Name = "ProcessName";
            ProcessName.Size = new Size(195, 30);
            ProcessName.TabIndex = 36;
            ProcessName.Text = "Process Name Here";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label3.Location = new Point(758, 178);
            label3.Margin = new Padding(5, 0, 5, 0);
            label3.Name = "label3";
            label3.Size = new Size(200, 38);
            label3.TabIndex = 35;
            label3.Text = "Process Name";
            // 
            // hWnd
            // 
            hWnd.AutoSize = true;
            hWnd.Location = new Point(1090, 116);
            hWnd.Margin = new Padding(5, 0, 5, 0);
            hWnd.Name = "hWnd";
            hWnd.Size = new Size(69, 30);
            hWnd.TabIndex = 34;
            hWnd.Text = "hWnd";
            hWnd.Visible = false;
            // 
            // RefreshWindowButton
            // 
            RefreshWindowButton.BackgroundImage = Properties.Resources.refresh_blue_arrows;
            RefreshWindowButton.BackgroundImageLayout = ImageLayout.Zoom;
            RefreshWindowButton.Location = new Point(562, 648);
            RefreshWindowButton.Margin = new Padding(5, 6, 5, 6);
            RefreshWindowButton.Name = "RefreshWindowButton";
            RefreshWindowButton.Size = new Size(62, 68);
            RefreshWindowButton.TabIndex = 33;
            RefreshWindowButton.UseVisualStyleBackColor = true;
            RefreshWindowButton.Click += RefreshWindowButton_Click;
            // 
            // WindowId
            // 
            WindowId.AutoSize = true;
            WindowId.Location = new Point(478, 108);
            WindowId.Margin = new Padding(5, 0, 5, 0);
            WindowId.Name = "WindowId";
            WindowId.Size = new Size(34, 30);
            WindowId.TabIndex = 32;
            WindowId.Text = "ID";
            WindowId.Visible = false;
            // 
            // WindowDisplayName
            // 
            WindowDisplayName.Location = new Point(718, 108);
            WindowDisplayName.Margin = new Padding(5, 6, 5, 6);
            WindowDisplayName.Name = "WindowDisplayName";
            WindowDisplayName.Size = new Size(277, 35);
            WindowDisplayName.TabIndex = 30;
            WindowDisplayName.KeyDown += WindowDisplayName_KeyDown;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(607, 116);
            label1.Margin = new Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new Size(106, 30);
            label1.TabIndex = 31;
            label1.Text = "Nickname";
            // 
            // AutoPosition
            // 
            AutoPosition.AutoSize = true;
            AutoPosition.Location = new Point(607, 572);
            AutoPosition.Margin = new Padding(5, 6, 5, 6);
            AutoPosition.Name = "AutoPosition";
            AutoPosition.Size = new Size(163, 34);
            AutoPosition.TabIndex = 20;
            AutoPosition.Text = "Auto Position";
            AutoPosition.UseVisualStyleBackColor = true;
            AutoPosition.CheckedChanged += AutoPosition_CheckedChanged;
            // 
            // IgnoreButton
            // 
            IgnoreButton.BackgroundImage = Properties.Resources.ignore_list;
            IgnoreButton.BackgroundImageLayout = ImageLayout.Stretch;
            IgnoreButton.Location = new Point(396, 16);
            IgnoreButton.Margin = new Padding(5, 6, 5, 6);
            IgnoreButton.Name = "IgnoreButton";
            IgnoreButton.Size = new Size(62, 68);
            IgnoreButton.TabIndex = 27;
            IgnoreButton.UseVisualStyleBackColor = true;
            IgnoreButton.Click += IgnoreButton_Click;
            // 
            // RestoreAll
            // 
            RestoreAll.BackgroundImage = Properties.Resources.redo;
            RestoreAll.BackgroundImageLayout = ImageLayout.Zoom;
            RestoreAll.Location = new Point(1562, 16);
            RestoreAll.Margin = new Padding(5, 6, 5, 6);
            RestoreAll.Name = "RestoreAll";
            RestoreAll.Size = new Size(62, 68);
            RestoreAll.TabIndex = 26;
            RestoreAll.UseVisualStyleBackColor = true;
            RestoreAll.Click += RestoreAll_Click;
            // 
            // RefreshAllRunningApps
            // 
            RefreshAllRunningApps.BackgroundImage = Properties.Resources.refresh_blue_arrows;
            RefreshAllRunningApps.BackgroundImageLayout = ImageLayout.Zoom;
            RefreshAllRunningApps.Location = new Point(21, 16);
            RefreshAllRunningApps.Margin = new Padding(5, 6, 5, 6);
            RefreshAllRunningApps.Name = "RefreshAllRunningApps";
            RefreshAllRunningApps.Size = new Size(62, 68);
            RefreshAllRunningApps.TabIndex = 25;
            RefreshAllRunningApps.UseVisualStyleBackColor = true;
            RefreshAllRunningApps.Click += RefreshAllRunningApps_Click;
            // 
            // KeepWindowOnTop
            // 
            KeepWindowOnTop.AutoSize = true;
            KeepWindowOnTop.Location = new Point(897, 572);
            KeepWindowOnTop.Margin = new Padding(5, 6, 5, 6);
            KeepWindowOnTop.Name = "KeepWindowOnTop";
            KeepWindowOnTop.Size = new Size(154, 34);
            KeepWindowOnTop.TabIndex = 24;
            KeepWindowOnTop.Text = "Keep on Top";
            KeepWindowOnTop.UseVisualStyleBackColor = true;
            KeepWindowOnTop.CheckedChanged += KeepWindowOnTop_CheckedChanged;
            // 
            // lblAllRunningApps
            // 
            lblAllRunningApps.AutoSize = true;
            lblAllRunningApps.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblAllRunningApps.Location = new Point(132, 30);
            lblAllRunningApps.Margin = new Padding(5, 0, 5, 0);
            lblAllRunningApps.Name = "lblAllRunningApps";
            lblAllRunningApps.Size = new Size(202, 38);
            lblAllRunningApps.TabIndex = 22;
            lblAllRunningApps.Text = "Running Apps";
            // 
            // lblSavedApps
            // 
            lblSavedApps.AutoSize = true;
            lblSavedApps.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblSavedApps.Location = new Point(1318, 10);
            lblSavedApps.Margin = new Padding(5, 0, 5, 0);
            lblSavedApps.Name = "lblSavedApps";
            lblSavedApps.Size = new Size(169, 38);
            lblSavedApps.TabIndex = 21;
            lblSavedApps.Text = "Saved Apps";
            // 
            // toolTip1
            // 
            toolTip1.ToolTipIcon = ToolTipIcon.Info;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(12F, 30F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackgroundImage = Properties.Resources.blue_mesh_background;
            BackgroundImageLayout = ImageLayout.Center;
            ClientSize = new Size(1642, 786);
            Controls.Add(UpdateTimerInterval);
            Controls.Add(panel1);
            Controls.Add(LogOutput);
            Controls.Add(Time);
            Controls.Add(lblTime);
            Controls.Add(lblUpdateTimerInterval);
            DoubleBuffered = true;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(5, 6, 5, 6);
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
        private CheckBox KeepWindowOnTop;
        private Button RefreshAllRunningApps;
        private Button RestoreAll;
        private Button IgnoreButton;
        private TextBox WindowDisplayName;
        private Label label1;
        private Label WindowId;
        private Button RefreshWindowButton;
        private Label hWnd;
        private Label ProcessName;
        private Label label3;
        private Label WindowTitle;
        private Label label2;
        private ToolTip toolTip1;
        private RadioButton WinTitleRadioButton;
        private RadioButton ProcNameRadioButton;
        private ComboBox profileComboBox;
    }
}