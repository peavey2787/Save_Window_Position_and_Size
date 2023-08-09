using System;
using System.Diagnostics;
using System.Runtime.InteropServices; // For the P/Invoke signatures.
using System.Text;

namespace Save_Window_Position_and_Size.Classes
{
    public class InteractWithWindow
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

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

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

        // Get window handle for "hidden" windows
        delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);
        static IEnumerable<IntPtr> EnumerateProcessWindowHandles(int processId)
        {
            var handles = new List<IntPtr>();

            foreach (ProcessThread thread in Process.GetProcessById(processId).Threads)
                EnumThreadWindows(thread.Id,
                    (hWnd, lParam) => { handles.Add(hWnd); return true; }, IntPtr.Zero);

            return handles;
        }



        // Public Actions
        public static Dictionary<IntPtr, String> GetAllRunningApps(List<string> exceptions)
        {
            Dictionary<IntPtr, String> allWindows = new Dictionary<IntPtr, String>();

            // Get all windows
            var dicWindows = GetAllMainWindowHandles();
            foreach(var dicWin in dicWindows)
            {
                // That are visible
                int windowStyle = GetWindowLong(dicWin.Value, GWL_STYLE);
                if ((windowStyle & WS_VISIBLE) == WS_VISIBLE)
                {
                    // And not on the ignore list
                    if (!exceptions.Contains(dicWin.Key))
                        allWindows[dicWin.Value] = dicWin.Key;
                }
            }

            return allWindows; //return runningApps;
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
            var windowPosAndSize = new WindowPosAndSize();
            windowPosAndSize.X = rect.Location.X;
            windowPosAndSize.Y = rect.Location.Y;
            windowPosAndSize.Width = rect.Width - rect.X;
            windowPosAndSize.Height = rect.Height - rect.Y;
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
        public static IntPtr GetMainWindowHandle(string windowTitle)
        {
            IntPtr mainWindowHandle = IntPtr.Zero;

            EnumWindows(delegate (IntPtr hWnd, IntPtr lParam)
            {
                StringBuilder className = new StringBuilder(256);
                GetClassName(hWnd, className, className.Capacity);

                if (className.ToString() == "ApplicationFrameWindow")
                {
                    EnumChildWindows(hWnd, delegate (IntPtr childHWnd, IntPtr lParamChild)
                    {
                        StringBuilder title = new StringBuilder(256);
                        GetWindowText(childHWnd, title, title.Capacity);

                        if (title.ToString() == windowTitle)
                        {
                            mainWindowHandle = childHWnd;
                            return false; // Stop enumerating child windows
                        }

                        return true;
                    }, IntPtr.Zero);
                }
                else
                {
                    StringBuilder title = new StringBuilder(256);
                    GetWindowText(hWnd, title, title.Capacity);

                    if (title.ToString() == windowTitle)
                    {
                        mainWindowHandle = hWnd;
                        return false; // Stop enumerating top-level windows
                    }
                }

                return true;
            }, IntPtr.Zero);

            return mainWindowHandle;
        }
        public static Dictionary<string, IntPtr> GetAllMainWindowHandles()
        {
            Dictionary<string, IntPtr> windowHandles = new Dictionary<string, IntPtr>();

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

        public bool CompareIsEqual(WindowPosAndSize win1, WindowPosAndSize win2)
        {
            if (win1.X == win2.X && win1.Y == win2.Y && win1.Width == win2.Width && win1.Height == win2.Height)
                return true;

            return false;
        }
    }
}
