using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

namespace Save_Window_Position_and_Size
{
    public partial class Form1 : Form
    {
        List<Window> savedWindows = new List<Window>();
        List<string> ignoreList = new List<string>();
        Dictionary<IntPtr, Process> runningApps = new Dictionary<IntPtr, Process>();
        System.Windows.Forms.Timer refreshTimer = new System.Windows.Forms.Timer();
        Random random = new Random();

        // Load/Close
        public Form1()
        {
            InitializeComponent();

            refreshTimer.Interval = 1000; // 1sec
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();

            var appSize = new Size(974, 434);
            this.MinimumSize = appSize;
            this.MaximumSize = appSize;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // Show all running apps
            Task.Run(() => { UpdateRunningApps(); });

            // Load saved windows
            string json = AppSettings.Load("SavedWindows");
            if (json != null)
                savedWindows = JsonConvert.DeserializeObject<List<Window>>(json);

            foreach (Window window in savedWindows) AppsSaved.Items.Add(window.DisplayName + " / " + window.Id);

            // Load ignore list
            json = AppSettings.Load("IgnoreList");
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
            hWnd.Text = "";
            WindowId.Text = "";

            // Tool tip
            toolTip1.SetToolTip(RefreshAllRunningApps, "Refresh running apps and their locations/sizes");
            toolTip1.SetToolTip(IgnoreButton, "List of apps to hide from showing in the running apps list");
            toolTip1.SetToolTip(RestoreAll, "Restore all saved apps' window sizes/locations");
            toolTip1.SetToolTip(Save, "Save/Update the selected app's window settings");
            toolTip1.SetToolTip(RefreshWindowButton, "Refresh and get the selected app's current window size/location");
            toolTip1.SetToolTip(Restore, "Restore the selected app's saved window size/location");
        }


        // Timers
        int minutes = 0;
        int seconds = 60;
        private async void RefreshTimer_Tick(object? sender, EventArgs e)
        {
            // Update refresh time
            seconds--;

            if (seconds.ToString().Length == 1)
                Time.Text = minutes.ToString() + ":0" + seconds.ToString();
            else
                Time.Text = minutes.ToString() + ":" + seconds.ToString();


            if (seconds == 0)
            {
                seconds = 60;

                minutes--;
                if (minutes < 0)
                {
                    if (int.TryParse(UpdateTimerInterval.Text, out var mins))
                        minutes = mins - 1;
                    else
                        minutes = 1; // defautl to 1mins

                    LogOutput.AppendText("Checking window positions...");

                    await Task.Run(() => { RestoreAllWindows(); });

                    LogOutput.AppendText("Window positions are all set.");
                }
            }
        }



        // User Actions
        private void RefreshAllRunningApps_Click(object sender, EventArgs e)
        {
            UpdateRunningApps();
        }
        private void Save_Click(object sender, EventArgs e)
        {
            var newItem = WindowDisplayName.Text + " / " + WindowId.Text;

            // Check if item is already on the list
            foreach (string savedItem in AppsSaved.Items)
            {
                var item = savedItem.Split('/');
                item[1] = item[1].Trim();

                if (item[1] == WindowId.Text)
                {
                    int id = int.TryParse(item[1], out int value) ? value : 0;
                    // update existing
                    var existing = savedWindows.Find(s => s.Id.Equals(id));
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
            AppsSaved.Items.Add(newItem);

            var window = GetWindowFromGui();
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

            // Get the window of the running app 
            var process = GetRunningAppProcessBy(window);
            IntPtr hWnd = process.MainWindowHandle;

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
        private void AppsSaved_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AppsSaved.SelectedIndex == -1 && AppsSaved.Items.Count > 0) return;
            AllRunningApps.SelectedIndex = -1;

            // Get the id and set it to gui
            var parts = AppsSaved.Text.Split('/');
            string displayName = parts[0];
            if (parts.Length > 1 && int.TryParse(parts.Last(), out int id))
                WindowId.Text = id.ToString();
            else return;

            // Get the window and show it
            var window = savedWindows.Find(s => s.Id.Equals(id));
            if (window != null)
                SetWindowGui(window);
        }
        private void AllRunningApps_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AllRunningApps.SelectedIndex == -1) return;
            AppsSaved.SelectedIndex = -1;

            var parts = AllRunningApps.SelectedItem.ToString().Split('/');
            var processName = parts[0];
            var hWnd = IntPtr.Parse(parts.Last());

            // Get process from running apps
            var process = runningApps.TryGetValue(hWnd, out var proc) ? proc : null;

            // Check if we have a saved window for this running app
            var window = savedWindows.Find(w => (!w.ProcessName.Equals("cmd") && w.ProcessName.Equals(process.ProcessName))
                || (w.ProcessName.Equals("cmd") && w.TitleName.Equals(process.MainWindowTitle)));

            if (window == null)
            {
                // Create a new window
                window = new Window();
                window.Id = random.Next(300, 32034);
                window.DisplayName = process.MainWindowTitle;
                window.ProcessName = process.ProcessName;
                window.TitleName = process.MainWindowTitle;
            }

            window.hWnd = hWnd;

            // Get the current position and size
            var windowPosAndSize = new WindowPosAndSize();

            windowPosAndSize = InteractWithWindow.GetWindowPositionAndSize(hWnd);

            window.WindowPosAndSize = windowPosAndSize;

