using Newtonsoft.Json;
using Save_Window_Position_and_Size.Classes;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using Window = Save_Window_Position_and_Size.Classes.Window;

namespace Save_Window_Position_and_Size
{
    public partial class Form1 : Form
    {
        #region Variables
        List<Window> savedWindows = new List<Window>();
        List<string> ignoreList = new List<string>();
        Dictionary<IntPtr, String> runningApps = new Dictionary<IntPtr, String>();
        System.Windows.Forms.Timer refreshTimer = new System.Windows.Forms.Timer();
        Random random = new Random();

        // Timer
        int minutes = 0;
        int seconds = 60;
        private async void RefreshTimer_Tick(object? sender, EventArgs e)
        {
            seconds--;

            // Update GUI refresh time count down
            if (seconds.ToString().Length == 1)
                Time.Text = minutes.ToString() + ":0" + seconds.ToString();
            else
                Time.Text = minutes.ToString() + ":" + seconds.ToString();

            // Reset counters if timer reaches 0
            if (seconds == 0)
            {
                seconds = 60;

                minutes--;
                if (minutes < 0)
                {
                    if (int.TryParse(UpdateTimerInterval.Text, out var mins))
                        minutes = mins - 1;
                    else
                        minutes = 1;

                    // Timer elapsed perform window restores

                    LogOutput.AppendText("Checking window positions...");

                    await Task.Run(() => { RestoreAllWindows(); });

                    LogOutput.AppendText("Window positions are all set.");
                }
            }
        }
        #endregion


        #region Startup
        // Startup
        public Form1()
        {
            InitializeComponent();

            // Initialize the timer
            refreshTimer.Interval = 1000; // 1sec
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // Show all running apps
            UpdateRunningApps();

            // Load last selected profile
            int profileIndex = int.TryParse(AppSettings.Load("SelectedProfile"), out int parsedProfileIndex) ? parsedProfileIndex : 0;
            profileComboBox.SelectedIndex = profileIndex;

            // Load ignore list
            string json = AppSettings.Load("IgnoreList");
            if (json != null)
                ignoreList = JsonConvert.DeserializeObject<List<string>>(json);

            // Load saved refresh time
            if (int.TryParse(AppSettings.Load("RefreshTime"), out int refreshTime))
            {
                UpdateTimerInterval.Text = refreshTime.ToString();
                minutes = refreshTime;
            }

            // Allow user to right click running app and ignore it
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            ToolStripMenuItem ignoreMenuItem = new ToolStripMenuItem("Ignore");
            ignoreMenuItem.Click += (sender, e) =>
            {
                if (AllRunningApps.SelectedItem != null)
                {
                    if (!ignoreList.Contains(AllRunningApps.Text))
                        ignoreList.Add(AllRunningApps.Text);
                    AllRunningApps.Items.Remove(AllRunningApps.SelectedItem);
                }
            };
            contextMenuStrip.Items.Add(ignoreMenuItem);
            AllRunningApps.ContextMenuStrip = contextMenuStrip;

            // Make label auto word wrap
            WindowTitle.AutoSize = false;
            WindowTitle.AutoEllipsis = true;
            WindowTitle.MaximumSize = new System.Drawing.Size(200, 100);

            // Reset labels
            ClearWindowGUI();

            // Set text when user hovers over a button
            toolTip1.SetToolTip(RefreshAllRunningApps, "Refresh running apps and their locations/sizes");
            toolTip1.SetToolTip(IgnoreButton, "List of apps to hide from showing in the running apps list");
            toolTip1.SetToolTip(RestoreAll, "Restore all saved apps' window sizes/locations");
            toolTip1.SetToolTip(Save, "Save/Update the selected app's window settings");
            toolTip1.SetToolTip(RefreshWindowButton, "Refresh and get the selected app's current window size/location");
            toolTip1.SetToolTip(Restore, "Restore the selected app's saved window size/location");
        }
        private Windows LoadAllSavedProfiles()
        {
            string json = AppSettings.Load("SavedWindows");

            if (string.IsNullOrWhiteSpace(json))
                return new Windows();

            Windows allSavedWindows = JsonConvert.DeserializeObject<Windows>(json);

            if (allSavedWindows == null || allSavedWindows.Profiles.Count() == 0)
                return new Windows();

            return allSavedWindows;
        }
        private void LoadSavedWindows(int profileIndex)
        {
            // Clear current window settings
            AppsSaved.Items.Clear();

            Windows allSavedProfiles = LoadAllSavedProfiles();

            if (allSavedProfiles.Profiles[profileIndex] == null) return;

            savedWindows = allSavedProfiles.Profiles[profileIndex];

            // Add each saved window to GUI
            foreach (Window window in savedWindows)
            {
                window.hWnd = IntPtr.Zero; // clear to ensure latest window is used
                AppsSaved.Items.Add(window);
            }
        }
        private void SaveWindows()
        {
            Windows allSavedProfiles = LoadAllSavedProfiles();

            // Get current profile index
            int profileIndex = profileComboBox.SelectedIndex;

            // Save window settings to selected profile
            allSavedProfiles.Profiles[profileIndex] = savedWindows;

            // Save all window settings
            string json = JsonConvert.SerializeObject(allSavedProfiles);
            AppSettings.Save("SavedWindows", json);
        }
        #endregion


