using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;

namespace Save_Window_Position_and_Size.Classes
{
    /// <summary>
    /// Manages window saving, loading and profile management
    /// This is the central class for all window-related operations
    /// </summary>
    internal class WindowManager
    {
        private ProfileCollection profileCollection;
        private IgnoreListManager ignoreListManager;

        public WindowManager(IgnoreListManager ignoreListManager)
        {
            this.ignoreListManager = ignoreListManager;

            // Initialize profiles
            profileCollection = new ProfileCollection(Constants.Defaults.MaxProfiles);

            // Load profiles from settings
            LoadAllProfiles();

            // Try to load the saved profile index, or default to 0
            int savedIndex = AppSettings<int>.Load(Constants.AppSettingsConstants.SelectedProfileKey);

            // Ensure the profile index is valid
            if (savedIndex >= 0 && savedIndex < profileCollection.Profiles.Count)
            {
                profileCollection.SelectedProfileIndex = savedIndex;
            }
        }

        #region Public Profile Management Methods

        /// <summary>
        /// Gets all windows in the current profile
        /// </summary>
        public List<Window> GetCurrentProfileWindows()
        {
            return profileCollection.SelectedProfile?.Windows ?? new List<Window>();
        }

        /// <summary>
        /// Gets the current profile index
        /// </summary>
        public int GetCurrentProfileIndex()
        {
            return profileCollection.SelectedProfileIndex;
        }

        /// <summary>
        /// Sets the current profile index and loads its windows
        /// </summary>
        public List<Window> SwitchToProfile(int profileIndex)
        {
            if (profileIndex < 0 || profileIndex >= profileCollection.Profiles.Count)
                return new List<Window>();

            // Update current profile index
            profileCollection.SelectedProfileIndex = profileIndex;
            AppSettings<int>.Save(Constants.AppSettingsConstants.SelectedProfileKey, profileIndex);

            return profileCollection.SelectedProfile.Windows;
        }

        /// <summary>
        /// Gets all profile names
        /// </summary>
        public List<string> GetAllProfileNames()
        {
            return profileCollection.Profiles.Select(p => p.Name).ToList();
        }

        /// <summary>
        /// Renames the current profile
        /// </summary>
        public bool RenameCurrentProfile(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                return false;

            profileCollection.SelectedProfile.Name = newName;
            SaveAllProfiles();
            return true;
        }

        #endregion

        #region Public Window Management Methods

        /// <summary>
        /// Adds or updates a window in the current profile
        /// </summary>
        public bool AddOrUpdateWindow(Window window)
        {
            if (window == null || !window.IsValid() || profileCollection.SelectedProfile == null)
                return false;

            // Check if the window already exists in the current profile
            var existingWindow = profileCollection.SelectedProfile.Windows
                .FirstOrDefault(w => w.TitleName == window.TitleName && w.hWnd == window.hWnd);

            if (existingWindow != null)
            {
                // Update existing window by removing it first
                profileCollection.SelectedProfile.Windows.Remove(existingWindow);
            }

            // Add the window to the current profile
            profileCollection.SelectedProfile.Windows.Add(window);
            SaveAllProfiles();
            return true;
        }

        /// <summary>
        /// Removes a window from the current profile
        /// </summary>
        public bool RemoveWindow(Window window)
        {
            if (window == null || !window.IsValid() || profileCollection.SelectedProfile == null)
                return false;

            // Try to remove the window from the current profile
            bool removed = profileCollection.SelectedProfile.Windows.Remove(window);
            if (removed)
            {
                SaveAllProfiles();
            }
            return removed;
        }

        /// <summary>
        /// Gets a window by ID from the current profile
        /// </summary>
        public Window GetWindowById(int id)
        {
            // Look for the window in the current profile's saved windows
            if (profileCollection.SelectedProfile == null)
                return new Window();

            return profileCollection.SelectedProfile.Windows.FirstOrDefault(w => w.Id == id) ?? new Window();
        }

