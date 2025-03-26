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
        const int SW_MINIMIZE = 6;
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly IntPtr HWND_TOP = new IntPtr(0);
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

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

            // Create the result dictionary with window handles as keys and titles as values
            var allWindows = new Dictionary<IntPtr, string>();

            // Now, get all regular application windows
            EnumWindows(delegate (IntPtr hWnd, IntPtr lParam)
            {
                StringBuilder title = new StringBuilder(256);
                GetWindowText(hWnd, title, title.Capacity);
                string windowTitle = title.ToString();

                if (string.IsNullOrWhiteSpace(windowTitle))
                    return true;

                // Skip based on exceptions list
                if (exceptions.Contains(windowTitle))
                    return true;

                // Get window info to check its properties
                WINDOWINFO winInfo = new WINDOWINFO();
                winInfo.cbSize = (uint)Marshal.SizeOf(typeof(WINDOWINFO));
                if (!GetWindowInfo(hWnd, ref winInfo))
                    return true;

                // Skip windows that are too small to be real application windows
                int width = winInfo.rcWindow.Right - winInfo.rcWindow.Left;
                int height = winInfo.rcWindow.Bottom - winInfo.rcWindow.Top;
                if (width < 50 || height < 50)
                    return true;

                // Skip system tray windows and toolbars
                // We only want windows with app window style or those that don't have tool window style
                if ((winInfo.dwExStyle & WS_EX_TOOLWINDOW) != 0 && (winInfo.dwExStyle & WS_EX_APPWINDOW) == 0)
                    return true;

                // Skip child windows since top-level application windows aren't children
                if ((winInfo.dwStyle & WS_CHILD) != 0)
                    return true;

                // Get class name - many system windows have specific class names
                StringBuilder className = new StringBuilder(256);
                GetClassName(hWnd, className, className.Capacity);
                string windowClass = className.ToString();

                // Skip common system window classes
                if (IsSystemWindowClass(windowClass))
                    return true;

                // Check window owner - real apps usually don't have owners
                IntPtr owner = GetWindow(hWnd, GW_OWNER);
                if (owner != IntPtr.Zero)
                {
                    // It has an owner, check if the owner is visible
                    // If the owner is invisible, this is likely a system window
                    if (!IsWindowVisible(owner))
                        return true;
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
                            // Skip explorer windows as we've already handled them separately
                            if (process.ProcessName.ToLowerInvariant() == "explorer")
                            {
                                // Check if this is a file explorer window 
                                if (windowTitle.Contains(":\\") || windowTitle.Contains("This PC") ||
                                    windowTitle.StartsWith("File Explorer", StringComparison.OrdinalIgnoreCase))
                                {
                                    // Skip as we've already collected File Explorer windows through Shell32
                                    return true;
                                }
                            }

                            // For browsers and other multi-window applications, don't skip windows
                            // that aren't the main window
                            bool isKnownMultiWindowApp = IsMultiWindowApplication(process.ProcessName);

                            // If it's not a multi-window app, apply the main window check
                            if (!isKnownMultiWindowApp &&
                                process.MainWindowHandle != IntPtr.Zero &&
                                process.MainWindowHandle != hWnd &&
                                process.ProcessName.ToLowerInvariant() != "explorer")
                            {
                                // Skip if the window style suggests it's not a main window
                                if ((winInfo.dwExStyle & WS_EX_APPWINDOW) == 0)
                                    return true;
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
                    // Make sure we don't already have this window from the file explorer collection
                    if (!allWindows.ContainsKey(hWnd))
                    {
                        allWindows[hWnd] = windowTitle;
                    }
                }

                return true;
            }, IntPtr.Zero);

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

        /// <summary>
        /// Determines if a process name belongs to an application known to support multiple windows
        /// </summary>
        private static bool IsMultiWindowApplication(string processName)
        {
            if (string.IsNullOrEmpty(processName))
                return false;

            processName = processName.ToLowerInvariant();

            // List of applications known to support multiple windows
            string[] multiWindowApps = {
                "firefox",
                "chrome",
                "msedge",
                "iexplore",
                "brave",
                "opera",
                "vivaldi",
                "safari",
                "notepad++",
                "code",          // VS Code
                "devenv",        // Visual Studio
                "rider",         // JetBrains Rider
                "pycharm",       // JetBrains PyCharm
                "intellij",      // JetBrains IntelliJ
                "webstorm",      // JetBrains WebStorm
                "atom",          // Atom Editor
                "sublime_text",  // Sublime Text
                "windowsterminal", // Windows Terminal
                "powershell",
                "cmd",
                "putty"
            };

            return multiWindowApps.Contains(processName);
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

                // Look for partial matches for applications like Firefox
                foreach (var entry in allWindowHandles)
                {
                    if (entry.Key.Contains(window.TitleName))
                    {
                        return entry.Value;
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

                    // For multi-window applications (like browsers), enumerate all windows
                    // belonging to this process and find matching titles
                    if (processes.Length > 0)
                    {
                        var windowsByProcess = new Dictionary<IntPtr, string>();

                        EnumWindows(delegate (IntPtr hWnd, IntPtr lParam)
                        {
                            uint processId;
                            GetWindowThreadProcessId(hWnd, out processId);

                            if (processes.Any(p => p.Id == processId))
                            {
                                // This window belongs to our target process
                                if (IsWindowVisible(hWnd))
                                {
                                    StringBuilder title = new StringBuilder(256);
                                    GetWindowText(hWnd, title, title.Capacity);
                                    string windowTitle = title.ToString();

                                    if (!string.IsNullOrWhiteSpace(windowTitle))
                                    {
                                        windowsByProcess[hWnd] = windowTitle;
                                    }
                                }
                            }

                            return true;
                        }, IntPtr.Zero);

                        // Try to match with window title if specified
                        if (!string.IsNullOrWhiteSpace(window.TitleName))
                        {
                            foreach (var pair in windowsByProcess)
                            {
                                // Check for exact title match
                                if (pair.Value == window.TitleName)
                                {
                                    return pair.Key;
                                }

                                // Check for partial title match
                                if (pair.Value.Contains(window.TitleName))
                                {
                                    return pair.Key;
                                }

                                // For browser windows, the title might be "Page Title - Browser Name"
                                if (window.TitleName.Contains(pair.Value))
                                {
                                    return pair.Key;
                                }
                            }
                        }

                        // If we have windows for this process but none match the title,
                        // return the first visible one
                        if (windowsByProcess.Count > 0)
                        {
                            return windowsByProcess.Keys.First();
                        }

                        // If all else fails, try the main window handle
                        if (processes[0].MainWindowHandle != IntPtr.Zero)
                        {
                            return processes[0].MainWindowHandle;
                        }
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


        #region Get Window Title/Process Name
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

        /// <summary>
        /// Attempts to move windows that resist standard movement methods (like D3D applications)
        /// </summary>
        private static bool MoveStubornWindow(Window window, WindowPosAndSize posAndSize)
        {
            // First: Try using the direct approach with different combination of flags
            for (int attempt = 0; attempt < 3; attempt++)
            {
                uint flags = SWP_SHOWWINDOW;
                IntPtr insertAfter = IntPtr.Zero;

                if (attempt == 1)
                {
                    // Second attempt: Make sure window is restored and has focus first
                    ShowWindow(window.hWnd, SW_RESTORE);
                    SetForegroundWindow(window.hWnd);
                    insertAfter = HWND_TOP;
                    flags = SWP_SHOWWINDOW;
                }
                else if (attempt == 2)
                {
                    // Third attempt: Try topmost
                    insertAfter = HWND_TOPMOST;
                    flags = SWP_SHOWWINDOW;

                    // Wait a moment before trying (some apps need this)
                    System.Threading.Thread.Sleep(50);
                }

                SetWindowPos(window.hWnd, insertAfter, posAndSize.X, posAndSize.Y,
                    posAndSize.Width, posAndSize.Height, flags);

                // Check if it moved
                GetWindowRect(window.hWnd, out RECT rect);
                if (rect.Left == posAndSize.X && rect.Top == posAndSize.Y)
                {
                    // Restore non-topmost state if necessary
                    if (attempt == 2)
                    {
                        SetWindowPos(window.hWnd, HWND_NOTOPMOST, 0, 0, 0, 0,
                            SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
                    }
                    return true;
                }
            }

            // Second: Get window class to adapt strategy
            StringBuilder className = new StringBuilder(256);
            GetClassName(window.hWnd, className, className.Capacity);
            string windowClass = className.ToString();

            // Get process info
            uint processId;
            GetWindowThreadProcessId(window.hWnd, out processId);
            string processName = "";

            try
            {
                using (Process process = Process.GetProcessById((int)processId))
                {
                    processName = process.ProcessName.ToLower();
                }
            }
            catch
            {
                // Process may have exited
            }

            // Try to find all windows with the same title
            var windowsWithSameTitle = new Dictionary<IntPtr, string>();
            EnumWindows(delegate (IntPtr hWnd, IntPtr lParam)
            {
                StringBuilder title = new StringBuilder(256);
                GetWindowText(hWnd, title, title.Capacity);
                string windowTitle = title.ToString();

                if (windowTitle == window.TitleName)
                {
                    windowsWithSameTitle[hWnd] = windowTitle;
                }
                return true;
            }, IntPtr.Zero);

            // Try to move each window with the same title using different strategies
            foreach (var hwnd in windowsWithSameTitle.Keys)
            {
                if (hwnd != window.hWnd && IsWindowVisible(hwnd))
                {
                    // First try to bring the window to the foreground
                    ShowWindow(hwnd, SW_RESTORE);
                    SetForegroundWindow(hwnd);

                    // Try with different Z-orders and flags
                    for (int i = 0; i < 3; i++)
                    {
                        IntPtr insertAfter = (i == 0) ? IntPtr.Zero : (i == 1) ? HWND_TOP : HWND_TOPMOST;
                        uint flags = SWP_SHOWWINDOW;

                        SetWindowPos(hwnd, insertAfter, posAndSize.X, posAndSize.Y,
                            posAndSize.Width, posAndSize.Height, flags);

                        // Check if it moved
                        GetWindowRect(hwnd, out RECT rect);
                        if (rect.Left == posAndSize.X && rect.Top == posAndSize.Y)
                        {
                            // Restore non-topmost state if necessary
                            if (i == 2)
                            {
                                SetWindowPos(hwnd, HWND_NOTOPMOST, 0, 0, 0, 0,
                                    SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
                            }
                            return true; // Successfully moved a window
                        }
                    }
                }
            }

            // If title-based approach failed, try by process relationships with broader criteria
            if (!string.IsNullOrEmpty(window.ProcessName))
            {
                // Find all processes with similar names (base name or variants)
                var relatedProcesses = new List<Process>();
                try
                {
                    // Get the base name (without any potential suffixes)
                    string baseName = window.ProcessName;
                    const string win64Suffix = "-Win64-Shipping";

                    if (baseName.EndsWith(win64Suffix, StringComparison.OrdinalIgnoreCase))
                    {
                        baseName = baseName.Substring(0, baseName.Length - win64Suffix.Length);
                    }

                    // Find all processes that might be related
                    foreach (var process in Process.GetProcesses())
                    {
                        try
                        {
                            // Check if the process name starts with the base name OR
                            // if the process is a known companion process
                            if (process.ProcessName.StartsWith(baseName, StringComparison.OrdinalIgnoreCase) ||
                                (process.ProcessName.EndsWith("-Win64-Shipping", StringComparison.OrdinalIgnoreCase) &&
                                 process.ProcessName.StartsWith(baseName, StringComparison.OrdinalIgnoreCase)) ||
                                process.ProcessName == baseName + "-Win64-Shipping" ||
                                process.ProcessName == baseName.Replace("-Win64-Shipping", ""))
                            {
                                relatedProcesses.Add(process);
                            }
                        }
                        catch
                        {
                            // Skip this process if we can't access its name
                        }
                    }

                    // Try to move each related process's window using multiple techniques
                    foreach (var process in relatedProcesses)
                    {
                        if (process.MainWindowHandle != IntPtr.Zero && IsWindowVisible(process.MainWindowHandle))
                        {
                            // First try to restore and focus the window
                            ShowWindow(process.MainWindowHandle, SW_RESTORE);
                            SetForegroundWindow(process.MainWindowHandle);

                            // Try multiple approaches with different z-order
                            for (int i = 0; i < 3; i++)
                            {
                                IntPtr insertAfter = (i == 0) ? IntPtr.Zero : (i == 1) ? HWND_TOP : HWND_TOPMOST;
                                uint flags = SWP_SHOWWINDOW;

                                // Try to move this window
                                SetWindowPos(process.MainWindowHandle, insertAfter,
                                    posAndSize.X, posAndSize.Y,
                                    posAndSize.Width, posAndSize.Height, flags);

                                // Check if it moved
                                GetWindowRect(process.MainWindowHandle, out RECT rect);
                                if (rect.Left == posAndSize.X && rect.Top == posAndSize.Y)
                                {
                                    // Restore non-topmost state if necessary
                                    if (i == 2)
                                    {
                                        SetWindowPos(process.MainWindowHandle, HWND_NOTOPMOST, 0, 0, 0, 0,
                                            SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
                                    }
                                    return true; // Successfully moved a window
                                }
                            }

                            // If the main window didn't move, try to find and move other windows of this process
                            EnumWindows(delegate (IntPtr hWnd, IntPtr lParam)
                            {
                                uint windowProcessId;
                                GetWindowThreadProcessId(hWnd, out windowProcessId);

                                if (windowProcessId == process.Id && IsWindowVisible(hWnd) &&
                                    hWnd != process.MainWindowHandle)
                                {
                                    // Try to move this alternate window with different z-order options
                                    for (int j = 0; j < 3; j++)
                                    {
                                        IntPtr insertAfter = (j == 0) ? IntPtr.Zero : (j == 1) ? HWND_TOP : HWND_TOPMOST;
                                        uint flags = SWP_SHOWWINDOW;

                                        SetWindowPos(hWnd, insertAfter, posAndSize.X, posAndSize.Y,
                                            posAndSize.Width, posAndSize.Height, flags);

                                        // Check if it moved
                                        GetWindowRect(hWnd, out RECT rect);
                                        if (rect.Left == posAndSize.X && rect.Top == posAndSize.Y)
                                        {
                                            // Restore non-topmost state if necessary
                                            if (j == 2)
                                            {
                                                SetWindowPos(hWnd, HWND_NOTOPMOST, 0, 0, 0, 0,
                                                    SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
                                            }
                                            return false; // Stop enumeration
                                        }
                                    }
                                }
                                return true; // Continue enumeration
                            }, IntPtr.Zero);
                        }
                    }
                }
                catch
                {
                    // Silently handle exceptions
                }

                // Cleanup
                foreach (var process in relatedProcesses)
                {
                    try { process.Dispose(); } catch { }
                }
            }

            return false; // Failed to move any window
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

        #region Window Operations
        /// <summary>
        /// Minimizes a window
        /// </summary>
        /// <param name="hWnd">Window handle to minimize</param>
        public static void MinimizeWindow(IntPtr hWnd)
        {
            if (hWnd != IntPtr.Zero && IsWindow(hWnd))
            {
                ShowWindow(hWnd, SW_MINIMIZE);
            }
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
