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




        public bool SetWindowPositionAndSize(string windowTitle, int x, int y, int width, int height)
        {
            var hWnd = GetWindowHandle(windowTitle);

            if (hWnd != IntPtr.Zero)
            {
                SetWindowPos(hWnd, new IntPtr(), x, y, width, height, SWP_NOZORDER);
                return true;
            }
            return false;
        }

        private IntPtr GetWindowHandle(string windowClass, string? windowTitle = "")
        {
            var hWnd = new IntPtr();

            // Find (the first-in-Z-order) Notepad window.
            if (windowTitle.Length > 0)
            {
                hWnd = FindWindow(windowTitle, null);

                if (hWnd == IntPtr.Zero)
                    hWnd = FindWindow(null, windowTitle);

                if (hWnd != IntPtr.Zero)
                    return hWnd;
                
            }
            else if (windowClass.Length > 0)
            {
                hWnd = FindWindow(windowClass, null);

                if (hWnd == IntPtr.Zero)
                    hWnd = FindWindow(null, windowClass);

                if (hWnd != IntPtr.Zero)
                    return hWnd;
            }
            else if (windowTitle.Length > 0 && windowClass.Length > 0)
            {
                hWnd = FindWindow(windowClass, windowTitle);

                if (hWnd == IntPtr.Zero)
                    hWnd = FindWindow(windowClass, windowTitle);

                if (hWnd != IntPtr.Zero)
                    return hWnd;
            }

            return hWnd;
        }

        public WindowPosAndSize GetWindowPositionAndSize(string windowClass, string? windowTitle = "")
        {
            var hWnd = GetWindowHandle(windowClass, windowTitle);
            GetWindowRect(hWnd, out var rect);
            return ConvertRectToWindowPosAndSize(rect);
        }

        public Rectangle GetWindowPositionAndSize(IntPtr hWnd)
        {
            GetWindowRect(hWnd, out var rect);
            return rect;
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
            var hwnd = GetWindowHandle(windowTitle);
            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
            SetForegroundWindow(hwnd);
            ShowWindow(hwnd, SW_RESTORE);
        }
        public void UnsetWindowAlwaysOnTop(string windowTitle)
        {
            var hwnd = GetWindowHandle(windowTitle);
            SetWindowPos(hwnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
            SetWindowPos(hwnd, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
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
