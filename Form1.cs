using System.Configuration;
using System.Diagnostics;

namespace Save_Window_Position_and_Size
{
    public partial class Form1 : Form
    {
        WindowPosition windowPosition = new WindowPosition();
        System.Windows.Forms.Timer updateTimer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer refreshTimer = new System.Windows.Forms.Timer();
        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        // Load/Close
        public Form1()
        {
            InitializeComponent();
            
            updateTimer.Interval = 60 * 1000; // 1min
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Start();

            refreshTimer.Interval = 1000; // 1sec
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();

            var appSize = new Size(816, 434);
            this.MinimumSize = appSize;
            this.MaximumSize = appSize;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            // Go through all the settings
            int counter = 4;
            int index = 0;
            foreach (var key in config.AppSettings.Settings.AllKeys)
            {
                // Load the saved apps
                if (index == 0)
                    AppsSaved.Items.Add(key.Replace("x", ""));

                index++;

                if (index == counter)
                    index = 0;
            }

            // Show all running apps
            var allApps = GetAllRunningApps();

            foreach(var app in allApps)
                AllRunningApps.Items.Add(app);

            // Load saved refresh time
            var refreshTime = LoadRefreshTimeSetting();
            UpdateTimerInterval.Text = refreshTime.ToString();
            minutes = refreshTime - 1;

            // Load auto position setting
            AutoPosition.Checked = LoadAutoPositionSetting();
            StartOrStopTimers(AutoPosition.Checked);

        }

        // Timers
        int minutes = 0;
        int seconds = 60;
        private void RefreshTimer_Tick(object? sender, EventArgs e)
        {
            // Update refresh time
            seconds--;
            Time.Text = minutes.ToString() + ":" + seconds.ToString();

            if (seconds == 0)
            {
                seconds = 60;

                minutes--;
                if (minutes < 0)
                {
                    int.TryParse(UpdateTimerInterval.Text, out var mins);
                    minutes = mins - 1;
                }
            }
        }
        private void UpdateTimer_Tick(object? sender, EventArgs e)
        {
            AddToOutputLog(Environment.NewLine + "Checking window positions...");

            // Get all running apps
            var allApps = GetAllRunningApps();

            foreach (var app in allApps)
            {
                if (AppsSaved.Items.Contains(app))
                    CheckWindowPosAndSize(app);
            }

            AddToOutputLog(Environment.NewLine + "Window positions are all set.");
        }
        private void StartOrStopTimers(bool start)
        {
            if (start)
            {
                updateTimer.Start();
                refreshTimer.Start();
                AddToOutputLog(Environment.NewLine + "Auto positioning windows enabled.");
            }
            else
            {
                updateTimer.Stop();
                refreshTimer.Stop();
                Time.Text = "";
                AddToOutputLog(Environment.NewLine + "Auto positioning windows disabled.");
            }
        }


