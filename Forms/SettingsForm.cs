using System;
using System.Drawing;
using System.Windows.Forms;
using Save_Window_Position_and_Size.Classes;

namespace Save_Window_Position_and_Size
{
    internal partial class SettingsForm : Form
    {
        // Event to notify Form1 when settings change
        internal event EventHandler<EventArgs> SettingsChanged;

        internal SettingsForm()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            // Load skip confirmation setting
            string skipConfirmation = AppSettings.Load(Constants.AppSettingsConstants.SkipConfirmationKey);
            SkipConfirmationCheckbox.Checked = skipConfirmation == "true";

            // Load minimize other windows setting
            string minimizeOtherWindows = AppSettings.Load(Constants.AppSettingsConstants.MinimizeOtherWindowsKey);
            MinimizeOtherWindowsCheckbox.Checked = minimizeOtherWindows == "true";

            // Load auto-start timer setting
            string autoStartTimer = AppSettings.Load(Constants.AppSettingsConstants.AutoStartTimerKey);
            AutoStartTimerCheckbox.Checked = autoStartTimer == "true";

            // Load start applications if not running setting
            string startAppsIfNotRunning = AppSettings.Load(Constants.AppSettingsConstants.StartAppsIfNotRunningKey);
            StartAppsCheckbox.Checked = startAppsIfNotRunning == "true";

            // Load refresh timer interval
            if (int.TryParse(AppSettings.Load(Constants.AppSettingsConstants.RefreshTimeKey), out int refreshTime))
            {
                UpdateTimerInterval.Text = refreshTime.ToString();
            }
            else
            {
                UpdateTimerInterval.Text = Constants.Defaults.DefaultRefreshTime.ToString();
            }
        }

        private void SaveSettings()
        {
            // Save skip confirmation setting
            AppSettings.Save(Constants.AppSettingsConstants.SkipConfirmationKey,
                SkipConfirmationCheckbox.Checked ? "true" : "false");

            // Save minimize other windows setting
            AppSettings.Save(Constants.AppSettingsConstants.MinimizeOtherWindowsKey,
                MinimizeOtherWindowsCheckbox.Checked ? "true" : "false");

            // Save auto-start timer setting
            AppSettings.Save(Constants.AppSettingsConstants.AutoStartTimerKey,
                AutoStartTimerCheckbox.Checked ? "true" : "false");

            // Save start applications if not running setting
            AppSettings.Save(Constants.AppSettingsConstants.StartAppsIfNotRunningKey,
                StartAppsCheckbox.Checked ? "true" : "false");

            // Save refresh timer interval
            if (int.TryParse(UpdateTimerInterval.Text, out int refreshTime))
            {
                refreshTime = Math.Max(1, refreshTime);
                AppSettings.Save(Constants.AppSettingsConstants.RefreshTimeKey, refreshTime.ToString());
            }

            // Notify Form1 that settings have changed
            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
            this.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void UpdateTimerInterval_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SaveSettings();
                this.Close();
            }
        }
    }
}