        /// <summary>
        /// Restores all windows in the current profile
        /// </summary>
        public void RestoreAllWindows()
        {
            if (profileCollection.SelectedProfile == null)
                return;

            foreach (Window window in profileCollection.SelectedProfile.Windows)
            {
                if (!window.AutoPosition)
                    continue;

                RestoreWindow(window);
            }
        }

        /// <summary>
        /// Restores a specific window to its saved position and size
        /// </summary>
        public void RestoreWindow(Window window)
        {
            if (window == null || !window.IsValid())
                return;

            // Get window handle
            IntPtr hWnd = GetWindowHandle(window);
            if (hWnd == IntPtr.Zero)
                return;

            // Check if the window is minimized
            if (InteractWithWindow.IsIconic(hWnd) || InteractWithWindow.IsWindowMinimized(window))
            {
                // Restore the window from minimized state
                InteractWithWindow.ShowWindow(hWnd, 9); // SW_RESTORE = 9

                // Give the window time to restore
                System.Threading.Thread.Sleep(100);
            }

            // Set window position and size
            UpdateWindowPositionAndSize(window);

            // Set always on top if needed
            if (window.KeepOnTop)
            {
                SetWindowAlwaysOnTop(window, true);
            }
        }

        /// <summary>
        /// Updates the window position and size
        /// </summary>
        public void UpdateWindowPositionAndSize(Window window)
        {
            if (window == null || !window.IsValid())
                return;

            InteractWithWindow.SetWindowPositionAndSize(window, window.WindowPosAndSize);
        }

        /// <summary>
        /// Gets running applications with option to filter for only visible ones
        /// </summary>
        public List<Window> GetRunningApps(bool onlyVisible = false)
        {
            var windows = new List<Window>();

            // Get a list of window titles to ignore
            var ignoreList = ignoreListManager?.GetIgnoreList() ?? new List<string>();

            // Get all application windows using the exact original approach
            var appWindows = InteractWithWindow.GetApplicationWindows(onlyVisible, ignoreList);

            // Process each application window
            foreach (var kvp in appWindows)
            {
                IntPtr handle = kvp.Key;
                string title = kvp.Value;

                // Create a Window object
                Window window = new Window(handle, title, title);

                // Get process information
                try
                {
                    uint processId;
                    InteractWithWindow.GetWindowThreadProcessId(handle, out processId);
                    if (processId > 0)
                    {
                        Process process = Process.GetProcessById((int)processId);
                        window.ProcessName = process.ProcessName;

                        // Check if this is a file explorer window
                        if (process.ProcessName.Equals("explorer", StringComparison.OrdinalIgnoreCase) &&
                            (title.StartsWith("File Explorer", StringComparison.OrdinalIgnoreCase) ||
                            title.StartsWith("This PC", StringComparison.OrdinalIgnoreCase) ||
                            title.Contains(":\\") || title.Contains("Explorer")))
                        {
                            window.IsFileExplorer = true;
                        }
                    }
                }
                catch
                {
                    window.ProcessName = "Unknown";
                }

                // Get window position and size
                window.WindowPosAndSize = GetWindowPositionAndSize(window);

                windows.Add(window);
            }

            // Add file explorer windows from the special File Explorer handler
            var fileExplorerWindows = GetFileExplorerWindows();

            // Add them to the result if they're not already in the list
            foreach (var explorerWindow in fileExplorerWindows)
            {
                // Skip if already in the result (to avoid duplicates)
                if (windows.Any(w => w.TitleName == explorerWindow.TitleName))
                {
                    continue;
                }

                windows.Add(explorerWindow);
            }

            return windows;
        }

        /// <summary>
        /// Gets all visible running applications - convenience method that calls GetRunningApps(true)
        /// </summary>
        public List<Window> GetVisibleRunningApps()
        {
            return GetRunningApps(true);
        }

        /// <summary>
        /// Gets all running applications - convenience method that calls GetRunningApps(false)
        /// </summary>
        public List<Window> GetAllRunningApps()
        {
            return GetRunningApps(false);
        }

