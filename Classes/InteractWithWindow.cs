using System;
using System.Diagnostics;
using System.Runtime.InteropServices; // For the P/Invoke signatures.
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Shell32;
using static Save_Window_Position_and_Size.Classes.WindowHighlighter;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Drawing;

namespace Save_Window_Position_and_Size.Classes
{
    internal static class InteractWithWindow
    {
        #region P/Invoke Constants
        // P/Invoke constants
        const uint SWP_NOZORDER = 0x0004;
        const uint SWP_SHOWWINDOW = 0x0040;
        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_NOMOVE = 0x0002;
        const uint SWP_NOACTIVATE = 0x0010;
        const int SW_RESTORE = 9;
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly IntPtr HWND_TOP = new IntPtr(0);
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        // P/Invoke declarations.
        internal const int GWL_STYLE = -16;
        internal const int GWL_EXSTYLE = -20;
        internal const int WS_VISIBLE = 0x10000000;
        internal const long WS_EX_TOOLWINDOW = 0x00000080L;
        internal const long WS_EX_APPWINDOW = 0x00040000L;
        #endregion


        #region P/Invoke Declarations
        [DllImport("user32.dll")]
        internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWINFO
        {
            public uint cbSize;
            public RECT rcWindow;
            public RECT rcClient;
            public uint dwStyle;
            public uint dwExStyle;
            public uint dwWindowStatus;
            public uint cxWindowBorders;
            public uint cyWindowBorders;
            public ushort atomWindowType;
            public ushort wCreatorVersion;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern bool SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetWindowRect(IntPtr hwnd, out Rectangle rect);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        internal static extern bool IsIconic(IntPtr hWnd);
        #endregion


        #region Get All Running Apps
        public static List<Window> GetAllRunningApps(IgnoreListManager ignoreListManager)
        {
            var allWindows = new List<Window>();

            foreach (var dicWin in GetAllMainWindowHandles())
            {
                string windowTitle = dicWin.Key;
                IntPtr windowHandle = dicWin.Value;

                // Skip if window title is empty
                if (string.IsNullOrWhiteSpace(windowTitle))
                    continue;


                int windowStyle = GetWindowLong(windowHandle, GWL_STYLE);
                if ((windowStyle & WS_VISIBLE) == WS_VISIBLE)
                {
                    // Create a Window object for each window
                    Window window = new Window(windowHandle, windowTitle, windowTitle);

                    // Skip windows in the ignore list using the manager
                    if (ignoreListManager != null && ignoreListManager.ShouldIgnoreWindow(window))
                    {
                        continue;
                    }

                    try
                    {
                        // Get process name if possible
                        uint processId;
                        GetWindowThreadProcessId(windowHandle, out processId);
                        if (processId > 0)
                        {
                            Process process = Process.GetProcessById((int)processId);
                            window.ProcessName = process.ProcessName;

                            // Check if this is a file explorer window
                            if (process.ProcessName.Equals("explorer", StringComparison.OrdinalIgnoreCase) &&
                                (windowTitle.StartsWith("File Explorer", StringComparison.OrdinalIgnoreCase) ||
                                 windowTitle.StartsWith("This PC", StringComparison.OrdinalIgnoreCase) ||
                                 windowTitle.Contains(":\\") || windowTitle.Contains("Explorer")))
                            {
                                window.IsFileExplorer = true;
                            }
                        }
                    }
                    catch
                    {
                        window.ProcessName = "Unknown";
                    }

                    allWindows.Add(window);
                }
            }

            return allWindows;
        }

        public static List<Window> GetVisibleRunningApps(IgnoreListManager ignoreListManager)
        {
            var visibleWindows = new List<Window>();

            foreach (var dicWin in GetAllMainWindowHandles())
            {
                string windowTitle = dicWin.Key;
                IntPtr windowHandle = dicWin.Value;

                // Skip if window title is empty
                if (string.IsNullOrWhiteSpace(windowTitle)) continue;

                // Create a Window object
                Window window = new Window(windowHandle, windowTitle, windowTitle);

                try
                {
                    // Get process name if possible
                    uint processId;
                    GetWindowThreadProcessId(windowHandle, out processId);
                    if (processId > 0)
                    {
                        Process process = Process.GetProcessById((int)processId);
                        window.ProcessName = process.ProcessName;
                    }
                }
                catch
                {
                    window.ProcessName = "Unknown";
                }

                // Skip windows in the ignore list using the manager
                if (ignoreListManager != null && ignoreListManager.ShouldIgnoreWindow(window))
                {
                    continue;
                }

                int windowStyle = GetWindowLong(windowHandle, GWL_STYLE);
                if ((windowStyle & WS_VISIBLE) == WS_VISIBLE && !IsIconic(windowHandle))
                {
                    // Only add non-minimized windows
                    visibleWindows.Add(window);
                }
            }

            return visibleWindows;
        }

        public static Dictionary<string, IntPtr> GetAllMainWindowHandles()
        {
            var windowHandles = new Dictionary<string, IntPtr>();

            EnumWindows(delegate (IntPtr hWnd, IntPtr lParam)
            {
                // Get window title
                StringBuilder title = new StringBuilder(256);
                GetWindowText(hWnd, title, title.Capacity);

                // Check both visible and non-visible windows
                // Include all windows that have a title, regardless of style
                if (!string.IsNullOrWhiteSpace(title.ToString()))
                {
                    windowHandles[title.ToString()] = hWnd;
                }

                return true;
            }, IntPtr.Zero);

            return windowHandles;
        }
        #endregion


        #region Get Window Handle
        internal static IntPtr GetWindowHandleByWindowAndProcess(Window window)
        {
            // Try to find by window title first
            if (!string.IsNullOrWhiteSpace(window.TitleName))
            {
                var allRunningApps = GetAllRunningApps(null);

                foreach (var app in allRunningApps)
                {
                    if (app.TitleName == window.TitleName)
                    {
                        return app.hWnd;
                    }
                }
            }

            // Try to find by process name if window title search failed
            if (!string.IsNullOrWhiteSpace(window.ProcessName))
            {
                try
                {
                    var processes = Process.GetProcessesByName(window.ProcessName);

                    // First try to find an exact match with process name and title
                    foreach (var process in processes)
                    {
                        if (process.MainWindowTitle == window.TitleName)
                        {
                            return process.MainWindowHandle;
                        }
                    }

                    // If no exact match, try to find a window with this process name
                    if (processes.Length > 0)
                    {
                        var firstWindow = processes[0].MainWindowHandle;
                        return firstWindow;
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return IntPtr.Zero;
        }

        public static IntPtr GetWindowHandleByProcessName(string processName)
        {
            IntPtr hWnd = IntPtr.Zero;
            var processes = Process.GetProcessesByName(processName);

            foreach (var process in processes)
            {
                IntPtr mainWindowHandle = process.MainWindowHandle;
                if (mainWindowHandle != IntPtr.Zero)
                {
                    int windowStyle = GetWindowLong(mainWindowHandle, GWL_STYLE);
                    if ((windowStyle & WS_VISIBLE) == WS_VISIBLE)
                    {
                        hWnd = process.MainWindowHandle;
                    }
                }
            }

            return hWnd;
        }

        public static IntPtr FindWindowByTitle(string windowTitle)
        {
            if (string.IsNullOrWhiteSpace(windowTitle))
            {
                return IntPtr.Zero;
            }

            IntPtr result = IntPtr.Zero;
            List<string> matchedTitles = new List<string>();

            EnumWindows((hWnd, lParam) =>
            {
                StringBuilder title = new StringBuilder(256);
                GetWindowText(hWnd, title, title.Capacity);
                string currentTitle = title.ToString();

                // Track all titles for debugging
                if (!string.IsNullOrWhiteSpace(currentTitle))
                {
                    matchedTitles.Add(currentTitle);
                }

                // Exact match
                if (currentTitle == windowTitle)
                {
                    result = hWnd;
                    return false; // Stop enumerating
                }

                // Case-insensitive match as fallback
                if (string.Equals(currentTitle, windowTitle, StringComparison.OrdinalIgnoreCase))
                {
                    result = hWnd;
                    // Continue enumerating to find exact match if possible
                }

                return true;
            }, IntPtr.Zero);

            return result;
        }

        public static IntPtr GetWindowHandleByProcessNameAndTitle(string processName, string windowTitle)
        {
            IntPtr hWnd = IntPtr.Zero;
            var processes = Process.GetProcessesByName(processName);

            foreach (var process in processes)
            {
                IntPtr mainWindowHandle = process.MainWindowHandle;
                if (mainWindowHandle != IntPtr.Zero)
                {
                    int windowStyle = GetWindowLong(mainWindowHandle, GWL_STYLE);
                    if ((windowStyle & WS_VISIBLE) == WS_VISIBLE)
                    {
                        // Check if the window title matches or contains the saved title
                        if (process.MainWindowTitle.Equals(windowTitle, StringComparison.OrdinalIgnoreCase) ||
                            process.MainWindowTitle.Contains(windowTitle))
                        {
                            return process.MainWindowHandle;
                        }
                    }
                }
            }

            // If no exact match found, enumerate all windows
            Dictionary<IntPtr, string> allWindows = new Dictionary<IntPtr, string>();

            EnumWindows((hwnd, lParam) =>
            {
                uint pid;
                GetWindowThreadProcessId(hwnd, out pid);

                if (processes.Any(p => p.Id == pid))
                {
                    StringBuilder title = new StringBuilder(256);
                    GetWindowText(hwnd, title, title.Capacity);

                    if (!string.IsNullOrWhiteSpace(title.ToString()) &&
                        (title.ToString().Equals(windowTitle, StringComparison.OrdinalIgnoreCase) ||
                         title.ToString().Contains(windowTitle)))
                    {
                        hWnd = hwnd;
                        return false; // Stop enumeration if we found a match
                    }
                }
                return true; // Continue enumeration
            }, IntPtr.Zero);

            return hWnd;
        }
        #endregion


        #region Get Window Title
        public static string GetWindowTitleByProcessName(string processName)
        {
            string windowTitle = "";
            var processes = Process.GetProcessesByName(processName);

            foreach (var process in processes)
            {
                IntPtr mainWindowHandle = process.MainWindowHandle;
                if (mainWindowHandle != IntPtr.Zero)
                {
                    int windowStyle = GetWindowLong(mainWindowHandle, GWL_STYLE);
                    if ((windowStyle & WS_VISIBLE) == WS_VISIBLE)
                    {
                        windowTitle = process.MainWindowTitle;
                    }
                }
            }

            return windowTitle;
        }
        #endregion


        #region Get/Set Window Position and Size
        public static WindowPosAndSize GetWindowPositionAndSize(IntPtr hWnd)
        {
            GetWindowRect(hWnd, out var rect);
            var windowPosAndSize = ConvertRectToWindowPosAndSize(rect);
            return windowPosAndSize;
        }

        public static bool SetWindowPositionAndSize(IntPtr hWnd, int x, int y, int width, int height)
        {
            SetWindowPos(hWnd, new IntPtr(), x, y, width, height, SWP_NOZORDER);
            return true;
        }

        public static bool SetWindowPositionAndSize(IntPtr hWnd, WindowPosAndSize posAndSize)
        {
            SetWindowPos(hWnd, new IntPtr(), posAndSize.X, posAndSize.Y,
                posAndSize.Width, posAndSize.Height, SWP_NOZORDER | SWP_SHOWWINDOW);
            return true;
        }

        public static WindowPosAndSize ConvertRectToWindowPosAndSize(Rectangle rect)
        {
            var windowPosAndSize = new WindowPosAndSize
            {
                X = rect.Location.X,
                Y = rect.Location.Y,
                Width = rect.Width - rect.X,
                Height = rect.Height - rect.Y
            };
            return windowPosAndSize;
        }
        #endregion


        #region Set Window Always On Top
        public static void SetWindowAlwaysOnTop(IntPtr hWnd)
        {
            SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
            SetForegroundWindow(hWnd);
            ShowWindow(hWnd, SW_RESTORE);
        }

        public static void UnsetWindowAlwaysOnTop(IntPtr hWnd)
        {
            SetWindowPos(hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
            SetWindowPos(hWnd, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        }
        #endregion


        #region Get Process By Window Title
        public static Process GetProcessByWindowTitle(string windowTitle)
        {
            return Process.GetProcesses()
                .FirstOrDefault(process =>
                    !string.IsNullOrWhiteSpace(process.MainWindowTitle) &&
                    process.MainWindowTitle.Equals(windowTitle, StringComparison.OrdinalIgnoreCase)
                );
        }

        public static string GetProcessNameByWindowTitle(string windowTitle)
        {
            IntPtr hWnd = FindWindowByTitle(windowTitle);
            if (hWnd != IntPtr.Zero)
            {
                uint processId;
                GetWindowThreadProcessId(hWnd, out processId);
                try
                {
                    Process process = Process.GetProcessById((int)processId);
                    return process.ProcessName;
                }
                catch (ArgumentException)
                {
                    // Handle exceptions if the process is not found
                    return "";
                }
            }

            return ""; // Window not found
        }
        #endregion


        #region File Explorer Specific Methods
        public static List<Window> GetFileExplorerWindows(IgnoreListManager ignoreListManager)
        {
            List<Window> fileExplorers = new List<Window>();

            // Use the Shell32 COM object to get all open Explorer windows
            SHDocVw.ShellWindows shellWindows = new SHDocVw.ShellWindows();

            foreach (SHDocVw.InternetExplorer window in shellWindows)
            {
                try
                {
                    if (Path.GetFileNameWithoutExtension(window.FullName).ToLowerInvariant() == "explorer")
                    {
                        // Skip minimized windows
                        if (window.Left == -32000) continue;

                        // Get the location name (folder path being shown)
                        string locationName = window.LocationName;
                        string displayName = Path.GetFileName(locationName);

                        // Skip empty or invalid titles
                        if (string.IsNullOrWhiteSpace(displayName)) continue;

                        // Format the window title
                        string windowTitle = $"FileExplorer: {window.Name} - {window.LocationName}";

                        // Skip windows in the exceptions list
                        bool shouldSkip = false;
                        foreach (var exception in ignoreListManager.GetIgnoreList())
                        {
                            if (displayName.Contains(exception) || exception.Contains(displayName))
                            {
                                shouldSkip = true;
                                break;
                            }
                        }

                        if (shouldSkip)
                            continue;

                        // Create a Window object for the explorer window
                        Window explorerWindow = new Window();
                        explorerWindow.IsFileExplorer = true;
                        explorerWindow.TitleName = windowTitle;
                        explorerWindow.DisplayName = displayName;
                        explorerWindow.ProcessName = "explorer";

                        // Create a random ID for the window
                        explorerWindow.Id = new Random().Next(300, 32034);

                        // Get position and size
                        WindowPosAndSize posAndSize = new WindowPosAndSize
                        {
                            X = window.Left,
                            Y = window.Top,
                            Width = window.Width,
                            Height = window.Height
                        };
                        explorerWindow.WindowPosAndSize = posAndSize;

                        fileExplorers.Add(explorerWindow);
                    }
                }
                catch (Exception ex)
                {
                }
            }

            return fileExplorers;
        }

        public static WindowPosAndSize GetFileExplorerPosAndSize(Window fileExplorerWindow)
        {
            if (fileExplorerWindow == null || !fileExplorerWindow.IsFileExplorer || string.IsNullOrWhiteSpace(fileExplorerWindow.TitleName))
            {
                return new WindowPosAndSize();
            }

            return GetFileExplorerPosAndSize(fileExplorerWindow.TitleName);
        }

        public static WindowPosAndSize GetFileExplorerPosAndSize(string windowTitle)
        {
            var windowPosAndSize = new WindowPosAndSize();
            foreach (SHDocVw.InternetExplorer window in new SHDocVw.ShellWindows())
            {
                // Skip minimized windows
                if (window.Left == -32000) continue;

                if (Path.GetFileNameWithoutExtension(window.FullName).ToLowerInvariant() == "explorer"
                    && $"FileExplorer: {window.Name} - {window.LocationName}" == windowTitle)
                {
                    windowPosAndSize.X = window.Left;
                    windowPosAndSize.Y = window.Top;
                    windowPosAndSize.Width = window.Width;
                    windowPosAndSize.Height = window.Height;
                    return windowPosAndSize;
                }
            }
            return windowPosAndSize;
        }

        public static void SetFileExplorerWindowPosAndSize(Window fileExplorerWindow, WindowPosAndSize windowPosAndSize)
        {
            if (fileExplorerWindow == null || !fileExplorerWindow.IsFileExplorer || string.IsNullOrWhiteSpace(fileExplorerWindow.TitleName))
            {
                return;
            }

            SetFileExplorerWindowPosAndSize(fileExplorerWindow.TitleName, windowPosAndSize);
        }

        public static void SetFileExplorerWindowPosAndSize(string windowTitle, WindowPosAndSize windowPosAndSize)
        {
            foreach (SHDocVw.InternetExplorer window in new SHDocVw.ShellWindows())
            {
                if (Path.GetFileNameWithoutExtension(window.FullName).ToLowerInvariant() == "explorer"
                    && $"FileExplorer: {window.Name} - {window.LocationName}" == windowTitle)
                {
                    window.Left = windowPosAndSize.X;
                    window.Top = windowPosAndSize.Y;
                    window.Width = windowPosAndSize.Width;
                    window.Height = windowPosAndSize.Height;
                    return;
                }
            }
        }
        #endregion


        internal static void RestoreWindow(Window window, List<Window> allWindows)
        {
            if (window == null || !window.IsValid())
            {
                return;
            }

            if (window.WindowPosAndSize == null)
            {
                window.GetWindowPosAndSize();
                if (window.WindowPosAndSize == null) { return; }
            }

            // Create a copy of the window position and size for manipulation
            WindowPosAndSize posAndSize = window.WindowPosAndSize;

            // Convert from percentages to pixels if needed
            if (posAndSize.IsPercentageBased)
            {
                var (screenWidth, screenHeight) = GetScreenDimensions();
                posAndSize.ConvertToPixels(screenWidth, screenHeight, allWindows);
            }

            // Handle File Explorer windows
            if (window.IsFileExplorer)
            {
                SetFileExplorerWindowPosAndSize(window, posAndSize);
                return;
            }

            // Get the window handle
            IntPtr hWnd = FindWindowByTitle(window.TitleName);
            if (hWnd == IntPtr.Zero && !string.IsNullOrWhiteSpace(window.ProcessName))
            {
                // Try to find by process name if title search failed
                var processes = Process.GetProcessesByName(window.ProcessName);
                foreach (var process in processes)
                {
                    if (process.MainWindowTitle == window.TitleName)
                    {
                        hWnd = process.MainWindowHandle;
                        break;
                    }
                }

                // If still zero, just use the first window with this process name
                if (hWnd == IntPtr.Zero && processes.Length > 0)
                {
                    hWnd = processes[0].MainWindowHandle;
                }
            }

            if (hWnd == IntPtr.Zero)
            {
                return;
            }

            // Check if window is minimized and restore it first if needed
            if (IsIconic(hWnd))
            {
                // Restore the window from minimized state
                ShowWindow(hWnd, SW_RESTORE);

                // Give the window time to restore
                System.Threading.Thread.Sleep(100);
            }

            // Set window position and size            
            SetWindowPositionAndSize(hWnd, posAndSize);

            // Set always on top if needed
            if (window.KeepOnTop)
            {
                SetWindowAlwaysOnTop(hWnd);
            }
        }

        public static (int Width, int Height) GetScreenDimensions()
        {
            return (Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
        }
    }



    internal class WindowPosAndSize
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public bool IsPercentageBased { get; set; } = false;

        // Helper method to convert from pixels to relative percentages
        public void ConvertToPercentages(int screenWidth, int screenHeight, List<Window> allWindows)
        {
            if (!IsPercentageBased)
            {
                try
                {
                    // Simply use screen dimensions for percentage calculations
                    // This ensures percentages always make sense relative to the screen
                    X = Math.Max(0, Math.Min(100, (int)Math.Round((double)X / screenWidth * 100)));
                    Y = Math.Max(0, Math.Min(100, (int)Math.Round((double)Y / screenHeight * 100)));
                    Width = Math.Max(1, Math.Min(100, (int)Math.Round((double)Width / screenWidth * 100)));
                    Height = Math.Max(1, Math.Min(100, (int)Math.Round((double)Height / screenHeight * 100)));

                    IsPercentageBased = true;
                }
                catch (Exception ex)
                {
                    // Fallback with error checking
                    X = Math.Max(0, Math.Min(100, (int)Math.Round((double)X / screenWidth * 100)));
                    Y = Math.Max(0, Math.Min(100, (int)Math.Round((double)Y / screenHeight * 100)));
                    Width = Math.Max(1, Math.Min(100, (int)Math.Round((double)Width / screenWidth * 100)));
                    Height = Math.Max(1, Math.Min(100, (int)Math.Round((double)Height / screenHeight * 100)));
                    IsPercentageBased = true;
                }
            }
        }

        // Helper method to convert from percentages to pixels
        internal void ConvertToPixels(int screenWidth, int screenHeight, List<Window> allWindows)
        {
            if (IsPercentageBased)
            {
                try
                {
                    // Constrain percentage values to valid range
                    int safeX = Math.Max(0, Math.Min(100, X));
                    int safeY = Math.Max(0, Math.Min(100, Y));
                    int safeWidth = Math.Max(1, Math.Min(100, Width));
                    int safeHeight = Math.Max(1, Math.Min(100, Height));

                    // Simple conversion from percentage to actual screen pixels
                    X = (int)Math.Round(screenWidth * (safeX / 100.0));
                    Y = (int)Math.Round(screenHeight * (safeY / 100.0));
                    Width = (int)Math.Round(screenWidth * (safeWidth / 100.0));
                    Height = (int)Math.Round(screenHeight * (safeHeight / 100.0));

                    // Ensure minimum dimensions
                    Width = Math.Max(50, Width);
                    Height = Math.Max(50, Height);

                    IsPercentageBased = false;
                }
                catch (Exception ex)
                {
                    // Fallback with constraints
                    int safeX = Math.Max(0, Math.Min(100, X));
                    int safeY = Math.Max(0, Math.Min(100, Y));
                    int safeWidth = Math.Max(1, Math.Min(100, Width));
                    int safeHeight = Math.Max(1, Math.Min(100, Height));

                    X = (int)Math.Round(screenWidth * (safeX / 100.0));
                    Y = (int)Math.Round(screenHeight * (safeY / 100.0));
                    Width = Math.Max(50, (int)Math.Round(screenWidth * (safeWidth / 100.0)));
                    Height = Math.Max(50, (int)Math.Round(screenHeight * (safeHeight / 100.0)));
                    IsPercentageBased = false;
                }
            }
        }
    }
}