        #region GUI Controls
        // Buttons
        private void RefreshAllRunningApps_Click(object sender, EventArgs e)
        {
            UpdateRunningApps();
        }
        private void Save_Click(object sender, EventArgs e)
        {
            // Check if item is already on the list
            foreach (Window savedItem in AppsSaved.Items)
            {
                if (savedItem.Id.ToString() == WindowId.Text)
                {
                    // update existing
                    var existing = savedWindows.Find(s => s.Id.Equals(savedItem.Id));
                    if (existing != null)
                    {
                        savedWindows.Remove(existing);
                        var newWindow = GetWindowFromGui();
                        savedWindows.Add(newWindow);

                        SaveWindows();
                    }
                    return;
                }
            }

            // Create and add new item
            var window = GetWindowFromGui();
            AppsSaved.Items.Add(window);
            savedWindows.Add(window);

            SaveWindows();
        }
        private void Restore_Click(object sender, EventArgs e)
        {
            var window = GetWindowFromGui();

            // Restore saved window pos/size if this app is saved
            var saved = savedWindows.Find(s => s.Id.Equals(window.Id));
            if (saved != null)
            {
                window.WindowPosAndSize.X = saved.WindowPosAndSize.X;
                window.WindowPosAndSize.Y = saved.WindowPosAndSize.Y;
                window.WindowPosAndSize.Width = saved.WindowPosAndSize.Width;
                window.WindowPosAndSize.Height = saved.WindowPosAndSize.Height;
            }

            // Handle File Explorer windows
            if (window.IsFileExplorer)
            {
                InteractWithWindow.SetFileExplorerWindowPosAndSize(window.TitleName, window.WindowPosAndSize);
                SetWindowGui(window);
                return;
            }

            // Get the window of the running app 
            var process = GetRunningAppProcessBy(window);
            IntPtr hWnd = process;

            // Update the window pos/size if the window was found
            if (hWnd != null && hWnd != IntPtr.Zero)
            {
                InteractWithWindow.SetWindowPositionAndSize(hWnd, window.WindowPosAndSize.X, window.WindowPosAndSize.Y, window.WindowPosAndSize.Width, window.WindowPosAndSize.Height);

                // Update the window pos/size textboxes
                SetWindowGui(window);
            }
        }
        private void RestoreAll_Click(object sender, EventArgs e)
        {
            RestoreAllWindows(false);
        }
        private void IgnoreButton_Click(object sender, EventArgs e)
        {
            var ignoreForm = new IgnoreForm(ignoreList);
            var result = ignoreForm.ShowDialog();

            ignoreList = ignoreForm.GetIgnoreList();
            string json = JsonConvert.SerializeObject(ignoreList);
            AppSettings.Save("IgnoreList", json);

            ignoreForm.Close();
        }
        private void RefreshWindowButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(WindowId.Text)) return;

            Window window = GetWindowFromGui();

            // Handle File Explorer windows
            if (window.IsFileExplorer)
            {
                window.WindowPosAndSize = InteractWithWindow.GetFileExplorerPosAndSize(window.TitleName);
                SetWindowGui(window);
                return;
            }

            var process = GetRunningAppProcessBy(window);
            if (process == null) return;

