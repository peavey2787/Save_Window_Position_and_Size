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

        [DllImport("user32.dll")]
        private static extern int GetWindowRect(IntPtr hwnd, out Rectangle rect);

        public bool SetWindowPositionAndSize(string windowTitle, int x, int y, int width, int height)
        {
            var hWnd = GetWindowHandle(windowTitle);

            // If found, position it.
            if (hWnd != IntPtr.Zero)
            {
                SetWindowPos(hWnd, IntPtr.Zero, x, y, width, height, SWP_NOZORDER);
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

        public WindowPosAndSize ConvertRectToWindowPosAndSize(Rectangle rect)
        {
            var windowPosAndSize = new WindowPosAndSize();
            windowPosAndSize.X = rect.Location.X;
            windowPosAndSize.Y = rect.Location.Y;
            windowPosAndSize.Width = rect.Width - rect.X;
            windowPosAndSize.Height = rect.Height - rect.Y;
            return windowPosAndSize;
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
