using System;
using System.Diagnostics;
using System.Runtime.InteropServices; // For the P/Invoke signatures.
using System.Text;

namespace Save_Window_Position_and_Size.Classes
{
    public static class InteractWithWindow
    {
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
        private const int GWL_STYLE = -16;
        private const int WS_VISIBLE = 0x10000000;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

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
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);


        // Public Actions
        public static Dictionary<IntPtr, string> GetAllRunningApps(List<string> exceptions)
        {
            var allWindows = new Dictionary<IntPtr, string>();

            foreach (var dicWin in GetAllMainWindowHandles().Where(dicWin => !exceptions.Contains(dicWin.Key)))
            {
                int windowStyle = GetWindowLong(dicWin.Value, GWL_STYLE);
                if ((windowStyle & WS_VISIBLE) == WS_VISIBLE)
                {
                    allWindows[dicWin.Value] = dicWin.Key;
                }
            }

            return allWindows;
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
        public static IntPtr FindWindowByTitle(string windowTitle)
        {
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


        // File Explorer Specific
        public static List<string> GetFileExplorerWindows(List<string> exceptions)
        {
            var windows = new List<string>();

            foreach (SHDocVw.InternetExplorer window in new SHDocVw.ShellWindows())
            {
                if (Path.GetFileNameWithoutExtension(window.FullName).ToLowerInvariant() == "explorer")
                {
                    // Skip minimized windows
                    if (window.Left == -32000) continue;

                    var appName = $"FileExplorer: {window.Name} - {window.LocationName}";
                    windows.Add(appName);
                }
            }

            return windows;
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
                }
            }

            return windowPosAndSize;
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


        // Helpers
        public static Dictionary<string, IntPtr> GetAllMainWindowHandles()
        {
            var windowHandles = new Dictionary<string, IntPtr>();

            EnumWindows(delegate (IntPtr hWnd, IntPtr lParam)
            {
                StringBuilder title = new StringBuilder(256);
                GetWindowText(hWnd, title, title.Capacity);

                if (!string.IsNullOrWhiteSpace(title.ToString()))
                {
                    windowHandles[title.ToString()] = hWnd;
                }

                return true;
            }, IntPtr.Zero);

            return windowHandles;
        }
    }

    public class WindowPosAndSize
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
    }
}
