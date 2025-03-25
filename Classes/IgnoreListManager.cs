using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Save_Window_Position_and_Size.Classes;
using Save_Window_Position_and_Size.Classes;

namespace Save_Window_Position_and_Size.Classes
{
    /// <summary>
    /// Manages the list of windows that should be ignored by the application.
    /// 
    /// This class handles:
    /// - Loading/saving the ignore list from application settings
    /// - Adding/removing items from the ignore list
    /// - Checking if windows should be ignored based on their titles
    /// - Extracting window titles from display formats in the UI
    /// 
    /// The ignore list is automatically saved to application settings whenever it is modified.
    /// </summary>
    public class IgnoreListManager
    {
        private List<string> _ignoreList = new List<string>();

        /// <summary>
        /// Gets the current ignore list
        /// </summary>
        public List<string> IgnoreList => _ignoreList;

        /// <summary>
        /// Creates a new IgnoreListManager and loads any existing ignore list from settings
        /// </summary>
        public IgnoreListManager()
        {
            LoadIgnoreList();
        }

        /// <summary>
        /// Loads the ignore list from app settings
        /// </summary>
        public void LoadIgnoreList()
        {
            string json = AppSettings.Load(Constants.AppSettingsConstants.IgnoreListKey);
            if (string.IsNullOrEmpty(json))
            {
                _ignoreList = new List<string>();
                return;
            }

            try
            {
                var loadedList = JsonConvert.DeserializeObject<List<string>>(json);
                _ignoreList = loadedList ?? new List<string>();
                Debug.WriteLine($"Loaded ignore list with {_ignoreList.Count} items");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading ignore list: {ex.Message}");
                _ignoreList = new List<string>();
            }
        }

        /// <summary>
        /// Saves the current ignore list to app settings
        /// </summary>
        public void SaveIgnoreList()
        {
            try
            {
                string json = JsonConvert.SerializeObject(_ignoreList);
                AppSettings.Save(Constants.AppSettingsConstants.IgnoreListKey, json);
                Debug.WriteLine($"Saved ignore list with {_ignoreList.Count} items");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving ignore list: {ex.Message}");
            }
        }

        /// <summary>
        /// Adds a window title to the ignore list
        /// </summary>
        /// <param name="windowTitle">The window title to add</param>
        /// <returns>True if the item was added, false if it already exists</returns>
        public bool AddToIgnoreList(string windowTitle)
        {
            if (string.IsNullOrWhiteSpace(windowTitle)) return false;

            // First check if the item is already in the list
            if (_ignoreList.Any(item => string.Equals(item, windowTitle, StringComparison.OrdinalIgnoreCase)))
            {
                Debug.WriteLine($"Item already in ignore list: {windowTitle}");
                return false;
            }

            // Add the item to the list
            _ignoreList.Add(windowTitle);
            Debug.WriteLine($"Added to ignore list: {windowTitle}");
            
            // Save the updated list
            SaveIgnoreList();
            return true;
        }

        /// <summary>
        /// Removes a window title from the ignore list
        /// </summary>
        /// <param name="windowTitle">The window title to remove</param>
        /// <returns>True if the item was removed, false if it wasn't found</returns>
        public bool RemoveFromIgnoreList(string windowTitle)
        {
            if (string.IsNullOrWhiteSpace(windowTitle)) return false;

            // Find and remove the item
            var itemToRemove = _ignoreList.FirstOrDefault(item => 
                string.Equals(item, windowTitle, StringComparison.OrdinalIgnoreCase));

            if (itemToRemove != null)
            {
                _ignoreList.Remove(itemToRemove);
                Debug.WriteLine($"Removed from ignore list: {windowTitle}");
                
                // Save the updated list
                SaveIgnoreList();
                return true;
            }

            Debug.WriteLine($"Item not found in ignore list: {windowTitle}");
            return false;
        }

        /// <summary>
        /// Updates the ignore list with a new list of items
        /// </summary>
        /// <param name="newList">The new list of window titles to ignore</param>
        public void UpdateIgnoreList(List<string> newList)
        {
            if (newList == null) return;

            _ignoreList = new List<string>(newList);
            Debug.WriteLine($"Updated ignore list with {_ignoreList.Count} items");
            
            // Save the updated list
            SaveIgnoreList();
        }

        /// <summary>
        /// Checks if a window should be ignored
        /// </summary>
        /// <param name="window">The window to check</param>
        /// <returns>True if the window should be ignored, false otherwise</returns>
        internal bool ShouldIgnoreWindow(Window window)
        {
            if (window == null) return false;
            
            // Check the window's title name
            return ShouldIgnoreWindow(window.TitleName);
        }

        /// <summary>
        /// Checks if a window title should be ignored
        /// </summary>
        /// <param name="windowTitle">The window title to check</param>
        /// <returns>True if the window should be ignored, false otherwise</returns>
        public bool ShouldIgnoreWindow(string windowTitle)
        {
            // Perform basic validation
            if (string.IsNullOrWhiteSpace(windowTitle)) return false;
            if (_ignoreList == null || _ignoreList.Count == 0) return false;

            try
            {
                // First check for exact matches (case-insensitive)
                if (_ignoreList.Any(ignored => string.Equals(ignored, windowTitle, StringComparison.OrdinalIgnoreCase)))
                {
                    Debug.WriteLine($"Ignore list exact match: \"{windowTitle}\"");
                    return true;
                }

                // Check if any ignore list entry is a substring of the window title
                if (_ignoreList.Any(ignored => 
                    !string.IsNullOrEmpty(ignored) && 
                    windowTitle.IndexOf(ignored, StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    Debug.WriteLine($"Ignore list substring match: \"{windowTitle}\"");
                    return true;
                }
                
                // Check if window title is a substring of any ignore list entry
                if (_ignoreList.Any(ignored => 
                    !string.IsNullOrEmpty(windowTitle) && 
                    ignored.IndexOf(windowTitle, StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    Debug.WriteLine($"Window title is substring of ignore list item: \"{windowTitle}\"");
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in ShouldIgnoreWindow: {ex.Message}");
                return false; // Default to not ignoring in case of error
            }
        }

        /// <summary>
        /// Clears the ignore list
        /// </summary>
        public void ClearIgnoreList()
        {
            _ignoreList.Clear();
            Debug.WriteLine("Cleared ignore list");
            
            // Save the updated list
            SaveIgnoreList();
        }

        /// <summary>
        /// Gets a copy of the current ignore list
        /// </summary>
        public List<string> GetIgnoreList()
        {
            return new List<string>(_ignoreList);
        }

        /// <summary>
        /// Parses a window entry from the running apps list and extracts just the title
        /// </summary>
        /// <param name="selectedText">The text from the running apps list</param>
        /// <returns>The extracted window title</returns>
        public string ExtractWindowTitle(string selectedText)
        {
            if (string.IsNullOrWhiteSpace(selectedText)) return string.Empty;

            // If it's a regular window with format "Title / handle", extract just the title
            if (selectedText.Contains(" / "))
            {
                return selectedText.Split('/')[0].Trim();
            }

            // Otherwise return the text as is (for File Explorer windows)
            return selectedText;
        }

        /// <summary>
        /// Sets the ignore list directly from an external list
        /// </summary>
        /// <param name="list">The new list to set</param>
        public void SetIgnoreList(List<string> list)
        {
            if (list == null)
            {
                _ignoreList = new List<string>();
            }
            else
            {
                _ignoreList = new List<string>(list);
            }
            
            Debug.WriteLine($"Set ignore list with {_ignoreList.Count} items");
            SaveIgnoreList();
        }
    }
} 