using Newtonsoft.Json;
using Save_Window_Position_and_Size.Classes;
using Save_Window_Position_and_Size.Forms;
using System;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using Window = Save_Window_Position_and_Size.Classes.Window;
using Timer = System.Windows.Forms.Timer;

namespace Save_Window_Position_and_Size
{
    public partial class Form1 : Form
    {
        #region Variables
        private NotifyIcon notify_icon = new NotifyIcon();
        private IgnoreListManager ignoreListManager;
        private Dictionary<IntPtr, String> runningApps = new Dictionary<IntPtr, String>();
        private System.Windows.Forms.Timer refreshTimer = new System.Windows.Forms.Timer();
        private bool loading = false;

        // Manager classes
        private WindowManager windowManager;
        private WindowHighlighter windowHighlighter;
        private QuickLayoutManager quickLayoutManager;

        // Timer
        private int minutes = 0;
        private int seconds = 60;
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
                    // Get the saved refresh time from settings
                    if (int.TryParse(AppSettings.Load(Constants.AppSettingsConstants.RefreshTimeKey), out var mins))
                        minutes = mins - 1;
                    else
                        minutes = 1;

                    // Timer elapsed perform window restores
                    await Task.Run(() => { windowManager.RestoreAllWindows(); });
                }
            }
        }

        #endregion


        #region Startup/Shutdown
        // Startup
        public Form1()
        {
            InitializeComponent();

            // Initialize the ignore list manager first
            ignoreListManager = new IgnoreListManager();

            // Initialize the window manager with the ignore list manager
            windowManager = new WindowManager(ignoreListManager);

            // Initialize the window highlighter
            InitializeHighlighter();

            // Initialize the quick layout manager
            quickLayoutManager = new QuickLayoutManager(windowManager);

            // Create notify icon
            CreateNotifyIcon();

            // Initialize the timer
            refreshTimer.Interval = Constants.Defaults.TimerIntervalMs;
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            // Show all running apps
            await UpdateAllRunningAppsTask();

            // Initialize profiles and load profile names
            InitializeProfiles();

            // Load last selected profile
            int profileIndex = AppSettings<int>.Load(Constants.AppSettingsConstants.SelectedProfileKey);

            // Ensure profile index is valid
            if (profileIndex < 0 || profileIndex >= Constants.Defaults.MaxProfiles)
                profileIndex = 0;

            profileComboBox.SelectedIndex = profileIndex;

            // Load saved refresh time from settings
            if (int.TryParse(AppSettings.Load(Constants.AppSettingsConstants.RefreshTimeKey), out int refreshTime))
            {
                minutes = refreshTime - 1;
                seconds = 60;
            }

            // Allow user to right click running app and ignore it
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            ToolStripMenuItem ignoreMenuItem = new ToolStripMenuItem("Ignore");
            ignoreMenuItem.Click += (sender, e) =>
            {
                if (AllRunningApps.SelectedItem != null && AllRunningApps.SelectedItem is Window window)
                {
                    if (ignoreListManager.AddToIgnoreList(window.TitleName))
                    {
                        // Remove from listbox if added to ignore list
                        AllRunningApps.Items.Remove(AllRunningApps.SelectedItem);
                        ClearWindowGUI();
                    }
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
            toolTip1.SetToolTip(ClearAllButton, "Clear all saved windows in current profile");
            toolTip1.SetToolTip(Save, "Save/Update the selected app's window settings");
            toolTip1.SetToolTip(RefreshWindowButton, "Refresh and get the selected app's current window size/location");
            toolTip1.SetToolTip(Restore, "Restore the selected app's saved window size/location");
            toolTip1.SetToolTip(SettingsButton, "Open application settings");
            toolTip1.SetToolTip(CreateQuickLayoutButton, "Create a minimized form in the taskbar that saves the current window layout and can restore it with a single click");

            // Handle the UsePercentagesCheckBox changes
            UsePercentagesCheckBox.CheckedChanged += UsePercentagesCheckBox_CheckedChanged;
        }

        // Shutdown
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If user presses the X (exit) button
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;

                // Stop the window highlighter
                windowHighlighter?.StopHighlighting();

                // Hide form and only show the notify icon
                this.Hide();
                this.ShowInTaskbar = false;
                this.WindowState = FormWindowState.Minimized;
            }
            else
                CloseApp();
        }

        private void CloseApp()
        {
            try
            {
                // Save any pending changes before hiding
                windowManager.SaveChanges();

                // Stop any active highlighting
                windowHighlighter.StopHighlighting();

                // Close all quick layout forms
                if (quickLayoutManager != null)
                {
                    quickLayoutManager.CloseAll();
                }

                // Clean up notify icon (make invisible first to prevent ghost icons)
                if (notify_icon != null)
                {
                    notify_icon.Visible = false;
                    notify_icon.Icon = null;
                    notify_icon.Dispose();
                }

                // Stop and dispose of timers
                if (refreshTimer != null)
                {
                    refreshTimer.Stop();
                    refreshTimer.Dispose();
                }

                // Dispose of highlighter
                if (windowHighlighter != null)
                {
                    windowHighlighter.Dispose();
                }

                // Unhook the FormClosing event to prevent cancellation
                this.FormClosing -= Form1_FormClosing;

                // Force application to exit
                Application.Exit();
                Environment.Exit(0);
            }
            catch (Exception)
            {
                Environment.Exit(1); // Force exit
            }
        }
        #endregion


        #region Notify Icon
        // Notify Icon
        private void CreateNotifyIcon()
        {
            // Create the notify icon
            notify_icon = new NotifyIcon();
            notify_icon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            notify_icon.Text = "Save Win Size/Pos"; // Set the tooltip text
            notify_icon.Visible = true;

            notify_icon.MouseDoubleClick += (sender, e) =>
            {
                if (this.WindowState == FormWindowState.Minimized)
                {
                    this.WindowState = FormWindowState.Normal;
                    this.Focus();
                    this.ShowInTaskbar = true;
                    Thread.Sleep(250); // Slight delay to prevent a bunch of flickering as form reloads
                    this.Show();
                }
                else if (this.WindowState != FormWindowState.Minimized)
                    this.WindowState = FormWindowState.Minimized;
            };

            // Add a context menu to the notify icon
            ContextMenuStrip contextMenu = new ContextMenuStrip();

            // Create menu items with the desired text and image size
            int imageSize = 24;
            contextMenu.Items.Add(CreateMenuItem("Show Gui", Properties.Resources.pin, OnShowGui, imageSize));
            contextMenu.Items.Add(CreateMenuItem("Start Auto Moving", Properties.Resources.play_button, OnStartAuto, imageSize));
            contextMenu.Items.Add(CreateMenuItem("Stop Auto Moving", Properties.Resources.stop_button, OnStopAuto, imageSize));

            // Add a separator
            contextMenu.Items.Add(new ToolStripSeparator());

            // Add Capture Current Layout menu item
            contextMenu.Items.Add(CreateMenuItem("Capture Current Layout", Properties.Resources.refresh_blue_arrows, OnCaptureCurrentLayout, imageSize));

            // Add Create Quick Layout menu item
            contextMenu.Items.Add(CreateMenuItem("Create Quick Layout", Properties.Resources.magic_wand, OnCreateQuickLayout, imageSize));

            // Add profile switcher submenu
            ToolStripMenuItem profileSwitcherMenu = new ToolStripMenuItem("Switch Profile");
            profileSwitcherMenu.Image = new Bitmap(Properties.Resources.redo, new Size(imageSize, imageSize));
            profileSwitcherMenu.Font = new Font("Arial", 10, FontStyle.Regular);

            // We'll populate this submenu dynamically when it's opened
            profileSwitcherMenu.DropDownOpening += ProfileSwitcherMenu_DropDownOpening;

            contextMenu.Items.Add(profileSwitcherMenu);

            // Add settings menu item
            contextMenu.Items.Add(CreateMenuItem("Settings", Properties.Resources.settings, OnOpenSettings, imageSize));

            // Add a separator before Exit
            contextMenu.Items.Add(new ToolStripSeparator());

            contextMenu.Items.Add(CreateMenuItem("Exit", Properties.Resources.exit_button, OnExit, imageSize));

            // Set the image scaling to None to prevent automatic resizing
            contextMenu.ImageScalingSize = new Size(imageSize, imageSize); // Set the desired size

            notify_icon.ContextMenuStrip = contextMenu;
        }

        private ToolStripMenuItem CreateMenuItem(string text, Image image, EventHandler onClick, int imageSize)
        {
            var resizedImage = new Bitmap(image, new Size(imageSize, imageSize));
            var item = new ToolStripMenuItem(text, resizedImage, onClick);
            item.Font = new Font("Arial", 10, FontStyle.Regular);
            return item;
        }

        private void OnExit(object? sender, EventArgs e)
        {
            // Force application to close
            CloseApp();
        }

        private void OnStopAuto(object? sender, EventArgs e)
        {
            refreshTimer.Stop();
        }

        private void OnStartAuto(object? sender, EventArgs e)
        {
            refreshTimer.Start();
        }

        private void OnShowGui(object? sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.Show();
            this.Focus();
            this.ShowInTaskbar = true;
        }

        private void OnOpenSettings(object? sender, EventArgs e)
        {
            OpenSettingsForm();
        }

        private void ProfileSwitcherMenu_DropDownOpening(object sender, EventArgs e)
        {
            // Get the profile switcher menu
            ToolStripMenuItem profileSwitcherMenu = sender as ToolStripMenuItem;
            if (profileSwitcherMenu == null) return;

            // Clear existing items
            profileSwitcherMenu.DropDownItems.Clear();

            // Get all profile names
            var profileNames = windowManager.GetAllProfileNames();

            // Get current profile index
            int currentProfileIndex = windowManager.GetCurrentProfileIndex();

            // Add each profile to the submenu
            for (int i = 0; i < profileNames.Count; i++)
            {
                string profileName = profileNames[i];
                var profileMenuItem = new ToolStripMenuItem(profileName);

                // Check the current profile
                profileMenuItem.Checked = (i == currentProfileIndex);

                // Set font to bold for current profile
                profileMenuItem.Font = new Font("Arial", 10,
                    (i == currentProfileIndex) ? FontStyle.Bold : FontStyle.Regular);

                // Set the profile index as the Tag property
                profileMenuItem.Tag = i;

                // Add click handler
                profileMenuItem.Click += ProfileMenuItem_Click;

                // Add to submenu
                profileSwitcherMenu.DropDownItems.Add(profileMenuItem);
            }
        }

        private void ProfileMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem == null || menuItem.Tag == null) return;

            // Get profile index from Tag
            int profileIndex = (int)menuItem.Tag;

            // Switch to the selected profile
            windowManager.SwitchToProfile(profileIndex);

            // Update UI if the main form is visible
            if (this.Visible)
            {
                // Update profile combobox selection
                if (profileComboBox.SelectedIndex != profileIndex)
                {
                    profileComboBox.SelectedIndex = profileIndex;
                }
                else
                {
                    // If already selected, manually refresh the window list
                    RefreshSavedWindowsListBox();
                }
            }
        }

        private void OnCaptureCurrentLayout(object sender, EventArgs e)
        {
            // Check skip confirmation setting from AppSettings
            bool skipConfirmation = AppSettings.Load(Constants.AppSettingsConstants.SkipConfirmationKey) == "true";

            // Ask for confirmation if not configured to skip
            if (!skipConfirmation)
            {
                DialogResult result = MessageBox.Show(
                    "This will replace all windows in the current profile with the current window layout. Continue?",
                    "Confirm Replace",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes) return;
            }

            // Get current profile index
            int profileIndex = windowManager.GetCurrentProfileIndex();

            // Clear existing windows in the current profile
            windowManager.ClearCurrentProfileWindows();

            // Make sure the ignore list is up-to-date
            ignoreListManager.LoadIgnoreList();

            // Get all windows for layout capture using window manager
            var windowsForCapture = windowManager.GetVisibleRunningApps();

            // Add each window to current profile
            foreach (var window in windowsForCapture)
            {
                // Set each window's Auto Position to true
                window.AutoPosition = true;
                windowManager.AddOrUpdateWindow(window);
            }

            // Save changes explicitly to ensure they're persisted
            windowManager.SaveChanges();

            // Update UI if the main form is visible
            if (this.Visible)
            {
                // Update the profile combobox to show the current profile
                if (profileComboBox.SelectedIndex != profileIndex)
                {
                    profileComboBox.SelectedIndex = profileIndex;
                }
                RefreshSavedWindowsListBox();
            }
        }

        private void OnCreateQuickLayout(object sender, EventArgs e)
        {
            // Create a quick layout form
            var form = quickLayoutManager.CreateQuickLayoutForm();

            // Form will automatically minimize itself when shown
        }

        private void CreateQuickLayoutButton_Click(object sender, EventArgs e)
        {
            // Create a quick layout form
            var form = quickLayoutManager.CreateQuickLayoutForm();

            // Form will automatically minimize itself when shown
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
            // Get the current window
            var window = GetWindowFromGui();

            // Save window to current profile
            windowManager.AddOrUpdateWindow(window);

            RefreshSavedWindowsListBox();
        }

        private void Restore_Click(object sender, EventArgs e)
        {
            var window = GetWindowFromGui();

            if (!window.IsValid()) return;

            // Get the saved window from current profile
            if (window.Id > 0)
            {
                var saved = windowManager.GetWindowById(window.Id);
                if (saved.IsValid())
                {
                    windowManager.UpdateWindowPositionAndSize(saved);
                }
            }

            // Restore the window
            windowManager.RestoreWindow(window);

            // Update the UI
            SetWindowGui(window);
        }

        private void RestoreAll_Click(object sender, EventArgs e)
        {
            windowManager.RestoreAllWindows();
        }

        private void IgnoreButton_Click(object sender, EventArgs e)
        {
            var ignoreForm = new IgnoreForm(ignoreListManager);
            var result = ignoreForm.ShowDialog();

            // Refresh the UI to reflect any ignore list changes
            UpdateAllRunningAppsTask().ConfigureAwait(false);
        }

        private void RefreshWindowButton_Click(object sender, EventArgs e)
        {
            Window window = GetWindowFromGui();
            if (!window.IsValid()) return;

            // Get current window position and size using the window manager
            window.WindowPosAndSize = windowManager.GetWindowPositionAndSize(window);
            SetWindowGui(window);
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            UpdateAllRunningAppsTask().ConfigureAwait(false);
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            OpenSettingsForm();
        }

        private void ClearAllButton_Click(object sender, EventArgs e)
        {
            // Ask for confirmation
            DialogResult result = MessageBox.Show(
                "Are you sure you want to clear all saved windows in the current profile?",
                "Confirm Clear All",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            // Clear all windows in the current profile
            windowManager.ClearCurrentProfileWindows();

            // Refresh the UI
            RefreshSavedWindowsListBox();
            ClearWindowGUI();
        }

        // ListBoxes
        private void AppsSaved_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Deselect any running apps when a saved app is selected
            AllRunningApps.SelectedIndex = -1;

            if (AppsSaved.SelectedIndex == -1)
            {
                ClearWindowGUI();
                return;
            }

            // Get selected window directly from the listbox
            Window window = (Window)AppsSaved.SelectedItem;

            if (!window.IsValid())
            {
                MessageBox.Show("Selected window is not valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Fill GUI with window data using the existing helper method
            SetWindowGui(window);

            // Enable form controls
            SetWindowGUIState(true);

            // Try to highlight the window if it's currently running
            if (window.hWnd != IntPtr.Zero)
            {
                HighlightWindow(window.hWnd);
            }
        }

        private void AllRunningApps_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AllRunningApps.SelectedIndex == -1) return;

            ClearWindowGUI();

            try
            {
                // All items in AllRunningApps should always be Window objects
                if (!(AllRunningApps.SelectedItem is Window window))
                {
                    return;
                }

                // Get current window position and size
                window.WindowPosAndSize = windowManager.GetWindowPositionAndSize(window);

                // Fill GUI with window data using the helper method
                SetWindowGui(window);

                // Enable form controls
                SetWindowGUIState(true);

                // Highlight the window
                HighlightWindow(window.hWnd);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing selected window: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private void AppsSaved_KeyDown(object sender, KeyEventArgs e)
        {
            int selectedIndex = AppsSaved.SelectedIndex;

            if (e.KeyCode == Keys.Delete && selectedIndex > -1)
            {
                // Get the selected window directly from the list item
                Window selectedWindow = (Window)AppsSaved.SelectedItem;

                // Remove the window from the saved windows list
                if (windowManager.RemoveWindow(selectedWindow))
                {
                    // Refresh the saved windows list
                    RefreshSavedWindowsListBox();

                    // Clear the window GUI
                    ClearWindowGUI();
                }
            }
        }


        // ComboBoxes
        private void profileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loading) return;

            AppsSaved.SelectedIndex = -1;
            ClearWindowGUI();

            // Get selected profile index
            int profileIndex = profileComboBox.SelectedIndex;

            // Switch to the selected profile
            windowManager.SwitchToProfile(profileIndex);

            // Update UI
            RefreshSavedWindowsListBox();
        }


        // CheckBoxes
        private void AutoPosition_CheckedChanged(object sender, EventArgs e)
        {
            if (loading || (AppsSaved.SelectedItems.Count == 0 && AllRunningApps.SelectedItems.Count == 0))
                return;

            Window window = GetWindowFromGui();

            if (!window.IsValid())
                return;

            // Save the window with updated auto position flag
            window.AutoPosition = AutoPosition.Checked;
            windowManager.AddOrUpdateWindow(window);
        }

        private void KeepWindowOnTop_CheckedChanged(object sender, EventArgs e)
        {
            // Skip processing during loading or when nothing is selected
            if (loading) return;

            // Get window from GUI
            Window window = GetWindowFromGui();

            if (!window.IsValid())
                return;

            // Skip for file explorer windows as we can't use keep on top with them
            if (window.IsFileExplorer)
            {
                window.KeepOnTop = false;
                KeepWindowOnTop.Checked = false;
                return;
            }

            // Set the keep on top flag on the window object
            window.KeepOnTop = KeepWindowOnTop.Checked;

            // Apply keep on top setting to actual window
            windowManager.SetWindowAlwaysOnTop(window, window.KeepOnTop);

            // Save the window
            windowManager.AddOrUpdateWindow(window);
        }

        private void UsePercentagesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (loading) return;

            Window window = GetWindowFromGui();
            if (!window.IsValid()) return;

            // Get screen dimensions
            var (screenWidth, screenHeight) = windowManager.GetScreenDimensions();

            // Update the window's percentage setting
            window.UsePercentages = UsePercentagesCheckBox.Checked;
            window.WindowPosAndSize.IsPercentageBased = UsePercentagesCheckBox.Checked;

            // Convert values as needed
            if (UsePercentagesCheckBox.Checked)
            {
                // Convert current pixel values to percentages for display
                window.WindowPosAndSize.ConvertToPercentages(screenWidth, screenHeight);
            }
            else
            {
                // Convert current percentage values to pixels for display
                window.WindowPosAndSize.ConvertToPixels(screenWidth, screenHeight);
            }

            // Update the UI with the new values
            SetWindowGui(window);

            // Save the changes
            windowManager.AddOrUpdateWindow(window);
        }


        // TextBoxes
        private void WindowDisplayName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Do nothing if nothing selected
                if (AppsSaved.SelectedIndex == -1) return;

                var newNickname = WindowDisplayName.Text;

                // Get the selected window object
                Window selectedWindow = AppsSaved.SelectedItem as Window;
                if (selectedWindow != null)
                {
                    // Update the DisplayName of the Window object
                    selectedWindow.DisplayName = newNickname;

                    // Save the updated window
                    windowManager.AddOrUpdateWindow(selectedWindow);

                    // Update UI
                    var currentIndex = AppsSaved.SelectedItem;
                    RefreshSavedWindowsListBox();
                    AppsSaved.SelectedItem = currentIndex;
                }
            }
        }

        private void WindowWidth_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && AppsSaved.SelectedIndex != -1)
            {
                Save.PerformClick();
            }
        }

        private void WindowPosX_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && AppsSaved.SelectedIndex != -1)
            {
                Save.PerformClick();
            }
        }

        private void WindowPosY_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && AppsSaved.SelectedIndex != -1)
            {
                Save.PerformClick();
            }
        }

        private void WindowHeight_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && AppsSaved.SelectedIndex != -1)
            {
                Save.PerformClick();
            }
        }
        #endregion


        #region Helpers
        // UI Changes
        private void UpdateRunningApps()
        {
            AllRunningApps.Items.Clear();

            var taskbarApps = windowManager.GetAllRunningApps();

            // Add each application to the listbox
            foreach (var app in taskbarApps)
            {
                AllRunningApps.Items.Add(app);
            }
        }

        private Window GetWindowFromGui()
        {
            Window window;

            // Try to parse window ID
            if (int.TryParse(WindowId.Text, out int id) && id > 0)
            {
                // Try to get existing window
                window = windowManager.GetWindowById(id);
                if (window.IsValid())
                {
                    // Update existing window with UI values
                    UpdateWindowFromUI(window);
                    return window;
                }
            }

            // Create a new window
            window = new Window();
            UpdateWindowFromUI(window);

            return window;
        }

        private void UpdateWindowFromUI(Window window)
        {
            if (IntPtr.TryParse(this.hWnd.Text, out var handle))
                window.hWnd = handle;

            window.ProcessName = ProcessName.Text;
            window.IsFileExplorer = window.ProcessName == Constants.ProcessNames.FileExplorer;

            // Update DisplayName
            if (!string.IsNullOrWhiteSpace(WindowDisplayName.Text))
                window.DisplayName = WindowDisplayName.Text;

            window.TitleName = WindowTitle.Text;

            // Parse values removing any % sign
            string xText = WindowPosX.Text.Replace("%", "");
            string yText = WindowPosY.Text.Replace("%", "");
            string widthText = WindowWidth.Text.Replace("%", "");
            string heightText = WindowHeight.Text.Replace("%", "");

            window.WindowPosAndSize.X = int.TryParse(xText, out int X) ? X : 0;
            window.WindowPosAndSize.Y = int.TryParse(yText, out int Y) ? Y : 0;
            window.WindowPosAndSize.Height = int.TryParse(heightText, out int Height) ? Height : 0;
            window.WindowPosAndSize.Width = int.TryParse(widthText, out int Width) ? Width : 0;
            window.WindowPosAndSize.IsPercentageBased = UsePercentagesCheckBox.Checked;
            window.UsePercentages = UsePercentagesCheckBox.Checked;

            window.AutoPosition = AutoPosition.Checked;
            window.KeepOnTop = KeepWindowOnTop.Checked;
        }

        private void SetWindowGui(Window window)
        {
            loading = true;

            if (window == null || !window.IsValid())
            {
                ClearWindowGUI();
                return;
            }

            WindowId.Text = window.Id.ToString();

            if (window.hWnd != IntPtr.Zero)
                this.hWnd.Text = window.hWnd.ToString();

            ProcessName.Text = !string.IsNullOrWhiteSpace(window.ProcessName)
                ? window.ProcessName
                : "";

            WindowDisplayName.Text = !string.IsNullOrWhiteSpace(window.DisplayName)
                ? window.DisplayName
                : window.TitleName;

            WindowTitle.Text = !string.IsNullOrWhiteSpace(window.TitleName)
                ? window.TitleName
                : "";

            // Adjust the label's size to fit the wrapped text:
            WindowTitle.Size = new System.Drawing.Size(WindowTitle.MaximumSize.Width,
                TextRenderer.MeasureText(WindowTitle.Text, WindowTitle.Font, WindowTitle.MaximumSize).Height);

            AutoPosition.Checked = window.AutoPosition;
            KeepWindowOnTop.Checked = window.KeepOnTop;

            if (window.WindowPosAndSize == null)
            {
                return;
            }

            string suffix = window.UsePercentages ? "%" : "";
            WindowPosX.Text = window.WindowPosAndSize.X.ToString() + suffix;
            WindowPosY.Text = window.WindowPosAndSize.Y.ToString() + suffix;
            WindowWidth.Text = window.WindowPosAndSize.Width.ToString() + suffix;
            WindowHeight.Text = window.WindowPosAndSize.Height.ToString() + suffix;

            // Set the percentage checkbox
            UsePercentagesCheckBox.Checked = window.UsePercentages;

            loading = false;
        }

        private void RefreshSavedWindowsListBox()
        {
            AppsSaved.Items.Clear();

            // Get all windows from current profile
            var windows = windowManager.GetCurrentProfileWindows();

            // Add each window object to the listbox
            foreach (var window in windows)
            {
                AppsSaved.Items.Add(window);
            }
        }

        private void InitializeProfiles()
        {
            // Clear existing items
            profileComboBox.Items.Clear();

            // Add profile names
            var profileNames = windowManager.GetAllProfileNames();
            foreach (var name in profileNames)
            {
                profileComboBox.Items.Add(name);
            }
        }

        private void InitializeHighlighter()
        {
            windowHighlighter = new WindowHighlighter();
            windowHighlighter.BorderWidth = Constants.Defaults.HighlighterBorderThickness;
            windowHighlighter.InnerBorderThickness = Constants.Defaults.InnerHighlighterBorderThickness;
            windowHighlighter.BorderColor = Color.Red;
            windowHighlighter.InnerBorderColor = Color.Red;
            windowHighlighter.HighlightDurationMs = Constants.Defaults.HighlighterDurationMs;

            // Check if there's a custom color in AppSettings
            string colorHex = AppSettings.Load(Constants.AppSettingsConstants.HighlighterColorKey);
            if (!string.IsNullOrEmpty(colorHex))
            {
                try
                {
                    Color customColor = ColorTranslator.FromHtml(colorHex);
                    windowHighlighter.BorderColor = customColor;
                    windowHighlighter.InnerBorderColor = customColor;
                }
                catch (Exception)
                {
                    // Silently handle exception
                }
            }
        }

        private Task UpdateAllRunningAppsTask()
        {
            return Task.Run(() =>
            {
                try
                {
                    // Clear the listbox safely
                    AllRunningApps.ClearItemsThreadSafe();

                    // Get running applications through window manager (which already includes filtering by the ignore list)
                    var allApps = windowManager.GetAllRunningApps();

                    // Add each application to the listbox safely
                    foreach (var app in allApps)
                    {
                        AllRunningApps.AddItemThreadSafe(app);
                    }
                }
                catch (Exception)
                {
                    // Silently handle exception
                }
            });
        }

        private void HighlightWindow(IntPtr hWnd)
        {
            try
            {
                if (windowHighlighter != null && hWnd != IntPtr.Zero)
                {
                    // Bring window to front first
                    windowManager.SetForegroundWindow(hWnd);

                    // Highlight the window
                    windowHighlighter.HighlightWindow(hWnd);

                    // Return focus to our app after a brief delay
                    Task.Delay(300).ContinueWith(_ =>
                    {
                        this.Invoke(new Action(() =>
                        {
                            windowManager.SetForegroundWindow(this.Handle);
                        }));
                    });
                }
            }
            catch (Exception)
            {
                // Silently handle exception
            }
        }

        private void SetWindowGUIState(bool enabled)
        {
            // Enable or disable window controls
            WindowPosX.Enabled = enabled;
            WindowPosY.Enabled = enabled;
            WindowWidth.Enabled = enabled;
            WindowHeight.Enabled = enabled;
            WindowDisplayName.Enabled = enabled;
            ProcessName.Enabled = enabled;
            WindowTitle.Enabled = enabled;
            AutoPosition.Enabled = enabled;
        }

        private void ClearWindowGUI()
        {
            // Clear window fields
            WindowPosX.Text = "";
            WindowPosY.Text = "";
            WindowWidth.Text = "";
            WindowHeight.Text = "";
            WindowDisplayName.Text = "";
            ProcessName.Text = "";
            WindowTitle.Text = "";
            AutoPosition.Checked = false;
        }

        private void OpenSettingsForm()
        {
            // Create and show settings form
            var settingsForm = new SettingsForm();

            // Subscribe to settings changed event
            settingsForm.SettingsChanged += SettingsForm_SettingsChanged;

            // Show the form as a dialog to prevent interaction with the main form until closed
            settingsForm.ShowDialog(this);
        }

        private void SettingsForm_SettingsChanged(object sender, EventArgs e)
        {
            // Update timer values from settings when settings change
            if (int.TryParse(AppSettings.Load(Constants.AppSettingsConstants.RefreshTimeKey), out int refreshTime))
            {
                minutes = refreshTime - 1;
                seconds = 60;
            }
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();

                // Clean up the window highlighter
                if (windowHighlighter != null)
                {
                    windowHighlighter.StopHighlighting();
                    windowHighlighter.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}

