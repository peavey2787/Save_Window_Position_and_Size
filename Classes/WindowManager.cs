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
        private Dictionary<int, List<Window>> quickLayouts = new Dictionary<int, List<Window>>();

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

            // Load quick layouts
            LoadQuickLayouts();
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

            // Check if we should minimize other windows
            bool minimizeOtherWindows = AppSettings.Load(Constants.AppSettingsConstants.MinimizeOtherWindowsKey) == "true";

            if (minimizeOtherWindows)
            {
                // Get all running application windows
                var runningWindows = GetAllRunningApps();

                // Minimize all running windows first
                foreach (var window in runningWindows)
                {
                    IntPtr hWnd = GetWindowHandle(window);
                    if (hWnd != IntPtr.Zero)
                    {
                        InteractWithWindow.MinimizeWindow(hWnd);
                    }
                }
            }

            // Get the windows for the selected profile
            var profileWindows = profileCollection.SelectedProfile.Windows;

            // Restore only the windows in the profile
            foreach (var window in profileWindows)
            {
                if (window.AutoPosition)
                {
                    RestoreWindow(window);
                }
            }

            return profileCollection.SelectedProfile.Windows;
        }

        /// <summary>
        /// Sets the current profile index without restoring windows
        /// </summary>
        public void SwitchToProfileWithoutRestore(int profileIndex)
        {
            if (profileIndex < 0 || profileIndex >= profileCollection.Profiles.Count)
                return;

            // Update current profile index
            profileCollection.SelectedProfileIndex = profileIndex;
            AppSettings<int>.Save(Constants.AppSettingsConstants.SelectedProfileKey, profileIndex);
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

            // For File Explorer windows, check if the window already exists based on title only
            // For regular windows, check based on both title and handle
            Window existingWindow = null;
            if (window.IsFileExplorer)
            {
                existingWindow = profileCollection.SelectedProfile.Windows
                    .FirstOrDefault(w => w.IsFileExplorer && w.TitleName == window.TitleName);
            }
            else
            {
                existingWindow = profileCollection.SelectedProfile.Windows
                    .FirstOrDefault(w => w.TitleName == window.TitleName && w.hWnd == window.hWnd);
            }

            if (existingWindow != null)
            {
                // Update existing window by removing it first
                profileCollection.SelectedProfile.Windows.Remove(existingWindow);

                // Preserve the existing window ID for continuity
                window.Id = existingWindow.Id;
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
        /// Clears all windows from the current profile
        /// </summary>
        public bool ClearCurrentProfileWindows()
        {
            if (profileCollection.SelectedProfile == null)
                return false;

            // Clear all windows in the current profile
            profileCollection.SelectedProfile.Windows.Clear();

            // Save changes
            SaveAllProfiles();
            return true;
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
                {
                    continue;
                }

                RestoreWindow(window);
            }
        }

        /// <summary>
        /// Restores a specific window to its saved position and size
        /// </summary>
        public void RestoreWindow(Window window)
        {
            if (window == null || !window.IsValid())
            {
                return;
            }

            // Get window handle
            IntPtr hWnd = GetWindowHandle(window);
            if (hWnd == IntPtr.Zero)
            {
                return;
            }

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
            {
                return;
            }

            // Get a copy of the position and size values
            WindowPosAndSize posAndSize = window.WindowPosAndSize;

            // If the position is percentage-based, convert to pixels first
            if (posAndSize.IsPercentageBased)
            {
                var (screenWidth, screenHeight) = GetScreenDimensions();

                // Create a copy with pixel values
                posAndSize = new WindowPosAndSize
                {
                    X = (int)(posAndSize.X * screenWidth / 100),
                    Y = (int)(posAndSize.Y * screenHeight / 100),
                    Width = (int)(posAndSize.Width * screenWidth / 100),
                    Height = (int)(posAndSize.Height * screenHeight / 100),
                    IsPercentageBased = false
                };

            }

            // Get the window handle
            IntPtr hWnd = GetWindowHandle(window);
            if (hWnd == IntPtr.Zero)
            {
                return;
            }

            // Try to set the window position and size
            try
            {
                InteractWithWindow.SetWindowPositionAndSize(window, posAndSize);
            }
            catch (Exception ex)
            {
            }
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

                // Skip entries ending with "(explorer)" as these are typically drives without actual windows
                if (title.EndsWith("(explorer)", StringComparison.OrdinalIgnoreCase))
                    continue;

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

        #region Quick Layout Methods

        /// <summary>
        /// Saves a quick layout with the given index and windows
        /// </summary>
        public void SaveQuickLayout(int index, List<Window> windows)
        {
            // Create deep copies of windows to ensure they're not modified elsewhere
            var windowsCopy = windows.Select(w => w.Clone()).ToList();

            // Save to dictionary
            quickLayouts[index] = windowsCopy;

            // Persist changes
            SaveQuickLayouts();
        }

        /// <summary>
        /// Restores all windows in a quick layout
        /// </summary>
        public void RestoreQuickLayout(int index)
        {
            if (!quickLayouts.TryGetValue(index, out var windows))
                return;

            foreach (var window in windows)
            {
                if (!window.AutoPosition)
                    continue;

                try
                {
                    RestoreWindow(window);
                }
                catch
                {
                    // Continue with next window if there's an error
                    continue;
                }
            }
        }

        /// <summary>
        /// Gets all windows from a quick layout
        /// </summary>
        public List<Window> GetQuickLayout(int index)
        {
            if (quickLayouts.TryGetValue(index, out var windows))
                return new List<Window>(windows);

            return new List<Window>();
        }

        private void LoadQuickLayouts()
        {
            string savedLayouts = AppSettings.Load(Constants.AppSettingsConstants.QuickLayoutsKey);

            if (!string.IsNullOrEmpty(savedLayouts))
            {
                try
                {
                    quickLayouts = JsonConvert.DeserializeObject<Dictionary<int, List<Window>>>(savedLayouts)
                        ?? new Dictionary<int, List<Window>>();
                }
                catch
                {
                    quickLayouts = new Dictionary<int, List<Window>>();
                }
            }
        }

        private void SaveQuickLayouts()
        {
            try
            {
                string json = JsonConvert.SerializeObject(quickLayouts);
                AppSettings.Save(Constants.AppSettingsConstants.QuickLayoutsKey, json);
            }
            catch
            {
                // Silently handle exceptions
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the window handle using window title or process name
        /// </summary>
        private IntPtr GetWindowHandle(Window window)
        {
            // For File Explorer windows, always search by title
            if (window.IsFileExplorer)
                return InteractWithWindow.FindWindowByTitle(window.TitleName);

            // For regular windows, first try the stored handle (it might still be valid)
            if (window.hWnd != IntPtr.Zero)
            {
                // Check if the handle is still valid
                if (InteractWithWindow.IsWindow(window.hWnd))
                {
                    System.Diagnostics.Debug.WriteLine($"Using existing handle for {window.DisplayName}: {window.hWnd}");
                    return window.hWnd;
                }
            }

            // If the stored handle isn't valid, try to find the window by title and process
            IntPtr newHandle = InteractWithWindow.GetWindowHandleByWindowAndProcess(window, ignoreListManager);

            if (newHandle != IntPtr.Zero)
            {
                // Update the window's handle with the new one we found
                window.hWnd = newHandle;
                System.Diagnostics.Debug.WriteLine($"Found new handle for {window.DisplayName}: {newHandle}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Could not find handle for {window.DisplayName} using title or process");
            }

            return newHandle;
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