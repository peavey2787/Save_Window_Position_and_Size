using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Net;
using Save_Window_Position_and_Size.Classes;
using static Save_Window_Position_and_Size.Classes.WindowHighlighter;
using Newtonsoft.Json;

namespace Save_Window_Position_and_Size.Classes
{

    internal class InteractWithWindow
    {
        IgnoreListManager ignoreListManager;

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
        internal struct WINDOWINFO
        {
            internal uint cbSize;
            internal RECT rcWindow;
            internal RECT rcClient;
            internal uint dwStyle;
            internal uint dwExStyle;
            internal uint dwWindowStatus;
            internal uint cxWindowBorders;
            internal uint cyWindowBorders;
            internal ushort atomWindowType;
            internal ushort wCreatorVersion;
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
        internal static extern bool SetForegroundWindow(IntPtr hwnd);

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

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out RECT rect);

        [DllImport("user32.dll")]
        private static extern IntPtr GetAncestor(IntPtr hWnd, uint gaFlags);

        [DllImport("dwmapi.dll")]
        private static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out int pvAttribute, int cbAttribute);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetParent(IntPtr hWnd);

        private const uint GA_ROOTOWNER = 3;
        private const int DWMWA_CLOAKED = 14;
        #endregion

        #region Cache of all app windows
        private List<Window> _cachedAllAppWindows; // Holds the cached value
        private DateTime _lastFetchedTime; // Tracks the last fetch time
        private readonly TimeSpan _cacheDuration = TimeSpan.FromSeconds(5); // Cache timeout duration

        internal async Task<List<Window>> GetAllAppWindows(bool isVisibleOnly = false, List<string> exceptions = null)
        {
            if(exceptions == null)
            {
                ignoreListManager.LoadIgnoreList();
                exceptions = ignoreListManager.IgnoreList;
            }

            // Check if cache duration has expired
            if (_cachedAllAppWindows == null || (DateTime.Now - _lastFetchedTime) > _cacheDuration)
            {
                // Update cache
                _cachedAllAppWindows = await Task.Run( ()=> GetApplicationWindows(false, exceptions));
                _lastFetchedTime = DateTime.Now;
            }


            // Filter app windows
            var allAppWindows = new List<Window>();
            foreach (var appWindow in _cachedAllAppWindows)
            {
                // Visible check 
                bool isMinimized = IsIconic(appWindow.hWnd);
                
                if ((isVisibleOnly && !isMinimized) || !isVisibleOnly)
                {
                    allAppWindows.Add(appWindow);
                }
            }

            return allAppWindows;

        }
        #endregion