        // User Actions
        private void Save_Click(object sender, EventArgs e)
        {
            if(!AppsSaved.Items.Contains(WindowTitle.Text))
                AppsSaved.Items.Add(WindowTitle.Text);
            
            var windowPosAndSize = windowPosition.GetWindowPositionAndSize(WindowTitle.Text);

            SaveAppSettings(WindowTitle.Text, windowPosAndSize);
        }
        private void Restore_Click(object sender, EventArgs e)
        {
            // Load last settings 
            var windowSizeAndPos = LoadAppSettings(WindowTitle.Text, true);
            windowPosition.SetWindowPositionAndSize(WindowTitle.Text, windowSizeAndPos.X, windowSizeAndPos.Y, windowSizeAndPos.Width, windowSizeAndPos.Height);
        }
        private void AppsSaved_SelectedIndexChanged(object sender, EventArgs e)
        {
            WindowTitle.Text = AppsSaved.SelectedItem.ToString();

            var windowPosAndSize = LoadAppSettings(WindowTitle.Text, false);
            PopulateWindowSettings(windowPosAndSize);
        }
        private void AllRunningApps_SelectedIndexChanged(object sender, EventArgs e)
        {
            var windowTitle = AllRunningApps.SelectedItem.ToString();
            var windowPosAndSize = windowPosition.GetWindowPositionAndSize(windowTitle);
            WindowTitle.Text = AllRunningApps.SelectedItem.ToString();
            PopulateWindowSettings(windowPosAndSize);
        }
        private void AppsSaved_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                var app = AppsSaved.SelectedItem.ToString();
                if (RemoveSavedAppFromSettings(app))
                    AddToOutputLog(app + " Removed From Settings");
                AppsSaved.Items.RemoveAt(AppsSaved.SelectedIndex);
            }
        }
        private void UpdateTimerInterval_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (int.TryParse(UpdateTimerInterval.Text, out int newInterval))
                {
                    minutes = newInterval - 1;
                    SaveRefreshTimeSetting(newInterval.ToString());
                    AddToOutputLog(Environment.NewLine + "Minutes between checking window positions has been updated.");
                }
                else
                    AddToOutputLog(Environment.NewLine + "Minutes interval { " + UpdateTimerInterval.Text + " } is not a whole number, timer not updated.");
            }
        }
        private void AutoPosition_CheckedChanged(object sender, EventArgs e)
        {
            SaveAutoPositionSetting(AutoPosition.Checked);
            StartOrStopTimers(AutoPosition.Checked);
        }

        // UI
        private void AddToOutputLog(string message)
        {
            LogOutput.AppendText(message);
        }
        private void PopulateWindowSettings(WindowPosAndSize windowPosAndSize)
        {
            WindowPosX.Text = windowPosAndSize.X.ToString();
            WindowPosY.Text = windowPosAndSize.Y.ToString();
            WindowWidth.Text = windowPosAndSize.Width.ToString();
            WindowHeight.Text = windowPosAndSize.Height.ToString();
        }


        // CRUD Settings
        private bool RemoveSavedAppFromSettings(string? windowTitle)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var error = false;

            try { config.AppSettings.Settings.Remove(WindowTitle.Text + "x"); }
            catch { error = true; }

            try { config.AppSettings.Settings.Remove(WindowTitle.Text + "y"); }
            catch { error = true; }

            try { config.AppSettings.Settings.Remove(WindowTitle.Text + "width"); }
            catch { error = true; }

            try { config.AppSettings.Settings.Remove(WindowTitle.Text + "height"); }
            catch { error = true; }

            return error;
        }
        private bool IsAppOnSavedSettings(string? windowTitle)
        {
            if (windowTitle == null)
                return false;

            if (ConfigurationManager.AppSettings[windowTitle + "x"] != null)
                return true;

            return false;
        }
        private WindowPosAndSize LoadAppSettings(string windowTitle, bool updateLog)
        {
            var windowPosAndSize = new WindowPosAndSize();
            
            if (windowTitle == null)
                return windowPosAndSize;

            var error = false;
            int x = 0;
            int y = 0;
            int width = 0;
            int height = 0;


            if (config.AppSettings.Settings[windowTitle + "x"] != null)
            {
                //ConfigurationManager.RefreshSection("appSettings");
                
                try { int.TryParse(config.AppSettings.Settings[windowTitle + "x"].Value, out x); } catch { error = true; LogOutput.AppendText(Environment.NewLine + "Unable to load X position settings for " + windowTitle + ". Please save new settings."); }
                try { int.TryParse(config.AppSettings.Settings[windowTitle + "y"].Value, out y); } catch { error = true; LogOutput.AppendText(Environment.NewLine + "Unable to load Y position settings for " + windowTitle + ". Please save new settings."); }
                try { int.TryParse(config.AppSettings.Settings[windowTitle + "width"].Value, out width); } catch { error = true; LogOutput.AppendText(Environment.NewLine + "Unable to load width settings for " + windowTitle + ". Please save new settings."); }
                try { int.TryParse(config.AppSettings.Settings[windowTitle + "height"].Value, out height); } catch { error = true; LogOutput.AppendText(Environment.NewLine + "Unable to load height settings for " + windowTitle + ". Please save new settings."); }
            }

            windowPosAndSize.X = x;
            windowPosAndSize.Y = y;
            windowPosAndSize.Width = width;
            windowPosAndSize.Height = height;

            if (updateLog)
            {
                AddToOutputLog(Environment.NewLine + WindowTitle.Text + " Settings Loaded: " + Environment.NewLine + " x = " + x);
                AddToOutputLog(" y = " + y);
                AddToOutputLog(" width = " + width);
                AddToOutputLog(" height = " + height);
            }

            return windowPosAndSize;
        }
        private void SaveAppSettings(string windowTitle, WindowPosAndSize windowPosAndSize)
        {
            // Save settings before closing

            var x = windowPosAndSize.X.ToString();
            var y = windowPosAndSize.Y.ToString();
            var width = windowPosAndSize.Width.ToString();
            var height = windowPosAndSize.Height.ToString();

            AddToOutputLog(Environment.NewLine + windowTitle + " Settings Saved: " + Environment.NewLine + " x = " + x);
            AddToOutputLog(" y = " + y);
            AddToOutputLog(" width = " + width);
            AddToOutputLog(" height = " + height);

            if (config.AppSettings.Settings[windowTitle + "x"] != null)
                config.AppSettings.Settings[windowTitle + "x"].Value = x;
            else
            {
                // Only add valid running apps
                if (!WindowTitleIsRunningProcess(windowTitle))
                {
                    AddToOutputLog("Couldn't find running process " + windowTitle + " settings not saved.");
                    return;
                }

                config.AppSettings.Settings.Add(windowTitle + "x", x);
            }

            if (config.AppSettings.Settings[windowTitle + "y"] != null)
                config.AppSettings.Settings[windowTitle + "y"].Value = y;
            else
                config.AppSettings.Settings.Add(windowTitle + "y", y);

            if (config.AppSettings.Settings[windowTitle + "width"] != null)
                config.AppSettings.Settings[windowTitle + "width"].Value = width;
            else
                config.AppSettings.Settings.Add(windowTitle + "width", width);

            if (config.AppSettings.Settings[windowTitle + "height"] != null)
                config.AppSettings.Settings[windowTitle + "height"].Value = height;
            else
                config.AppSettings.Settings.Add(windowTitle + "height", height);

            config.Save();
        }
        private void SaveRefreshTimeSetting(string minutes)
        {
            if (config.AppSettings.Settings["refreshTime"] != null)
                config.AppSettings.Settings["refreshTime"].Value = minutes;
            else
                config.AppSettings.Settings.Add("refreshTime", minutes);

            config.Save();
        }
        private int LoadRefreshTimeSetting()
        {
            var strMinutes = "";
            var intMinutes = 0;
            if (config.AppSettings.Settings["refreshTime"] != null)
                strMinutes = config.AppSettings.Settings["refreshTime"].Value;

            int.TryParse(strMinutes, out intMinutes);

            if (intMinutes == 0)
                intMinutes = 1;
            return intMinutes;
        }
        private void SaveAutoPositionSetting(bool autoPosition)
        {
            if (config.AppSettings.Settings["AutoPosition"] != null)
                config.AppSettings.Settings["AutoPosition"].Value = autoPosition.ToString();
            else
                config.AppSettings.Settings.Add("AutoPosition", autoPosition.ToString());

            config.Save();
        }
        private bool LoadAutoPositionSetting()
        {
            if (config.AppSettings.Settings["AutoPosition"] != null)
                return bool.Parse(config.AppSettings.Settings["AutoPosition"].Value);

            return false;
        }


        // Misc
        private bool WindowTitleIsRunningProcess(string windowTitle)
        {
            var isValid = false;

            Process.GetProcesses().ToList().ForEach(p =>
            {
                if (p.ProcessName == windowTitle || p.MainWindowTitle == windowTitle)
                    isValid = true;                    
            });
            return isValid;
        }
        private List<string> GetAllRunningApps()
        {
            var runningApps = new List<string>();
            Process.GetProcesses().ToList().ForEach(p =>
            {
                if (p.MainWindowTitle.Length > 0)
                {
                    var rect = windowPosition.GetWindowPositionAndSize(p.ProcessName);
                    if (rect.X == 0 && rect.Y == 0 && rect.Width == 0 && rect.Height == 0)
                    {
                        rect = windowPosition.GetWindowPositionAndSize(p.MainWindowTitle);
                        runningApps.Add(p.MainWindowTitle);
                    }
                    else
                        runningApps.Add(p.ProcessName);
                }
            });

            return runningApps;
        }
        private void CheckWindowPosAndSize(string windowTitle)
        {
            var savedPosAndSize = LoadAppSettings(windowTitle, false);

            var currentPosAndSize = windowPosition.GetWindowPositionAndSize(windowTitle);

            if (!currentPosAndSize.CompareIsEqual(currentPosAndSize, savedPosAndSize))
            {
                windowPosition.SetWindowPositionAndSize(windowTitle, savedPosAndSize.X, savedPosAndSize.Y, savedPosAndSize.Width, savedPosAndSize.Height);
                AddToOutputLog(Environment.NewLine + windowTitle + " was repositioned to saved location.");
            }
        }

    }
}