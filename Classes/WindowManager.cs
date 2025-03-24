using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Save_Window_Position_and_Size.Classes
{
    /// <summary>
    /// Represents a profile containing a collection of windows
    /// </summary>
    internal class Profile
    {
        public string Name { get; set; }
        public List<Window> Windows { get; set; } = new List<Window>();

        public Profile(string name)
        {
            Name = name;
        }

        public Profile Clone()
        {
            Profile clone = new Profile(this.Name);
            foreach (Window window in this.Windows.Where(w => w.IsValid()))
            {
                clone.Windows.Add(window.Clone());
            }
            return clone;
        }
    }

    /// <summary>
    /// Collection of profiles with active profile tracking
    /// </summary>
    internal class ProfileCollection
    {
        public List<Profile> Profiles { get; set; } = new List<Profile>();
        public Profile SelectedProfile { get; set; }
        public int SelectedProfileIndex
        {
            get
            {
                return Profiles.IndexOf(SelectedProfile);
            }
            set
            {
                if (value >= 0 && value < Profiles.Count)
                {
                    SelectedProfile = Profiles[value];
                }
            }
        }

        public ProfileCollection(int maxProfiles)
        {
            // Initialize with default profiles
            for (int i = 0; i < maxProfiles; i++)
            {
                Profiles.Add(new Profile($"Profile {i + 1}"));
            }

            // Set first profile as selected by default
            SelectedProfile = Profiles.FirstOrDefault();
        }
    }

    /// <summary>
    /// Manages window saving, loading and profile management
    /// </summary>
    internal class WindowManager
    {
        private ProfileCollection profileCollection;
        private Random random;

        public WindowManager()
        {
            random = new Random();

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
        /// Gets a window by title from the current profile
        /// </summary>
        public Window GetWindowByTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title) || profileCollection.SelectedProfile == null)
                return new Window();

            // Look for the window in the current profile's saved windows
            return profileCollection.SelectedProfile.Windows.FirstOrDefault(w => w.TitleName == title) ?? new Window();
        }

        /// <summary>
        /// Gets a window by display name from the current profile
        /// </summary>
        public Window GetWindowByName(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName) || profileCollection.SelectedProfile == null)
                return new Window();

            // Look for the window in the current profile's saved windows by DisplayName
            return profileCollection.SelectedProfile.Windows.FirstOrDefault(w => w.DisplayName == displayName) ?? new Window();
        }

        /// <summary>
        /// Creates a new window with a random ID
        /// </summary>
        public Window CreateWindow()
        {
            return Window.CreateWithRandomId(random);
        }

        /// <summary>
        /// Restores all windows in the current profile
        /// </summary>
        public void RestoreAllWindows(IgnoreListManager ignoreListManager, List<Window> allWindows,  bool useAutoPositionFlag = true)
        {
            if (profileCollection.SelectedProfile == null)
                return;

            foreach (Window window in profileCollection.SelectedProfile.Windows)
            {
                if (useAutoPositionFlag && !window.AutoPosition)
                    continue;

                InteractWithWindow.RestoreWindow(window, allWindows);
            }
        }

        /// <summary>
        /// Saves any pending changes to the current profile
        /// </summary>
        public void SavePendingChanges()
        {
            SaveAllProfiles();
        }

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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading profiles: {ex.Message}");

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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving profiles: {ex.Message}");
            }
        }
        #endregion
    }
}