        // Callable methods for WindowManager
        #region Get Application Windows
        private static List<Window> GetApplicationWindows(bool isVisibleOnly = false, List<string> exceptions = null)
        {
            exceptions = exceptions ?? new List<string>();
            var windowsWithTaskbarIcons = new List<Window>();

            EnumWindows(delegate (IntPtr hWnd, IntPtr lParam)
            {
                // --- Basic visibility & cloaked checks ---
                bool isCurrentlyVisible = IsWindowVisible(hWnd);
                bool isMinimized = IsIconic(hWnd);
                if (!isCurrentlyVisible && !isMinimized) return true;

                int cloakedValue = 0;
                try { DwmGetWindowAttribute(hWnd, DWMWA_CLOAKED, out cloakedValue, sizeof(int)); } catch { }
                if (cloakedValue != 0) return true;

                // --- Taskbar button logic  ---
                IntPtr owner = GetWindow(hWnd, GW_OWNER);
                int exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
                bool hasTaskbarButton = ((exStyle & WS_EX_TOOLWINDOW) == 0) &&
                                        (owner == IntPtr.Zero || (exStyle & WS_EX_APPWINDOW) != 0);

                if (hasTaskbarButton)
                {
                    // --- isVisibleOnly check  ---
                    if ( (isVisibleOnly && !isMinimized) || !isVisibleOnly)
                    {
                        // --- Window size check  ---
                        if (GetWindowRect(hWnd, out RECT rect) != 0 && rect.Right > rect.Left && rect.Bottom > rect.Top)
                        {
                            // --- Get Title ---
                            int length = GetWindowTextLength(hWnd);
                            if (length > 0)
                            {
                                StringBuilder titleBuilder = new StringBuilder(length + 1);
                                GetWindowText(hWnd, titleBuilder, titleBuilder.Capacity);
                                string windowTitle = titleBuilder.ToString();

                                // --- Check Title/Exceptions ---
                                if (!string.IsNullOrWhiteSpace(windowTitle) && !exceptions.Contains(windowTitle))
                                {
                                    try
                                    {
                                        // --- Get Process ---
                                        uint processId;
                                        GetWindowThreadProcessId(hWnd, out processId);
                                        Process process = Process.GetProcessById((int)processId);

                                        if (process != null && !process.HasExited)
                                        {
                                            // --- File Explorer check ---
                                            bool isFileExplorerBrowser = false;
                                            if (process.ProcessName.Equals("explorer", StringComparison.OrdinalIgnoreCase))
                                            {
                                                StringBuilder classNameBuilder = new StringBuilder(256);
                                                if (GetClassName(hWnd, classNameBuilder, classNameBuilder.Capacity) > 0)
                                                {
                                                    if (classNameBuilder.ToString() == "CabinetWClass")
                                                    {
                                                        isFileExplorerBrowser = true;
                                                    }
                                                }
                                            }

                                            var window = new Window // Using Save_Window_Position_and_Size.Classes.Window
                                            {
                                                hWnd = hWnd,                 
                                                TitleName = windowTitle,     
                                                ProcessName = process.ProcessName, 
                                                IsFileExplorer = isFileExplorerBrowser,
                                                WindowPosAndSize = ConvertRectToWindowPosAndSize(rect)
                                            };

                                            // Add the populated custom object to the list
                                            windowsWithTaskbarIcons.Add(window);
                                        }
                                    }
                                    catch (ArgumentException) { }
                                    catch (Exception ex) 
                                    { 
                                        //Debug.WriteLine($"Error processing window '{windowTitle}' (hWnd: {hWnd}): {ex.Message}"); 
                                    }
                                }
                            }
                        }
                    }
                }
                return true; // Continue enumeration
            }, IntPtr.Zero);

            windowsWithTaskbarIcons = PopulateFileExplorerPaths(windowsWithTaskbarIcons);

            // Return the list of your custom Window objects
            return windowsWithTaskbarIcons;
        }
        #endregion
        private static WindowPosAndSize ConvertRectToWindowPosAndSize(RECT rect)
        {
            var windowPosAndSize = new WindowPosAndSize();
            windowPosAndSize.X = rect.Left;
            windowPosAndSize.Y = rect.Top;
            windowPosAndSize.Width = rect.Right - rect.Left;
            windowPosAndSize.Height = rect.Bottom - rect.Top;
            return windowPosAndSize;
        }

        #region Get Window Handle
        async internal Task<IntPtr> GetWindowHandle(Window window)
        {
            if (window == null)
                return IntPtr.Zero;

            IntPtr newHandle = IntPtr.Zero;

            var allAppWindows = await GetAllAppWindows();
            foreach (var appWindow in allAppWindows)
            {
                if (appWindow.IsSameWindowWithoutHandle(window))
                {
                    newHandle = appWindow.hWnd;
                    break;
                }
            }

            return newHandle;
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

                // Create the exact format to match window title
                string explorerTitle = $"FileExplorer: {fileExplorerWindow.Name} - {fileExplorerWindow.LocationName}";

                if (Path.GetFileNameWithoutExtension(fileExplorerWindow.FullName).ToLowerInvariant() == "explorer"
                    && explorerTitle == windowTitle)
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
            int movedInvisAndActualWindow = 0;
            foreach (SHDocVw.InternetExplorer fileExplorerWindow in new SHDocVw.ShellWindows())
            {
                if (Path.GetFileNameWithoutExtension(fileExplorerWindow.FullName).ToLowerInvariant() != "explorer")
                    continue;

                // Only work with windows that have an EXACT title match
                if (fileExplorerWindow.LocationName == window.TitleName)
                {
                    // Found exact matching window - set position
                    fileExplorerWindow.Left = windowPosAndSize.X;
                    fileExplorerWindow.Top = windowPosAndSize.Y;
                    fileExplorerWindow.Width = windowPosAndSize.Width;
                    fileExplorerWindow.Height = windowPosAndSize.Height;

                    movedInvisAndActualWindow++;
                }
                if(movedInvisAndActualWindow == 2) { break; }
            }
        }

