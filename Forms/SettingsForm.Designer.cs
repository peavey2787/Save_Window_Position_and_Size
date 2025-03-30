﻿namespace Save_Window_Position_and_Size
{
    partial class SettingsForm
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
            components = new System.ComponentModel.Container();
            SkipConfirmationCheckbox = new CheckBox();
            MinimizeOtherWindowsCheckbox = new CheckBox();
            AutoStartTimerCheckbox = new CheckBox();
            StartAppsCheckbox = new CheckBox();
            lblUpdateTimerInterval = new Label();
            UpdateTimerInterval = new TextBox();
            SaveButton = new Button();
            CancelButton = new Button();
            titleLabel = new Label();
            toolTip1 = new ToolTip(components);
            SuspendLayout();
            // 
            // SkipConfirmationCheckbox
            // 
            SkipConfirmationCheckbox.AutoSize = true;
            SkipConfirmationCheckbox.BackColor = Color.Transparent;
            SkipConfirmationCheckbox.Location = new Point(30, 80);
            SkipConfirmationCheckbox.Name = "SkipConfirmationCheckbox";
            SkipConfirmationCheckbox.Size = new Size(190, 19);
            SkipConfirmationCheckbox.TabIndex = 0;
            SkipConfirmationCheckbox.Text = "Skip Confirmation for Captures";
            SkipConfirmationCheckbox.UseVisualStyleBackColor = false;
            // 
            // MinimizeOtherWindowsCheckbox
            // 
            MinimizeOtherWindowsCheckbox.AutoSize = true;
            MinimizeOtherWindowsCheckbox.BackColor = Color.Transparent;
            MinimizeOtherWindowsCheckbox.Location = new Point(30, 120);
            MinimizeOtherWindowsCheckbox.Name = "MinimizeOtherWindowsCheckbox";
            MinimizeOtherWindowsCheckbox.Size = new Size(247, 19);
            MinimizeOtherWindowsCheckbox.TabIndex = 1;
            MinimizeOtherWindowsCheckbox.Text = "Minimize other windows on profile switch";
            MinimizeOtherWindowsCheckbox.UseVisualStyleBackColor = false;
            // 
            // AutoStartTimerCheckbox
            // 
            AutoStartTimerCheckbox.AutoSize = true;
            AutoStartTimerCheckbox.BackColor = Color.Transparent;
            AutoStartTimerCheckbox.Location = new Point(30, 160);
            AutoStartTimerCheckbox.Name = "AutoStartTimerCheckbox";
            AutoStartTimerCheckbox.Size = new Size(184, 19);
            AutoStartTimerCheckbox.TabIndex = 2;
            AutoStartTimerCheckbox.Text = "Auto-start timer on application start";
            AutoStartTimerCheckbox.UseVisualStyleBackColor = false;
            // 
            // StartAppsCheckbox
            // 
            StartAppsCheckbox.AutoSize = true;
            StartAppsCheckbox.BackColor = Color.Transparent;
            StartAppsCheckbox.Location = new Point(30, 200);
            StartAppsCheckbox.Name = "StartAppsCheckbox";
            StartAppsCheckbox.Size = new Size(198, 19);
            StartAppsCheckbox.TabIndex = 3;
            StartAppsCheckbox.Text = "Start applications if not running";
            StartAppsCheckbox.UseVisualStyleBackColor = false;
            // 
            // lblUpdateTimerInterval
            // 
            lblUpdateTimerInterval.AutoSize = true;
            lblUpdateTimerInterval.BackColor = Color.Transparent;
            lblUpdateTimerInterval.Location = new Point(30, 240);
            lblUpdateTimerInterval.Name = "lblUpdateTimerInterval";
            lblUpdateTimerInterval.Size = new Size(164, 15);
            lblUpdateTimerInterval.TabIndex = 4;
            lblUpdateTimerInterval.Text = "Auto Position Interval (mins): ";
            // 
            // UpdateTimerInterval
            // 
            UpdateTimerInterval.Location = new Point(200, 237);
            UpdateTimerInterval.Name = "UpdateTimerInterval";
            UpdateTimerInterval.Size = new Size(40, 23);
            UpdateTimerInterval.TabIndex = 5;
            UpdateTimerInterval.KeyDown += UpdateTimerInterval_KeyDown;
            // 
            // SaveButton
            // 
            SaveButton.Location = new Point(80, 270);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(75, 23);
            SaveButton.TabIndex = 6;
            SaveButton.Text = "Save";
            SaveButton.UseVisualStyleBackColor = true;
            SaveButton.Click += SaveButton_Click;
            // 
            // CancelButton
            // 
            CancelButton.Location = new Point(180, 270);
            CancelButton.Name = "CancelButton";
            CancelButton.Size = new Size(75, 23);
            CancelButton.TabIndex = 7;
            CancelButton.Text = "Cancel";
            CancelButton.UseVisualStyleBackColor = true;
            CancelButton.Click += CancelButton_Click;
            // 
            // titleLabel
            // 
            titleLabel.AutoSize = true;
            titleLabel.BackColor = Color.Transparent;
            titleLabel.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point);
            titleLabel.Location = new Point(100, 30);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(126, 25);
            titleLabel.TabIndex = 8;
            titleLabel.Text = "App Settings";
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.blue_mesh_background;
            ClientSize = new Size(334, 321);
            Controls.Add(titleLabel);
            Controls.Add(CancelButton);
            Controls.Add(SaveButton);
            Controls.Add(UpdateTimerInterval);
            Controls.Add(lblUpdateTimerInterval);
            Controls.Add(StartAppsCheckbox);
            Controls.Add(MinimizeOtherWindowsCheckbox);
            Controls.Add(AutoStartTimerCheckbox);
            Controls.Add(SkipConfirmationCheckbox);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SettingsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Settings";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox SkipConfirmationCheckbox;
        private CheckBox MinimizeOtherWindowsCheckbox;
        private CheckBox AutoStartTimerCheckbox;
        private CheckBox StartAppsCheckbox;
        private Label lblUpdateTimerInterval;
        private TextBox UpdateTimerInterval;
        private Button SaveButton;
        private new Button CancelButton;
        private Label titleLabel;
        private ToolTip toolTip1;
    }
}