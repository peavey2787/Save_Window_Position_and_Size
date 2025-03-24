namespace Save_Window_Position_and_Size
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;


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
            panel1 = new Panel();
            UsePercentagesCheckBox = new CheckBox();
            profileComboBox = new ComboBox();
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
            SkipConfirmationCheckbox = new CheckBox();
            toolTip1 = new ToolTip(components);
            Time = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // Save
            // 
            Save.BackgroundImage = Properties.Resources.floppy_disk;
            Save.BackgroundImageLayout = ImageLayout.Zoom;
            Save.Location = new Point(476, 324);
            Save.Name = "Save";
            Save.Size = new Size(36, 34);
            Save.TabIndex = 0;
            Save.UseVisualStyleBackColor = true;
            Save.Click += Save_Click;
            // 
            // lbl_Window_Title
            // 
            lbl_Window_Title.AutoSize = true;
            lbl_Window_Title.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lbl_Window_Title.Location = new Point(521, 89);
            lbl_Window_Title.Name = "lbl_Window_Title";
            lbl_Window_Title.Size = new Size(112, 21);
            lbl_Window_Title.TabIndex = 2;
            lbl_Window_Title.Text = "Window Title";
            // 
            // Restore
            // 
            Restore.BackgroundImage = Properties.Resources.redo;
            Restore.BackgroundImageLayout = ImageLayout.Zoom;
            Restore.Location = new Point(619, 324);
            Restore.Name = "Restore";
            Restore.Size = new Size(36, 34);
            Restore.TabIndex = 3;
            Restore.UseVisualStyleBackColor = true;
            Restore.Click += Restore_Click;
            // 
            // AppsSaved
            // 
            AppsSaved.BackColor = Color.FromArgb(11, 83, 144);
            AppsSaved.ForeColor = SystemColors.Window;
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
            WindowPosX.Location = new Point(347, 195);
            WindowPosX.Name = "WindowPosX";
            WindowPosX.Size = new Size(50, 23);
            WindowPosX.TabIndex = 6;
            WindowPosX.KeyDown += WindowPosX_KeyDown;
            // 
            // lblWindowPosX
            // 
            lblWindowPosX.AutoSize = true;
            lblWindowPosX.Location = new Point(276, 198);
            lblWindowPosX.Name = "lblWindowPosX";
            lblWindowPosX.Size = new Size(63, 15);
            lblWindowPosX.TabIndex = 7;
            lblWindowPosX.Text = "Position X:";
            // 
            // lblWindowPosY
            // 
            lblWindowPosY.AutoSize = true;
            lblWindowPosY.Location = new Point(276, 232);
            lblWindowPosY.Name = "lblWindowPosY";
            lblWindowPosY.Size = new Size(63, 15);
            lblWindowPosY.TabIndex = 8;
            lblWindowPosY.Text = "Position Y:";
            // 
            // WindowPosY
            // 
            WindowPosY.Location = new Point(346, 229);
            WindowPosY.Name = "WindowPosY";
            WindowPosY.Size = new Size(49, 23);
            WindowPosY.TabIndex = 9;
            WindowPosY.KeyDown += WindowPosY_KeyDown;
            // 
            // WindowWidth
            // 
            WindowWidth.Location = new Point(503, 195);
            WindowWidth.Name = "WindowWidth";
            WindowWidth.Size = new Size(50, 23);
            WindowWidth.TabIndex = 10;
            WindowWidth.KeyDown += WindowWidth_KeyDown;
            // 
            // lblWindowWidth
            // 
            lblWindowWidth.AutoSize = true;
            lblWindowWidth.Location = new Point(456, 198);
            lblWindowWidth.Name = "lblWindowWidth";
            lblWindowWidth.Size = new Size(42, 15);
            lblWindowWidth.TabIndex = 11;
            lblWindowWidth.Text = "Width:";
            // 
            // lblWindowHeight
            // 
            lblWindowHeight.AutoSize = true;
            lblWindowHeight.Location = new Point(451, 232);
            lblWindowHeight.Name = "lblWindowHeight";
            lblWindowHeight.Size = new Size(46, 15);
            lblWindowHeight.TabIndex = 12;
            lblWindowHeight.Text = "Height:";
            // 
            // WindowHeight
            // 
            WindowHeight.Location = new Point(503, 229);
            WindowHeight.Name = "WindowHeight";
            WindowHeight.Size = new Size(50, 23);
            WindowHeight.TabIndex = 13;
            WindowHeight.KeyDown += WindowHeight_KeyDown;
            // 
            // AllRunningApps
            // 
            AllRunningApps.BackColor = Color.FromArgb(11, 83, 144);
            AllRunningApps.ForeColor = SystemColors.Window;
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
            UpdateTimerInterval.Location = new Point(171, 364);
            UpdateTimerInterval.Name = "UpdateTimerInterval";
            UpdateTimerInterval.Size = new Size(25, 23);
            UpdateTimerInterval.TabIndex = 15;
            UpdateTimerInterval.Text = "1";
            UpdateTimerInterval.KeyDown += UpdateTimerInterval_KeyDown;
            // 
            // lblUpdateTimerInterval
            // 
            lblUpdateTimerInterval.AutoSize = true;
            lblUpdateTimerInterval.BackColor = Color.Transparent;
            lblUpdateTimerInterval.ForeColor = SystemColors.ControlLightLight;
            lblUpdateTimerInterval.Location = new Point(12, 368);
            lblUpdateTimerInterval.Name = "lblUpdateTimerInterval";
            lblUpdateTimerInterval.Size = new Size(164, 15);
            lblUpdateTimerInterval.TabIndex = 16;
            lblUpdateTimerInterval.Text = "Auto Position Interval (mins): ";
            // 
            // lblTime
            // 
            lblTime.AutoSize = true;
            lblTime.BackColor = Color.Transparent;
            lblTime.ForeColor = SystemColors.ControlLightLight;
            lblTime.Location = new Point(417, 374);
            lblTime.Name = "lblTime";
            lblTime.Size = new Size(65, 15);
            lblTime.TabIndex = 17;
            lblTime.Text = "Refresh in: ";
            // 
            // panel1
            // 
            panel1.AutoSize = true;
            panel1.BackColor = Color.Transparent;
            panel1.Controls.Add(UpdateTimerInterval);
            panel1.Controls.Add(lblTime);
            panel1.Controls.Add(UsePercentagesCheckBox);
            panel1.Controls.Add(profileComboBox);
            panel1.Controls.Add(Time);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(WindowTitle);
            panel1.Controls.Add(lblUpdateTimerInterval);
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
            panel1.Controls.Add(SkipConfirmationCheckbox);
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(958, 393);
            panel1.TabIndex = 19;
            // 
            // UsePercentagesCheckBox
            // 
            UsePercentagesCheckBox.AutoSize = true;
            UsePercentagesCheckBox.Location = new Point(577, 267);
            UsePercentagesCheckBox.Name = "UsePercentagesCheckBox";
            UsePercentagesCheckBox.Size = new Size(112, 19);
            UsePercentagesCheckBox.TabIndex = 42;
            UsePercentagesCheckBox.Text = "Use Percentages";
            UsePercentagesCheckBox.UseVisualStyleBackColor = true;
            UsePercentagesCheckBox.CheckedChanged += UsePercentagesCheckBox_CheckedChanged;
            // 
            // profileComboBox
            // 
            profileComboBox.FormattingEnabled = true;
            profileComboBox.Items.AddRange(new object[] { "Profile 1", "Profile 2", "Profile 3", "Profile 4", "Profile 5" });
            profileComboBox.Location = new Point(766, 29);
            profileComboBox.Name = "profileComboBox";
            profileComboBox.Size = new Size(103, 23);
            profileComboBox.TabIndex = 41;
            profileComboBox.SelectedIndexChanged += profileComboBox_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label2.Location = new Point(443, 15);
            label2.Name = "label2";
            label2.Size = new Size(110, 21);
            label2.TabIndex = 38;
            label2.Text = "Selected App";
            // 
            // WindowTitle
            // 
            WindowTitle.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            WindowTitle.Location = new Point(516, 110);
            WindowTitle.Name = "WindowTitle";
            WindowTitle.Size = new Size(159, 66);
            WindowTitle.TabIndex = 37;
            WindowTitle.Text = "Window Title Name Here";
            // 
            // ProcessName
            // 
            ProcessName.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            ProcessName.Location = new Point(279, 110);
            ProcessName.Name = "ProcessName";
            ProcessName.Size = new Size(136, 66);
            ProcessName.TabIndex = 36;
            ProcessName.Text = "Process Name Here";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label3.Location = new Point(298, 89);
            label3.Name = "label3";
            label3.Size = new Size(117, 21);
            label3.TabIndex = 35;
            label3.Text = "Process Name";
            // 
            // hWnd
            // 
            hWnd.AutoSize = true;
            hWnd.Location = new Point(636, 58);
            hWnd.Name = "hWnd";
            hWnd.Size = new Size(39, 15);
            hWnd.TabIndex = 34;
            hWnd.Text = "hWnd";
            hWnd.Visible = false;
            // 
            // RefreshWindowButton
            // 
            RefreshWindowButton.BackgroundImage = Properties.Resources.refresh_blue_arrows;
            RefreshWindowButton.BackgroundImageLayout = ImageLayout.Zoom;
            RefreshWindowButton.Location = new Point(328, 324);
            RefreshWindowButton.Name = "RefreshWindowButton";
            RefreshWindowButton.Size = new Size(36, 34);
            RefreshWindowButton.TabIndex = 33;
            RefreshWindowButton.UseVisualStyleBackColor = true;
            RefreshWindowButton.Click += RefreshWindowButton_Click;
            // 
            // WindowId
            // 
            WindowId.AutoSize = true;
            WindowId.Location = new Point(279, 54);
            WindowId.Name = "WindowId";
            WindowId.Size = new Size(18, 15);
            WindowId.TabIndex = 32;
            WindowId.Text = "ID";
            WindowId.Visible = false;
            // 
            // WindowDisplayName
            // 
            WindowDisplayName.Location = new Point(419, 54);
            WindowDisplayName.Name = "WindowDisplayName";
            WindowDisplayName.Size = new Size(163, 23);
            WindowDisplayName.TabIndex = 30;
            WindowDisplayName.KeyDown += WindowDisplayName_KeyDown;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(354, 58);
            label1.Name = "label1";
            label1.Size = new Size(61, 15);
            label1.TabIndex = 31;
            label1.Text = "Nickname";
            // 
            // AutoPosition
            // 
            AutoPosition.AutoSize = true;
            AutoPosition.Location = new Point(577, 199);
            AutoPosition.Name = "AutoPosition";
            AutoPosition.Size = new Size(98, 19);
            AutoPosition.TabIndex = 20;
            AutoPosition.Text = "Auto Position";
            AutoPosition.UseVisualStyleBackColor = true;
            AutoPosition.CheckedChanged += AutoPosition_CheckedChanged;
            // 
            // IgnoreButton
            // 
            IgnoreButton.BackgroundImage = Properties.Resources.ignore_list;
            IgnoreButton.BackgroundImageLayout = ImageLayout.Stretch;
            IgnoreButton.Location = new Point(231, 8);
            IgnoreButton.Name = "IgnoreButton";
            IgnoreButton.Size = new Size(36, 34);
            IgnoreButton.TabIndex = 27;
            IgnoreButton.UseVisualStyleBackColor = true;
            IgnoreButton.Click += IgnoreButton_Click;
            // 
            // RestoreAll
            // 
            RestoreAll.BackgroundImage = Properties.Resources.redo;
            RestoreAll.BackgroundImageLayout = ImageLayout.Zoom;
            RestoreAll.Location = new Point(911, 8);
            RestoreAll.Name = "RestoreAll";
            RestoreAll.Size = new Size(36, 34);
            RestoreAll.TabIndex = 26;
            RestoreAll.UseVisualStyleBackColor = true;
            RestoreAll.Click += RestoreAll_Click;
            // 
            // RefreshAllRunningApps
            // 
            RefreshAllRunningApps.BackgroundImage = Properties.Resources.refresh_blue_arrows;
            RefreshAllRunningApps.BackgroundImageLayout = ImageLayout.Zoom;
            RefreshAllRunningApps.Location = new Point(12, 8);
            RefreshAllRunningApps.Name = "RefreshAllRunningApps";
            RefreshAllRunningApps.Size = new Size(36, 34);
            RefreshAllRunningApps.TabIndex = 25;
            RefreshAllRunningApps.UseVisualStyleBackColor = true;
            RefreshAllRunningApps.Click += RefreshAllRunningApps_Click;
            // 
            // KeepWindowOnTop
            // 
            KeepWindowOnTop.AutoSize = true;
            KeepWindowOnTop.Location = new Point(577, 233);
            KeepWindowOnTop.Name = "KeepWindowOnTop";
            KeepWindowOnTop.Size = new Size(91, 19);
            KeepWindowOnTop.TabIndex = 24;
            KeepWindowOnTop.Text = "Keep on Top";
            KeepWindowOnTop.UseVisualStyleBackColor = true;
            KeepWindowOnTop.CheckedChanged += KeepWindowOnTop_CheckedChanged;
            // 
            // lblAllRunningApps
            // 
            lblAllRunningApps.AutoSize = true;
            lblAllRunningApps.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblAllRunningApps.Location = new Point(77, 15);
            lblAllRunningApps.Name = "lblAllRunningApps";
            lblAllRunningApps.Size = new Size(117, 21);
            lblAllRunningApps.TabIndex = 22;
            lblAllRunningApps.Text = "Running Apps";
            // 
            // lblSavedApps
            // 
            lblSavedApps.AutoSize = true;
            lblSavedApps.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblSavedApps.Location = new Point(769, 5);
            lblSavedApps.Name = "lblSavedApps";
            lblSavedApps.Size = new Size(98, 21);
            lblSavedApps.TabIndex = 21;
            lblSavedApps.Text = "Saved Apps";
            // 
            // SkipConfirmationCheckbox
            // 
            SkipConfirmationCheckbox.AutoSize = true;
            SkipConfirmationCheckbox.Location = new Point(740, 371);
            SkipConfirmationCheckbox.Name = "SkipConfirmationCheckbox";
            SkipConfirmationCheckbox.Size = new Size(161, 19);
            SkipConfirmationCheckbox.TabIndex = 30;
            SkipConfirmationCheckbox.Text = "Skip confirmation dialogs";
            SkipConfirmationCheckbox.UseVisualStyleBackColor = true;
            // 
            // toolTip1
            // 
            toolTip1.ToolTipIcon = ToolTipIcon.Info;
            // 
            // Time
            // 
            Time.AutoSize = true;
            Time.BackColor = Color.Transparent;
            Time.ForeColor = SystemColors.ControlLightLight;
            Time.Location = new Point(488, 374);
            Time.Name = "Time";
            Time.Size = new Size(22, 15);
            Time.TabIndex = 18;
            Time.Text = "0   ";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackgroundImage = Properties.Resources.blue_mesh_background;
            BackgroundImageLayout = ImageLayout.Center;
            ClientSize = new Size(958, 397);
            Controls.Add(panel1);
            DoubleBuffered = true;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "Save Window Size and Position";
            FormClosing += Form1_FormClosing;
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
        private Label ProcessName;
        private Label label3;
        private Label WindowTitle;
        private Label label2;
        private ToolTip toolTip1;
        private ComboBox profileComboBox;
        private Label hWnd;
        private CheckBox SkipConfirmationCheckbox;
        private CheckBox UsePercentagesCheckBox;
        private Label Time;
    }
}