        // Add file paths for any File Explorer windows
        private static List<Window> PopulateFileExplorerPaths(List<Window> windows)
        {
            try
            {
                // Skip if no windows or no explorer windows
                if (windows == null || windows.Count == 0 || !windows.Any(w => w.IsFileExplorer))
                    return windows;

                // Create a list of File Explorer windows to process
                var explorerWindows = windows.Where(w => w.IsFileExplorer).ToList();

                // Use SHDocVw to get paths
                foreach (SHDocVw.InternetExplorer fileExplorerWindow in new SHDocVw.ShellWindows())
                {
                    if (Path.GetFileNameWithoutExtension(fileExplorerWindow.FullName).ToLowerInvariant() != "explorer")
                        continue;

                    // Create matching window title
                    StringBuilder titleBuilder = new StringBuilder(256);
                    GetWindowText(new IntPtr(fileExplorerWindow.HWND), titleBuilder, titleBuilder.Capacity);
                    string windowTitle = titleBuilder.ToString();

                    // Find matching windows by title
                    foreach (var window in explorerWindows)
                    {
                        if (window.TitleName == windowTitle)
                        {
                            // Add file path and location URL to the window object
                            window.FilePath = fileExplorerWindow.LocationURL;
                            break;
                        }
                    }
                }
            }
            catch
            {
                // Silently handle exceptions during File Explorer path lookup
            }

            return windows;
        }

        internal static bool OpenFileExplorerWindow(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            try
            {
                // Check for special folder names and convert to real paths
                if (filePath.Equals("Downloads", StringComparison.OrdinalIgnoreCase))
                {
                    filePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";
                }
                else if (filePath.Equals("Documents", StringComparison.OrdinalIgnoreCase))
                {
                    filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }
                else if (filePath.Equals("Pictures", StringComparison.OrdinalIgnoreCase))
                {
                    filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                }

                // Check if the path contains special folder GUID
                if (filePath.Contains("::"))
                {
                    // Try to handle common special folders by GUID
                    if (filePath.Contains("::{374DE290-123F-4565-9164-39C4925E467B}"))
                    {
                        // This is the Downloads folder
                        filePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";
                    }
                }

                // Make sure path exists
                if (!Directory.Exists(filePath) && !File.Exists(filePath))
                {
                    // If the path doesn't exist as a direct path, but looks like a drive letter, try to open just the drive
                    if (filePath.Length >= 2 && filePath[1] == ':' && char.IsLetter(filePath[0]))
                    {
                        filePath = filePath.Substring(0, 2) + "\\";
                    }
                    else
                    {
                        // If it's not a valid path at all, return failure
                        return false;
                    }
                }

                // Open the Windows Explorer window with the path
                System.Diagnostics.Process.Start("explorer.exe", filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion


        #region Window Operations
        internal static WindowPosAndSize GetWindowPositionAndSize(Window window)
        {
            // Get current hWnd

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

        internal static void SetWindowPositionAndSize(Window window, WindowPosAndSize posAndSize)
        {
            if (window == null || (!window.IsFileExplorer && window.hWnd == IntPtr.Zero) ||
                string.IsNullOrEmpty(window.TitleName))
                return;

            if (window.ProcessName == "explorer")
            {
                SetFileExplorerWindowPosAndSize(window, posAndSize);
            }
            else
            {
                SetWindowPos(window.hWnd, IntPtr.Zero, posAndSize.X, posAndSize.Y,
                    posAndSize.Width, posAndSize.Height, SWP_NOZORDER | SWP_SHOWWINDOW);
            }
        }

        internal static void SetWindowAlwaysOnTop(IntPtr hWnd)
        {
            SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
            SetForegroundWindow(hWnd);
        }

        internal static void UnsetWindowAlwaysOnTop(IntPtr hWnd)
        {
            SetWindowPos(hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
            SetWindowPos(hWnd, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        }

        internal static bool IsWindowMinimized(Window window)
        {
            GetWindowRect(window.hWnd, out var rect);
            return (rect.Left <= -32000 || rect.Top <= -32000);
        }

        internal static (int Width, int Height) GetScreenDimensions()
        {
            return (Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
        }

        internal static void MinimizeWindow(IntPtr hWnd)
        {
            if (hWnd != IntPtr.Zero && IsWindow(hWnd))
            {
                ShowWindow(hWnd, SW_MINIMIZE);
            }
        }
        #endregion
    }

    [Serializable]
    internal class WindowPosAndSize
    {
        [JsonProperty]
        internal int X;
        [JsonProperty]
        internal int Y;
        [JsonProperty]
        internal int Width;
        [JsonProperty]
        internal int Height;
        [JsonProperty]
        internal bool IsPercentageBased;

        internal void ConvertToPercentages(int screenWidth, int screenHeight)
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

        internal void ConvertToPixels(int screenWidth, int screenHeight)
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
    internal struct RECT
    {
        internal int Left;
        internal int Top;
        internal int Right;
        internal int Bottom;
    }
}