            SetWindowGui(window);
        }
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
                    AppSettings.Save("RefreshTime", refreshTime.ToString());
                else
                    LogOutput.AppendText($"Auto Position Interval {UpdateTimerInterval.Text} must be an integer, timer not updated.");
            }
        }
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

            if(window.KeepOnTop)
                InteractWithWindow.SetWindowAlwaysOnTop(process.MainWindowHandle);
            else
                InteractWithWindow.UnsetWindowAlwaysOnTop(process.MainWindowHandle);

            SaveWindows();
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
            var process = GetRunningAppProcessBy(window);
            if (process == null) return;

            var windowPosAndSize = InteractWithWindow.GetWindowPositionAndSize(process.MainWindowHandle);
            WindowPosX.Text = windowPosAndSize.X.ToString();
            WindowPosY.Text = windowPosAndSize.Y.ToString();
            WindowHeight.Text = windowPosAndSize.Height.ToString();
            WindowWidth.Text = windowPosAndSize.Width.ToString();
            this.hWnd.Text = process.MainWindowHandle.ToString();
        }
        private void AllRunningApps_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = AllRunningApps.IndexFromPoint(e.Location);
                if (index >= 0 && index < AllRunningApps.Items.Count)
                {
                    AllRunningApps.SelectedIndex = index;
                }
            }
        }



        // UI
        private void UpdateRunningApps()
        {
            AllRunningApps.ClearItemsThreadSafe();

            // Show all running apps
            runningApps = InteractWithWindow.GetAllRunningApps(ignoreList);

            foreach (var app in runningApps)
            {
                if (!String.IsNullOrWhiteSpace(app.Value.MainWindowTitle))
                    AllRunningApps.AddItemThreadSafe($"{app.Value.MainWindowTitle} / {app.Key}");
                else
                    AllRunningApps.AddItemThreadSafe($"{app.Value.ProcessName} / {app.Key}");
            }
        }
        private Window GetWindowFromGui()
        {
            var window = new Window();

            if (int.TryParse(WindowId.Text, out int id))
                window.Id = id;

            if (IntPtr.TryParse(this.hWnd.Text, out var handle))
                window.hWnd = handle;

            window.ProcessName = ProcessName.Text;
            window.DisplayName = !string.IsNullOrWhiteSpace(WindowDisplayName.Text) ? WindowDisplayName.Text : WindowTitle.Text;
            window.TitleName = WindowTitle.Text;
            window.WindowPosAndSize.X = int.TryParse(WindowPosX.Text, out int X) ? X : 0;
            window.WindowPosAndSize.Y = int.TryParse(WindowPosY.Text, out int Y) ? Y : 0;
            window.WindowPosAndSize.Height = int.TryParse(WindowHeight.Text, out int Height) ? Height : 0;
            window.WindowPosAndSize.Width = int.TryParse(WindowWidth.Text, out int Width) ? Width : 0;
            window.AutoPosition = AutoPosition.Checked;
            window.KeepOnTop = KeepWindowOnTop.Checked;

            return window;
        }
        private void SetWindowGui(Window window)
        {
            if (window == null)
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

            if (window.Id != null)
                WindowId.Text = window.Id.ToString();
            if (window.hWnd != null && window.hWnd != IntPtr.Zero)
                this.hWnd.Text = window.hWnd.ToString();
            ProcessName.Text = window.ProcessName;
            WindowDisplayName.Text = !string.IsNullOrWhiteSpace(window.DisplayName) ? window.DisplayName : window.TitleName;
            // Adjust the label's size to fit the wrapped text:
            WindowTitle.Size = new System.Drawing.Size(WindowTitle.MaximumSize.Width,
                TextRenderer.MeasureText(WindowTitle.Text, WindowTitle.Font, WindowTitle.MaximumSize).Height);
            WindowTitle.Text = window.TitleName;
            AutoPosition.Checked = window.AutoPosition;
            KeepWindowOnTop.Checked = window.KeepOnTop;
            WindowPosX.Text = window.WindowPosAndSize.X.ToString();
            WindowPosY.Text = window.WindowPosAndSize.Y.ToString();
            WindowWidth.Text = window.WindowPosAndSize.Width.ToString();
            WindowHeight.Text = window.WindowPosAndSize.Height.ToString();
        }



        // Misc
        private Process GetRunningAppProcessBy(Window window)
        {
            var allRunningApps = InteractWithWindow.GetAllRunningApps(ignoreList);

            foreach (var app in allRunningApps)
            {
                if (app.Value.MainWindowTitle == window.TitleName)
                {
                    return app.Value;
                }
                if (!(app.Value.ProcessName == "cmd" || app.Value.ProcessName == "windows terminal")
                    && app.Value.ProcessName == window.ProcessName)
                {
                    return app.Value;
                }
            }
            return null;
        }
        private void SaveWindows()
        {
            // Save settings
            string json = JsonConvert.SerializeObject(savedWindows);
            AppSettings.Save("SavedWindows", json);
        }
        private void RestoreAllWindows(bool useSavedAutoPos = true)
        {

            foreach (Window window in savedWindows)
            {
                IntPtr hWnd = IntPtr.Zero;

                if (useSavedAutoPos && !window.AutoPosition) continue;

                var process = GetRunningAppProcessBy(window);
                if (process != null)
                    hWnd = process.MainWindowHandle;

                if (hWnd != IntPtr.Zero)
                    InteractWithWindow.SetWindowPositionAndSize(hWnd, window.WindowPosAndSize.X, window.WindowPosAndSize.Y, window.WindowPosAndSize.Width, window.WindowPosAndSize.Height);
            }
        }



    }




}

