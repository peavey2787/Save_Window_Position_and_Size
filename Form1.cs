using System;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Save_Window_Position_and_Size
{
    public partial class Form1 : Form
    {
        WindowPosition windowPosition = new WindowPosition();
        System.Windows.Forms.Timer updateTimer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer refreshTimer = new System.Windows.Forms.Timer();
        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        const string WinPosX = "WinPosX";
        const string WinPosY = "WinPosY";
        const string WinWidth = "WinWidth";
        const string WinHeight = "WinHeight";
        const string WinKeepOnTop = "WinKeepOnTop";

        // Load/Close
        public Form1()
        {
            InitializeComponent();
            
            updateTimer.Interval = 60 * 1000; // 1min
            updateTimer.Tick += UpdateTimer_Tick;

            refreshTimer.Interval = 1000; // 1sec
            refreshTimer.Tick += RefreshTimer_Tick;

            var appSize = new Size(974, 434);
            this.MinimumSize = appSize;
            this.MaximumSize = appSize;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            // Go through all the settings
            foreach (var key in config.AppSettings.Settings.AllKeys)
            {
                if(key.Contains(WinPosX))
                    AppsSaved.Items.Add(key.Replace(WinPosX, ""));
            }

            // Show all running apps
            var allApps = windowPosition.GetAllRunningApps();

            foreach(var app in allApps)
                AllRunningApps.Items.Add(app);

            // Load saved refresh time
            var refreshTime = LoadRefreshTimeSetting();
            UpdateTimerInterval.Text = refreshTime.ToString();
            minutes = refreshTime - 1;

            // Load auto position setting
            AutoPosition.Checked = LoadAutoPositionSetting();
        }


        // Timers
        int minutes = 0;
        int seconds = 60;
        private void RefreshTimer_Tick(object? sender, EventArgs e)
        {
            // Update refresh time
            seconds--;

            if(seconds.ToString().Length == 1)
                Time.Text = minutes.ToString() + ":0" + seconds.ToString();
            else
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
            AddToOutputLog("Checking window positions...");

            RestoreAllWindows();

            AddToOutputLog("Window positions are all set.");
        }
        private void StartOrStopTimers(bool start)
        {
            if (start)
            {
                updateTimer.Start();
                refreshTimer.Start();
                AddToOutputLog("Auto positioning windows enabled.");
            }
            else
            {
                updateTimer.Stop();
                refreshTimer.Stop();
                Time.Text = "";
                AddToOutputLog("Auto positioning windows disabled.");
            }
        }


        // User Actions
        private void RefreshAllRunningApps_Click(object sender, EventArgs e)
        {
            AllRunningApps.Items.Clear();

            // Show all running apps
            var allApps = windowPosition.GetAllRunningApps();

            foreach (var app in allApps)
                AllRunningApps.Items.Add(app);
        }
        private void Save_Click(object sender, EventArgs e)
        {
            if(!AppsSaved.Items.Contains(WindowTitle.Text))
                AppsSaved.Items.Add(WindowTitle.Text);

            var windowPosAndSize = new WindowPosAndSize();

            if (int.TryParse(WindowPosX.Text, out int posX))
                windowPosAndSize.X = posX;
            else
                AddToOutputLog(" Window Pos X is not a number, unable to save.");

            if (int.TryParse(WindowPosY.Text, out int posY))
                windowPosAndSize.Y = posY;
            else
                AddToOutputLog(" Window Pos Y is not a number, unable to save.");

            if (int.TryParse(WindowWidth.Text, out int width))
                windowPosAndSize.Width = width;
            else
                AddToOutputLog(" Window Width is not a number, unable to save.");

            if (int.TryParse(WindowHeight.Text, out int height))
                windowPosAndSize.Height = height;
            else
                AddToOutputLog(" Window Height is not a number, unable to save.");

            // Save settings
            SaveAppSettings(WindowTitle.Text, windowPosAndSize);
            SaveKeepWindowOnTopSetting(WindowTitle.Text, KeepWindowOnTop.Checked);
        }
        private void Restore_Click(object sender, EventArgs e)
        {
            // Load last settings 
            var windowSizeAndPos = LoadAppSettings(WindowTitle.Text, true);
            if (WindowTitle.Text.StartsWith("File Explorer"))
                windowPosition.SetFileExplorerWindowPosAndSize(WindowTitle.Text, windowSizeAndPos);
            else
                windowPosition.SetWindowPositionAndSize(WindowTitle.Text, windowSizeAndPos.X, windowSizeAndPos.Y, windowSizeAndPos.Width, windowSizeAndPos.Height);
        }
        private void RestoreAll_Click(object sender, EventArgs e)
        {
            RestoreAllWindows();
        }
        private void AppsSaved_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AppsSaved.SelectedIndex == -1)
                return;

            WindowTitle.Text = AppsSaved.SelectedItem.ToString();

            var windowPosAndSize = LoadAppSettings(WindowTitle.Text, false);
            PopulateWindowSettings(windowPosAndSize);
            KeepWindowOnTop.Checked = LoadKeepWindowOnTopSetting(WindowTitle.Text);
        }
        private void AllRunningApps_SelectedIndexChanged(object sender, EventArgs e)
        {
            var windowPosAndSize = new WindowPosAndSize();

            var windowTitle = AllRunningApps.SelectedItem.ToString();            
            WindowTitle.Text = windowTitle;

            if(windowTitle.StartsWith("File Explorer"))
                windowPosAndSize = windowPosition.GetFileExplorerWindow(windowTitle);            
            else
                windowPosAndSize = windowPosition.GetWindowPositionAndSize(windowTitle);            

            PopulateWindowSettings(windowPosAndSize);
        }
        private void AppsSaved_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                var app = AppsSaved.SelectedItem.ToString();
                if (RemoveSavedAppFromSettings(app))
                {
                    AddToOutputLog(app + " Removed From Settings");
                    AppsSaved.Items.RemoveAt(AppsSaved.SelectedIndex);
                }
                else
                    AddToOutputLog($"Unable To Remove {app} From Settings");
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
                    AddToOutputLog("Minutes between checking window positions has been updated.");
                }
                else
                    AddToOutputLog("Minutes interval { " + UpdateTimerInterval.Text + " } is not a whole number, timer not updated.");
            }
        }
        private void AutoPosition_CheckedChanged(object sender, EventArgs e)
        {
            SaveAutoPositionSetting(AutoPosition.Checked);
            StartOrStopTimers(AutoPosition.Checked);
        }
        private void KeepWindowOnTop_CheckedChanged(object sender, EventArgs e)
        {
            if (KeepWindowOnTop.Checked)
                windowPosition.SetWindowAlwaysOnTop(WindowTitle.Text);
            else
                windowPosition.UnsetWindowAlwaysOnTop(WindowTitle.Text);
            
            SaveKeepWindowOnTopSetting(WindowTitle.Text, KeepWindowOnTop.Checked);
        }


        // UI
        private void AddToOutputLog(string message)
        {
            LogOutput.AppendText(Environment.NewLine + message);
        }
        private void PopulateWindowSettings(WindowPosAndSize windowPosAndSize)
        {
            WindowPosX.Text = windowPosAndSize.X.ToString();
            WindowPosY.Text = windowPosAndSize.Y.ToString();
            WindowWidth.Text = windowPosAndSize.Width.ToString();
            WindowHeight.Text = windowPosAndSize.Height.ToString();
        }
        private void LogWindowSize(WindowPosAndSize windowPosAndSize)
        {
            AddToOutputLog(" x = " + windowPosAndSize.X.ToString());
            AddToOutputLog(" y = " + windowPosAndSize.Y.ToString());
            AddToOutputLog(" width = " + windowPosAndSize.Width.ToString());
            AddToOutputLog(" height = " + windowPosAndSize.Height.ToString());
        }


        // CRUD Settings
        private bool RemoveSavedAppFromSettings(string? windowTitle)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            try { config.AppSettings.Settings.Remove(WindowTitle.Text + WinPosX); }
            catch { return false; }

            try { config.AppSettings.Settings.Remove(WindowTitle.Text + WinPosY); }
            catch { return false; }

            try { config.AppSettings.Settings.Remove(WindowTitle.Text + WinWidth); }
            catch { return false; }

            try { config.AppSettings.Settings.Remove(WindowTitle.Text + WinHeight); }
            catch { return false; }

            try { config.AppSettings.Settings.Remove(WindowTitle.Text + WinHeight); }
            catch { return false; }

            config.Save();
            return true;
        }
        private bool IsAppOnSavedSettings(string? windowTitle)
        {
            if (windowTitle == null)
                return false;

            if (ConfigurationManager.AppSettings[windowTitle + WinPosX] != null)
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


            if (config.AppSettings.Settings[windowTitle + WinPosX] != null)
            {
                try { int.TryParse(config.AppSettings.Settings[windowTitle + WinPosX].Value, out x); } catch { error = true; LogOutput.AppendText(Environment.NewLine + "Unable to load X position settings for " + windowTitle + ". Please save new settings."); }
                try { int.TryParse(config.AppSettings.Settings[windowTitle + WinPosY].Value, out y); } catch { error = true; LogOutput.AppendText(Environment.NewLine + "Unable to load Y position settings for " + windowTitle + ". Please save new settings."); }
                try { int.TryParse(config.AppSettings.Settings[windowTitle + WinWidth].Value, out width); } catch { error = true; LogOutput.AppendText(Environment.NewLine + "Unable to load width settings for " + windowTitle + ". Please save new settings."); }
                try { int.TryParse(config.AppSettings.Settings[windowTitle + WinHeight].Value, out height); } catch { error = true; LogOutput.AppendText(Environment.NewLine + "Unable to load height settings for " + windowTitle + ". Please save new settings."); }
            }

            windowPosAndSize.X = x;
            windowPosAndSize.Y = y;
            windowPosAndSize.Width = width;
            windowPosAndSize.Height = height;

            if (updateLog)
            {
                AddToOutputLog(WindowTitle.Text + " Settings Loaded: ");
                LogWindowSize(windowPosAndSize);
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

            AddToOutputLog(windowTitle + " Settings Saved: ");
            LogWindowSize(windowPosAndSize);

            if (config.AppSettings.Settings[windowTitle + WinPosX] != null)
                config.AppSettings.Settings[windowTitle + WinPosX].Value = x;
            else
            {
                // Only add valid running apps
                if (!WindowTitleIsRunningProcess(windowTitle) && windowTitle.StartsWith("File Explorer") == false)
                {
                    AddToOutputLog("Couldn't find running process " + windowTitle + " settings not saved.");
                    return;
                }

                config.AppSettings.Settings.Add(windowTitle + WinPosX, x);
            }

            if (config.AppSettings.Settings[windowTitle + WinPosY] != null)
                config.AppSettings.Settings[windowTitle + WinPosY].Value = y;
            else
                config.AppSettings.Settings.Add(windowTitle + WinPosY, y);

            if (config.AppSettings.Settings[windowTitle + WinWidth] != null)
                config.AppSettings.Settings[windowTitle + WinWidth].Value = width;
            else
                config.AppSettings.Settings.Add(windowTitle + WinWidth, width);

            if (config.AppSettings.Settings[windowTitle + WinHeight] != null)
                config.AppSettings.Settings[windowTitle + WinHeight].Value = height;
            else
                config.AppSettings.Settings.Add(windowTitle + WinHeight, height);

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
            if (config.AppSettings.Settings["autoPosition"] != null)
                config.AppSettings.Settings["autoPosition"].Value = autoPosition.ToString();
            else
                config.AppSettings.Settings.Add("autoPosition", autoPosition.ToString());

            config.Save();
        }
        private bool LoadAutoPositionSetting()
        {
            if (config.AppSettings.Settings["autoPosition"] != null)
                return bool.Parse(config.AppSettings.Settings["autoPosition"].Value);

            return false;
        }
        private void SaveKeepWindowOnTopSetting(string windowTitle, bool keepWindowOnTop)
        {
            if (config.AppSettings.Settings[windowTitle + "keepWindowOnTopSetting"] != null)
                config.AppSettings.Settings[windowTitle + "keepWindowOnTopSetting"].Value = keepWindowOnTop.ToString();
            else
                config.AppSettings.Settings.Add(windowTitle + "keepWindowOnTopSetting", keepWindowOnTop.ToString());

            config.Save();
        }
        private bool LoadKeepWindowOnTopSetting(string windowTitle)
        {
            if (config.AppSettings.Settings[windowTitle + "keepWindowOnTopSetting"] != null)
                return bool.Parse(config.AppSettings.Settings[windowTitle + "keepWindowOnTopSetting"].Value);

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
        private void RestoreAllWindows()
        {
            foreach (var savedApp in AppsSaved.Items)
            {
                var windowTitle = savedApp.ToString();
                var windowSizeAndPos = LoadAppSettings(windowTitle, true);
                if (WindowTitle.Text.StartsWith("File Explorer"))
                    windowPosition.SetFileExplorerWindowPosAndSize(windowTitle, windowSizeAndPos);
                else
                    windowPosition.SetWindowPositionAndSize(windowTitle, windowSizeAndPos.X, windowSizeAndPos.Y, windowSizeAndPos.Width, windowSizeAndPos.Height);
            }

        }
        private void CheckWindowPosAndSize(string windowTitle)
        {
            var repositioned = false;
            var savedPosAndSize = LoadAppSettings(windowTitle, false);
            var currentPosAndSize = windowPosition.GetWindowPositionAndSize(windowTitle);

            if (!currentPosAndSize.CompareIsEqual(currentPosAndSize, savedPosAndSize))
            {
                windowPosition.SetWindowPositionAndSize(windowTitle, savedPosAndSize.X, savedPosAndSize.Y, savedPosAndSize.Width, savedPosAndSize.Height);

                if (!currentPosAndSize.CompareIsEqual(currentPosAndSize, savedPosAndSize))
                {
                    windowPosition.SetFileExplorerWindowPosAndSize(windowTitle, savedPosAndSize);

                    if (currentPosAndSize.CompareIsEqual(currentPosAndSize, savedPosAndSize))
                        repositioned = true;
                }
                else
                    repositioned = true;
            }

            if(repositioned)
                AddToOutputLog(windowTitle + " was repositioned to saved location.");

        }
     
    }




}

