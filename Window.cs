using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Save_Window_Position_and_Size
{
    internal class Window
    {
        public IntPtr? hWnd { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool KeepOnTop { get; set; }
        public bool AutoPosition { get; set; }
        public int Id { get; set; }
        public string? ProcessName { get; set; }
        public string? TitleName { get; set; }
        public string? ClassName { get; set; }
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public bool IsFileExplorer { get; set; }

        public bool CompareIsEqual(Window win1, Window win2)
        {
            if (win1.X == win2.X && win1.Y == win2.Y && win1.Width == win2.Width && win1.Height == win2.Height)
                return true;

            return false;
        }
        public WindowPosAndSize GetWindowPosAndSize()
        {
            var windowPosAndSize = new WindowPosAndSize();
            windowPosAndSize.X = X;
            windowPosAndSize.Y = Y;
            windowPosAndSize.Width = Width;
            windowPosAndSize.Height = Height;
            return windowPosAndSize;
        }
        public void SetWindowPosAndSize(WindowPosAndSize windowPosAndSize)
        {
            X = windowPosAndSize.X; 
            Y = windowPosAndSize.Y;
            Width = windowPosAndSize.Width; 
            Height = windowPosAndSize.Height;
        }
    }
}
