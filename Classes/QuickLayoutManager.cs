using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Save_Window_Position_and_Size.Classes
{
    internal class QuickLayoutManager
    {
        private WindowManager windowManager;
        private Dictionary<int, Process> activeProcesses;
        private string quickLaunchPath;

        internal QuickLayoutManager(WindowManager windowManager)
        {
            this.windowManager = windowManager;
            this.activeProcesses = new Dictionary<int, Process>();

            // Set up the path to the QuickLaunch folder
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            this.quickLaunchPath = Path.Combine(baseDirectory, Constants.Defaults.QuickLaunchFolderName);

            // Ensure the folder exists
            if (!Directory.Exists(quickLaunchPath))
            {
                try
                {
                    Directory.CreateDirectory(quickLaunchPath);
                }
                catch
                {
                    // If we can't create it, use the base directory
                    this.quickLaunchPath = baseDirectory;
                }
            }
        }

        internal async Task<bool> CreateQuickLayout()
        {
            try
            {
                // Determine next available index
                int nextIndex = FindNextAvailableIndex();

                // Check if we've hit the maximum number of layouts
                if (nextIndex > Constants.Defaults.MaxQuickLayouts)
                {
                    MessageBox.Show($"Maximum of {Constants.Defaults.MaxQuickLayouts} quick layouts reached. Please close one before creating another.",
                        "Maximum Layouts Reached", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                // Get all currently visible windows 
                var windows = await windowManager.GetVisibleRunningApps();

                // Create deep copies to avoid reference issues
                List<Window> windowCopies = new List<Window>();
                foreach (var window in windows)
                {
                    var copy = window.Clone();
                    copy.AutoPosition = true;
                    windowCopies.Add(copy);
                }

                // Launch the app with the windows
                return LaunchQuickLayoutApp(nextIndex, windowCopies);
            }
            catch (Exception)
            {
                // Show a message but don't expose the exception details
                MessageBox.Show("Could not create quick layout. Please try again.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Creates a quick layout with predefined windows from a profile
        /// </summary>
        /// <param name="profileWindows">List of windows from a specific profile</param>
        /// <returns>True if the layout was created successfully</returns>
        internal bool CreateQuickLayout(List<Window> profileWindows)
        {
            try
            {
                // Determine next available index
                int nextIndex = FindNextAvailableIndex();

                // Check if we've hit the maximum number of layouts
                if (nextIndex > Constants.Defaults.MaxQuickLayouts)
                {
                    MessageBox.Show($"Maximum of {Constants.Defaults.MaxQuickLayouts} quick layouts reached. Please close one before creating another.",
                        "Maximum Layouts Reached", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                // Create deep copies of the windows to ensure no reference issues
                List<Window> windowCopies = new List<Window>();
                foreach (var window in profileWindows)
                {
                    var copy = window.Clone();
                    copy.AutoPosition = true;
                    windowCopies.Add(copy);
                }

                // Launch the app with the copied windows
                return LaunchQuickLayoutApp(nextIndex, windowCopies);
            }
            catch (Exception)
            {
                // Show a message but don't expose the exception details
                MessageBox.Show("Could not create quick layout from profile. Please try again.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private int FindNextAvailableIndex()
        {
            // Check for closed processes first
            for (int i = 1; i <= Constants.Defaults.MaxQuickLayouts; i++)
            {
                if (activeProcesses.TryGetValue(i, out Process process))
                {
                    if (process == null || process.HasExited)
                    {
                        // Remove the process from our tracking
                        activeProcesses.Remove(i);
                        return i;
                    }
                }
            }

            // If all tracked processes are still running, find the next available index
            int nextIndex = 1;
            while (activeProcesses.ContainsKey(nextIndex) && nextIndex <= Constants.Defaults.MaxQuickLayouts)
            {
                nextIndex++;
            }

            return nextIndex;
        }

        private bool LaunchQuickLayoutApp(int index, List<Window> windows)
        {
            try
            {
                // Determine the app path
                string appName = $"{index}_Quick_Launch.exe";
                string appPath = Path.Combine(quickLaunchPath, appName);

                // Check if the app exists
                if (!File.Exists(appPath))
                {
                    MessageBox.Show($"Quick launch application not found: {appName}\nExpected path: {appPath}",
                        "Missing Application", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Serialize the list of windows to JSON
                string windowsJson = JsonConvert.SerializeObject(windows);

                // Convert to Base64 to avoid command line parsing issues
                string base64Json = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(windowsJson));

                // Launch the app with the Base64 encoded JSON
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = appPath,
                    Arguments = base64Json,
                    UseShellExecute = true
                };

                Process process = Process.Start(startInfo);

                // Add to active processes
                if (process != null)
                {
                    activeProcesses[index] = process;
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error launching quick layout app: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        internal void CloseAll()
        {
            // Create a copy of the dictionary keys to avoid modification during enumeration
            var processIndicesToClose = new List<int>(activeProcesses.Keys);

            foreach (var index in processIndicesToClose)
            {
                try
                {
                    if (activeProcesses.TryGetValue(index, out Process process))
                    {
                        if (process != null && !process.HasExited)
                        {
                            process.Kill();
                            process.Dispose();
                        }

                        // Remove from our tracking dictionary
                        activeProcesses.Remove(index);
                    }
                }
                catch
                {
                    // Ignore errors when closing processes
                }
            }

            // Clear the dictionary to be sure
            activeProcesses.Clear();
        }
    }
}