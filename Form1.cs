using Newtonsoft.Json;
using System.Diagnostics;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Save_Window_Position_and_Size
{
    public partial class Form1 : Form
    {
        List<Window> savedWindows = new List<Window>();
        List<string> ignoreList = new List<string>();
        Dictionary<IntPtr, Process> runningApps = new Dictionary<IntPtr, Process>();
        System.Windows.Forms.Timer refreshTimer = new System.Windows.Forms.Timer();
        Random random = new Random();

        const string WinPosX = "WinPosX";
        const string WinPosY = "WinPosY";
        const string WinWidth = "WinWidth";
        const string WinHeight = "WinHeight";
        const string WinKeepOnTop = "WinKeepOnTop";

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
                        minutes = mins;
                    else
                        minutes = 1; // defautl to 1mins

                    AddToOutputLog("Checking window positions...");

                    await Task.Run(() => { RestoreAllWindows(); });

                    AddToOutputLog("Window positions are all set.");
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
            var window = GetSelectedWindow();
            UpdateWindowFromGui(window);

            var item = window.DisplayName + " / " + window.Id;
            if (!AppsSaved.Items.Contains(item))
            {
                AppsSaved.Items.Add(item);

                var existing = savedWindows.Find(s => s.Id.Equals(window.Id));

                if (existing == null)
                    savedWindows.Add(window);
                else
                    UpdateWindowFromGui(existing);

                SaveWindowSettings();
            }
        }
        private void Restore_Click(object sender, EventArgs e)
        {
            //var window = GetSelectedWindow();
            var hWnd = IntPtr.Parse(this.hWnd.Text);
            //if (window.IsFileExplorer)
            //  InteractWithWindow.SetFileExplorerWindowPosAndSize(WindowTitle.Text, window.GetWindowPosAndSize());
            //else
            if (hWnd != null)
            {
                //IntPtr hWnd = (IntPtr)window.hWnd;
                InteractWithWindow.SetWindowPositionAndSize(hWnd, int.Parse(WindowPosX.Text), int.Parse(WindowPosY.Text), int.Parse(WindowWidth.Text), int.Parse(WindowHeight.Text));
            }
        }
        private void RestoreAll_Click(object sender, EventArgs e)
        {
            RestoreAllWindows(false);
        }
        private void AppsSaved_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AppsSaved.SelectedIndex == -1 && AppsSaved.Items.Count > 0) return;

            // Get the id and set it to gui
            var parts = AppsSaved.Text.Split('/');
            string displayName = parts[0];
            if (parts.Length > 1 && int.TryParse(parts[1], out int id))
                WindowId.Text = id.ToString();

            PopulateWindowSettings(GetSelectedWindow());
        }
        private void AllRunningApps_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AllRunningApps.SelectedIndex == -1) return;

            PopulateRunningApp();
        }
        private void AppsSaved_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && AppsSaved.SelectedIndex > -1)
            {
                var window = GetSelectedWindow();
                if (window == null) return;
                savedWindows.Remove(window);
                AppsSaved.Items.RemoveAt(AppsSaved.SelectedIndex);
            }
        }
        private void UpdateTimerInterval_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (int.TryParse(UpdateTimerInterval.Text, out int refreshTime))
                    AppSettings.Save("RefreshTime", refreshTime.ToString());
                else
                    AddToOutputLog($"Auto Position Interval {UpdateTimerInterval.Text} must be an integer, timer not updated.");
            }
        }
        private void AutoPosition_CheckedChanged(object sender, EventArgs e)
        {
            var window = GetSelectedWindow();
            window.AutoPosition = AutoPosition.Checked;
            SaveWindowSettings();
        }
        private void KeepWindowOnTop_CheckedChanged(object sender, EventArgs e)
        {
            var window = GetSelectedWindow();
            window.KeepOnTop = KeepWindowOnTop.Checked;
            SaveWindowSettings();
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
            PopulateRunningApp();
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
        void PopulateRunningApp()
        {
            if (AllRunningApps.SelectedIndex == -1) return;
            var parts = AllRunningApps.SelectedItem.ToString().Split('/');
            var processName = parts[0];
            var hWnd = IntPtr.Parse(parts[1]);

            // Check if we have a saved window for this running app
            var window = savedWindows.Find(w => w.hWnd?.Equals(hWnd) == true || w.TitleName.Equals(processName));

            if (window == null)
            {
                // Create a new window
                window = new Window();
                window.Id = random.Next(300, 30000);
            }

            window.hWnd = hWnd;

            // Get process from running apps
            if (runningApps.TryGetValue(hWnd, out var process))
            {
                window.ProcessName = process.ProcessName;
                window.ClassName = InteractWithWindow.GetWindowClassName(process);
                window.TitleName = process.MainWindowTitle;
            }

            // Get the current position and size
            var windowPosAndSize = new WindowPosAndSize();
            //if (windowTitle.StartsWith("File Explorer"))
            //  windowPosAndSize = InteractWithWindow.GetFileExplorerWindow(windowTitle);
            //else
            windowPosAndSize = InteractWithWindow.GetWindowPositionAndSize(hWnd);

            window.SetWindowPosAndSize(windowPosAndSize);

            PopulateWindowSettings(window);
        }
        private void UpdateRunningApps()
        {
            AllRunningApps.ClearItemsThreadSafe();

            // Show all running apps
            runningApps = InteractWithWindow.GetAllRunningApps(ignoreList);

            foreach (var app in runningApps)
            {
                if (app.Value.ProcessName == "cmd")
                    AllRunningApps.AddItemThreadSafe($"{app.Value.MainWindowTitle} / {app.Key}");
                else
                    AllRunningApps.AddItemThreadSafe($"{app.Value.ProcessName} / {app.Key}");
            }
        }
        private Window GetSelectedWindow()
        {
            var window = new Window();

            // Get existing saved window or generate new one
            if (int.TryParse(WindowId.Text, out int id))
            {
                var existing = savedWindows.Find(w => w.Id.Equals(id));
                if (existing != null)
                {
                    window = existing;
                    WindowId.Text = window.Id.ToString();
                }
                else
                {
                    UpdateWindowFromGui(window);
                }
            }

            // Get hWnd 
            foreach (var entry in runningApps)
            {
                bool add = false;
                
                if (entry.Value.ProcessName == "cmd" && entry.Value.MainWindowTitle.Equals(window.TitleName))
                {
                    add = true;
                }
                if (entry.Value.ProcessName != "cmd" && entry.Value.ProcessName == window.ProcessName)
                {
                    add = true;
                }

                if (add)
                {
                    window.hWnd = entry.Value.MainWindowHandle;
                    this.hWnd.Text = window.hWnd.ToString();
                    break;
                }
            }


            return window;
        }
        private void UpdateWindowFromGui(Window window)
        {
            if (window == null) return;

            // Set name to nickname or title if none given
            if (int.TryParse(WindowId.Text, out int id))
                window.Id = id;
            if (IntPtr.TryParse(this.hWnd.Text, out var handle))
                window.hWnd = handle;
            window.ProcessName = ProcessName.Text;
            window.DisplayName = !string.IsNullOrWhiteSpace(WindowDisplayName.Text) ? WindowDisplayName.Text : WindowTitle.Text;
            window.ClassName = WindowClass.Text;
            window.TitleName = WindowTitle.Text;
            window.X = int.TryParse(WindowPosX.Text, out int X) ? X : 0;
            window.Y = int.TryParse(WindowPosY.Text, out int Y) ? Y : 0;
            window.Height = int.TryParse(WindowHeight.Text, out int Height) ? Height : 0;
            window.Width = int.TryParse(WindowWidth.Text, out int Width) ? Width : 0;
            window.AutoPosition = AutoPosition.Checked;
            window.KeepOnTop = KeepWindowOnTop.Checked;
        }
        private void AddToOutputLog(string message)
        {
            LogOutput.AppendText(Environment.NewLine + message);
        }
        private void PopulateWindowSettings(Window window)
        {
            if (window == null) return;
            if (window.Id != null)
                WindowId.Text = window.Id.ToString();
            if (window.hWnd != null && window.hWnd != IntPtr.Zero)
                this.hWnd.Text = window.hWnd.ToString();
            ProcessName.Text = window.ProcessName;
            WindowDisplayName.Text = !string.IsNullOrWhiteSpace(window.DisplayName) ? window.DisplayName : window.TitleName;
            WindowTitle.Text = window.TitleName;
            WindowClass.Text = window.ClassName;
            AutoPosition.Checked = window.AutoPosition;
            KeepWindowOnTop.Checked = window.KeepOnTop;
            WindowPosX.Text = window.X.ToString();
            WindowPosY.Text = window.Y.ToString();
            WindowWidth.Text = window.Width.ToString();
            WindowHeight.Text = window.Height.ToString();
        }
        private void LogWindowSize(WindowPosAndSize windowPosAndSize)
        {
            AddToOutputLog(" x = " + windowPosAndSize.X.ToString());
            AddToOutputLog(" y = " + windowPosAndSize.Y.ToString());
            AddToOutputLog(" width = " + windowPosAndSize.Width.ToString());
            AddToOutputLog(" height = " + windowPosAndSize.Height.ToString());
        }


        // Misc
        private void SaveWindowSettings()
        {
            // Save settings
            string json = JsonConvert.SerializeObject(savedWindows);
            AppSettings.Save("SavedWindows", json);
        }

        private void RestoreAllWindows(bool useSavedAutoPos = true)
        {
            foreach (Window window in savedWindows)
            {
                if (useSavedAutoPos && !window.AutoPosition) continue;

                //if (window.IsFileExplorer)
                //  InteractWithWindow.SetFileExplorerWindowPosAndSize(window.TitleName, window.GetWindowPosAndSize());
                //else
                IntPtr hWnd = (IntPtr)window.hWnd;
                InteractWithWindow.SetWindowPositionAndSize(hWnd, window.X, window.Y, window.Width, window.Height);
            }
        }
        private void CheckWindowPosAndSize(Window window)
        {
            var repositioned = false;

            var currentPosAndSize = InteractWithWindow.GetWindowPositionAndSize(IntPtr.Parse(window.TitleName));

            if (window.IsFileExplorer && !currentPosAndSize.CompareIsEqual(currentPosAndSize, window.GetWindowPosAndSize()))
            {
                InteractWithWindow.SetFileExplorerWindowPosAndSize(window.TitleName, window.GetWindowPosAndSize());
                repositioned = true;
            }
            else if (!window.IsFileExplorer && !currentPosAndSize.CompareIsEqual(currentPosAndSize, window.GetWindowPosAndSize()))
            {
                IntPtr hWnd = (IntPtr)window.hWnd;
                InteractWithWindow.SetWindowPositionAndSize(hWnd, window.X, window.Y, window.Width, window.Height);
                repositioned = true;
            }

            if (repositioned)
                AddToOutputLog(window.DisplayName + " was repositioned to saved location.");

        }




    }




}

