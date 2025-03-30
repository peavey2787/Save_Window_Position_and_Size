using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Save_Window_Position_and_Size.Classes
{
    /// <summary>
    /// Manages window saving, loading and profile management
    /// This is the central class for all window-related operations
    /// </summary>
    internal class WindowManager
    {
        private InteractWithWindow _interactWithWindow = new InteractWithWindow();
        private ProfileCollection profileCollection;
        private Dictionary<int, List<Window>> quickLayouts = new Dictionary<int, List<Window>>();

        internal WindowManager()
        {
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



        #region internal Profile Management Methods

        /// <summary>
        /// Gets all windows in the current profile
        /// </summary>
        internal List<Window> GetCurrentProfileWindows()
        {
            return profileCollection.SelectedProfile?.Windows ?? new List<Window>();
        }

        /// <summary>
        /// Gets the current profile index
        /// </summary>
        internal int GetCurrentProfileIndex()
        {
            return profileCollection.SelectedProfileIndex;
        }

        /// <summary>
        /// Sets the current profile index and loads its windows
        /// </summary>
        internal List<Window> SwitchToProfileAndRestore(int profileIndex)
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
                MinimizeAllRunningApps();
            }

            // Get the windows for the selected profile
            var profileWindows = profileCollection.SelectedProfile.Windows;

            // Restore only the windows in the profile
            RestoreSelectedWindows(profileWindows);

            return profileWindows;
        }

        /// <summary>
        /// Sets the current profile index without restoring windows
        /// </summary>
        internal void SwitchToProfileWithoutRestore(int profileIndex)
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
        internal List<string> GetAllProfileNames()
        {
            return profileCollection.Profiles.Select(p => p.Name).ToList();
        }

        /// <summary>
        /// Renames the current profile
        /// </summary>
        internal bool RenameCurrentProfile(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                return false;

            profileCollection.SelectedProfile.Name = newName;
            SaveAllProfiles();
            return true;
        }

        /// <summary>
        /// Adds or updates a window in the current profile
        /// </summary>
        internal bool AddOrUpdateWindow(Window window)
        {
            if (window == null || !window.IsValid() || profileCollection.SelectedProfile == null
                || (window.WindowPosAndSize.Width == 0 && window.WindowPosAndSize.Height == 0
                && window.WindowPosAndSize.X == 0 && window.WindowPosAndSize.Y == 0) )
                return false;

            Window existingWindow = profileCollection.SelectedProfile.Windows
                    .FirstOrDefault(w => w.TitleName == window.TitleName && w.hWnd == window.hWnd);            

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
        internal bool RemoveWindow(Window window)
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

        internal WindowPosAndSize GetSavedWindowPosAndSize(Window window)
        {
            foreach(Window savedWindow in profileCollection.SelectedProfile.Windows)
            {
                if(savedWindow.IsSameWindowWithoutHandle(window))
                {
                    return savedWindow.WindowPosAndSize;
                }                
            }
            return new WindowPosAndSize();
        }
        #endregion


        #region internal Window Management Methods    
        internal async Task<IntPtr> GetCurrentWindowHandle(Window window)
        {
            return await _interactWithWindow.GetWindowHandle(window);
        }
        internal void SetForegroundWindow(IntPtr hWnd)
        {
            InteractWithWindow.SetForegroundWindow(hWnd);
        }
        internal async void SetWindowAlwaysOnTop(Window window)
        {
            window.hWnd = await _interactWithWindow.GetWindowHandle(window);
            if (window.KeepOnTop)
            {                
                InteractWithWindow.SetWindowAlwaysOnTop(window.hWnd);
            }
            else
            {
                InteractWithWindow.UnsetWindowAlwaysOnTop(window.hWnd);
            }
        }
        internal async void SetWindowPosAndSize(Window window, WindowPosAndSize windowPosAndSize)
        {
            window.hWnd = await _interactWithWindow.GetWindowHandle(window);
            InteractWithWindow.SetWindowPositionAndSize(window, windowPosAndSize);
        }
        internal WindowPosAndSize GetWindowPositionAndSize(Window window)
        {
             return InteractWithWindow.GetWindowPositionAndSize(window);
        }
        internal async Task<List<Window>> GetAllRunningApps()
        {
            return await _interactWithWindow.GetAllAppWindows(false);
        }
        internal async Task<List<Window>> GetVisibleRunningApps()
        {
            return await _interactWithWindow.GetAllAppWindows(true);
        }
        internal (int screenWidth, int screenHeight) GetScreenDimensions()
        {
            return InteractWithWindow.GetScreenDimensions();
        }
        /// <summary>
        /// Clears all windows from the current profile
        /// </summary>
        internal bool ClearCurrentProfileWindows()
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
        internal Window GetWindowById(int id)
        {
            // Look for the window in the current profile's saved windows
            if (profileCollection.SelectedProfile == null)
                return new Window();

            return profileCollection.SelectedProfile.Windows.FirstOrDefault(w => w.Id == id) ?? new Window();
        }

        /// <summary>
        /// Restores all windows in the current profile
        /// </summary>
        internal void RestoreAllWindows()
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
        internal async void RestoreWindow(Window window)
        {
            if (window == null || !window.IsValid())
            {
                return;
            }

            // Get updated window handle
            IntPtr hWnd = await _interactWithWindow.GetWindowHandle(window);            

            if (hWnd == IntPtr.Zero)
            {
                // Window is not running, check if we should start the application
                bool shouldStartApp = AppSettings.Load(Constants.AppSettingsConstants.StartAppsIfNotRunningKey) == "true";

                if (shouldStartApp && !string.IsNullOrEmpty(window.ProcessName))
                {
                    try
                    {
                        // Start the application
                        System.Diagnostics.Process.Start(window.ProcessName);

                        // Wait briefly for the application to start
                        System.Threading.Thread.Sleep(2000);

                        // Try to get the window handle again
                        hWnd = await _interactWithWindow.GetWindowHandle(window);
                    }
                    catch (Exception)
                    {
                        // Failed to start the application, continue with other windows
                        return;
                    }

                    // If we still don't have a handle, give up
                    if (hWnd == IntPtr.Zero)
                    {
                        return;
                    }
                }
                else
                {
                    // We don't want to start the application or don't have a process name
                    return;
                }
            }

            // Update window's hWnd
            window.hWnd = hWnd;

            // Check if the window is minimized
            if (InteractWithWindow.IsIconic(hWnd) || InteractWithWindow.IsWindowMinimized(window))
            {
                // Restore the window from minimized state
                InteractWithWindow.ShowWindow(hWnd, 9); // SW_RESTORE = 9

                // Give the window time to restore
                System.Threading.Thread.Sleep(100);
            }

            // Set window position and size            
            InteractWithWindow.SetWindowPositionAndSize(window, window.WindowPosAndSize);

            // Set always on top if needed
            SetWindowAlwaysOnTop(window);
        }

        /// <summary>
        /// Save any pending changes
        /// </summary>
        internal void SaveChanges()
        {
            SaveAllProfiles();
        }


        private async void MinimizeAllRunningApps()
        {
            // Get all running application windows
            var runningWindows = await GetAllRunningApps();

            // Minimize all running windows first
            foreach (var window in runningWindows)
            {
                if (window.hWnd != IntPtr.Zero)
                {
                    InteractWithWindow.MinimizeWindow(window.hWnd);
                }
            }
        }
        private void RestoreSelectedWindows(List<Window> windows)
        {
            if (profileCollection.SelectedProfile == null)
                return;

            foreach (Window window in windows)
            {
                if (!window.AutoPosition)
                {
                    continue;
                }

                RestoreWindow(window);
            }
        }
        #endregion



        #region Private Methods

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