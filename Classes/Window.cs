using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Save_Window_Position_and_Size.Classes
{
    internal class Windows
    {
        public List<Window>[] Profiles;
        public Windows() 
        {
            Profiles = new List<Window>[5];            
        }
    }
    internal class Window
    {
        public IntPtr hWnd { get; set; }
        public WindowPosAndSize WindowPosAndSize { get; set; }
        public bool KeepOnTop { get; set; }
        public bool AutoPosition { get; set; }
        public int Id { get; set; }
        public string ProcessName { get; set; }
        public string TitleName { get; set; }
        public string DisplayName { get; set; }
        public bool IsFileExplorer { get; set; }
        public bool UseProcessName { get; set; }
        public bool UseTitleName { get; set; }

        public Window()
        {
            WindowPosAndSize = new WindowPosAndSize();
        }

    }
}