        /// <summary>
        /// Gets the File Explorer windows
        /// </summary>
        private List<Window> GetFileExplorerWindows()
        {
            var windows = new List<Window>();

            // Get file explorer windows from InteractWithWindow
            var fileExplorerWindows = InteractWithWindow.GetFileExplorerWindows();

            foreach (var explorerWindow in fileExplorerWindows)
            {
                var (handle, title, processName, isFileExplorer, position) = explorerWindow;

                // Create a window object
                Window window = new Window();
                window.IsFileExplorer = true;
                window.TitleName = title;
                window.DisplayName = Path.GetFileName(title.Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault() ?? "");
                window.ProcessName = processName;
                window.WindowPosAndSize = position;

                // Skip windows in the ignore list
                if (ignoreListManager != null && ignoreListManager.ShouldIgnoreWindow(window))
                {
                    continue;
                }

                windows.Add(window);
            }

            return windows;
        }

        /// <summary>
        /// Gets the current position and size of a window
        /// </summary>
        public WindowPosAndSize GetWindowPositionAndSize(Window window)
        {
            return InteractWithWindow.GetWindowPositionAndSize(window);
        }

        /// <summary>
        /// Sets the always-on-top state for a window
        /// </summary>
        public void SetWindowAlwaysOnTop(Window window, bool keepOnTop)
        {
            if (window == null || !window.IsValid())
                return;

            IntPtr hWnd = GetWindowHandle(window);

            if (hWnd == IntPtr.Zero)
                return;

            if (keepOnTop)
                InteractWithWindow.SetWindowAlwaysOnTop(hWnd);
            else
                InteractWithWindow.UnsetWindowAlwaysOnTop(hWnd);
        }

        /// <summary>
        /// Gets screen dimensions
        /// </summary>
        public (int Width, int Height) GetScreenDimensions()
        {
            return InteractWithWindow.GetScreenDimensions();
        }

        /// <summary>
        /// Sets the foreground window (brings window to front)
        /// </summary>
        public void SetForegroundWindow(IntPtr hWnd)
        {
            InteractWithWindow.SetForegroundWindow(hWnd);
        }

        /// <summary>
        /// Save any pending changes
        /// </summary>
        public void SaveChanges()
        {
            SaveAllProfiles();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the saved window position and size
        /// </summary>
        private WindowPosAndSize GetSavedWindowPosAndSize(Window window)
        {
            var windowPosAndSize = new WindowPosAndSize();

            var currentWindows = GetCurrentProfileWindows();

            foreach (var currentWindow in currentWindows)
            {
                if (currentWindow.Id == window.Id)
                    windowPosAndSize = window.WindowPosAndSize;
            }

            return windowPosAndSize;
        }

        /// <summary>
        /// Gets the window handle using window title or process name
        /// </summary>
        private IntPtr GetWindowHandle(Window window)
        {
            if (window.IsFileExplorer)
                return InteractWithWindow.FindWindowByTitle(window.TitleName);
            else
                return InteractWithWindow.GetWindowHandleByWindowAndProcess(window, ignoreListManager);
        }

        private void LoadAllProfiles()
        {
            try
            {
                // Use generic AppSettings to load the ProfileCollection directly
                var savedProfileCollection = AppSettings<ProfileCollection>.Load(Constants.AppSettingsConstants.SavedWindowsKey);

                if (savedProfileCollection != null && savedProfileCollection.Profiles != null &&
                    savedProfileCollection.Profiles.Count > 0)
                {
                    profileCollection = savedProfileCollection;

                    // Ensure we don't exceed the max profiles
                    while (profileCollection.Profiles.Count < Constants.Defaults.MaxProfiles)
                    {
                        int index = profileCollection.Profiles.Count + 1;
                        profileCollection.Profiles.Add(new Profile($"Profile {index}"));
                    }
                }
            }
            catch (Exception)
            {
                // Initialize with defaults in case of error
                profileCollection = new ProfileCollection(Constants.Defaults.MaxProfiles);
            }
        }

        private void SaveAllProfiles()
        {
            try
            {
                // Save the entire ProfileCollection directly
                AppSettings<ProfileCollection>.Save(Constants.AppSettingsConstants.SavedWindowsKey, profileCollection);
            }
            catch (Exception)
            {
                // Error saving profiles - silently handle
            }
        }
        #endregion
    }
}