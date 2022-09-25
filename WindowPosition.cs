using System;
using System.Diagnostics;
using System.Runtime.InteropServices; // For the P/Invoke signatures.

namespace Save_Window_Position_and_Size
{
    public class WindowPosition
    {
        // P/Invoke declarations.

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        const UInt32 SWP_NOZORDER = 0x0004;
        const UInt32 SWP_SHOWWINDOW = 0x0040;
        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 SWP_NOACTIVATE = 0x0010;
        const int SW_RESTORE = 9;
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly IntPtr HWND_TOP = new IntPtr(0);
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);


        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern bool SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern int GetWindowRect(IntPtr hwnd, out Rectangle rect);
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);


        // Get window handle for "hidden" windows
        delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn,
            IntPtr lParam);

        static IEnumerable<IntPtr> EnumerateProcessWindowHandles(int processId)
        {
            var handles = new List<IntPtr>();

            foreach (ProcessThread thread in Process.GetProcessById(processId).Threads)
                EnumThreadWindows(thread.Id,
                    (hWnd, lParam) => { handles.Add(hWnd); return true; }, IntPtr.Zero);

            return handles;
        }


        public List<string> GetAllRunningApps()
        {
            var runningApps = new List<string>();
            var runningProcesses = new List<string>();

            Process.GetProcesses().ToList().ForEach(p =>
            {
                if (p.MainWindowTitle.Length > 0 && p.MainWindowTitle != "Microsoft Text Input Application" && p.MainWindowTitle != "Settings" && p.MainWindowTitle != "Documents")
                {
                    var rect = GetWindowPositionAndSize(p.MainWindowTitle);
                    if (rect.X == 0 && rect.Y == 0 && rect.Width == 0 && rect.Height == 0)
                    {
                        rect = GetWindowPositionAndSize(p.ProcessName);
                        runningApps.Add(p.ProcessName);
                    }
                    else
                        runningApps.Add(p.MainWindowTitle);

                }
                else if (p.MainWindowTitle.Length == 0 && p.HandleCount > 0)
                {
                    try
                    {
                        // Get any "hidden" windows
                        var windowHandles = EnumerateProcessWindowHandles(p.Id).ToList();
                        foreach(var windowHandle in windowHandles)
                        {
                            var windowSize = GetWindowPositionAndSize(windowHandle);
                            if (windowHandle != IntPtr.Zero && windowSize.Height - windowSize.Y > 110 )
                            {
                                runningProcesses.Add(p.ProcessName);
                                break;
                            }
                        }

                    }
                    catch (Exception e) { }
                }

            });

            // Add Windows File Explorer's; if any
            var fileExplorers = GetFileExplorerWindows();

            foreach (var file in fileExplorers)
                runningApps.Add(file);

            // Add all running processes
            runningApps.AddRange(runningProcesses);

            return runningApps;
        }

        public bool SetWindowPositionAndSize(string windowTitle, int x, int y, int width, int height)
        {
            var hWnds = GetWindowHandles(windowTitle);
            var moved = false;

            foreach(var hWnd in hWnds)
            {
                if (hWnd != IntPtr.Zero)
                {
                    SetWindowPos(hWnd, new IntPtr(), x, y, width, height, SWP_NOZORDER);
                    moved = true;
                }
            }
            return moved;
        }

        private List<IntPtr> GetWindowHandles(string windowClass, string? windowTitle = "")
        {
            var hWnds = new List<IntPtr>();
            var hWnd = new IntPtr();

            // Try to get the process by name
            var proc = Process.GetProcessesByName(windowClass).Where(p => p.ProcessName == windowClass).FirstOrDefault();
            if (proc != null && proc.MainWindowHandle == IntPtr.Zero)
            {
                // Get any "hidden" windows
                hWnds = EnumerateProcessWindowHandles(proc.Id).ToList();
                if (hWnds.Count > 0 && hWnds[0] != IntPtr.Zero)
                    return hWnds;

            }

            // Find (the first-in-Z-order) Notepad window.
            if (windowTitle.Length > 0)
            {
                hWnd = FindWindow(windowTitle, null); 

                if (hWnd == IntPtr.Zero)
                    hWnd = FindWindow(null, windowTitle);

                if (hWnd != IntPtr.Zero)
                {
                    hWnds.Add(hWnd);
                    return hWnds;
                }
                
            }
            else if (windowClass.Length > 0)
            {
                hWnd = FindWindow(windowClass, null);

                if (hWnd == IntPtr.Zero)
                    hWnd = FindWindow(null, windowClass);

                if (hWnd != IntPtr.Zero)
                {
                    hWnds.Add(hWnd);
                    return hWnds;
                }
            }
            else if (windowTitle.Length > 0 && windowClass.Length > 0)
            {
                hWnd = FindWindow(windowClass, windowTitle);

                if (hWnd == IntPtr.Zero)
                    hWnd = FindWindow(windowClass, windowTitle);

                if (hWnd != IntPtr.Zero)
                {
                    hWnds.Add(hWnd);
                    return hWnds;
                }
            }

            hWnds.Add(hWnd);
            return hWnds;
        }



        public WindowPosAndSize GetWindowPositionAndSize(string windowClass, string? windowTitle = "")
        {
            var windowHandles = GetWindowHandles(windowClass, windowTitle);
            foreach (var windowHandle in windowHandles)
            {
                var rect = GetWindowPositionAndSize(windowHandle);
                if (windowHandle != IntPtr.Zero && IsValidWindow(rect) )
                {
                    return ConvertRectToWindowPosAndSize(rect);
                }
            }

            return new WindowPosAndSize();
        }

        public Rectangle GetWindowPositionAndSize(IntPtr hWnd)
        {
            GetWindowRect(hWnd, out var rect);
            return rect;
        }

        private bool IsValidWindow(Rectangle rect)
        {
            if (rect.Height - rect.Y > 110)
                return true;

            return false;
        }

        public WindowPosAndSize ConvertRectToWindowPosAndSize(Rectangle rect)
        {
            var windowPosAndSize = new WindowPosAndSize();
            windowPosAndSize.X = rect.Location.X;
            windowPosAndSize.Y = rect.Location.Y;
            windowPosAndSize.Width = rect.Width - rect.X;
            windowPosAndSize.Height = rect.Height - rect.Y;
            return windowPosAndSize;
        }

        public void SetWindowAlwaysOnTop(string windowTitle)
        {
            var hwnd = GetWindowHandles(windowTitle)[0];
            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
            SetForegroundWindow(hwnd);
            ShowWindow(hwnd, SW_RESTORE);
        }
        public void UnsetWindowAlwaysOnTop(string windowTitle)
        {
            var hwnd = GetWindowHandles(windowTitle)[0];
            SetWindowPos(hwnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
            SetWindowPos(hwnd, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        }



        // File Explorer Specific
        public List<string> GetFileExplorerWindows()
        {
            var windows = new List<string>();

            foreach (SHDocVw.InternetExplorer window in new SHDocVw.ShellWindows())
            {
                if (Path.GetFileNameWithoutExtension(window.FullName).ToLowerInvariant() == "explorer")
                {
                    var appName = window.Name + " - " + window.LocationName;
                    windows.Add(appName);
                }
            }

            return windows;
        }
        public WindowPosAndSize GetFileExplorerWindow(string windowTitle)
        {
            var windowPosAndSize = new WindowPosAndSize();

            foreach (SHDocVw.InternetExplorer window in new SHDocVw.ShellWindows())
            {
                if (Path.GetFileNameWithoutExtension(window.FullName).ToLowerInvariant() == "explorer" && window.Name + " - " + window.LocationName == windowTitle)
                {
                    windowPosAndSize.X = window.Left;
                    windowPosAndSize.Y = window.Top;
                    windowPosAndSize.Width = window.Width;
                    windowPosAndSize.Height = window.Height;
                }
            }

            return windowPosAndSize;
        }
        public void SetFileExplorerWindowPosAndSize(string windowTitle, WindowPosAndSize windowPosAndSize)
        {
            foreach (SHDocVw.InternetExplorer window in new SHDocVw.ShellWindows())
            {
                if (Path.GetFileNameWithoutExtension(window.FullName).ToLowerInvariant() == "explorer" && window.Name + " - " + window.LocationName == windowTitle)
                {
                    window.Left = windowPosAndSize.X;
                    window.Top = windowPosAndSize.Y;
                    window.Width = windowPosAndSize.Width;
                    window.Height = windowPosAndSize.Height;
                    return;
                }
            }
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
            if(win1.X == win2.X && win1.Y == win2.Y && win1.Width == win2.Width && win1.Height == win2.Height)
                return true;
            
            return false;
        }
    }
}