            window.WindowPosAndSize = InteractWithWindow.GetWindowPositionAndSize(process);
            SetWindowGui(window);
        }


        // ListBoxes
        private void AppsSaved_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AppsSaved.SelectedIndex == -1 && AppsSaved.Items.Count > 0) return;
            AllRunningApps.SelectedIndex = -1;

            // Get the id and set it to gui
            Window selectedWindow = AppsSaved.SelectedItem as Window;

            // Get the window and show it
            var window = savedWindows.Find(s => s.Id.Equals(selectedWindow.Id));
            if (selectedWindow != null)
                SetWindowGui(selectedWindow);
        }
        private void AllRunningApps_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Do nothing if nothing selected
            if (AllRunningApps.SelectedIndex == -1) return;

            AppsSaved.SelectedIndex = -1; // Deselect any saved apps

            string selected = AllRunningApps.SelectedItem.ToString();

            // Handle File explorer windows
            if (selected.StartsWith("FileExplorer: "))
            {
                var newWindow = new Window();
                newWindow.IsFileExplorer = true;
                newWindow.TitleName = selected;
                newWindow.ProcessName = "File Explorer";
                newWindow.WindowPosAndSize = InteractWithWindow.GetFileExplorerPosAndSize(selected);

                SetWindowGui(newWindow); // Update GUI
                return;
            }

            // Handle all other apps by getting their window handle
            var parts = selected.Split('/');
            var processName = parts[0];
            var hWnd = IntPtr.Parse(parts.Last());

            // Bring window to front to get correct size/pos
            InteractWithWindow.SetForegroundWindow(hWnd);
            InteractWithWindow.SetForegroundWindow(this.Handle);

            // Get process from running apps
            var mainWindowTitle = runningApps.TryGetValue(hWnd, out var proc) ? proc : null;

            // Check if we have a saved window for this running app
            var window = savedWindows.Find(w => w.TitleName.Equals(mainWindowTitle));

            if (window == null)
            {
                // Create a new window if not
                window = new Window();
                window.Id = random.Next(300, 32034);
                window.DisplayName = mainWindowTitle;
                window.ProcessName = InteractWithWindow.GetProcessNameByWindowTitle(mainWindowTitle);
                window.TitleName = mainWindowTitle;
            }

            window.hWnd = hWnd;

            // Get the current position and size
            window.WindowPosAndSize = InteractWithWindow.GetWindowPositionAndSize(hWnd);

            SetWindowGui(window); // Update GUI
        }
        private void AllRunningApps_MouseDown(object sender, MouseEventArgs e)
        {
            // If right mouse btn clicked show menu to add app to ignore list
            if (e.Button == MouseButtons.Right)
            {
                int index = AllRunningApps.IndexFromPoint(e.Location);
                if (index >= 0 && index < AllRunningApps.Items.Count)
                {
                    AllRunningApps.SelectedIndex = index;
                }
            }
        }


        // ComboBoxes
        private void profileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppsSaved.SelectedIndex = -1;
            ClearWindowGUI();

            // Get selected profile index and save it
            int profileIndex = profileComboBox.SelectedIndex;
            AppSettings.Save("SelectedProfile", profileIndex.ToString());

            // Load selected profile
            LoadSavedWindows(profileIndex);
        }


        // CheckBoxes
        private void AutoPosition_CheckedChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(this.hWnd.Text)) return;

            Window window = null;

            if (!String.IsNullOrWhiteSpace(WindowId.Text))
            {
                int id = int.TryParse(WindowId.Text, out int value) ? value : 0;
                window = savedWindows.Find(w => w.Id.Equals(id));
            }

            if (window == null)
            {
                window = GetWindowFromGui();
                savedWindows.Add(window);
                AppsSaved.Items.Add(window.TitleName + " / " + WindowId.Text);
            }
            else
                window.AutoPosition = AutoPosition.Checked;

            SaveWindows();
        }
        private void KeepWindowOnTop_CheckedChanged(object sender, EventArgs e)
        {
            // Do nothing if no app selected
            if (String.IsNullOrWhiteSpace(this.hWnd.Text))
            {
                KeepWindowOnTop.Checked = false;
                return;
            }

            // Try getting the saved app
            Window window = null;

            if (!String.IsNullOrWhiteSpace(WindowId.Text))
            {
                int id = int.TryParse(WindowId.Text, out int value) ? value : 0;
                window = savedWindows.Find(w => w.Id.Equals(id));
            }

            // If no saved app, create and add one
            if (window == null)
            {
                window = GetWindowFromGui();
                savedWindows.Add(window);
                AppsSaved.Items.Add(window.TitleName + " / " + WindowId.Text);
            }
            else
                window.KeepOnTop = KeepWindowOnTop.Checked;

            // Get current app's window
            var process = GetRunningAppProcessBy(window);

            if (window.KeepOnTop)
                InteractWithWindow.SetWindowAlwaysOnTop(process);
            else
                InteractWithWindow.UnsetWindowAlwaysOnTop(process);

            SaveWindows();
        }


        // Radio Buttons
        private void ProcNameRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            // Try getting the saved app
            Window window = null;

            if (!String.IsNullOrWhiteSpace(WindowId.Text))
            {
                int id = int.TryParse(WindowId.Text, out int value) ? value : 0;
                window = savedWindows.Find(w => w.Id.Equals(id));
            }

            // If no saved app, create and add one
            if (window == null)
            {
                window = GetWindowFromGui();
                savedWindows.Add(window);
                AppsSaved.Items.Add(window.TitleName + " / " + WindowId.Text);
            }

            SaveWindows();
        }
        private void WinTitleRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            // Try getting the saved app
            Window window = null;

            if (!String.IsNullOrWhiteSpace(WindowId.Text))
            {
                int id = int.TryParse(WindowId.Text, out int value) ? value : 0;
                window = savedWindows.Find(w => w.Id.Equals(id));
            }

            // If no saved app, create and add one
            if (window == null)
            {
                window = GetWindowFromGui();
                savedWindows.Add(window);
                AppsSaved.Items.Add(window.TitleName + " / " + WindowId.Text);
            }

            SaveWindows();
        }


        // TextBoxes
        private void AppsSaved_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && AppsSaved.SelectedIndex > -1)
            {
                Window window = null;

                if (!String.IsNullOrWhiteSpace(WindowId.Text))
                {
                    int id = int.TryParse(WindowId.Text, out int value) ? value : 0;
                    window = savedWindows.Find(w => w.Id.Equals(id));
                }

                if (window == null) return;

                savedWindows.Remove(window);
                AppsSaved.Items.RemoveAt(AppsSaved.SelectedIndex);

                SaveWindows();

                AllRunningApps.SelectedIndex = 0;
            }
        }
        private void UpdateTimerInterval_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (int.TryParse(UpdateTimerInterval.Text, out int refreshTime))
                {
                    refreshTime--;
                    AppSettings.Save("RefreshTime", refreshTime.ToString());

                    minutes = refreshTime;
                    seconds = 60;
                }
                else
                    LogOutput.AppendText($"Auto Position Interval {UpdateTimerInterval.Text} must be an integer, timer not updated.");
            }
        }
        private void WindowDisplayName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Do nothing if nothing selected
                if (AppsSaved.SelectedIndex == -1) return;

                var newNickname = WindowDisplayName.Text;

                // Update GUI listbox item
                AppsSaved.Items[AppsSaved.SelectedIndex] = $"{newNickname} / {WindowId.Text}";

                WindowDisplayName.Text = newNickname;

                // Update the saved item
                Save.PerformClick();
            }
        }
        private void WindowWidth_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Do nothing if nothing selected
                if (AppsSaved.SelectedIndex == -1) return;

                Save.PerformClick();
            }
        }
        private void WindowPosX_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Do nothing if nothing selected
                if (AppsSaved.SelectedIndex == -1) return;

                Save.PerformClick();
            }
        }
        private void WindowPosY_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Do nothing if nothing selected
                if (AppsSaved.SelectedIndex == -1) return;

                Save.PerformClick();
            }
        }
        private void WindowHeight_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Do nothing if nothing selected
                if (AppsSaved.SelectedIndex == -1) return;

                Save.PerformClick();
            }
        }
        #endregion


        #region Helpers
        // UI Changes
        private Task UpdateRunningApps()
        {
            return Task.Run(() =>
            {
                AllRunningApps.ClearItemsThreadSafe();

                // Show all running apps
                runningApps = InteractWithWindow.GetAllRunningApps(ignoreList);

                foreach (var app in runningApps)
                {
                    if (!String.IsNullOrWhiteSpace(app.Value))
                        AllRunningApps.AddItemThreadSafe($"{app.Value} / {app.Key}");
                }

                // Add file explorer windows
                var fileExplorers = InteractWithWindow.GetFileExplorerWindows(ignoreList);

                foreach (var fileExplorer in fileExplorers)
                    AllRunningApps.AddItemThreadSafe(fileExplorer);
            });
        }
        private Window GetWindowFromGui()
        {
            var window = new Window();

            if (int.TryParse(WindowId.Text, out int id))
                window.Id = id;

            if (IntPtr.TryParse(this.hWnd.Text, out var handle))
                window.hWnd = handle;

            window.ProcessName = ProcessName.Text;
            if (window.ProcessName == "File Explorer")
                window.IsFileExplorer = true;
            else
                window.IsFileExplorer = false;
            window.DisplayName = !string.IsNullOrWhiteSpace(WindowDisplayName.Text) ? WindowDisplayName.Text : WindowTitle.Text;
            window.TitleName = WindowTitle.Text;
            window.WindowPosAndSize.X = int.TryParse(WindowPosX.Text, out int X) ? X : 0;
            window.WindowPosAndSize.Y = int.TryParse(WindowPosY.Text, out int Y) ? Y : 0;
            window.WindowPosAndSize.Height = int.TryParse(WindowHeight.Text, out int Height) ? Height : 0;
            window.WindowPosAndSize.Width = int.TryParse(WindowWidth.Text, out int Width) ? Width : 0;
            window.AutoPosition = AutoPosition.Checked;
            window.KeepOnTop = KeepWindowOnTop.Checked;
            window.UseProcessName = ProcNameRadioButton.Checked;
            window.UseTitleName = WinTitleRadioButton.Checked;

            return window;
        }
        private void SetWindowGui(Window window)
        {
            if (window == null)
            {
                ClearWindowGUI();
                return;
            }

            if (window.Id != null)
                WindowId.Text = window.Id.ToString();
            if (window.hWnd != null && window.hWnd != IntPtr.Zero)
                this.hWnd.Text = window.hWnd.ToString();

            ProcessName.Text = !string.IsNullOrWhiteSpace(window.ProcessName) ? window.ProcessName : InteractWithWindow.GetProcessNameByWindowTitle(window.TitleName);

            WindowDisplayName.Text = !string.IsNullOrWhiteSpace(window.DisplayName) ? window.DisplayName : window.TitleName;

            if (window.UseProcessName)
            {
                WinTitleRadioButton.Checked = false;
                ProcNameRadioButton.Checked = true;
            }
            else
            {
                WinTitleRadioButton.Checked = true;
                ProcNameRadioButton.Checked = false;
            }

            WindowTitle.Text = !string.IsNullOrWhiteSpace(window.TitleName) ? window.TitleName : InteractWithWindow.GetWindowTitleByProcessName(window.ProcessName);
            // Adjust the label's size to fit the wrapped text:
            WindowTitle.Size = new System.Drawing.Size(WindowTitle.MaximumSize.Width,
                TextRenderer.MeasureText(WindowTitle.Text, WindowTitle.Font, WindowTitle.MaximumSize).Height);

            AutoPosition.Checked = window.AutoPosition;
            KeepWindowOnTop.Checked = window.KeepOnTop;
            WindowPosX.Text = window.WindowPosAndSize.X.ToString();
            WindowPosY.Text = window.WindowPosAndSize.Y.ToString();
            WindowWidth.Text = window.WindowPosAndSize.Width.ToString();
            WindowHeight.Text = window.WindowPosAndSize.Height.ToString();

            // Hide keep on top for file explorer windows as I'm not sure how to get their hWnd yet
            KeepWindowOnTop.Visible = !window.IsFileExplorer;
        }
        private void ClearWindowGUI()
        {
            WindowId.Text = "";
            this.hWnd.Text = "";
            ProcessName.Text = "";
            WindowDisplayName.Text = "";
            WindowTitle.Text = "";
            WindowPosX.Text = "";
            WindowPosY.Text = "";
            WindowHeight.Text = "";
            WindowWidth.Text = "";
            AutoPosition.Checked = false;
            KeepWindowOnTop.Checked = false;
        }


        // Misc 
        private IntPtr GetRunningAppProcessBy(Window window)
        {
            var allRunningApps = new Dictionary<IntPtr, string>();

            if (window.UseTitleName || string.IsNullOrWhiteSpace(window.ProcessName))
                allRunningApps = InteractWithWindow.GetAllRunningApps(ignoreList);
            else if (!string.IsNullOrWhiteSpace(window.ProcessName))
                return InteractWithWindow.GetWindowHandleByProcessName(window.ProcessName);

            foreach (var app in allRunningApps)
            {
                if (app.Value.Contains("mail"))
                {
                    var come = "get me";
                }
                if (app.Value == window.TitleName)
                {
                    return app.Key;
                }
            }
            return IntPtr.Zero;
        }
        private void RestoreAllWindows(bool useSavedAutoPos = true)
        {
            foreach (Window window in savedWindows)
            {
                // Handle File Explorer windows
                if (window.IsFileExplorer)
                {
                    InteractWithWindow.SetFileExplorerWindowPosAndSize(window.TitleName, window.WindowPosAndSize);
                }

                IntPtr hWnd = IntPtr.Zero;

                if (useSavedAutoPos && !window.AutoPosition) continue;

                var process = GetRunningAppProcessBy(window);
                if (process != IntPtr.Zero)
                {
                    hWnd = process;
                    InteractWithWindow.SetWindowPositionAndSize(hWnd, window.WindowPosAndSize.X, window.WindowPosAndSize.Y, window.WindowPosAndSize.Width, window.WindowPosAndSize.Height);
                }
            }
        }
        #endregion



    }
}

