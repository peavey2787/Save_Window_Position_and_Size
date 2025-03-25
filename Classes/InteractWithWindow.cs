using System;
using System.Runtime.InteropServices; // For the P/Invoke signatures.
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Shell32;
using static Save_Window_Position_and_Size.Classes.WindowHighlighter;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

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
        internal const int GWLP_WNDPROC = -4;
        internal const int GW_OWNER = 4;
        internal const int WS_VISIBLE = 0x10000000;
        internal const int WS_CHILD = 0x40000000;
        internal const int WS_EX_TOOLWINDOW = 0x00000080;
        internal const long WS_EX_APPWINDOW = 0x00040000L;
        #endregion


        #region P/Invoke Declarations
        [DllImport("user32.dll")]
        internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

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
        internal static extern int GetWindowRect(IntPtr hwnd, out RECT rect);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        internal static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWindowVisible(IntPtr hWnd);
        #endregion


        #region Get Application Windows
        /// <summary>
        /// Gets all application windows filtered by visibility if specified.
        /// This is the primary method for getting window information directly from the Windows API.
        /// </summary>
        /// <param name="onlyVisible">When true, returns only visible windows</param>
        /// <param name="exceptions">List of window titles to exclude</param>
        /// <returns>Dictionary mapping window handles to their titles</returns>
        public static Dictionary<IntPtr, string> GetApplicationWindows(bool onlyVisible = false, List<string> exceptions = null)
        {
            // Initialize the exceptions list if null
            exceptions = exceptions ?? new List<string>();

            // First, get all window handles 
            var windowHandles = new Dictionary<string, IntPtr>();

            EnumWindows(delegate (IntPtr hWnd, IntPtr lParam)
            {
                StringBuilder title = new StringBuilder(256);
                GetWindowText(hWnd, title, title.Capacity);
                string windowTitle = title.ToString();

                if (!string.IsNullOrWhiteSpace(windowTitle))
                {
                    windowHandles[windowTitle] = hWnd;
                }

                return true;
            }, IntPtr.Zero);

            // Now create the result dictionary with window handles as keys and titles as values
            var allWindows = new Dictionary<IntPtr, string>();

            // Filter windows based on window API properties
            foreach (var dicWin in windowHandles)
            {
                string windowTitle = dicWin.Key;
                IntPtr hWnd = dicWin.Value;

                // Skip based on exceptions list
                if (exceptions.Contains(windowTitle))
                    continue;

                // Get window info to check its properties
                WINDOWINFO winInfo = new WINDOWINFO();
                winInfo.cbSize = (uint)Marshal.SizeOf(typeof(WINDOWINFO));
                if (!GetWindowInfo(hWnd, ref winInfo))
                    continue;

                // Skip windows that are too small to be real application windows
                int width = winInfo.rcWindow.Right - winInfo.rcWindow.Left;
                int height = winInfo.rcWindow.Bottom - winInfo.rcWindow.Top;
                if (width < 50 || height < 50)
                    continue;

                // Skip system tray windows and toolbars
                // We only want windows with app window style or those that don't have tool window style
                if ((winInfo.dwExStyle & WS_EX_TOOLWINDOW) != 0 && (winInfo.dwExStyle & WS_EX_APPWINDOW) == 0)
                    continue;

                // Skip child windows since top-level application windows aren't children
                if ((winInfo.dwStyle & WS_CHILD) != 0)
                    continue;

                // Get class name - many system windows have specific class names
                StringBuilder className = new StringBuilder(256);
                GetClassName(hWnd, className, className.Capacity);
                string windowClass = className.ToString();

                // Skip common system window classes
                if (IsSystemWindowClass(windowClass))
                    continue;

                // Check window owner - real apps usually don't have owners
                IntPtr owner = GetWindow(hWnd, GW_OWNER);
                if (owner != IntPtr.Zero)
                {
                    // It has an owner, check if the owner is visible
                    // If the owner is invisible, this is likely a system window
                    if (!IsWindowVisible(owner))
                        continue;
                }

                // Check process details - see if this is a background service
                try
                {
                    uint processId;
                    GetWindowThreadProcessId(hWnd, out processId);
                    if (processId > 0)
                    {
                        using (Process process = Process.GetProcessById((int)processId))
                        {
                            // Check main window handle
                            // If the process has a different main window, this might be a supporting window
                            if (process.MainWindowHandle != IntPtr.Zero &&
                                process.MainWindowHandle != hWnd &&
                                process.ProcessName.ToLowerInvariant() != "explorer")
                            {
                                // Skip if the window style suggests it's not a main window
                                if ((winInfo.dwExStyle & WS_EX_APPWINDOW) == 0)
                                    continue;
                            }
                        }
                    }
                }
                catch
                {
                    // If we can't get process info, continue with other checks
                }

                // Check visibility if required
                if (!onlyVisible || (winInfo.dwStyle & WS_VISIBLE) != 0)
                {
                    allWindows[hWnd] = windowTitle;
                }
            }

            return allWindows;
        }

        /// <summary>
        /// Determines if a window class name is a known system window class
        /// </summary>
        private static bool IsSystemWindowClass(string className)
        {
            // Common system window class names
            string[] systemClasses = {
                "Shell_TrayWnd",        // Taskbar
                "DummyDWMListenerWindow", // DWM related
                "WorkerW",              // Desktop
                "Progman",              // Program Manager
                "SysShadow",            // Shadow windows
                "NotifyIconOverflowWindow", // System tray
                "Shell_SecondaryTrayWnd", // Secondary taskbar
                "Button",               // Generic button
                "tooltips_class",       // Tooltips
                "#32768",               // Menu
                "ToolbarWindow32",      // Toolbar
                "CiceroUIWndFrame",     // Input method
                "TrayNotifyWnd",        // System tray notification
                "SysListView32",        // ListView control
                "FolderView",           // Explorer folder view
                "MSTaskSwWClass",       // Task Switcher
                "Windows.UI.Core.CoreWindow", // UWP core window
            };

            return systemClasses.Contains(className);
        }

        // These methods are deprecated and will be removed in a future version
        [Obsolete("This method is deprecated. Use GetApplicationWindows() instead.")]
        public static List<Window> GetAllRunningApps(IgnoreListManager ignoreListManager)
        {
            throw new NotImplementedException("This method is deprecated. Use WindowManager.GetRunningApps() instead.");
        }

        [Obsolete("This method is deprecated. Use GetApplicationWindows() instead.")]
        public static List<Window> GetVisibleRunningApps(IgnoreListManager ignoreListManager)
        {
            throw new NotImplementedException("This method is deprecated. Use WindowManager.GetRunningApps(true) instead.");
        }

        [Obsolete("This method is deprecated. Use GetApplicationWindows() instead.")]
        public static Dictionary<string, IntPtr> GetAllMainWindowHandles()
        {
            throw new NotImplementedException("This method is deprecated. Use GetApplicationWindows() instead.");
        }
        #endregion


        #region Get Window Handle
        public static IntPtr GetWindowHandleByWindowAndProcess(Window window, IgnoreListManager ignoreListManager)
        {
            // Try to find by window title first
            if (!string.IsNullOrWhiteSpace(window.TitleName))
            {
                // Try to find window directly by title using FindWindowByTitle
                IntPtr hWnd = FindWindowByTitle(window.TitleName);
                if (hWnd != IntPtr.Zero)
                {
                    return hWnd;
                }

                // If direct find failed, try to find by enumerating windows
                var allWindowHandles = new Dictionary<string, IntPtr>();

                EnumWindows(delegate (IntPtr hwnd, IntPtr lParam)
                {
                    StringBuilder title = new StringBuilder(256);
                    GetWindowText(hwnd, title, title.Capacity);

                    if (!string.IsNullOrWhiteSpace(title.ToString()))
                    {
                        allWindowHandles[title.ToString()] = hwnd;
                    }

                    return true;
                }, IntPtr.Zero);

                // Look for exact match
                if (allWindowHandles.ContainsKey(window.TitleName))
                {
                    return allWindowHandles[window.TitleName];
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
                catch
                {
                    // Silently handle exception
                }
            }
            return IntPtr.Zero;
        }

        public static IntPtr FindWindowByTitle(string windowTitle)
        {
            if (string.IsNullOrWhiteSpace(windowTitle))
            {
                return IntPtr.Zero;
            }

            IntPtr result = IntPtr.Zero;

            EnumWindows((hWnd, lParam) =>
            {
                StringBuilder title = new StringBuilder(256);
                GetWindowText(hWnd, title, title.Capacity);

                if (title.ToString() == windowTitle)
                {
                    result = hWnd;
                    return false; // Stop enumerating
                }

                return true;
            }, IntPtr.Zero);

            return result;
        }
        #endregion


        #region Get Window Title
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
                catch
                {
                    return "";
                }
            }

            return ""; // Window not found
        }

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
        public static WindowPosAndSize GetWindowPositionAndSize(Window window)
        {
            var windowPosAndSize = new WindowPosAndSize();
            if (window.IsFileExplorer)
            {
                windowPosAndSize = GetFileExplorerWindowPosAndSize(window);
            }
            else
            {
                GetWindowRect(window.hWnd, out var rect);
                windowPosAndSize = new WindowPosAndSize
                {
                    X = rect.Left,
                    Y = rect.Top,
                    Width = rect.Right - rect.Left,
                    Height = rect.Bottom - rect.Top
                };
            }

            return windowPosAndSize;
        }

        public static void SetWindowPositionAndSize(Window window, WindowPosAndSize posAndSize)
        {
            if (window == null || (!window.IsFileExplorer && window.hWnd == IntPtr.Zero) ||
                string.IsNullOrEmpty(window.TitleName))
                return;

            if (window.IsFileExplorer)
            {
                SetFileExplorerWindowPosAndSize(window, posAndSize);
            }
            else
            {
                SetWindowPos(window.hWnd, IntPtr.Zero, posAndSize.X, posAndSize.Y,
                    posAndSize.Width, posAndSize.Height, SWP_NOZORDER | SWP_SHOWWINDOW);
            }
        }
        #endregion


        #region Set Window Always On Top
        public static void SetWindowAlwaysOnTop(IntPtr hWnd)
        {
            SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
            SetForegroundWindow(hWnd);
        }

        public static void UnsetWindowAlwaysOnTop(IntPtr hWnd)
        {
            SetWindowPos(hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
            SetWindowPos(hWnd, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        }
        #endregion


        #region File Explorer Specific Methods
        public static List<(IntPtr Handle, string Title, string ProcessName, bool IsFileExplorer, WindowPosAndSize Position)> GetFileExplorerWindows()
        {
            List<(IntPtr Handle, string Title, string ProcessName, bool IsFileExplorer, WindowPosAndSize Position)> fileExplorers = new List<(IntPtr Handle, string Title, string ProcessName, bool IsFileExplorer, WindowPosAndSize Position)>();

            try
            {
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

                            // Create position and size info
                            WindowPosAndSize posAndSize = new WindowPosAndSize
                            {
                                X = window.Left,
                                Y = window.Top,
                                Width = window.Width,
                                Height = window.Height
                            };

                            // Add to the list with null handle (FileExplorer windows are handled differently)
                            fileExplorers.Add((IntPtr.Zero, windowTitle, "explorer", true, posAndSize));
                        }
                    }
                    catch
                    {
                        // Silently handle exceptions
                    }
                }
            }
            catch
            {
                // Silently handle exceptions in case COM components aren't available
            }

            return fileExplorers;
        }

        private static WindowPosAndSize GetFileExplorerWindowPosAndSize(Window window)
        {
            string windowTitle = window.TitleName;
            var windowPosAndSize = new WindowPosAndSize();
            foreach (SHDocVw.InternetExplorer fileExplorerWindow in new SHDocVw.ShellWindows())
            {
                // Skip minimized windows
                if (fileExplorerWindow.Left == -32000) continue;

                if (Path.GetFileNameWithoutExtension(fileExplorerWindow.FullName).ToLowerInvariant() == "explorer"
                    && $"FileExplorer: {fileExplorerWindow.Name} - {fileExplorerWindow.LocationName}" == windowTitle)
                {
                    windowPosAndSize.X = fileExplorerWindow.Left;
                    windowPosAndSize.Y = fileExplorerWindow.Top;
                    windowPosAndSize.Width = fileExplorerWindow.Width;
                    windowPosAndSize.Height = fileExplorerWindow.Height;
                    return windowPosAndSize;
                }
            }
            return windowPosAndSize;
        }

        private static void SetFileExplorerWindowPosAndSize(Window window, WindowPosAndSize windowPosAndSize)
        {
            string windowTitle = window.TitleName;
            foreach (SHDocVw.InternetExplorer fileExplorerWindow in new SHDocVw.ShellWindows())
            {
                if (Path.GetFileNameWithoutExtension(fileExplorerWindow.FullName).ToLowerInvariant() == "explorer"
                    && $"FileExplorer: {fileExplorerWindow.Name} - {fileExplorerWindow.LocationName}" == windowTitle)
                {
                    fileExplorerWindow.Left = windowPosAndSize.X;
                    fileExplorerWindow.Top = windowPosAndSize.Y;
                    fileExplorerWindow.Width = windowPosAndSize.Width;
                    fileExplorerWindow.Height = windowPosAndSize.Height;
                    return;
                }
            }
        }
        #endregion


        #region Helper Methods
        public static bool IsWindowMinimized(Window window)
        {
            GetWindowRect(window.hWnd, out var rect);
            return (rect.Left <= -32000 || rect.Top <= -32000);
        }

        public static (int Width, int Height) GetScreenDimensions()
        {
            return (Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
        }
        #endregion
    }

    internal class WindowPosAndSize
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public bool IsPercentageBased;

        public void ConvertToPercentages(int screenWidth, int screenHeight)
        {
            if (!IsPercentageBased)
            {
                X = (int)((float)X / screenWidth * 100);
                Y = (int)((float)Y / screenHeight * 100);
                Width = (int)((float)Width / screenWidth * 100);
                Height = (int)((float)Height / screenHeight * 100);
                IsPercentageBased = true;
            }
        }

        public void ConvertToPixels(int screenWidth, int screenHeight)
        {
            if (IsPercentageBased)
            {
                X = (int)((float)X * screenWidth / 100);
                Y = (int)((float)Y * screenHeight / 100);
                Width = (int)((float)Width * screenWidth / 100);
                Height = (int)((float)Height * screenHeight / 100);
                IsPercentageBased = false;
            }
        }
    }

    // Add the RECT struct
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}
